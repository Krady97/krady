using UnityEngine;
using Soulwake.Game.Core;
using Soulwake.Game.Inventory;

namespace Soulwake.Game.Loot
{
    /// <summary>
    /// Trigger pickup that grants gold to the player wallet.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class GoldPickup2D : MonoBehaviour
    {
        [SerializeField] private int goldAmount = 1;

        private bool consumed;

        public int GoldAmount => goldAmount;

        public void SetGoldAmount(int amount)
        {
            goldAmount = Mathf.Max(0, amount);
        }

        private void Reset()
        {
            Collider2D collider2D = GetComponent<Collider2D>();
            if (collider2D != null)
            {
                collider2D.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (consumed || !other.CompareTag(VerticalSliceConventions.PlayerTag))
            {
                return;
            }

            PlayerGoldWallet wallet = other.GetComponentInParent<PlayerGoldWallet>();
            if (wallet == null)
            {
                return;
            }

            int amount = Mathf.Max(0, goldAmount);
            wallet.AddGold(amount);
            GameplayTextEvents.Raise($"+{amount} Gold");

            consumed = true;
            Destroy(gameObject);
        }
    }
}
