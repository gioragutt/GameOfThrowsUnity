using UnityEngine;

namespace Assets.Scripts
{
    public class Loader : MonoBehaviour
    {
        public bool enabledOnStart = true;
        public GameObject gameManager;

        // Use this for initialization
        void Awake()
        {
            if (!enabledOnStart) return;
            if (GameManager.instance == null)
                Instantiate(gameManager);
        }
    }
}
