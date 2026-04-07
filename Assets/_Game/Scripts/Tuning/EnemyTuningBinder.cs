using UnityEngine;
using Soulwake.Game.Enemy;
using Soulwake.Game.Stats;

namespace Soulwake.Game.Tuning
{
    /// <summary>
    /// Applies an enemy tuning profile to an enemy prefab/instance.
    /// Keeps balancing values centralized in ScriptableObjects.
    /// </summary>
    public class EnemyTuningBinder : MonoBehaviour
    {
        [SerializeField] private EnemyTuningProfile tuningProfile;
        [SerializeField] private bool applyOnAwake = true;

        [Header("Targets (auto-detected if empty)")]
        [SerializeField] private PlayerStats stats;
        [SerializeField] private EnemyMovement2D movement;
        [SerializeField] private EnemyMeleeAttack2D meleeAttack;
        [SerializeField] private EnemyController2D controller;

        private void Reset()
        {
            ResolveReferences();
        }

        private void Awake()
        {
            ResolveReferences();
            if (applyOnAwake)
            {
                Apply();
            }
        }

        [ContextMenu("Apply Enemy Tuning")]
        public void Apply()
        {
            if (tuningProfile == null)
            {
                return;
            }

            if (stats != null)
            {
                stats.ConfigureBaseStats(tuningProfile.MaxHp, tuningProfile.AttackDamage, true);
            }

            movement?.SetMoveSpeed(tuningProfile.MoveSpeed);
            meleeAttack?.SetAttackTuning(
                tuningProfile.AttackRange,
                tuningProfile.AttackDamage,
                tuningProfile.AttackCooldown);
            meleeAttack?.SetMaxTargetsPerSwing(tuningProfile.MaxTargetsPerSwing);
            if (controller != null)
            {
                controller.SetDetectionTuning(
                    tuningProfile.AggroRange,
                    tuningProfile.LoseInterestRange,
                    tuningProfile.AttackDistancePadding);
            }
            EnemyDeathNotifier deathNotifier = GetComponent<EnemyDeathNotifier>();
            if (deathNotifier != null)
            {
                deathNotifier.ConfigureIdentity(tuningProfile.EnemyId, tuningProfile.IsUniqueEnemy);
            }
        }

        private void ResolveReferences()
        {
            if (stats == null)
            {
                stats = GetComponent<PlayerStats>();
            }

            if (movement == null)
            {
                movement = GetComponent<EnemyMovement2D>();
            }

            if (meleeAttack == null)
            {
                meleeAttack = GetComponent<EnemyMeleeAttack2D>();
            }

            if (controller == null)
            {
                controller = GetComponent<EnemyController2D>();
            }
        }
    }
