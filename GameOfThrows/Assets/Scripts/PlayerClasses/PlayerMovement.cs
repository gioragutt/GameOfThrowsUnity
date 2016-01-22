using System.Text;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable UnusedMember.Local

namespace Assets.Scripts.PlayerClasses
{
    public enum Directions
    {
        Back,
        Left,
        Front,
        Right,
        Idle = -1
    }

    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public abstract class PlayerMovement : MonoBehaviour
    {
        #region Public Members

        public float speed;
        public Text debugText;
        public float debugSpeedBoost = 5f;

        public Vector2 Position
        {
            get { return rb2D.position; }
        }

        #endregion

        #region Constants

        /// <summary>
        /// Constant for decaying the velocity on updates
        /// </summary>
        private const float VELOCITY_DECAY_FACTOR = 0.85f;

        /// <summary>
        /// Constant to convert normal speed sizes to fit the scale
        /// Of UnityEngine.Vector2
        /// </summary>
        private const float HUMAN_TO_VECTOR_SCALE_FACTOR = 850f;

        /// <summary>
        /// Constant to set the base speed of the player animation
        /// </summary>
        private const float BASE_ANIM_SPEED = 0.7f;

        /// <summary>
        /// Constant to slightly reduce the animation speed after 
        /// It is multiplied by the velocity of the player
        /// </summary>
        private const float POST_VELOCITY_MULTIPLICATION_ANIM_SPEED_FACTOR = 0.5f;

        /// <summary>
        /// Constant to set the animation speed
        /// </summary>
        private const float ANIM_SPEED_MODIFIER = BASE_ANIM_SPEED * POST_VELOCITY_MULTIPLICATION_ANIM_SPEED_FACTOR;

        /// <summary>
        /// Epsilon before velocity zerofication
        /// </summary>
        private const float VELOCITY_EPSILON = 0.1f;

        #endregion

        #region Protected Members

        protected abstract bool IsLocal { get; }

        #endregion

        #region Private Members

        private Rigidbody2D rb2D;
        private Animator animator;
        private Vector2 velocity;
        private Vector2 input;
        private Directions direction;

        #endregion

        #region Game Loop Methods

        private void Awake()
        {
            animator = GetComponent<Animator>();
            rb2D = GetComponent<Rigidbody2D>();
            input = Vector2.zero;
        }

        private void Update()
        {
            UpdateInput();
        }

        private void FixedUpdate()
        {
            UpdateVelocity();
            UpdateAnimation();
            UpdateMovement();
        }

        #endregion

        #region Input Methods

        abstract protected void UpdateInput();

        protected void UpdateInput(float horizontal, float vertical)
        {
            input.Set(horizontal, vertical);
        }

        #endregion

        #region Animation Methods

        protected void UpdateAnimation()
        {
            if (input.y > 0)
                direction = Directions.Back;
            else if (input.y < 0)
                direction = Directions.Front;
            else if (input.x > 0)
                direction = Directions.Right;
            else if (input.x < 0)
                direction = Directions.Left;
            else
                direction = Directions.Idle;

            SetDirection(direction);
        }

        protected void SetDirection(Directions value)
        {
            animator.SetInteger("Direction", (int)value);
        }

        #endregion

        #region DEBUG

        private StringBuilder GetDebugPrintDetails()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("(HOR : {0})\n(VER : {1})\n(DIR : {2}/{3})\n(X : {4})\n(Y : {5})",
                velocity.x,
                velocity.y,
                ((int)direction).ToString().PadLeft(2),
                direction,
                rb2D.position.x,
                rb2D.position.y);
            return builder;
        }

        private void KinematicsDebugPrints()
        {
            var details = GetDebugPrintDetails();
            if (debugText != null && IsLocal)
                debugText.text = new StringBuilder(details.ToString()).Replace("(", "").Replace(")", "").ToString();
            //Debug.Log(details.Replace('\n', ' ').ToString());
        }

        #endregion

        #region Movement Methods

        private void UpdateMovement()
        {
            rb2D.MovePosition(rb2D.position + velocity * Time.fixedDeltaTime);
            KinematicsDebugPrints();
            ApplySpeedDecay();
        }

        private void UpdateVelocity()
        {
            UpdateVelocity(input.x, input.y);
        }

        private float SpeedBoost()
        {
            if (!IsLocal) return 1f;

            return Input.GetKey(KeyCode.LeftControl) ? debugSpeedBoost : 1f;
        }

        private static float HumanToVectorSpeed(float humanScaleSpeed)
        {
            return humanScaleSpeed / HUMAN_TO_VECTOR_SCALE_FACTOR;
        }

        private void UpdateVelocity(float horizontal, float vertical)
        {
            if (vertical != 0)
            {
                velocity.y += Mathf.Sign(vertical) * HumanToVectorSpeed(speed) * SpeedBoost();
            }
            if (horizontal != 0)
            {
                velocity.x += Mathf.Sign(horizontal) * HumanToVectorSpeed(speed) * SpeedBoost();
            }

            animator.speed = ANIM_SPEED_MODIFIER * velocity.GetDimensionByDirection(direction);
        }

        private void ApplySpeedDecay()
        {
            if (velocity == Vector2.zero) return;

            velocity *= VELOCITY_DECAY_FACTOR;
            velocity = velocity.ZerofiyTinyValues(VELOCITY_EPSILON);
        }

        #endregion
    }
}
