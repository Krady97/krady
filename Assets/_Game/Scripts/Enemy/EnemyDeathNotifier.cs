using System;
using UnityEngine;
using Soulwake.Game.Combat;

namespace Soulwake.Game.Enemy
{
    /// <summary>
    /// Emits enemy death events for loot and soul systems.
    /// Keep this separate from AI/movement so death consumers stay decoupled.
    /// </summary>
    [RequireComponent(typeof(Damageable2D))]
    public class EnemyDeathNotifier : MonoBehaviour
    {
        [Header("Identity")]
        [SerializeField] private string enemyId = "enemy_default";
        [SerializeField] private bool isUniqueEnemy;

        private Damageable2D damageable;

        public static event Action<EnemyDeathInfo> AnyEnemyDied;
        public event Action<EnemyDeathInfo> EnemyDied;

        private void Awake()
        {
            damageable = GetComponent<Damageable2D>();
            damageable.OnDied += HandleDied;
        }

        private void OnDestroy()
        {
            if (damageable != null)
            {
                damageable.OnDied -= HandleDied;
            }
        }

        private void HandleDied(Damageable2D _)
        {
            EnemyDeathInfo info = new EnemyDeathInfo(
                enemyId,
                isUniqueEnemy,
                transform.position,
                gameObject);

            EnemyDied?.Invoke(info);
            AnyEnemyDied?.Invoke(info);
        }
    }
}
