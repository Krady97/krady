using UnityEngine;

namespace Soulwake.Game.Enemy
{
    public readonly struct EnemyDeathInfo
    {
        public EnemyDeathInfo(string enemyId, bool isUniqueEnemy, Vector2 worldPosition, GameObject source)
        {
            EnemyId = enemyId;
            IsUniqueEnemy = isUniqueEnemy;
            WorldPosition = worldPosition;
            Source = source;
        }

        public string EnemyId { get; }
        public bool IsUniqueEnemy { get; }
        public Vector2 WorldPosition { get; }
        public GameObject Source { get; }
    }
}
