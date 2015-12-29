#region Resharper Disable Comments
// ReSharper disable LoopCanBePartlyConvertedToQuery
// ReSharper disable LocalizableElement
#endregion

using System;
using System.Windows.Forms;
using GotLib;
using GotServerLibrary;

namespace GotServer
{
    public partial class ServerForm : Form
    {
        #region Private Members

        private Server server;

        #endregion

        #region Constructor

        public ServerForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void Server_Load(object sender, EventArgs e)
        {
            try
            {
                // Initialize the server
                server = new Server();

                // Initialize the delegate which updates the data
                server.DataRecieved += Server_OnDataRecieved;
                server.ClientConnected += Server_OnClientConnected;
                server.ClientDisconneced += Server_OnClientDisconneced;
                server.ClientUpdated += Server_OnClientUpdated;

                server.StartListen();

                lblStatus.Text = "Listening";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error";
                txtStatus.Text = string.Format("Load Error: {0}", ex.Message);
                MessageBox.Show("Load Error: " + ex.Message, "UDP ServerForm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Server_OnClientUpdated(Client client)
        {
            if (InvokeRequired)
            {
                Server.ClientUpdateDelegate clientUpdateOperation = Server_OnClientUpdated;
                Invoke(clientUpdateOperation, client);
            }
            else
            {
                var indexInList = clientsListBox.Items.IndexOf(client);

                if (indexInList != -1)
                {
                    clientsListBox.Items[indexInList] = client;
                }
                else
                {
                    ShowError("Client {0} updated while not in list", client.endPoint);
                }
            }
        }

        private void Server_OnClientDisconneced(Client client)
        {
            if (InvokeRequired)
            {
                Server.ClientUpdateDelegate clientUpdateOperation = Server_OnClientDisconneced;
                Invoke(clientUpdateOperation, client);
            }
            else
            {
                clientsListBox.Items.Remove(client);
            }
        }

        private void Server_OnClientConnected(Client client)
        {
            if (InvokeRequired)
            {
                Server.ClientUpdateDelegate clientUpdateOperation = Server_OnClientConnected;
                Invoke(clientUpdateOperation, client);
            }
            else
            {
                clientsListBox.Items.Add(client);
            }
        }

        private void Server_OnDataRecieved(Packet status)
        {
            if (InvokeRequired)
            {
                Server.DataRecievedDelegate dataRecievedOperation = Server_OnDataRecieved;
                Invoke(dataRecievedOperation, status);
            }
            else
            {
                HandleUpdatePacket(status);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Dispose();
        }

        #endregion

        #region Other Methods

        private Packet previousStatus;

        private void HandleUpdatePacket(Packet status)
        {
            if (previousStatus == status)
                return;

            if (status.DataIdentifier == DataIdentifier.Error)
            {
                ShowError(status.Message);
                return;
            }

            messagesTextbox.AppendText(status.Message + Environment.NewLine);
            previousStatus = status;
        }

        private void ShowError(string format, params object[] data)
        {
            ShowError(string.Format(format, data));
        }

        private void ShowError(string message)
        {
            lblStatus.Text = "Error";
            txtStatus.Text = message;
        }

        #endregion
    }
}