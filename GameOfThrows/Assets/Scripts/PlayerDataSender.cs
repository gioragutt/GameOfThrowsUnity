using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using GotLib;

namespace Assets.Scripts
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerDataSender : MonoBehaviour
    {
        #region Public Members

        public string playerName;
        public string ip;
        public bool useLocalHost;

        #endregion

        #region Private Members

        private PlayerMovement playerData;
        private GotClient Server { get; set; }
        private string Name { get { return playerName; } }

        #endregion

        #region Game Loop Methods

        private void Awake()
        {
            playerData = GetComponent<PlayerMovement>();

            Server = new GotClient
            {
                playerData = playerData,
                name = Name
            };
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
            }
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.F10)) return;

            Server.SendLogOutMessageAndDisconnect();

            Application.Quit();
        }

        private void FixedUpdate()
        {
            Server.SendPlayerDataToServer();
        }

        private void OnApplicationQuit()
        {
            Server.SendLogOutMessageAndDisconnect();
        }

        #endregion

        #region Other Methods

        public static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var localhost = host.AddressList.ToList().FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            if (localhost != null)
                return localhost.ToString();
            throw new Exception("Local IP Address Not Found!");
        }

        #endregion
    }
}
