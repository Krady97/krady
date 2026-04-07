using System;
using System.Collections.Generic;
using UnityEngine;
using Soulwake.Game.Core;
using Soulwake.Game.Enemy;

namespace Soulwake.Game.Loot
{
    /// <summary>
    /// Spawns simple loot pickups based on enemy death info.
    /// Vertical-slice implementation with inspector-configured drop entries.
    /// </summary>
    public class LootDropService : MonoBehaviour
    {
        [Serializable]
        private class LootEntry
        {
            [Header("Match")]
            [SerializeField] private string enemyId = VerticalSliceConventions.EnemyIds.NormalEnemyDefault;
            [SerializeField] private bool appliesToUniqueEnemies = true;
            [SerializeField] private bool appliesToNormalEnemies = true;

            [Header("Gold")]
            [SerializeField] private bool enableGoldDrop = true;
            [Range(0f, 1f)]
            [SerializeField] private float goldDropChance = 1f;
            [SerializeField] private int goldMin = 1;
            [SerializeField] private int goldMax = 5;

            [Header("Item")]
            [SerializeField] private bool enableItemDrop;
            [Range(0f, 1f)]
            [SerializeField] private float itemDropChance = 0.35f;
            [SerializeField] private string itemId = "item_scrap";
            [SerializeField] private string itemDisplayName = "Scrap";

            public string EnemyId => enemyId;
            public bool AppliesToUniqueEnemies => appliesToUniqueEnemies;
            public bool AppliesToNormalEnemies => appliesToNormalEnemies;
            public bool EnableGoldDrop => enableGoldDrop;
            public float GoldDropChance => goldDropChance;
            public int GoldMin => goldMin;
            public int GoldMax => goldMax;
            public bool EnableItemDrop => enableItemDrop;
            public float ItemDropChance => itemDropChance;
            public string ItemId => itemId;
            public string ItemDisplayName => itemDisplayName;
        }

        [Header("Pickup Prefabs")]
        [SerializeField] private GoldPickup2D goldPickupPrefab;
        [SerializeField] private ItemPickup2D itemPickupPrefab;

        [Header("Entries")]
        [SerializeField] private List<LootEntry> dropEntries = new List<LootEntry>();
        [SerializeField] private bool useFallbackForNormalEnemies = true;
        [SerializeField] private bool useFallbackForUniqueEnemies = true;
        [SerializeField] private LootEntry fallbackNormalEntry = new LootEntry();
        [SerializeField] private LootEntry fallbackUniqueEntry = new LootEntry();

        [Header("Spawn")]
        [SerializeField] private float scatterRadius = 0.35f;

        private void OnEnable()
        {
            EnemyDeathNotifier.AnyEnemyDied += HandleEnemyDied;
        }

        private void OnDisable()
        {
            EnemyDeathNotifier.AnyEnemyDied -= HandleEnemyDied;
        }

        private void HandleEnemyDied(EnemyDeathInfo info)
        {
            LootEntry entry = ResolveEntry(info);
            if (entry == null)
            {
                return;
            }

            TrySpawnGold(info.WorldPosition, entry);
            TrySpawnItem(info.WorldPosition, entry);
        }

        private LootEntry ResolveEntry(EnemyDeathInfo info)
        {
            for (int i = 0; i < dropEntries.Count; i++)
            {
                LootEntry entry = dropEntries[i];
                if (entry == null)
                {
                    continue;
                }

                if (!string.Equals(entry.EnemyId, info.EnemyId, StringComparison.Ordinal))
                {
                    continue;
                }

                if (info.IsUniqueEnemy && !entry.AppliesToUniqueEnemies)
                {
                    continue;
                }

                if (!info.IsUniqueEnemy && !entry.AppliesToNormalEnemies)
                {
                    continue;
                }

                return entry;
            }

            if (info.IsUniqueEnemy && useFallbackForUniqueEnemies)
            {
                return fallbackUniqueEntry;
            }

            if (!info.IsUniqueEnemy && useFallbackForNormalEnemies)
            {
                return fallbackNormalEntry;
            }

            return null;
        }

        private void TrySpawnGold(Vector2 basePosition, LootEntry entry)
        {
            if (!entry.EnableGoldDrop || goldPickupPrefab == null)
            {
                return;
            }

            if (UnityEngine.Random.value > entry.GoldDropChance)
            {
                return;
            }

            int min = Mathf.Min(entry.GoldMin, entry.GoldMax);
            int max = Mathf.Max(entry.GoldMin, entry.GoldMax);
            int amount = UnityEngine.Random.Range(min, max + 1);
            if (amount <= 0)
            {
                return;
            }

            Vector2 spawnPosition = basePosition + UnityEngine.Random.insideUnitCircle * scatterRadius;
            GoldPickup2D pickup = Instantiate(goldPickupPrefab, spawnPosition, Quaternion.identity);
            pickup.SetGoldAmount(amount);
        }

        private void TrySpawnItem(Vector2 basePosition, LootEntry entry)
        {
            if (!entry.EnableItemDrop || itemPickupPrefab == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(entry.ItemId))
            {
                return;
            }

            if (UnityEngine.Random.value > entry.ItemDropChance)
            {
                return;
            }

            Vector2 spawnPosition = basePosition + UnityEngine.Random.insideUnitCircle * scatterRadius;
            ItemPickup2D pickup = Instantiate(itemPickupPrefab, spawnPosition, Quaternion.identity);
            pickup.SetItemData(entry.ItemId, entry.ItemDisplayName);
        }
    }
}
