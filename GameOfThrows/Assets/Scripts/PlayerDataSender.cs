using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(LocalPlayerMovement))]
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    public class PlayerDataSender : MonoBehaviour
    {
        #region Public Members

        public string ip;
        public bool useLocalHost;
        public PlayerDataExtractor playerDataExtractor;

        #endregion

        #region Private Members

        private GotClient Server
        { get; set; }

        #endregion

        #region Game Loop Methods

        private void Awake()
        {
            Server = new GotClient(playerDataExtractor);
        }

        private void Start()
        {
            string connectionIp = useLocalHost ? GetLocalIpAddress() : ip;

            try
            {
                Server.ConnectToServerAndStartListening(connectionIp);
            }
            catch (Exception ex)
            {
                Debug.LogAssertionFormat("Error connecting to server {0} : {1}", connectionIp, ex.Message);
                Server.Dispose();
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F10))
                return;

            if (Server != null)
                Server.SendLogOutMessageAndDisconnect();

            Application.Quit();
        }

        private void FixedUpdate()
        {
            if (Server != null)
                Server.SendPlayerDataToServer();
        }

        private void OnApplicationQuit()
        {
            if (Server != null)
                Server.SendLogOutMessageAndDisconnect();
        }

        #endregion

        #region Other Methods

        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var localhost =
                host.AddressList.ToList().FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            if (localhost != null)
                return localhost.ToString();
            throw new Exception("Local IP Address Not Found!");
        }

        #endregion
    }
}
