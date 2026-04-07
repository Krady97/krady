using UnityEngine;
using Soulwake.Game.Stats;
using Soulwake.Game.Core;

namespace Soulwake.Game.Souls
{
    /// <summary>
    /// Auto-absorbed soul pickup for normal enemies.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class NormalSoulPickup : MonoBehaviour
    {
        [Header("Reward")]
        [SerializeField] private int bonusMaxHP = 1;
        [SerializeField] private int bonusAttackDamage = 0;

        private bool consumed;

        private void Reset()
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (consumed)
            {
                return;
            }

            PlayerStats playerStats = other.GetComponentInParent<PlayerStats>();
            if (playerStats == null || !other.CompareTag(VerticalSliceConventions.PlayerTag))
            {
                return;
            }

            if (bonusMaxHP > 0)
            {
                playerStats.AddPermanentMaxHP(bonusMaxHP, true);
            }

            if (bonusAttackDamage > 0)
            {
                playerStats.AddPermanentAttackDamage(bonusAttackDamage);
            }

            GameplayTextEvents.Raise($"Soul absorbed! +{bonusMaxHP} Max HP, +{bonusAttackDamage} ATK");
            consumed = true;
            Destroy(gameObject);
        }
    }
}
