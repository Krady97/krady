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
        public event Action<int, int> OnCoreStatsChanged;
        public event Action OnDied;

        public int CurrentHP { get; private set; }
        public bool IsDead => CurrentHP <= 0;

        private void Awake()
        {
            CurrentHP = maxHP;
            RaiseHealthChanged();
            RaiseCoreStatsChanged();
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

        public void AddPermanentMaxHP(int amount, bool healForAddedAmount = true)
        {
            int delta = Mathf.Max(0, amount);
            if (delta <= 0)
            {
                return;
            }

            maxHP += delta;
            if (healForAddedAmount && !IsDead)
            {
                CurrentHP = Mathf.Min(maxHP, CurrentHP + delta);
                RaiseHealthChanged();
            }
            else
            {
                CurrentHP = Mathf.Min(CurrentHP, maxHP);
                RaiseHealthChanged();
            }

            RaiseCoreStatsChanged();
        }

        public void AddPermanentAttackDamage(int amount)
        {
            int delta = Mathf.Max(0, amount);
            if (delta <= 0)
            {
                return;
            }

            attackDamage += delta;
            RaiseCoreStatsChanged();
        }

        /// <summary>
        /// Applies full stat state from save data.
        /// </summary>
        public void ApplyLoadedState(int loadedMaxHp, int loadedAttackDamage, int loadedCurrentHp)
        {
            maxHP = Mathf.Max(1, loadedMaxHp);
            attackDamage = Mathf.Max(0, loadedAttackDamage);
            CurrentHP = Mathf.Clamp(loadedCurrentHp, 0, maxHP);

            RaiseCoreStatsChanged();
            RaiseHealthChanged();
        }

        private void RaiseHealthChanged()
        {
            OnHealthChanged?.Invoke(CurrentHP, maxHP);
        }

        private void RaiseCoreStatsChanged()
        {
            OnCoreStatsChanged?.Invoke(maxHP, attackDamage);
        }
    }
}
