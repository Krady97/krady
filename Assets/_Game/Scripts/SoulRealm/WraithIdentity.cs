using UnityEngine;

namespace Soulwake.Game.SoulRealm
{
    /// <summary>
    /// Identifies which unique enemy a wraith instance belongs to.
    /// </summary>
    public class WraithIdentity : MonoBehaviour
    {
        [SerializeField] private string sourceEnemyId = "unique_enemy_01";

        public string SourceEnemyId => sourceEnemyId;

        public void SetSourceEnemyId(string enemyId)
        {
            if (!string.IsNullOrWhiteSpace(enemyId))
            {
                sourceEnemyId = enemyId;
            }
        }
    }
}
