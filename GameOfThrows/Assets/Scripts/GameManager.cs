using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public BoardManager boardScript;
        public static GameManager instance;
        public int extraWalls = 5;

        void InitGame()
        {
            boardScript.SetUpScene(extraWalls);
        }

        #region Game Loop Functions

        void Start()
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
    }
}
