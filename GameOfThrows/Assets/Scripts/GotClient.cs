using System;
using System.Net;
using System.Net.Sockets;
using GotLib;
using UnityEngine;

// ReSharper disable MergeSequentialChecks

namespace Assets.Scripts
{
    public class GotClient : IDisposable
    {
        #region Public Members

        public PlayerDataExtractor playerData;

        #endregion

        #region Private Members

        private UdpClient UdpClient { get; set; }
        private byte[] DataStream { get; set; }
        private EndPoint serverEndpoint;
        private bool logoutRequested;

        #endregion

        #region Constants

        private const int PORT = 30000;

        public GotClient(PlayerDataExtractor playerDataExtractor)
        {
            playerData = playerDataExtractor;
        }

        #endregion

        #region Public API

        public void ConnectToServerAndStartListening(string ipAddress)
        {
            logoutRequested = false;

            // Initialize the login packet that would be sent to the server
            Packet loginPacket = GetPacketToSend(DataIdentifier.LogIn);

            UdpClient = new UdpClient(AddressFamily.InterNetwork)
            {
                Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            };

            // Initialise the IPEndPoint for the server
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(ipAddress), PORT);

            // Initialise the EndPoint for the server
            serverEndpoint = server;

            // Send the login packet to the server
            SendPacketToServer(loginPacket);

            // Reset the data stream
            ResetDataStream();

            // Begin listening for broadcasts
            AwaitMessageFromServer();
        }

        public void SendPlayerDataToServer()
        {
            if (IsSendPossible())
                return;

            Packet data = GetPacketToSend(DataIdentifier.Message);
            SendPacketToServer(data);
        }

        private bool IsSendPossible()
        {
            return UdpClient == null || UdpClient.Client == null || !UdpClient.Client.Connected;
        }

        public void SendLogOutMessageAndDisconnect()
        {
            try
            {
                if (UdpClient.Client == null)
                    throw new Exception("Already disconnected from server");

                // Initialise a packet object to store the data to be sent
                Packet logoutPacket = GetPacketToSend(DataIdentifier.LogOut);

                SendPacketToServer(logoutPacket);

                logoutRequested = true;
            }
            catch (Exception ex)
            {
                Debug.LogAssertion("Closing Error: " + ex.Message);
            }
        }

        #endregion

        #region Server Interaction Methods

        private void ResetDataStream()
        {
            DataStream = new byte[UdpClient.Client.ReceiveBufferSize];
        }

        private void SendPacketToServer(Packet packet)
        {
            // Get packet as byte array
            var packetData = packet.ToByteArray();

            // Send data to server
            UdpClient.Client.BeginSendTo(packetData, 0, packetData.Length, SocketFlags.None, serverEndpoint, SendData,
                null);
        }

        private void AwaitMessageFromServer()
        {
            UdpClient.Client.BeginReceiveFrom(DataStream, 0, DataStream.Length, SocketFlags.None, ref serverEndpoint,
                ReceiveData, null);
        }

        private void ReceiveData(IAsyncResult ar)
        {
            try
            {
                // Receive all data
                UdpClient.Client.EndReceive(ar);

                // Initialise a packet object to store the received data
                Packet receivedData = Packet.PacketFromBytes(DataStream);

                // Process data received
                ProcessedReceivedPacket(receivedData);

                // Reset data stream
                ResetDataStream();

                // Continue listening for broadcasts
                AwaitMessageFromServer();
            }
            catch (ObjectDisposedException) { }
            catch (SocketException)
            {
                Debug.LogAssertion("Server disconnected!");
            }
            catch (Exception ex)
            {
                Debug.LogAssertion("Receive Data Error: " + ex.Message);
            }
        }

        private static void ProcessedReceivedPacket(Packet receivedData)
        {
            if (receivedData.Message != null)
                Debug.Log(receivedData.Message);
        }

        private void SendData(IAsyncResult ar)
        {
            try
            {
                UdpClient.Client.EndSend(ar);

                if (!logoutRequested)
                    return;

                UdpClient.Close();
            }
            catch (Exception ex)
            {
                Debug.LogAssertion("Send Data Error: " + ex.Message);
            }
        }

        #endregion

        #region Player Data And Packet Methods

        private Packet GetPacketToSend(DataIdentifier identifier)
        {
            return GetPacketToSend(identifier, playerData.GetPlayerData());
        }

        /// <summary>
        /// Gets a standard data packet to be sent to the server
        /// </summary>
        /// <param name="identifier">The identifier of the message</param>
        /// <param name="data">Data to be transfered about the player</param>
        /// <returns>Packet initialized with identifier, name and message</returns>
        private static Packet GetPacketToSend(DataIdentifier identifier, PlayerData data)
        {
            return new Packet
            {
                DataIdentifier = identifier,
                Message = SerializedPlayerData(data)
            };
        }

        /// <summary>
        /// Gets the serialized RawPlayerData 
        /// </summary>
        private static string SerializedPlayerData(PlayerData data)
        {
            return Serializers.SerializeObject(data);
        }

        #endregion

        public void Dispose()
        {
            try
            {
                SendLogOutMessageAndDisconnect();
            }
            catch
            {
                // ignored
            }
        }
    }
}
