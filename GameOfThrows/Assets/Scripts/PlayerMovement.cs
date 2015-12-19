using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public enum Directions
    {
        Back,
        Left,
        Front,
        Right,
        Idle = -1
    }

    public class PlayerMovement : MonoBehaviour
    {
        #region Public Members

        public float speed;
        public Text debugText;

        #endregion

        #region Constants

        private const float DECAY_FACTOR = 0.85f;
        private const float SPEED_FACTOR = 850f;
        private const float BASE_ANIM_SPEED = 0.7f;

        #endregion

        #region Private Members

        private Rigidbody2D rb2D;
        private Vector2 velocity;
        private Animator animator;

        #endregion

        #region Game Loop Methods

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb2D = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float horizontal = Input.GetAxisRaw("Horizontal");
            UpdateVelocity(vertical, horizontal);
            UpdateAnimation();
            UpdateMovment();
        }

        #endregion

        #region Animation Methods

        private void UpdateAnimation()
        {
            Directions direction;

            if (velocity.y > 0)
                direction = Directions.Back;
            else if (velocity.y < 0)
                direction = Directions.Front;
            else if (velocity.x > 0)
                direction = Directions.Right;
            else if (velocity.x < 0)
                direction = Directions.Left;
            else
                direction = Directions.Idle;

            SetDirection(direction);
        }

        private void SetDirection(Directions value)
        {
            animator.SetInteger("Direction", (int)value);
        }

        #endregion

        #region Movement Methods

        private void UpdateMovment()
        {
            debugText.text = string.Format("HOR - {0}\nVER - {1}\nDIR - {2}:{3}", velocity.x, velocity.y,
                animator.GetInteger("Direction").ToString().PadLeft(2), (Directions)animator.GetInteger("Direction"));
            rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime);
            //transform.Translate(velocity.x, velocity.y, 0f, transform);
            ApplySpeedDecay();
        }

        private void UpdateVelocity(float vertical, float horizontal)
        {
            if (vertical != 0)
                velocity.y += Mathf.Sign(vertical) * speed / SPEED_FACTOR;
            if (horizontal != 0)
                velocity.x += Mathf.Sign(horizontal) * speed / SPEED_FACTOR;
            animator.speed = BASE_ANIM_SPEED * (Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y)) * 0.5f);
        }

        private void ApplySpeedDecay()
        {
            // Apply speed decay
            velocity.x *= DECAY_FACTOR;
            velocity.y *= DECAY_FACTOR;

            // Zerofy tiny velocities
            const float EPSILON = 0.1f;

            if (Mathf.Abs(velocity.x) < EPSILON)
                velocity.x = 0;
            if (Mathf.Abs(velocity.y) < EPSILON)
                velocity.y = 0;
        }

        #endregion
    }
}
