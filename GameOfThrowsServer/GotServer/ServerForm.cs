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
        private BindingSource clientsSource;

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

                clientsSource = new BindingSource { DataSource = server.ClientList };

                clientsSource.ResetBindings(true);
                clientsListBox.DataSource = clientsSource;

                // Initialize the delegate which updates the data
                server.DataRecieved += UpdateStatus;
                server.ClientConnected += Server_ClientListUpdated;
                server.ClientDisconneced += Server_ClientListUpdated;
                server.ClientUpdated += Server_ClientListUpdated;

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

        private void ShowError(string message)
        {
            lblStatus.Text = "Error";
            txtStatus.Text = message;
        }

        private void Server_ClientListUpdated(Client client)
        {
            clientsSource.ResetBindings(true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Other Methods

        private void UpdateStatus(Packet status)
        {
            if (InvokeRequired)
            {
                Server.DataRecievedDelegate dataRecievedOperation = UpdateStatus;
                Invoke(dataRecievedOperation, status);
            }
            else
            {
                HandleUpdatePacket(status);
            }
        }

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

        #endregion

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            server.Dispose();
        }
    }
}