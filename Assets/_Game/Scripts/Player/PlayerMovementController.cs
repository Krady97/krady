using UnityEngine;

namespace Soulwake.Game.Player
{
    /// <summary>
    /// Handles quarter-view 2D player movement using Rigidbody2D.
    /// Exposes the last non-zero move direction for combat/interactions.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3.5f;

        private Rigidbody2D rb;
        private PlayerInputReader input;
        private Vector2 moveInput;

        public Vector2 CurrentMoveInput => moveInput;
        public Vector2 FacingDirection { get; private set; } = Vector2.down;

        public float MoveSpeed
        {
            get => moveSpeed;
            set => moveSpeed = Mathf.Max(0.1f, value);
        }

        public void SetMoveSpeed(float speed)
        {
            MoveSpeed = speed;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            input = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            moveInput = input.Move;

            if (moveInput.sqrMagnitude > 0.001f)
            {
                FacingDirection = moveInput;
            }
        }

        private void FixedUpdate()
        {
            rb.velocity = moveInput * moveSpeed;
        }
    }
}
