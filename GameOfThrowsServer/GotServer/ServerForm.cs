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
            server = new Server(false);
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
                server.ClientDisconnected += Server_OnClientDisconneced;
                server.ClientUpdated += Server_OnClientUpdated;
                server.InvalidLoginAttempt +=Server_OnInvalidLoginAttempt;
                server.StartListen();
                clientsListBox.DataSource = server.Clients;
                clientsListBox.DataBindings.Add("", server.Clients, "");

                txtStatus.Text = LISTENING;
                statusReseterTimer.Start();

                ShowStatus(Color.CornflowerBlue, "Server initiated!");
            }
            catch (Exception ex)
            {
                ShowError("Load Error: {0}", ex.Message);
                MessageBox.Show("Load Error: " + ex.Message, "UDP ServerForm", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Server_OnInvalidLoginAttempt(Server.ServerEventArgs e)
        {
            if (InvokeRequired)
            {
                Server.ServerUpdateDelegate serverUpdateOperation = Server_OnInvalidLoginAttempt;
                Invoke(serverUpdateOperation, e);
            }
            else
            {
                UpdateStatus("Client {0} trying to log in when already logged in", e.EndPoint.ToString());
            }
        }

        private void Server_OnClientUpdated(Server.ServerEventArgs e)
        {
            if (InvokeRequired)
            {
                Server.ServerUpdateDelegate serverUpdateOperation = Server_OnClientUpdated;
                Invoke(serverUpdateOperation, e);
            }
            else
            {
                UpdateStatus(e.PlayerData.ToString());
                clientsListBox.DataSource = server.Clients;
            }
        }

        private void Server_OnClientDisconneced(Server.ServerEventArgs e)
        {
            if (InvokeRequired)
            {
                Server.ServerUpdateDelegate serverUpdateOperation = Server_OnClientDisconneced;
                Invoke(serverUpdateOperation, e);
            }
            else
            {
                ShowStatus(Color.DarkGreen, "Client {0} : {1} Disconnected!", e.EndPoint, e.PlayerData);
                clientsListBox.DataSource = server.Clients;
            }
        }

        private void Server_OnClientConnected(Server.ServerEventArgs e)
        {
            if (InvokeRequired)
            {
                Server.ServerUpdateDelegate serverUpdateOperation = Server_OnClientConnected;
                Invoke(serverUpdateOperation, e);
            }
            else
            {
                ShowStatus(Color.LawnGreen, "Client {0} : {1} Connected!", e.EndPoint, e.PlayerData);
                clientsListBox.DataSource = server.Clients;
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Dispose();
            Process.GetCurrentProcess().Kill();
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

        private void UpdateStatus(string format, params object[] data)
        {
            UpdateStatus(string.Format(format, data));
        }

        private void UpdateStatus(string message)
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
            UpdateStatus(message);
        }

        private void ShowStatus(Color statusColor, string format, params object[] data)
        {
            ShowStatus(statusColor, string.Format(format, data));
        }

        private void ShowStatus(Color statusColor, string message)
        {
            txtStatusMessage.ForeColor = statusColor;
            txtStatusMessage.Text = message;
            statusReseterStopwatch.Restart();
            UpdateStatus(message);
        }

        #endregion
    }
}