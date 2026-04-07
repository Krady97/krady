using UnityEngine;
using Soulwake.Game.Player;
using Soulwake.Game.Stats;

namespace Soulwake.Game.Combat
{
    /// <summary>
    /// Simple radial melee attack for the player.
    /// Uses overlap checks against a target layer and applies damage
    /// to objects that expose Damageable2D.
    /// </summary>
    public class PlayerMeleeAttack2D : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerInputReader inputReader;
        [SerializeField] private PlayerMovementController movementController;
        [SerializeField] private PlayerStats playerStats;

        [Header("Attack")]
        [SerializeField] private float attackRange = 1.0f;
        [SerializeField] private bool usePlayerStatDamage = true;
        [SerializeField] private int baseAttackDamage = 3;
        [SerializeField] private int bonusAttackDamage;
        [SerializeField] private float attackCooldown = 0.35f;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private int maxTargetsPerSwing = 8;

        private readonly Collider2D[] hitBuffer = new Collider2D[32];
        private float nextAttackTime;

        public float AttackRange => attackRange;
        public int AttackDamage => ResolveAttackDamage();
        public float AttackCooldown => attackCooldown;

        private void Reset()
        {
            inputReader = GetComponent<PlayerInputReader>();
            movementController = GetComponent<PlayerMovementController>();
            playerStats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (!CanAttack())
            {
                return;
            }

            if (!inputReader.AttackPressedThisFrame)
            {
                return;
            }

            PerformAttack();
        }

        private bool CanAttack()
        {
            return inputReader != null
                && movementController != null
                && playerStats != null
                && Time.time >= nextAttackTime
                && playerStats.CurrentHP > 0;
        }

        private void PerformAttack()
        {
            nextAttackTime = Time.time + attackCooldown;

            Vector2 attackOrigin = (Vector2)transform.position + movementController.FacingDirection * attackRange;
            int hitCount = Physics2D.OverlapCircleNonAlloc(
                attackOrigin,
                attackRange,
                hitBuffer,
                targetLayers);

            int processed = 0;
            for (int i = 0; i < hitCount; i++)
            {
                if (processed >= maxTargetsPerSwing)
                {
                    break;
                }

                Collider2D hit = hitBuffer[i];
                if (hit == null)
                {
                    continue;
                }

                Damageable2D damageable = hit.GetComponentInParent<Damageable2D>();
                if (damageable == null)
                {
                    continue;
                }

                int finalDamage = ResolveAttackDamage();
                damageable.ApplyDamage(finalDamage);
                processed++;
            }
        }

        private int ResolveAttackDamage()
        {
            if (usePlayerStatDamage)
            {
                return Mathf.Max(1, playerStats.AttackDamage + Mathf.Max(0, bonusAttackDamage));
            }

            return Mathf.Max(1, baseAttackDamage + Mathf.Max(0, bonusAttackDamage));
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 facing = Vector2.down;
            if (movementController != null)
            {
                facing = movementController.FacingDirection;
            }

            if (facing.sqrMagnitude < 0.001f)
            {
                facing = Vector2.down;
            }

            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.75f);
            Vector2 attackOrigin = (Vector2)transform.position + facing * attackRange;
            Gizmos.DrawWireSphere(attackOrigin, attackRange);
        }
    }
}
