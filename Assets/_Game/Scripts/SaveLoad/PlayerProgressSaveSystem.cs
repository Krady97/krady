using System;
using System.IO;
using UnityEngine;
using Soulwake.Game.Inventory;
using Soulwake.Game.Skills;
using Soulwake.Game.Stats;

namespace Soulwake.Game.SaveLoad
{
    /// <summary>
    /// Minimal JSON save/load for vertical slice player progression.
    /// Saves to Application.persistentDataPath.
    /// </summary>
    public static class PlayerProgressSaveSystem
    {
        private const string FileName = "soulwake_player_progress.json";

        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static bool TrySave(PlayerStats stats, PlayerGoldWallet wallet, PlayerInventory inventory, PlayerSkillBook skillBook, out string error)
        {
            error = string.Empty;
            if (stats == null || wallet == null || inventory == null || skillBook == null)
            {
                error = "Save failed: one or more player components are missing.";
                return false;
            }

            try
            {
                PlayerProgressSaveData data = new PlayerProgressSaveData(
                    stats.MaxHP,
                    stats.AttackDamage,
                    stats.CurrentHP,
                    wallet.CurrentGold,
                    inventory.ItemIds,
                    skillBook.LearnedSkillIds);

                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
                return true;
            }
            catch (Exception ex)
            {
                error = $"Save exception: {ex.Message}";
                return false;
            }
        }

        public static bool TryLoad(out PlayerProgressSaveData data, out string error)
        {
            error = string.Empty;
            data = new PlayerProgressSaveData();

            try
            {
                if (!File.Exists(SavePath))
                {
                    error = "Save file does not exist.";
                    return false;
                }

                string json = File.ReadAllText(SavePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    error = "Save file is empty.";
                    return false;
                }

                PlayerProgressSaveData parsed = JsonUtility.FromJson<PlayerProgressSaveData>(json);
                if (parsed == null)
                {
                    error = "Failed to parse save JSON.";
                    return false;
                }

                data = parsed;
                return true;
            }
            catch (Exception ex)
            {
                error = $"Load exception: {ex.Message}";
                return false;
            }
        }

        public static bool TryApplyLoadedData(
            PlayerProgressSaveData data,
            PlayerStats stats,
            PlayerGoldWallet wallet,
            PlayerInventory inventory,
            PlayerSkillBook skillBook,
            out string error)
        {
            error = string.Empty;
            if (data == null || stats == null || wallet == null || inventory == null || skillBook == null)
            {
                error = "Apply failed: save data or player components are missing.";
                return false;
            }

            stats.ApplyLoadedState(data.maxHp, data.attackDamage, data.currentHp);
            wallet.SetGoldDirect(data.gold);
            inventory.ReplaceItems(data.inventoryItemIds);
            skillBook.ReplaceLearnedSkills(data.learnedSkillIds);
            return true;
        }
    }
}
