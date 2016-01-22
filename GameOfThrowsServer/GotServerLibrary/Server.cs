using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GotLib;
using GotLoggingService;
using ThreadState = System.Threading.ThreadState;

// ReSharper disable ForCanBeConvertedToForeach

// ReSharper disable LocalizableElement
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace GotServerLibrary
{
    public class Server : IDisposable
    {
        #region Delegate and Events

        public class ServerEventArgs : EventArgs
        {
            public PlayerData PlayerData { get; }
            public EndPoint EndPoint { get; }

            public static ServerEventArgs Create(PlayerData data, EndPoint endpoint)
            {
                return new ServerEventArgs(data, endpoint);
            }

            private ServerEventArgs(PlayerData data, EndPoint endpoint)
            {
                PlayerData = data;
                EndPoint = endpoint;
            }
        }

        public delegate void ServerUpdateDelegate(ServerEventArgs e);
        public event ServerUpdateDelegate ClientConnected;
        public event ServerUpdateDelegate ClientDisconnected;
        public event ServerUpdateDelegate ClientUpdated;
        public event ServerUpdateDelegate InvalidLoginAttempt;

        #endregion

        #region Constants

        public const int SERVER_PORT = 30000;

        #endregion

        public ClientList Clients { get; }
        public LoggerManager Logger { get; }

        private Thread ClientAcceptingThread { get; }
        private TcpListener Listener { get; }

        #region Constructor

        public Server(bool startOnConstructor = true)
        {
            Logger = LoggerManager.GetLogger("GAME-OF-THROWS-SERVER");
            Logger.Info("-----------------------------------------");
            Logger.Info("Server Initializing");
            Clients = new ClientList();
            ClientAcceptingThread = new Thread(AcceptMethod);
            Listener = new TcpListener(IPAddress.Any, SERVER_PORT);

            if (startOnConstructor)
                StartListen();
        }

        #endregion

        private void AcceptMethod()
        {
            /*
            
            Run forever (for as long as the thread is alive) and await new player connections.
            The block happens on Listener.AcceptTcpClient(), which blocks until a connection is received,
            At which point a ConnectedClient instance is instantiated and the client is connected
            
            */
            while (true)
            {
                var accepted = new ConnectedClient(Listener.AcceptTcpClient(), this);
                Logger.Info("Accepted client ip: " + accepted.ClientTcp.Client.RemoteEndPoint);
            }
        }

        public void StartListen()
        {
            if (ClientAcceptingThread.ThreadState == ThreadState.Running)
            {
                Logger.Debug("Attemp to start listening when already listening");
                throw new Exception("Server already running!");
            }

            Listener.Start();
            Logger.InfoFormat("Start Listening on {0}", Listener.Server.LocalEndPoint);
            ClientAcceptingThread.Start();
        }

        public void StopListen()
        {
            Logger.Info("Stop Listening");
            Logger.Info("-----------------------------------------");
            ClientAcceptingThread.Abort();
            Clients.DisconnectAll();
        }

        #region Write Data

        public void Write(ConnectedClient client)
        {
            /*
            +-----------------------------------------------------------------------+
            |   This is where you write the data the client receives.               |
            |   By the old protocol, you send the index and the amount of players   | 
            |   As the first to pieces Of information,                              |
            |   To be able to handle new players or removed players.                |
            |   Then, you can send general server data - scores, map changes,       |
            |   Etc. (stuff that are not related to each player)                    |
            +-----------------------------------------------------------------------+
            */

            #region Memory Stream Explanation
            // Buffer into a memory stream to cut on network time
            // When implemente enough of server and client works too,
            // Check if there's performance impact without buffer 
            #endregion
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    // Write index of player in list
                    writer.Write(Clients.IndexOf(client));

                    // Write amount of players connected
                    writer.Write(Clients.Count);

                    // Send player data in the same order as they are in the list
                    // Note: do not convert to foreach, CollectionChangedExceptions and such will be raised
                    // At most inappropriate times!
                    for (int i = 0; i < Clients.Count; ++i)
                    {
                        Clients[i].Write(writer);
                    }

                    // Write data to the player
                    client.ClientUdp.Client.Send(ms.ToArray());
                }
            }
        }

        #endregion

        #region Event Invocations

        internal virtual void OnClientConnected(ServerEventArgs e)
        {
            ClientConnected?.Invoke(e);
        }

        internal virtual void OnInvalidLoginAttempt(ServerEventArgs e)
        {
            InvalidLoginAttempt?.Invoke(e);
        }

        internal virtual void OnClientUpdated(ServerEventArgs e)
        {
            ClientUpdated?.Invoke(e);
        }

        internal virtual void OnClientDisconnected(ServerEventArgs e)
        {
            ClientDisconnected?.Invoke(e);
        }

        #endregion

        #region IDisposable Methods

        public void Dispose()
        {
            StopListen();
        }

        #endregion
    }
}
