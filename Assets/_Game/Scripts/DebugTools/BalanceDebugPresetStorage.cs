using System;
using System.IO;
using UnityEngine;

namespace Soulwake.Game.DebugTools
{
    /// <summary>
    /// JSON persistence for runtime balance debug multipliers.
    /// </summary>
    public static class BalanceDebugPresetStorage
    {
        private const string FileName = "soulwake_balance_debug_preset.json";
        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        public static bool TrySave(BalanceDebugPresetData data, out string error)
        {
            error = string.Empty;
            try
            {
                if (data == null)
                {
                    error = "Preset data is null.";
                    return false;
                }

                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(SavePath, json);
                return true;
            }
            catch (Exception ex)
            {
                error = $"Save preset exception: {ex.Message}";
                return false;
            }
        }

        public static bool TryLoad(out BalanceDebugPresetData data, out string error)
        {
            error = string.Empty;
            data = null;
            try
            {
                if (!File.Exists(SavePath))
                {
                    error = "No preset file found.";
                    return false;
                }

                string json = File.ReadAllText(SavePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    error = "Preset file is empty.";
                    return false;
                }

                BalanceDebugPresetData parsed = JsonUtility.FromJson<BalanceDebugPresetData>(json);
                if (parsed == null)
                {
                    error = "Failed to parse preset JSON.";
                    return false;
                }

                data = parsed;
                return true;
            }
            catch (Exception ex)
            {
                error = $"Load preset exception: {ex.Message}";
                return false;
            }
        }
    }
}
