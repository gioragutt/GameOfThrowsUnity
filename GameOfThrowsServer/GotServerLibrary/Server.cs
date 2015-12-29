using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GotLib;

// ReSharper disable LocalizableElement
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace GotServerLibrary
{
    public class Server : IDisposable
    {
        #region Delegate and Events

        /// <summary>
        /// Delegate that handles changes in data recieval
        /// </summary>
        /// <param name="data">Data to be sent outside of server</param>
        public delegate void DataRecievedDelegate(Packet data);

        /// <summary>
        /// Invoked when data is recieved on the server
        /// </summary>
        public event DataRecievedDelegate DataRecieved;

        /// <summary>
        /// Delegate that handles changes to clients
        /// </summary>
        /// <param name="client"></param>
        public delegate void ClientUpdateDelegate(Client client);

        /// <summary>
        /// Invoked when a client is connected to the server
        /// </summary>
        public event ClientUpdateDelegate ClientConnected;

        /// <summary>
        /// Invoked when a client is disconnceted from the server
        /// </summary>
        public event ClientUpdateDelegate ClientDisconneced;

        /// <summary>
        /// Invoked when a client's data is updated
        /// </summary>
        public event ClientUpdateDelegate ClientUpdated;

        #endregion

        #region Public Members

        // Listing of clients
        public ClientList ClientList { get; set; }

        #endregion

        #region Constants

        private const int SERVER_PORT = 30000;

        #endregion

        #region Private Members

        // Server socket
        private UdpClient UdpClient { get; }

        // Data stream
        private byte[] DataStream { get; }

        // save the endpoint to allow late listening start
        private EndPoint clientsEndPoint;

        #endregion

        #region Constructor

        public Server()
        {
            try
            {
                // Initialise the ArrayList of connected clients
                ClientList = new ClientList();

                // Initialise the socket
                UdpClient = new UdpClient(AddressFamily.InterNetwork)
                {
                    Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                };

                // Initialize the data buffer
                DataStream = new byte[UdpClient.Client.ReceiveBufferSize];

                // Initialise the IPEndPoint for the server and listen on port 30000
                IPEndPoint server = new IPEndPoint(IPAddress.Any, SERVER_PORT);

                // Associate the socket with this IP address and port
                UdpClient.Client.Bind(server);

                // Initialise the IPEndPoint for the clients
                IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);

                // Initialise the EndPoint for the clients
                clientsEndPoint = clients;
            }
            catch (Exception ex)
            {
                throw new GotServerException(ExceptionType.Initialization, ex);
            }
        }

        public void StartListen()
        {
            try
            {
                // Start listening for incoming data
                UdpClient.Client.BeginReceiveFrom(DataStream, 0, DataStream.Length, SocketFlags.None,
                    ref clientsEndPoint, ReceiveData, clientsEndPoint);
            }
            catch (Exception ex)
            {
                throw new GotServerException(ExceptionType.Initialization, ex);
            }
        }

        public void StopListen()
        {
            if (UdpClient == null || !UdpClient.Client.Connected)
                return;

            UdpClient.Close();
        }

        #endregion

        #region Send And Receive

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                // Initialise a packet object to store the received data
                Packet receivedData = Packet.PacketFromBytes(DataStream);

                // Initialise the IPEndPoint for the clients
                IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);

                // Initialise the EndPoint for the clients
                EndPoint epSender = clients;

                // Receive all data
                UdpClient.Client.EndReceiveFrom(asyncResult, ref epSender);

                // Analyza recieved packet and get return packet
                var sendData = GetSendDataPacket(receivedData, epSender);

                // Get packet as byte array
                var data = sendData.ToByteArray();

                // Send data to other clients
                DistributeData(clients, sendData, data);

                // Listen for more connections again...
                UdpClient.Client.BeginReceiveFrom(DataStream, 0, DataStream.Length, SocketFlags.None, ref epSender,
                    ReceiveData, epSender);

                // Update data through a delegate
                OnDataRecieved(sendData);
            }
            catch (Exception ex)
            {
                throw new GotServerException(ExceptionType.RecieveData, ex);
            }
        }

        private Packet GetSendDataPacket(Packet receivedData, EndPoint epSender)
        {
            Packet dataPacket = new Packet
            {
                DataIdentifier = receivedData.DataIdentifier,
                Name = receivedData.Name,
                Message = null
            };

            ProcessMessageData(receivedData, epSender, dataPacket);

            return dataPacket;
        }

        private void ProcessMessageData(Packet receivedData, EndPoint epSender, Packet dataPacket)
        {
            Client sendingClient = ClientList.GetClient(epSender);

            switch (receivedData.DataIdentifier)
            {
                case DataIdentifier.Message:
                    HandleMessageIdentifier(receivedData, epSender, dataPacket, sendingClient);
                    break;

                case DataIdentifier.LogIn:
                    HandleLogInIdentifier(receivedData, epSender, dataPacket, sendingClient);
                    break;

                case DataIdentifier.LogOut:
                    HandleLogOutIdentifier(receivedData, epSender, dataPacket, sendingClient);
                    break;
            }
        }

        private void DistributeData(EndPoint epSender, Packet sendData, byte[] data)
        {
            if (sendData.DataIdentifier == DataIdentifier.LogIn || sendData.DataIdentifier == DataIdentifier.Error) return;

            foreach (Client client in ClientList)
            {
                if (client.endPoint.ToString() != epSender.ToString())
                {
                    // Broadcast to all logged on users except for sending client
                    UdpClient.Client.BeginSendTo(data, 0, data.Length, SocketFlags.None, client.endPoint, SendData, client.endPoint);
                }
            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                UdpClient.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                throw new GotServerException(ExceptionType.SendData, ex);
            }
        }

        #endregion

        #region Identifier Handling Methods

        private void HandleLogOutIdentifier(Packet receivedData, EndPoint epSender, Packet dataPacket,
                                            Client disconnectingClient)
        {
            if (disconnectingClient == null)
            {
                HandleAlreadyLoggedOut(epSender, dataPacket);
                return;
            }

            dataPacket.Message = string.Format("-- {0} has gone offline --", receivedData.Name);
            OnClientDisconneced(disconnectingClient);
            ClientList.Remove(disconnectingClient);
        }

        private void HandleLogInIdentifier(Packet receivedData, EndPoint epSender, Packet dataPacket,
                                           Client loggingInClient)
        {
            if (loggingInClient != null)
            {
                HandleAlreadyLoggedIn(epSender, dataPacket);
                return;
            }

            Client newClient = new Client
            {
                endPoint = epSender,
                id = Guid.NewGuid(),
                data = Serializers.DeserializeObject<PlayerData>(receivedData.Message),
                name = receivedData.Name
            };

            OnClientConnected(newClient);
            ClientList.Add(newClient);
            dataPacket.Message = string.Format("-- {0} is online --", receivedData.Name);
        }

        private void HandleMessageIdentifier(Packet receivedData, EndPoint epSender, Packet dataPacket,
                                             Client sendingClient)
        {
            if (sendingClient == null)
            {
                HandleMessageRecievedFromUnknown(epSender, dataPacket);
                return;
            }

            if (sendingClient.name != receivedData.Name)
            {
                dataPacket.Message = string.Format("{0} : Player {1} changed his name to {2}", epSender,
                    sendingClient.name, receivedData.Name);
                sendingClient.name = receivedData.Name;
            }

            sendingClient.data = Serializers.DeserializeObject<PlayerData>(receivedData.Message);

            if (string.IsNullOrEmpty(dataPacket.Message))
                dataPacket.Message = string.Format("{0}: {1}", receivedData.Name, sendingClient.data);
            OnClientUpdated(sendingClient);
        }

        #endregion

        #region Error Handling Methods

        private static void HandleAlreadyLoggedOut(EndPoint epSender, Packet dataPacket)
        {
            var errorLogoutMessage = string.Format("Recieved LOGOUT message from a player that's not in the list: {0}",
                epSender);

            SetPacketAsErrorMessage(dataPacket, errorLogoutMessage);
        }

        private static void HandleAlreadyLoggedIn(EndPoint epSender, Packet dataPacket)
        {
            var errorLoginMessage = string.Format("Player already in list : {0}", epSender);
            SetPacketAsErrorMessage(dataPacket, errorLoginMessage);
        }

        private static void HandleMessageRecievedFromUnknown(EndPoint epSender, Packet dataPacket)
        {
            var messageRecievedErrorMessage = string.Format(
                "Recieved message from player that's not in the list : {0}", epSender);
            SetPacketAsErrorMessage(dataPacket, messageRecievedErrorMessage);
        }

        private static void SetPacketAsErrorMessage(Packet data, string errorMessage)
        {
            data.DataIdentifier = DataIdentifier.Error;
            data.Message = errorMessage;
        }

        #endregion

        #region Event Invocations

        protected virtual void OnDataRecieved(Packet data)
        {
            DataRecieved?.Invoke(data);
        }

        protected virtual void OnClientConnected(Client client)
        {
            ClientConnected?.Invoke(client);
        }

        protected virtual void OnClientDisconneced(Client client)
        {
            ClientDisconneced?.Invoke(client);
        }

        protected virtual void OnClientUpdated(Client client)
        {
            ClientUpdated?.Invoke(client);
        }

        #endregion

        public void Dispose()
        {
            StopListen();
        }
    }
}
