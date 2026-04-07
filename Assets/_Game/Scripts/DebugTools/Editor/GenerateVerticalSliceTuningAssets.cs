using UnityEditor;
using UnityEngine;
using Soulwake.Game.Core;
using Soulwake.Game.Tuning;

namespace Soulwake.Game.DebugTools.Editor
{
    /// <summary>
    /// Generates baseline tuning assets for the vertical slice.
    /// Safe by default: existing assets are preserved.
    /// </summary>
    public static class GenerateVerticalSliceTuningAssets
    {
        private const string RootFolder = "Assets/_Game/ScriptableObjects";
        private const string TuningFolder = "Assets/_Game/ScriptableObjects/Tuning";

        [MenuItem("Soulwake/Tools/Generate Default Vertical Slice Assets")]
        public static void Generate()
        {
            EnsureFolder(RootFolder);
            EnsureFolder(TuningFolder);

            int created = 0;
            int skipped = 0;

            created += CreatePlayerProfile("Player_VSlice", out bool createdPlayer);
            skipped += createdPlayer ? 0 : 1;

            created += CreateEnemyProfile(
                assetName: "Enemy_Normal",
                enemyId: VerticalSliceConventions.DefaultNormalEnemyId,
                isUnique: false,
                maxHp: 16,
                attackDamage: 3,
                moveSpeed: 2.2f,
                aggroRange: 6f,
                loseRange: 8f,
                attackPadding: 0.05f,
                attackRange: 0.9f,
                attackCooldown: 1.0f,
                maxTargets: 1,
                out bool createdNormal);
            skipped += createdNormal ? 0 : 1;

            created += CreateEnemyProfile(
                assetName: "Enemy_Unique",
                enemyId: VerticalSliceConventions.DefaultUniqueEnemyId,
                isUnique: true,
                maxHp: 32,
                attackDamage: 6,
                moveSpeed: 2.4f,
                aggroRange: 7f,
                loseRange: 9f,
                attackPadding: 0.05f,
                attackRange: 1.0f,
                attackCooldown: 0.85f,
                maxTargets: 1,
                out bool createdUnique);
            skipped += createdUnique ? 0 : 1;

            created += CreateEnemyProfile(
                assetName: "Enemy_Wraith",
                enemyId: VerticalSliceConventions.DefaultUniqueEnemyId,
                isUnique: true,
                maxHp: 44,
                attackDamage: 8,
                moveSpeed: 2.6f,
                aggroRange: 8f,
                loseRange: 10f,
                attackPadding: 0.05f,
                attackRange: 1.05f,
                attackCooldown: 0.75f,
                maxTargets: 1,
                out bool createdWraith);
            skipped += createdWraith ? 0 : 1;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Soulwake",
                $"Default tuning asset generation complete.\n\nCreated: {created}\nSkipped (already existed): {skipped}\n\nFolder:\n{TuningFolder}",
                "OK");
        }

        private static int CreatePlayerProfile(string assetName, out bool created)
        {
            string path = $"{TuningFolder}/{assetName}.asset";
            if (AssetDatabase.LoadAssetAtPath<PlayerTuningProfile>(path) != null)
            {
                created = false;
                return 0;
            }

            PlayerTuningProfile profile = ScriptableObject.CreateInstance<PlayerTuningProfile>();
            profile.moveSpeed = 3.5f;
            profile.maxHp = 20;
            profile.attackDamage = 5;
            profile.attackRange = 1.0f;
            profile.usePlayerStatDamage = true;
            profile.baseAttackDamage = 3;
            profile.bonusAttackDamage = 0;
            profile.attackCooldown = 0.35f;

            AssetDatabase.CreateAsset(profile, path);
            created = true;
            return 1;
        }

        private static int CreateEnemyProfile(
            string assetName,
            string enemyId,
            bool isUnique,
            int maxHp,
            int attackDamage,
            float moveSpeed,
            float aggroRange,
            float loseRange,
            float attackPadding,
            float attackRange,
            float attackCooldown,
            int maxTargets,
            out bool created)
        {
            string path = $"{TuningFolder}/{assetName}.asset";
            if (AssetDatabase.LoadAssetAtPath<EnemyTuningProfile>(path) != null)
            {
                created = false;
                return 0;
            }

            EnemyTuningProfile profile = ScriptableObject.CreateInstance<EnemyTuningProfile>();
            profile.enemyId = enemyId;
            profile.isUniqueEnemy = isUnique;
            profile.maxHp = maxHp;
            profile.attackDamage = attackDamage;
            profile.moveSpeed = moveSpeed;
            profile.aggroRange = aggroRange;
            profile.loseInterestRange = loseRange;
            profile.attackDistancePadding = attackPadding;
            profile.attackRange = attackRange;
            profile.attackCooldown = attackCooldown;
            profile.maxTargetsPerSwing = maxTargets;

            AssetDatabase.CreateAsset(profile, path);
            created = true;
            return 1;
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string[] segments = folderPath.Split('/');
            string current = segments[0];
            for (int i = 1; i < segments.Length; i++)
            {
                string next = $"{current}/{segments[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, segments[i]);
                }

                current = next;
            }
        }
    }
}
