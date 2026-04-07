using UnityEngine;
using Soulwake.Game.Enemy;

namespace Soulwake.Game.Souls
{
    /// <summary>
    /// Spawns soul prefabs in response to enemy death events.
    /// Keeps enemy and soul systems decoupled.
    /// </summary>
    public class SoulDropService : MonoBehaviour
    {
        [Header("Soul Prefabs")]
        [SerializeField] private GameObject normalSoulPrefab;
        [SerializeField] private GameObject specialSoulPrefab;

        [Header("Spawn")]
        [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 0.2f, 0f);

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
            GameObject prefab = info.IsUniqueEnemy ? specialSoulPrefab : normalSoulPrefab;
            if (prefab == null)
            {
                return;
            }

            Vector3 spawnPosition = info.WorldPosition + (Vector2)spawnOffset;
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }
}
