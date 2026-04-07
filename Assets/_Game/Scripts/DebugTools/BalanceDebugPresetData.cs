using System;

namespace Soulwake.Game.DebugTools
{
    [Serializable]
    public class BalanceDebugPresetData
    {
        public float playerHpMultiplier = 1f;
        public float enemyHpMultiplier = 1f;
        public float playerDamageMultiplier = 1f;
        public float enemyDamageMultiplier = 1f;
        public float playerMoveSpeedMultiplier = 1f;
        public float enemyMoveSpeedMultiplier = 1f;

        public BalanceDebugPresetData()
        {
        }

        public BalanceDebugPresetData(
            float playerHp,
            float enemyHp,
            float playerDamage,
            float enemyDamage,
            float playerMoveSpeed,
            float enemyMoveSpeed)
        {
            playerHpMultiplier = playerHp;
            enemyHpMultiplier = enemyHp;
            playerDamageMultiplier = playerDamage;
            enemyDamageMultiplier = enemyDamage;
            playerMoveSpeedMultiplier = playerMoveSpeed;
            enemyMoveSpeedMultiplier = enemyMoveSpeed;
        }
    }
}
