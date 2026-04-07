using UnityEngine;

namespace Soulwake.Game.Enemy
{
    /// <summary>
    /// Editor/runtime helper to visualize enemy identity setup.
    /// No gameplay logic; improves scene setup clarity for slice.
    /// </summary>
    [DisallowMultipleComponent]
    public class EnemyTagAuthoring : MonoBehaviour
    {
        [SerializeField] private EnemyDeathNotifier deathNotifier;

        private void Reset()
        {
            deathNotifier = GetComponent<EnemyDeathNotifier>();
        }

        private void OnValidate()
        {
            if (deathNotifier == null)
            {
                deathNotifier = GetComponent<EnemyDeathNotifier>();
            }
        }
    }
}
