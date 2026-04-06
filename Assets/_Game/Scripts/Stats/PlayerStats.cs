using System;
using UnityEngine;

namespace Soulwake.Game.Stats
{
    /// <summary>
    /// Owns core player combat stats for the vertical slice.
    /// Start simple and expand with additional stats later.
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [Header("Core Stats")]
        [SerializeField] private int maxHP = 20;
        [SerializeField] private int attackDamage = 5;

        public int MaxHP => maxHP;
        public int AttackDamage => attackDamage;

        public event Action<int, int> OnHealthChanged;
        public event Action OnDied;

        public int CurrentHP { get; private set; }
        public bool IsDead => CurrentHP <= 0;

        private void Awake()
        {
            CurrentHP = maxHP;
            RaiseHealthChanged();
        }

        public void ResetHealthToFull()
        {
            CurrentHP = maxHP;
            RaiseHealthChanged();
        }

        public void TakeDamage(int amount)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHP = Mathf.Max(0, CurrentHP - Mathf.Max(0, amount));
            RaiseHealthChanged();

            if (CurrentHP == 0)
            {
                OnDied?.Invoke();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHP = Mathf.Min(maxHP, CurrentHP + Mathf.Max(0, amount));
            RaiseHealthChanged();
        }

        private void RaiseHealthChanged()
        {
            OnHealthChanged?.Invoke(CurrentHP, maxHP);
        }
    }
}
