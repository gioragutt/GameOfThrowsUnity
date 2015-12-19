using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public Transform player;
        public Vector2 margin, smoothing;

        public bool IsFollowing { get; set; }

        public void Start()
        {
            IsFollowing = true;
        }

        public void Update()
        {
            var x = transform.position.x;
            var y = transform.position.y;

            if (IsFollowing)
            {
                if (Mathf.Abs(x - player.position.x) > margin.x)
                    x = Mathf.Lerp(x, player.position.x, smoothing.x * Time.deltaTime);

                if (Mathf.Abs(y - player.position.y) > margin.y)
                    y = Mathf.Lerp(y, player.position.y, smoothing.y * Time.deltaTime);
            }

            transform.position = new Vector3(x, y, transform.position.z);
        }
    }
}
