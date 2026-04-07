using System;
using UnityEngine;

namespace Soulwake.Game.DebugTools
{
    /// <summary>
    /// Runtime multipliers used by debug balancing tools.
    /// </summary>
    public static class BalanceDebugRuntime
    {
        private const float MinMultiplier = 0.25f;
        private const float MaxMultiplier = 3.0f;
        private static float playerDamageMultiplier = 1f;
        private static float enemyDamageMultiplier = 1f;
        private static float playerMoveSpeedMultiplier = 1f;
        private static float enemyMoveSpeedMultiplier = 1f;
        private static float playerHpMultiplier = 1f;
        private static float enemyHpMultiplier = 1f;

        public static event Action MultipliersChanged;

        public static float PlayerDamageMultiplier => playerDamageMultiplier;
        public static float EnemyDamageMultiplier => enemyDamageMultiplier;
        public static float PlayerMoveSpeedMultiplier => playerMoveSpeedMultiplier;
        public static float EnemyMoveSpeedMultiplier => enemyMoveSpeedMultiplier;
        public static float PlayerHpMultiplier => playerHpMultiplier;
        public static float EnemyHpMultiplier => enemyHpMultiplier;

        public static BalanceDebugPresetData CreateSnapshot()
        {
            return new BalanceDebugPresetData(
                playerHpMultiplier,
                enemyHpMultiplier,
                playerDamageMultiplier,
                enemyDamageMultiplier,
                playerMoveSpeedMultiplier,
                enemyMoveSpeedMultiplier);
        }

        public static void ApplySnapshot(BalanceDebugPresetData preset, bool notify = true)
        {
            if (preset == null)
            {
                return;
            }

            bool changed = false;
            changed |= SetInternal(ref playerHpMultiplier, preset.playerHpMultiplier);
            changed |= SetInternal(ref enemyHpMultiplier, preset.enemyHpMultiplier);
            changed |= SetInternal(ref playerDamageMultiplier, preset.playerDamageMultiplier);
            changed |= SetInternal(ref enemyDamageMultiplier, preset.enemyDamageMultiplier);
            changed |= SetInternal(ref playerMoveSpeedMultiplier, preset.playerMoveSpeedMultiplier);
            changed |= SetInternal(ref enemyMoveSpeedMultiplier, preset.enemyMoveSpeedMultiplier);

            if (changed && notify)
            {
                MultipliersChanged?.Invoke();
            }
        }

        // Backward-compatible aliases.
        public static BalanceDebugPresetData ToPreset() => CreateSnapshot();
        public static void ApplyPreset(BalanceDebugPresetData preset) => ApplySnapshot(preset, true);

        public static void SetPlayerDamageMultiplier(float value) => Set(ref playerDamageMultiplier, value);
        public static void SetEnemyDamageMultiplier(float value) => Set(ref enemyDamageMultiplier, value);
        public static void SetPlayerMoveSpeedMultiplier(float value) => Set(ref playerMoveSpeedMultiplier, value);
        public static void SetEnemyMoveSpeedMultiplier(float value) => Set(ref enemyMoveSpeedMultiplier, value);
        public static void SetPlayerHpMultiplier(float value) => Set(ref playerHpMultiplier, value);
        public static void SetEnemyHpMultiplier(float value) => Set(ref enemyHpMultiplier, value);

        public static void ResetAll()
        {
            bool changed = false;
            changed |= SetInternal(ref playerDamageMultiplier, 1f);
            changed |= SetInternal(ref enemyDamageMultiplier, 1f);
            changed |= SetInternal(ref playerMoveSpeedMultiplier, 1f);
            changed |= SetInternal(ref enemyMoveSpeedMultiplier, 1f);
            changed |= SetInternal(ref playerHpMultiplier, 1f);
            changed |= SetInternal(ref enemyHpMultiplier, 1f);
            if (changed)
            {
                MultipliersChanged?.Invoke();
            }
        }

        private static void Set(ref float field, float value)
        {
            if (SetInternal(ref field, value))
            {
                MultipliersChanged?.Invoke();
            }
        }

        private static bool SetInternal(ref float field, float value)
        {
            float clamped = Mathf.Clamp(value, MinMultiplier, MaxMultiplier);
            if (Mathf.Abs(field - clamped) < 0.0001f)
            {
                return false;
            }

            field = clamped;
            return true;
        }
    }
}
