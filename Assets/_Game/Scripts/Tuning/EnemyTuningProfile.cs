using UnityEngine;
using Soulwake.Game.Core;

namespace Soulwake.Game.Tuning
{
    [CreateAssetMenu(
        fileName = "EnemyTuningProfile",
        menuName = "Soulwake/Tuning/Enemy Tuning Profile")]
    public class EnemyTuningProfile : ScriptableObject
    {
        [Header("Identity")]
        public string enemyId = VerticalSliceConventions.DefaultNormalEnemyId;
        public bool isUniqueEnemy;

        [Header("Stats")]
        [Min(1)] public int maxHp = 20;
        [Min(0)] public int attackDamage = 4;

        [Header("Movement")]
        [Min(0f)] public float moveSpeed = 2.2f;

        [Header("Detection")]
        [Min(0f)] public float aggroRange = 6f;
        [Min(0f)] public float loseInterestRange = 8f;
        [Min(0f)] public float attackDistancePadding = 0.05f;

        [Header("Attack")]
        [Min(0f)] public float attackRange = 0.9f;
        [Min(0.05f)] public float attackCooldown = 1f;
        [Min(1)] public int maxTargetsPerSwing = 1;

        public string EnemyId => enemyId;
        public bool IsUniqueEnemy => isUniqueEnemy;
        public int MaxHp => maxHp;
        public int AttackDamage => attackDamage;
        public float MoveSpeed => moveSpeed;
        public float AggroRange => aggroRange;
        public float LoseInterestRange => loseInterestRange;
        public float AttackDistancePadding => attackDistancePadding;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public int MaxTargetsPerSwing => maxTargetsPerSwing;
    }
}
