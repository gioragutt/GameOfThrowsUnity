using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Scripts.PlayerClasses;
using UnityEngine;

// ReSharper disable ForCanBeConvertedToForeach

namespace Assets.Scripts
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

    public class Connection : MonoBehaviour
    {
        #region Public

        public LocalPlayer localPlayer;
        public string ipAddress;
        public bool useLocalHost;

        #endregion

        #region Private

        /// <summary>
        /// Gets the UDP connection client
        /// </summary>
        private UdpClient ServerUdp { get; set; }

        private TcpClient ServerTcp { get; set; }

        private BinaryWriter TcpPacketWriter { get; set; }
        private BinaryReader TcpPacketReader { get; set; }

        /// <summary>
        /// Gets and sets the list of players
        /// </summary>
        private List<Player> Players { get; set; }

        /// <summary>
        /// Gets and sets the ID of the local player that was received from the server
        /// </summary>
        private int LocalPlayerID { get; set; }

        /// <summary>
        /// Thread for updating from the server
        /// </summary>
        private Thread UpdateThread { get; set; }

        /// <summary>
        /// Counter for updating whether the server is up or not
        /// </summary>
        private Counter Timeout { get; set; }

        #region Commented out because not relavant in current implentation

        ///// <summary>
        ///// Gets or sets whether the team was sent to the server or not
        ///// </summary>
        //private bool WasTeamSent { get; set; }

        ///// <summary>
        ///// Gets and sets the last ping time from the server (in milliseconds)
        ///// </summary>
        //public long Ping { get; set; }

        ///// <summary>
        ///// Stopwatch for counting time between network updates
        ///// </summary>
        //public Stopwatch PingMeasurer { get; set; }

        #endregion

        #endregion

        #region Constants

        private const int SERVER_PORT = 30000;
        private const int TIMEOUT_TICKS = 500;

        #endregion

        #region Constructor

        private void Awake()
        {
            Players = new List<Player> { localPlayer };
            UpdateThread = new Thread(ReadFromServer);
            Timeout = new Counter();
        }

        #endregion

        #region Other Methods

        private void Start()
        {
            if (useLocalHost)
                ipAddress = GameManager.GetLocalIpAddress();

            ServerTcp = new TcpClient();
            var serverIp = IPAddress.Parse(ipAddress);

            if (!TryConnect(serverIp))
                return;

            InitializeMembers();

            ConnectToServerUdp(GetServerUdpEndpoint());
        }

        private void InitializeMembers()
        {
            TcpPacketReader = new BinaryReader(ServerTcp.GetStream());
            TcpPacketWriter = new BinaryWriter(ServerTcp.GetStream());
        }

        private IPEndPoint GetServerUdpEndpoint()
        {
            while (ServerTcp.Available < sizeof(int)) { }

            var serverUdpPort = TcpPacketReader.ReadInt32();
            var serverUdpEndpoint = new IPEndPoint(((IPEndPoint)ServerTcp.Client.RemoteEndPoint).Address, serverUdpPort);
            return serverUdpEndpoint;
        }

        private void ConnectToServerUdp(IPEndPoint serverUdpEndpoint)
        {
            Debug.LogError("Connecting UDP to: " + serverUdpEndpoint);
            ServerUdp = new UdpClient(27015);
            ServerUdp.Connect(serverUdpEndpoint);
            if (ServerUdp.Client.Connected)
            {
                WriteToServer();
                UpdateThread.Start();
                Debug.LogError("Connection to UDP successful!");
            }
            else
                Debug.LogError("Connection to UDP failed!");
        }

        private bool TryConnect(IPAddress serverIp)
        {
            bool success;

            try
            {
                ServerTcp.Connect(new IPEndPoint(serverIp, SERVER_PORT));
                success = ServerTcp.Connected;
            }
            catch (SocketException)
            {
                success = false;
            }

            if (success) return true;

            Debug.LogError("Server is not online!");
            return false;
        }

        private void ConnectToServerHandler(IPAddress serverIp, IPEndPoint server)
        {
            WriteToServer();
        }

        #endregion

        #region Read And Write Data

        /// <summary>
        /// Wrutes data to the server
        /// </summary>
        private void WriteToServer()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    localPlayer.Data.Write(writer);

                    ServerUdp.Client.Send(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// Reads data received from the server
        /// </summary>
        private void ReadFromServer()
        {
            while (true)
            {
                if (ServerUdp.Client.Available > 0)
                {
                    IPEndPoint server = null;
                    var data = ServerUdp.Receive(ref server);
                    Debug.LogErrorFormat("Received {0} bytes from server!", data.Length);

                    // Mark server as responded
                    ServerResponded();

                    using (var ms = new MemoryStream(data))
                    {
                        using (var reader = new BinaryReader(ms))
                        {
                            // Get ID of local player
                            LocalPlayerID = reader.ReadInt32();

                            // Update amount of players
                            SetAmountOfPlayers(reader.ReadInt32());

                            // Update players data
                            UpdatePlayers(reader);
                        }
                    }

                    // Write updated player data to server
                    WriteToServer();
                    Debug.Log("Writing to server!");
                }
                else
                {
                    ServerDidntRespond();
                }

                Thread.Sleep(20);
            }
        }

        private void UpdatePlayers(BinaryReader reader)
        {
            for (int i = 0; i < Players.Count; ++i)
            {
                Players[i].Read(reader);
            }
        }

        #endregion

        #region Other Methods

        void OnApplicationQuit()
        {
            Disconnect();
            Debug.Log("Application ending after " + Time.time + " seconds");
        }

        private void Disconnect()
        {
            try
            {
                UpdateThread.Abort();

                if (ServerUdp.Client.Connected)
                    ServerUdp.Close();
                if (ServerTcp.Connected)
                    ServerTcp.Close();
            }
            finally
            {
                Debug.Log("Disconnected from server!");
            }
        }

        private void ServerResponded()
        {
            Timeout.Reset();
        }

        private void ServerDidntRespond()
        {
            if (Timeout.Elasped >= TIMEOUT_TICKS)
                Disconnect();
            else
                Timeout.Count();
        }

        private void SetAmountOfPlayers(int playerCount) { }

        #endregion
    }
}
