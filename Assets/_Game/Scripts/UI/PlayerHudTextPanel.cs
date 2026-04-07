using TMPro;
using UnityEngine;
using Soulwake.Game.Inventory;
using Soulwake.Game.Stats;

namespace Soulwake.Game.UI
{
    /// <summary>
    /// Minimal vertical-slice HUD panel for HP, ATK, and Gold.
    /// </summary>
    public class PlayerHudTextPanel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerGoldWallet playerGoldWallet;

        [Header("Text")]
        [SerializeField] private TMP_Text hpText;
        [SerializeField] private TMP_Text atkText;
        [SerializeField] private TMP_Text goldText;

        private int cachedMaxHp;
        private int cachedAttack;
        private int cachedCurrentHp;
        private int cachedGold;

        private void OnEnable()
        {
            if (playerStats != null)
            {
                playerStats.OnHealthChanged += HandleHealthChanged;
                playerStats.OnCoreStatsChanged += HandleCoreStatsChanged;
            }

            if (playerGoldWallet != null)
            {
                playerGoldWallet.OnGoldChanged += HandleGoldChanged;
            }

            ForceRefresh();
        }

        private void OnDisable()
        {
            if (playerStats != null)
            {
                playerStats.OnHealthChanged -= HandleHealthChanged;
                playerStats.OnCoreStatsChanged -= HandleCoreStatsChanged;
            }

            if (playerGoldWallet != null)
            {
                playerGoldWallet.OnGoldChanged -= HandleGoldChanged;
            }
        }

        public void ForceRefresh()
        {
            if (playerStats != null)
            {
                cachedCurrentHp = playerStats.CurrentHP;
                cachedMaxHp = playerStats.MaxHP;
                cachedAttack = playerStats.AttackDamage;
            }

            if (playerGoldWallet != null)
            {
                cachedGold = playerGoldWallet.CurrentGold;
            }

            RefreshTexts();
        }

        private void HandleHealthChanged(int currentHp, int maxHp)
        {
            cachedCurrentHp = currentHp;
            cachedMaxHp = maxHp;
            RefreshTexts();
        }

        private void HandleCoreStatsChanged(int maxHp, int attackDamage)
        {
            cachedMaxHp = maxHp;
            cachedAttack = attackDamage;
            RefreshTexts();
        }

        private void HandleGoldChanged(int _, int currentGold)
        {
            cachedGold = currentGold;
            RefreshTexts();
        }

        private void RefreshTexts()
        {
            if (hpText != null)
            {
                hpText.text = $"HP: {cachedCurrentHp}/{cachedMaxHp}";
            }

            if (atkText != null)
            {
                atkText.text = $"ATK: {cachedAttack}";
            }

            if (goldText != null)
            {
                goldText.text = $"Gold: {cachedGold}";
            }
        }
    }
}
