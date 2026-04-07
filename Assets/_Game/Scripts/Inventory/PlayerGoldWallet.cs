using System;
using UnityEngine;

namespace Soulwake.Game.Inventory
{
    /// <summary>
    /// Runtime gold container for the player.
    /// Keep logic tiny so any source can add gold.
    /// </summary>
    public class PlayerGoldWallet : MonoBehaviour
    {
        [SerializeField] private int startingGold;

        public event Action<int, int> OnGoldChanged;

        public int CurrentGold { get; private set; }

        private void Awake()
        {
            CurrentGold = Mathf.Max(0, startingGold);
            RaiseChanged(0);
        }

        public void AddGold(int amount)
        {
            int delta = Mathf.Max(0, amount);
            if (delta == 0)
            {
                return;
            }

            CurrentGold += delta;
            RaiseChanged(delta);
        }

        private void RaiseChanged(int delta)
        {
            OnGoldChanged?.Invoke(delta, CurrentGold);
        }
    }
}
