using System;
using System.Collections.Generic;
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
    }
}
