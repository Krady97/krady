using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Soulwake.Game.Core;
using Soulwake.Game.Enemy;
using Soulwake.Game.Stats;

namespace Soulwake.Game.DebugTools
{
    /// <summary>
    /// Lightweight runtime balancing panel for fast combat feel iteration.
    /// </summary>
    public class BalanceDebugOverlayUI : MonoBehaviour
    {
        private enum Field
        {
            PlayerHp = 0,
            EnemyHp = 1,
            PlayerDamage = 2,
            EnemyDamage = 3,
            PlayerMoveSpeed = 4,
            EnemyMoveSpeed = 5
        }

        [Header("UI")]
        [SerializeField] private TMP_Text panelText;
        [SerializeField] private bool visibleOnStart = true;

        [Header("Input")]
        [SerializeField] private KeyCode toggleKey = KeyCode.F2;
        [SerializeField] private KeyCode resetKey = KeyCode.F3;
        [SerializeField] private KeyCode previousFieldKey = KeyCode.Comma;
        [SerializeField] private KeyCode nextFieldKey = KeyCode.Period;
        [SerializeField] private KeyCode decreaseKey = KeyCode.Minus;
        [SerializeField] private KeyCode increaseKey = KeyCode.Equals;
        [SerializeField] private float multiplierStep = 0.1f;

        [Header("Player")]
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private bool autoFindPlayer = true;
        [SerializeField] private string playerTag = VerticalSliceConventions.PlayerTag;

        [Header("Messages")]
        [SerializeField] private bool publishChangesToGameplayFeed;

        private readonly Dictionary<int, int> enemyBaseHpByInstanceId = new Dictionary<int, int>();
        private Field selectedField;
        private bool isVisible;
        private int playerBaseHp = -1;

        private void OnEnable()
        {
            BalanceDebugRuntime.MultipliersChanged += HandleMultipliersChanged;
        }

        private void OnDisable()
        {
            BalanceDebugRuntime.MultipliersChanged -= HandleMultipliersChanged;
        }

        private void Start()
        {
            isVisible = visibleOnStart;
            ResolvePlayerReference();
            if (playerStats != null && playerBaseHp < 0)
            {
                playerBaseHp = playerStats.MaxHP;
            }
            RefreshText();
        }

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                isVisible = !isVisible;
                RefreshText();
            }

            if (Input.GetKeyDown(resetKey))
            {
                BalanceDebugRuntime.ResetAll();
                Publish("Balance debug multipliers reset.");
                return;
            }

            if (Input.GetKeyDown(previousFieldKey))
            {
                SelectField(-1);
            }
            else if (Input.GetKeyDown(nextFieldKey))
            {
                SelectField(1);
            }

