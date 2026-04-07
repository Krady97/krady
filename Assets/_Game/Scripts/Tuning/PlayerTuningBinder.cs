using UnityEngine;
using Soulwake.Game.Combat;
using Soulwake.Game.Player;
using Soulwake.Game.Stats;

namespace Soulwake.Game.Tuning
{
    /// <summary>
    /// Applies player tuning profile values to runtime components.
    /// Keeps balancing data-driven and centralized.
    /// </summary>
    public class PlayerTuningBinder : MonoBehaviour
    {
        [SerializeField] private PlayerTuningProfile profile;
        [SerializeField] private bool applyOnAwake = true;

        [Header("References")]
        [SerializeField] private PlayerMovementController movementController;
        [SerializeField] private PlayerMeleeAttack2D meleeAttack;
        [SerializeField] private PlayerStats playerStats;

        private void Reset()
        {
            movementController = GetComponent<PlayerMovementController>();
            meleeAttack = GetComponent<PlayerMeleeAttack2D>();
            playerStats = GetComponent<PlayerStats>();
        }

        private void Awake()
        {
            Reset();
            if (applyOnAwake)
            {
                ApplyProfile();
            }
        }

        [ContextMenu("Apply Profile")]
        public void ApplyProfile()
        {
            if (profile == null)
            {
                return;
            }

            if (movementController != null)
            {
                movementController.MoveSpeed = profile.MoveSpeed;
            }

            if (playerStats != null)
            {
                playerStats.ConfigureBaseStats(profile.MaxHP, profile.AttackDamage, true);
            }

            if (meleeAttack != null)
            {
                meleeAttack.ConfigureAttack(
                    profile.AttackRange,
                    profile.UsePlayerStatDamage,
                    profile.BaseAttackDamage,
                    profile.BonusAttackDamage,
                    profile.AttackCooldown);
            }
        }
    }
}
