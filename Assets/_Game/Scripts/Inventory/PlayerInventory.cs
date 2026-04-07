using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soulwake.Game.Inventory
{
    /// <summary>
    /// Minimal inventory for vertical slice item pickups.
    /// Stores string item IDs for now.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private List<string> itemIds = new List<string>();

        public event Action<string, string> OnItemAdded;

        public IReadOnlyList<string> ItemIds => itemIds;

        public bool AddItem(string itemId, string displayName)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return false;
            }

            itemIds.Add(itemId);
            OnItemAdded?.Invoke(itemId, displayName);
            return true;
        }

        public void ReplaceItems(IEnumerable<string> newItemIds)
        {
            itemIds.Clear();
            if (newItemIds == null)
            {
                return;
            }

            itemIds.AddRange(newItemIds.Where(id => !string.IsNullOrWhiteSpace(id)));
        }
    }
}
