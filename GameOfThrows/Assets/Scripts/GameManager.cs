using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public BoardManager boardScript;
        public static GameManager instance;
        public int extraWalls = 5;

        private void InitGame()
        {
            boardScript.SetUpScene(extraWalls);
        }

        #region Game Loop Functions

        private void Start()
        {
            if (instance == null)
                instance = this;
            if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            boardScript = GetComponent<BoardManager>();
            InitGame();
        }

        #endregion

        #region Other Methods

        public static string GetLocalIpAddress()
        {
            return Dns.GetHostEntry("localhost").AddressList[0].ToString();
        }

        #endregion
    }
}
