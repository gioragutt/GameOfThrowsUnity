using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public BoardManager boardScript;
        public static GameManager instance = null;

        void InitGame()
        {
            boardScript.SetUpScene(5);
        }

        #region Game Loop Functions

        void Awake()
        {
            if (instance == null)
                instance = this;
            if (instance != this)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
            boardScript = GetComponent<BoardManager>();
            InitGame();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        
        #endregion
    }
}
