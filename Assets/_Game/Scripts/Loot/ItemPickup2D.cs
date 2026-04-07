using UnityEngine;
using Soulwake.Game.Core;
using Soulwake.Game.Inventory;

namespace Soulwake.Game.Loot
{
    /// <summary>
    /// Trigger pickup that grants a simple item entry.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup2D : MonoBehaviour
    {
        [SerializeField] private string itemId = "item_scrap";
        [SerializeField] private string displayName = "Scrap";

        private bool consumed;

        public string ItemId => itemId;
        public string DisplayName => displayName;

        public void SetItemData(string newItemId, string newDisplayName)
        {
            itemId = newItemId;
            displayName = string.IsNullOrWhiteSpace(newDisplayName) ? newItemId : newDisplayName;
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
            if (consumed || !other.CompareTag("Player"))
            {
                return;
            }

            PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();
            if (inventory == null)
            {
                return;
            }

            if (!inventory.AddItem(itemId, displayName))
            {
                return;
            }

            GameplayTextEvents.Raise($"Picked up: {displayName}");
            consumed = true;
            Destroy(gameObject);
        }
    }
}
