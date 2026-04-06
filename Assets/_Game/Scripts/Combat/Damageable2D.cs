using UnityEngine;
using Soulwake.Game.Stats;

namespace Soulwake.Game.Combat
{
    /// <summary>
    /// Generic damage receiver for 2D actors.
    /// Attach to enemies or test dummies; player can use it too.
    /// </summary>
    [RequireComponent(typeof(PlayerStats))]
    public class Damageable2D : MonoBehaviour, IDamageable2D
    {
        [Header("Death")]
        [SerializeField] private bool destroyOnDeath;

        private PlayerStats stats;

        public bool IsDead { get; private set; }

        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            IsDead = stats.IsDead;
            stats.OnDied += HandleDied;
        }

        private void OnDestroy()
        {
            if (stats != null)
            {
                stats.OnDied -= HandleDied;
            }
        }

        public void ApplyDamage(int damageAmount)
        {
            if (IsDead || damageAmount <= 0)
            {
                return;
            }

            stats.TakeDamage(damageAmount);
        }

        private void HandleDied()
        {
            IsDead = true;
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
    }
}
