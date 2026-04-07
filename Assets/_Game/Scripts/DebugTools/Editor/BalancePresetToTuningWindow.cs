using System.IO;
using UnityEditor;
using UnityEngine;
using Soulwake.Game.Tuning;

namespace Soulwake.Game.DebugTools.Editor
{
    /// <summary>
    /// Editor helper that applies debug multipliers to tuning assets.
    /// This is intended for quickly promoting playtest results.
    /// </summary>
    public class BalancePresetToTuningWindow : EditorWindow
    {
        private PlayerTuningProfile playerProfile;
        private EnemyTuningProfile normalEnemyProfile;
        private EnemyTuningProfile uniqueEnemyProfile;
        private EnemyTuningProfile wraithEnemyProfile;

        private BalanceDebugPresetData sourceData = new BalanceDebugPresetData();
        private string sourceLabel = "Manual / defaults";
        private Vector2 scroll;

        [MenuItem("Soulwake/Tools/Promote Balance Preset To Tuning")]
        public static void Open()
        {
            BalancePresetToTuningWindow window = GetWindow<BalancePresetToTuningWindow>();
            window.titleContent = new GUIContent("Balance -> Tuning");
            window.minSize = new Vector2(520f, 520f);
            window.Show();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            GUILayout.Space(8f);

            EditorGUILayout.LabelField("Soulwake Balance Promotion", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Promotes debug multipliers into tuning assets by multiplying current values.\n" +
                "Tip: duplicate your tuning assets before applying if you want a safe rollback.",
                MessageType.Info);

            GUILayout.Space(6f);
            DrawSourceControls();

            GUILayout.Space(10f);
            DrawSourcePreview();

            GUILayout.Space(10f);
            DrawTargetProfiles();

            GUILayout.Space(12f);
            DrawApplyButtons();

            EditorGUILayout.EndScrollView();
        }

        private void DrawSourceControls()
        {
            EditorGUILayout.LabelField("1) Multiplier Source", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current source:", sourceLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Load Saved Preset JSON"))
                {
                    LoadFromPresetFile();
                }

                if (GUILayout.Button("Read Runtime Multipliers"))
                {
                    ReadFromRuntime();
                }
            }

            if (GUILayout.Button("Reset Source To 1.00"))
            {
                sourceData = new BalanceDebugPresetData();
                sourceLabel = "Manual / defaults (1.00)";
            }
        }

        private void DrawSourcePreview()
        {
            EditorGUILayout.LabelField("2) Source Multipliers", EditorStyles.boldLabel);
            sourceData.playerHpMultiplier = EditorGUILayout.FloatField("Player HP", sourceData.playerHpMultiplier);
            sourceData.enemyHpMultiplier = EditorGUILayout.FloatField("Enemy HP", sourceData.enemyHpMultiplier);
            sourceData.playerDamageMultiplier = EditorGUILayout.FloatField("Player Damage", sourceData.playerDamageMultiplier);
            sourceData.enemyDamageMultiplier = EditorGUILayout.FloatField("Enemy Damage", sourceData.enemyDamageMultiplier);
            sourceData.playerMoveSpeedMultiplier = EditorGUILayout.FloatField("Player Speed", sourceData.playerMoveSpeedMultiplier);
            sourceData.enemyMoveSpeedMultiplier = EditorGUILayout.FloatField("Enemy Speed", sourceData.enemyMoveSpeedMultiplier);
        }

        private void DrawTargetProfiles()
        {
            EditorGUILayout.LabelField("3) Target Profiles", EditorStyles.boldLabel);
            playerProfile = (PlayerTuningProfile)EditorGUILayout.ObjectField("Player Profile", playerProfile, typeof(PlayerTuningProfile), false);
            normalEnemyProfile = (EnemyTuningProfile)EditorGUILayout.ObjectField("Normal Enemy Profile", normalEnemyProfile, typeof(EnemyTuningProfile), false);
            uniqueEnemyProfile = (EnemyTuningProfile)EditorGUILayout.ObjectField("Unique Enemy Profile", uniqueEnemyProfile, typeof(EnemyTuningProfile), false);
            wraithEnemyProfile = (EnemyTuningProfile)EditorGUILayout.ObjectField("Wraith Enemy Profile", wraithEnemyProfile, typeof(EnemyTuningProfile), false);
        }

        private void DrawApplyButtons()
        {
            EditorGUILayout.LabelField("4) Apply", EditorStyles.boldLabel);

            if (GUILayout.Button("Apply Multipliers To Selected Profiles"))
            {
                int changed = 0;
                changed += ApplyToPlayer(playerProfile) ? 1 : 0;
                changed += ApplyToEnemy(normalEnemyProfile) ? 1 : 0;
                changed += ApplyToEnemy(uniqueEnemyProfile) ? 1 : 0;
                changed += ApplyToEnemy(wraithEnemyProfile) ? 1 : 0;

                if (changed == 0)
                {
                    EditorUtility.DisplayDialog("Soulwake", "No profiles were selected.", "OK");
                    return;
                }

                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("Soulwake", $"Applied multipliers to {changed} profile(s).", "OK");
            }
        }

        private bool ApplyToPlayer(PlayerTuningProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            Undo.RecordObject(profile, "Apply Balance Multipliers To Player Profile");
            profile.maxHp = Mathf.Max(1, Mathf.RoundToInt(profile.maxHp * sourceData.playerHpMultiplier));
            profile.moveSpeed = Mathf.Max(0.1f, profile.moveSpeed * sourceData.playerMoveSpeedMultiplier);

            if (profile.usePlayerStatDamage)
            {
                profile.attackDamage = Mathf.Max(1, Mathf.RoundToInt(profile.attackDamage * sourceData.playerDamageMultiplier));
            }
            else
            {
                profile.baseAttackDamage = Mathf.Max(1, Mathf.RoundToInt(profile.baseAttackDamage * sourceData.playerDamageMultiplier));
            }

            EditorUtility.SetDirty(profile);
            return true;
        }

        private bool ApplyToEnemy(EnemyTuningProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            Undo.RecordObject(profile, "Apply Balance Multipliers To Enemy Profile");
            profile.maxHp = Mathf.Max(1, Mathf.RoundToInt(profile.maxHp * sourceData.enemyHpMultiplier));
            profile.attackDamage = Mathf.Max(1, Mathf.RoundToInt(profile.attackDamage * sourceData.enemyDamageMultiplier));
            profile.moveSpeed = Mathf.Max(0.1f, profile.moveSpeed * sourceData.enemyMoveSpeedMultiplier);

            EditorUtility.SetDirty(profile);
            return true;
        }

        private void LoadFromPresetFile()
        {
            if (!BalanceDebugPresetStorage.TryLoad(out BalanceDebugPresetData loaded, out string error))
            {
                EditorUtility.DisplayDialog("Soulwake", $"Failed to load preset:\n{error}", "OK");
                return;
            }

            sourceData = loaded;
            sourceLabel = $"Saved preset file ({Path.GetFileName(BalanceDebugPresetStorage.SavePath)})";
        }

        private void ReadFromRuntime()
        {
            sourceData = BalanceDebugRuntime.CreateSnapshot();
            sourceLabel = Application.isPlaying ? "Runtime multipliers (Play mode)" : "Runtime defaults (Edit mode)";
        }
    }
}
