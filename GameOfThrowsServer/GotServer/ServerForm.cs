#region Resharper Disable Comments

// ReSharper disable LoopCanBePartlyConvertedToQuery
// ReSharper disable LocalizableElement

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GotServerLibrary;

namespace GotServer
{
    public partial class ServerForm : Form
    {
        #region Constants

        private const string LISTENING = "Listening";
        private const string ERROR = "Error";

        #endregion

        #region Private Members

        private readonly Server server;
        private readonly Dictionary<string, string> previousPlayersMessages;
        private readonly Stopwatch statusReseterStopwatch;

        #endregion

        #region Constructor

        public ServerForm()
        {
            statusReseterStopwatch = new Stopwatch();
            InitializeComponent();
            previousPlayersMessages = new Dictionary<string, string>();
            server = new Server();
        }

        #endregion

        #region Events

        private void Server_Load(object sender, EventArgs e)
        {
            try
            {
                txtStatusMessage.BackColor = txtStatusMessage.BackColor;

                // Initialize the delegate which updates the data
                server.ClientConnected += Server_OnClientConnected;
                server.ClientDisconneced += Server_OnClientDisconneced;
                server.ClientUpdated += Server_OnClientUpdated;
                server.InvalidClientUpdateReceived += UpdateState;

                server.StartListen();

                txtStatus.Text = LISTENING;
                statusReseterTimer.Start();
            }
            catch (Exception ex)
            {
                ShowError("Load Error: {0}", ex.Message);
                MessageBox.Show("Load Error: " + ex.Message, "UDP ServerForm", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                HandlePlayerUpdate(client);
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
                HandlePlayerDisconnection(client);
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
                HandlePlayerConnection(client);
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Dispose();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void statusReseterTimer_Tick(object sender, EventArgs e)
        {
            const int MAX_STATUS_TIME = 2000;

            if (txtStatusMessage.Text == string.Empty ||
                statusReseterStopwatch.ElapsedMilliseconds < MAX_STATUS_TIME)
                return;

            txtStatus.Text = LISTENING;
            txtStatus.ResetBackColor();
            txtStatusMessage.ResetText();
            txtStatusMessage.ResetForeColor();
            statusReseterStopwatch.Restart();
        }

        #endregion

        #region Other Methods

        private void HandlePlayerConnection(Client client)
        {
            var connectionMessage = string.Format("[ {0} : {1} ] Connected!", client.endPoint, client.data.name);
            UpdateState(connectionMessage);
            clientsListBox.Items.Add(client);
            previousPlayersMessages.Add(client.endPoint.ToString(), string.Empty);

            txtStatusMessage.Text = connectionMessage;
            txtStatusMessage.ForeColor = Color.LimeGreen;
            statusReseterStopwatch.Restart();
        }

        private void HandlePlayerDisconnection(Client client)
        {
            var disconnectionMessage = string.Format("[ {0} : {1} ] Disconnected!", client.endPoint, client.data.name);
            UpdateState(disconnectionMessage);
            clientsListBox.Items.Remove(client);
            previousPlayersMessages.Remove(client.endPoint.ToString());

            txtStatusMessage.Text = disconnectionMessage;
            txtStatusMessage.ForeColor = Color.Green;
            statusReseterStopwatch.Restart();
        }

        private void HandlePlayerUpdate(Client client)
        {
            var indexInList = clientsListBox.Items.IndexOf(client);

            if (indexInList == -1)
                ShowError("Client {0} updated while not in list", client.endPoint);
            else
            {
                clientsListBox.Items[indexInList] = client;
                var playerData = client.data.ToString();
                if (previousPlayersMessages[client.endPoint.ToString()] == playerData) return;

                UpdateState(playerData);
                previousPlayersMessages[client.endPoint.ToString()] = playerData;
            }
        }
        
        private void UpdateState(string message)
        {
            if (!message.EndsWith("\n"))
                message += "\n";
            messagesTextbox.AppendText(string.Format("{0} | {1}", DateTime.Now.ToLongTimeString(), message));
        }

        private void ShowError(string format, params object[] data)
        {
            ShowError(string.Format(format, data));
        }

        private void ShowError(string message)
        {
            txtStatus.Text = ERROR;
            txtStatus.ForeColor = Color.Red;
            txtStatusMessage.Text = message;
            txtStatusMessage.ForeColor = Color.Red;
            statusReseterStopwatch.Restart();
        }

        #endregion
    }
}