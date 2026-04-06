using UnityEngine;
using Soulwake.Game.Combat;

namespace Soulwake.Game.Enemy
{
    /// <summary>
    /// Handles 2D chase movement for enemies.
    /// Does not contain decision logic; controller decides when to move/stop.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 2.2f;

        private Rigidbody2D rb;
        private Damageable2D damageable;

        public float MoveSpeed => moveSpeed;
        public Vector2 LastMoveDirection { get; private set; } = Vector2.down;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            damageable = GetComponent<Damageable2D>();
        }

        public void MoveTowards(Vector2 worldTarget)
        {
            if (damageable != null && damageable.IsDead)
            {
                Stop();
                return;
            }

            Vector2 current = rb.position;
            Vector2 delta = worldTarget - current;
            if (delta.sqrMagnitude <= 0.0001f)
            {
                Stop();
                return;
            }

            Vector2 direction = delta.normalized;
            rb.velocity = direction * moveSpeed;
            LastMoveDirection = direction;
        }

        public void Stop()
        {
            rb.velocity = Vector2.zero;
        }
    }
}