            if (Input.GetKeyDown(decreaseKey))
            {
                AdjustSelected(-multiplierStep);
            }
            else if (Input.GetKeyDown(increaseKey))
            {
                AdjustSelected(multiplierStep);
            }
        }

        private void SelectField(int delta)
        {
            int count = 6;
            int index = (((int)selectedField + delta) % count + count) % count;
            selectedField = (Field)index;
            RefreshText();
        }

        private void AdjustSelected(float delta)
        {
            switch (selectedField)
            {
                case Field.PlayerHp:
                    BalanceDebugRuntime.SetPlayerHpMultiplier(BalanceDebugRuntime.PlayerHpMultiplier + delta);
                    break;
                case Field.EnemyHp:
                    BalanceDebugRuntime.SetEnemyHpMultiplier(BalanceDebugRuntime.EnemyHpMultiplier + delta);
                    break;
                case Field.PlayerDamage:
                    BalanceDebugRuntime.SetPlayerDamageMultiplier(BalanceDebugRuntime.PlayerDamageMultiplier + delta);
                    break;
                case Field.EnemyDamage:
                    BalanceDebugRuntime.SetEnemyDamageMultiplier(BalanceDebugRuntime.EnemyDamageMultiplier + delta);
                    break;
                case Field.PlayerMoveSpeed:
                    BalanceDebugRuntime.SetPlayerMoveSpeedMultiplier(BalanceDebugRuntime.PlayerMoveSpeedMultiplier + delta);
                    break;
                case Field.EnemyMoveSpeed:
                    BalanceDebugRuntime.SetEnemyMoveSpeedMultiplier(BalanceDebugRuntime.EnemyMoveSpeedMultiplier + delta);
                    break;
            }
        }

        private void HandleMultipliersChanged()
        {
            ApplyHpMultipliers();
            RefreshText();
        }

        private void ResolvePlayerReference()
        {
            if (playerStats != null || !autoFindPlayer)
            {
                return;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject != null)
            {
                playerStats = playerObject.GetComponent<PlayerStats>();
            }
        }

        private void ApplyHpMultipliers()
        {
            ResolvePlayerReference();
            if (playerStats != null)
            {
                if (playerBaseHp < 0)
                {
                    playerBaseHp = playerStats.MaxHP;
                }

                ApplyScaledHp(playerStats, playerBaseHp, BalanceDebugRuntime.PlayerHpMultiplier);
            }

            EnemyDeathNotifier[] enemies = FindObjectsOfType<EnemyDeathNotifier>();
            for (int i = 0; i < enemies.Length; i++)
            {
                EnemyDeathNotifier notifier = enemies[i];
                if (notifier == null)
                {
                    continue;
                }

                PlayerStats stats = notifier.GetComponent<PlayerStats>();
                if (stats == null)
                {
                    continue;
                }

                int key = stats.GetInstanceID();
                if (!enemyBaseHpByInstanceId.TryGetValue(key, out int baseHp))
                {
                    baseHp = stats.MaxHP;
                    enemyBaseHpByInstanceId[key] = baseHp;
                }

                ApplyScaledHp(stats, baseHp, BalanceDebugRuntime.EnemyHpMultiplier);
            }
        }

        private static void ApplyScaledHp(PlayerStats stats, int baseHp, float multiplier)
        {
            int oldMax = Mathf.Max(1, stats.MaxHP);
            float hpRatio = (float)stats.CurrentHP / oldMax;

            int newMax = Mathf.Max(1, Mathf.RoundToInt(baseHp * multiplier));
            int newCurrent = Mathf.Clamp(Mathf.RoundToInt(newMax * hpRatio), 0, newMax);
            stats.ApplyLoadedState(newMax, stats.AttackDamage, newCurrent);
        }

        private void RefreshText()
        {
            if (panelText == null)
            {
                return;
            }

            panelText.gameObject.SetActive(isVisible);
            if (!isVisible)
            {
                return;
            }

            panelText.text =
                "BALANCE DEBUG\n" +
                $"{Marker(Field.PlayerHp)} Player HP x{BalanceDebugRuntime.PlayerHpMultiplier:0.00}\n" +
                $"{Marker(Field.EnemyHp)} Enemy HP x{BalanceDebugRuntime.EnemyHpMultiplier:0.00}\n" +
                $"{Marker(Field.PlayerDamage)} Player DMG x{BalanceDebugRuntime.PlayerDamageMultiplier:0.00}\n" +
                $"{Marker(Field.EnemyDamage)} Enemy DMG x{BalanceDebugRuntime.EnemyDamageMultiplier:0.00}\n" +
                $"{Marker(Field.PlayerMoveSpeed)} Player SPD x{BalanceDebugRuntime.PlayerMoveSpeedMultiplier:0.00}\n" +
                $"{Marker(Field.EnemyMoveSpeed)} Enemy SPD x{BalanceDebugRuntime.EnemyMoveSpeedMultiplier:0.00}\n\n" +
                $"[{previousFieldKey}] Prev  [{nextFieldKey}] Next\n" +
                $"[{decreaseKey}] -  [{increaseKey}] +  [{resetKey}] Reset  [{toggleKey}] Toggle";
        }

        private string Marker(Field field)
        {
            return field == selectedField ? ">" : " ";
        }

        private void Publish(string message)
        {
            if (publishChangesToGameplayFeed)
            {
                GameplayTextEvents.Raise(message);
            }
        }
    }
}
