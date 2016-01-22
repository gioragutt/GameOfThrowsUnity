#region Resharper Disabling Methods

// ReSharper disable UnusedAutoPropertyAccessor.Local 

#endregion

using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GotLib;
using GotLoggingService;

/*
This is a concept class to improve handling data between client and server
It is based on the previous server HandledPlayer 
*/

namespace GotServerLibrary
{
    public class Counter
    {
        public int Elasped { get; private set; }

        public Counter()
        {
            Elasped = 0;
        }

        public void Count()
        {
            Elasped++;
        }

        public void Reset()
        {
            Elasped = 0;
        }
    }

    public class ConnectedClient
    {
        #region Constants

        // Amount of times a player has to not responded to be disconnected by the server
        public const int TIMEOUT_TICKS = 500;
    
        #endregion

        #region Public

        public PlayerData PlayerData { get; set; }
        public UdpClient ClientUdp { get; }
        public TcpClient ClientTcp { get; }

        public EndPoint ClientEndpoint
        {
            get { return ClientUdp.Client.RemoteEndPoint; }
        }

        #endregion

        #region Private

        private Thread UpdateThread { get; }
        private Counter Timeout { get; }
        private Server Server { get; }
        private LoggerManager Logger { get; }

        private static bool _debug = false;

        private BinaryWriter TcpPacketWriter { get; }
        private BinaryReader TcpPacketReader { get; }

        private Server.ServerEventArgs EventArgs
        {
            get { return Server.ServerEventArgs.Create(PlayerData, ClientEndpoint); }
        }

        #endregion

        #region Constructor

        internal ConnectedClient(TcpClient clientTcp, Server server)
        {
            Server = server;
            Server.Clients.Add(this);

            Logger = LoggerManager.GetLogger("CLIENT-" + Server.Clients.IndexOf(this), Server.Logger);
            Logger.Info("Added client to server's clients list");

            ClientTcp = clientTcp;
            Logger.InfoFormat("Initializing ConnectedClient with TCP: {0}", ClientTcp.Client.RemoteEndPoint);

            TcpPacketReader = new BinaryReader(ClientTcp.GetStream());
            TcpPacketWriter = new BinaryWriter(ClientTcp.GetStream());

            ClientUdp = new UdpClient();
            ClientUdp.Connect(new IPEndPoint((((IPEndPoint)ClientTcp.Client.RemoteEndPoint)).Address, 27015));

            Logger.DebugFormat("Connected ClientUDP to {0}", ClientUdp.Client.RemoteEndPoint);

            TcpPacketWriter.Write(((IPEndPoint)ClientUdp.Client.LocalEndPoint).Port);

            Timeout = new Counter();
            PlayerData = new PlayerData();

            Server.OnClientConnected(EventArgs);

            // Send first data to client
            Server.Write(this);

            // Start interacting with client
            Logger.Info("Start listening to player updates!");
            UpdateThread = new Thread(Update);
            UpdateThread.Start();
        }

        #endregion

        #region Server Methods

        internal void Update()
        {
            while (ClientUdp.Client.Connected)
            {
                if (ClientUdp.Client.Available > 2)
                {
                    IPEndPoint client = null;

                    MemoryStream recievedData = new MemoryStream(ClientUdp.Receive(ref client));

                    // Mark client as responded
                    ClientResponded();

                    // Read PlayerData
                    using (BinaryReader reader = new BinaryReader(recievedData))
                    {
                        Read(reader);
                    }

                    // Invoke Client Updated
                    Server.OnClientUpdated(EventArgs);

                    Server.Write(this);
                }
                else
                {
                    if (!_debug)
                        ClientDidntRespond();
                    Debug.WriteLine(Timeout.Elasped);
                }

                Thread.Sleep(20);
            }
        }

        #endregion

        #region Read And Write Own Data

        /* Protocol definition (order of reading and writing of data members)
            1) PlayerData (own r/w method)        
        */

        private void Read(BinaryReader reader)
        {
            /* 
            InitializeFromFirstUpdate: will also read initial data from player log in.
            An example would be - Chosen team to connect. When a player launched the game through
            The GOT game launcher, he chose a team. When he first connected the 
            */
            PlayerData.Read(reader);
        }

        internal void Write(BinaryWriter writer)
        {
            PlayerData.Write(writer);
        }

        #endregion

        #region Other Methods

        internal void KickPlayer()
        {
            Server.Clients.Remove(this);
            DisconnectPlayer();
        }

        private void DisconnectPlayer()
        {
            Logger.InfoFormat("Disconnecting player : {0}", ClientTcp.Client.RemoteEndPoint);
            IPEndPoint client = null;
            try
            {
                UpdateThread.Abort();

                if (ClientUdp.Client.Connected)
                    ClientUdp.Close();
                if (ClientTcp.Connected)
                {
                    client = ClientTcp.Client.RemoteEndPoint as IPEndPoint;
                    ClientTcp.Close();
                }
            }
            finally
            {
                Server.OnClientDisconnected(Server.ServerEventArgs.Create(PlayerData, client));
            }
        }

        private void ClientDidntRespond()
        {
            if (Timeout.Elasped >= TIMEOUT_TICKS)
                KickPlayer();
            else
                Timeout.Count();
        }

        private void ClientResponded()
        {
            Timeout.Reset();
        }

        #endregion
    }
}
