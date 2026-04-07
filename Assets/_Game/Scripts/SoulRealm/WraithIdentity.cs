using UnityEngine;
using Soulwake.Game.Core;

namespace Soulwake.Game.SoulRealm
{
    /// <summary>
    /// Identifies which unique enemy a wraith instance belongs to.
    /// </summary>
    public class WraithIdentity : MonoBehaviour
    {
        [SerializeField] private string sourceEnemyId = VerticalSliceConventions.EnemyIds.UniqueEnemyDefault;

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
