using System;
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

        /// <summary>
        /// Delegate that handles invalid data from clients
        /// </summary>
        /// <param name="message"></param>
        public delegate void MessageDelegate(string message);

        /// <summary>
        /// Invoked when received invalid data from a client
        /// </summary>
        public event MessageDelegate InvalidClientUpdateReceived;

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
                throw new GotServerInitializationException(ex);
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
                throw new GotServerInitializationException(ex);
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
                ProcessMessageData(receivedData, epSender);

                // Get packet as byte array
                var data = receivedData.ToByteArray();

                // Send data to other clients
                DistributeData(clients, receivedData, data);

                // Listen for more connections again...
                UdpClient.Client.BeginReceiveFrom(DataStream, 0, DataStream.Length, SocketFlags.None, ref epSender,
                    ReceiveData, epSender);
            }
            catch (Exception ex)
            {
                throw new GotServerRecieveDataException(ex);
            }
        }

        private void ProcessMessageData(Packet receivedData, EndPoint epSender)
        {
            Client sendingClient = ClientList.GetClient(epSender);

            switch (receivedData.DataIdentifier)
            {
                case DataIdentifier.Message:
                    HandleMessageIdentifier(receivedData, epSender, sendingClient);
                    break;

                case DataIdentifier.LogIn:
                    HandleLogInIdentifier(receivedData, epSender, sendingClient);
                    break;

                case DataIdentifier.LogOut:
                    HandleLogOutIdentifier(epSender, sendingClient);
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
                throw new GotServerSendDataException(ex);
            }
        }

        #endregion

        #region Identifier Handling Methods

        private void HandleLogOutIdentifier(EndPoint epSender, Client disconnectingClient)
        {
            if (disconnectingClient == null)
            {
                HandleAlreadyLoggedOut(epSender);
                return;
            }

            OnClientDisconneced(disconnectingClient);
            ClientList.Remove(disconnectingClient);
        }

        private void HandleLogInIdentifier(Packet receivedData, EndPoint epSender, Client loggingInClient)
        {
            if (loggingInClient != null)
            {
                HandleAlreadyLoggedIn(epSender);
                return;
            }

            Client newClient = new Client
            {
                endPoint = epSender,
                id = Guid.NewGuid(),
                data = Serializers.DeserializeObject<PlayerData>(receivedData.Message)
            };

            OnClientConnected(newClient);
            ClientList.Add(newClient);
        }

        private void HandleMessageIdentifier(Packet receivedData, EndPoint epSender, Client sendingClient)
        {
            if (sendingClient == null)
            {
                HandleMessageRecievedFromUnknown(epSender);
                return;
            }

            sendingClient.data = Serializers.DeserializeObject<PlayerData>(receivedData.Message);
            OnClientUpdated(sendingClient);
        }

        #endregion

        #region Error Handling Methods

        private void HandleAlreadyLoggedOut(EndPoint epSender)
        {
            var errorLogoutMessage = string.Format("Recieved LOGOUT message from a player that's not in the list: {0}",
                epSender);
            SendErrorMessage(errorLogoutMessage);
        }

        private void HandleAlreadyLoggedIn(EndPoint epSender)
        {
            var errorLoginMessage = string.Format("Recieved LOGIN message from a player that's already in the list: {0}", epSender);
            SendErrorMessage(errorLoginMessage);
        }

        private void HandleMessageRecievedFromUnknown(EndPoint epSender)
        {
            var messageRecievedErrorMessage = string.Format(
                "Recieved message from player that's not in the list : {0}", epSender);
            SendErrorMessage(messageRecievedErrorMessage);
        }

        private void SendErrorMessage(string errorMessage)
        {
            OnUnkownClientRecieved(errorMessage);
        }

        #endregion

        #region Event Invocations

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

        protected virtual void OnUnkownClientRecieved(string error)
        {
            InvalidClientUpdateReceived?.Invoke(error);
        }

        #endregion

        public void Dispose()
        {
            StopListen();
        }
    }
}
