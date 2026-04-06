using UnityEngine;

namespace Soulwake.Game.Enemy
{
    /// <summary>
    /// Performs short-range melee attacks for enemies.
    /// Keeps attack logic isolated from AI decision flow.
    /// </summary>
    public class EnemyMeleeAttack2D : MonoBehaviour
    {
        [Header("Attack")]
        [SerializeField] private float attackRange = 0.9f;
        [SerializeField] private int attackDamage = 4;
        [SerializeField] private float attackCooldown = 1.0f;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private int maxTargetsPerSwing = 1;

        private readonly Collider2D[] hitBuffer = new Collider2D[16];
        private float nextAttackTime;
        private Vector2 facingDirection = Vector2.down;

        public float AttackRange => attackRange;
        public int AttackDamage => attackDamage;
        public float AttackCooldown => attackCooldown;

        public bool IsReady => Time.time >= nextAttackTime;

        public bool TryAttack(Vector2 direction)
        {
            if (!IsReady)
            {
                return false;
            }

            if (direction.sqrMagnitude > 0.0001f)
            {
                facingDirection = direction.normalized;
            }

            nextAttackTime = Time.time + attackCooldown;
            Vector2 attackOrigin = (Vector2)transform.position + facingDirection * attackRange;

            int hitCount = Physics2D.OverlapCircleNonAlloc(
                attackOrigin,
                attackRange,
                hitBuffer,
                targetLayers);

            int applied = 0;
            for (int i = 0; i < hitCount; i++)
            {
                if (applied >= maxTargetsPerSwing)
                {
                    break;
                }

                Collider2D hit = hitBuffer[i];
                if (hit == null)
                {
                    continue;
                }

                Soulwake.Game.Combat.IDamageable2D damageable =
                    hit.GetComponentInParent<Soulwake.Game.Combat.IDamageable2D>();
                if (damageable == null || damageable.IsDead)
                {
                    continue;
                }

                damageable.ApplyDamage(attackDamage);
                applied++;
            }

            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.95f, 0.55f, 0.1f, 0.75f);
            Vector2 origin = (Vector2)transform.position + facingDirection * attackRange;
            Gizmos.DrawWireSphere(origin, attackRange);
        }
    }
}
