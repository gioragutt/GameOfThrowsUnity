using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        #region Public Members

        public Transform player;
        public Vector2 margin, smoothing;
        public bool isFollowing;
        public bool isBound;

        #endregion

        #region Private Members

        private Vector2 min, max;
        private float aspect;
        private float maxSize;
        private BoardManager boardManager;

        #endregion

        #region Game Loop Methods

        public void Awake()
        {
            boardManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>().boardScript;
            isFollowing = true;
            isBound = true;
            aspect = Camera.main.aspect;

            min = new Vector2(BoardManager.BORDER_COL, BoardManager.BORDER_COL);
            max = new Vector2(boardManager.rows, boardManager.columns);
            maxSize = max.x <= max.y ? max.x / 2f / aspect : max.y / 2f;
        }

        public void Update()
        {
            UpdateCameraSize();
            UpdateCameraPosition();
        }

        #endregion

        private void UpdateCameraPosition()
        {
            var x = transform.position.x;
            var y = transform.position.y;

            if (isFollowing) FollowPlayer(ref x, ref y);
            if (isBound) BoundToMap(ref x, ref y);

            transform.position = new Vector3(x, y, transform.position.z);
        }

        private void BoundToMap(ref float x, ref float y)
        {
            const float FIX = 0.5f;
            float orthSize = Camera.main.orthographicSize;

            if (x <= (min.x + orthSize * aspect - FIX))
                x = min.x + orthSize * aspect - FIX;
            else if (x >= (max.x - orthSize * aspect + FIX))
                x = max.x - orthSize * aspect + FIX;

            if (y <= (min.y + orthSize - FIX))
                y = min.y + orthSize - FIX;
            else if (y >= (max.y - orthSize + FIX))
                y = max.y - orthSize + FIX;
        }

        private void FollowPlayer(ref float x, ref float y)
        {
            if (Mathf.Abs(x - player.position.x) > margin.x)
                x = Mathf.Lerp(x, player.position.x, smoothing.x * Time.deltaTime);

            if (Mathf.Abs(y - player.position.y) > margin.y)
                y = Mathf.Lerp(y, player.position.y, smoothing.y * Time.deltaTime);
        }


        private void UpdateCameraSize()
        {
            float size = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.orthographicSize += size;

            if (Camera.main.orthographicSize > maxSize)
                Camera.main.orthographicSize = maxSize;
            
        }
    }
}
