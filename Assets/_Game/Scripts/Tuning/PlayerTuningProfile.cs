using UnityEngine;

namespace Soulwake.Game.Tuning
{
    /// <summary>
    /// Player combat/movement baseline values for quick balancing.
    /// </summary>
    [CreateAssetMenu(
        fileName = "PlayerTuningProfile",
        menuName = "Soulwake/Tuning/Player Tuning Profile")]
    public class PlayerTuningProfile : ScriptableObject
    {
        [Header("Movement")]
        public float moveSpeed = 3.5f;

        [Header("Base Stats")]
        public int maxHp = 20;
        public int attackDamage = 5;

        [Header("Melee Attack")]
        public float attackRange = 1.0f;
        public bool usePlayerStatDamage = true;
        public int baseAttackDamage = 3;
        public int bonusAttackDamage = 0;
        public float attackCooldown = 0.35f;

        public float MoveSpeed => moveSpeed;
        public int MaxHP => maxHp;
        public int AttackDamage => attackDamage;
        public float AttackRange => attackRange;
        public bool UsePlayerStatDamage => usePlayerStatDamage;
        public int BaseAttackDamage => baseAttackDamage;
        public int BonusAttackDamage => bonusAttackDamage;
        public float AttackCooldown => attackCooldown;
    }
}
