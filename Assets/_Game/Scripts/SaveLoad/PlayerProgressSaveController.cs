using UnityEngine;
using Soulwake.Game.Core;
using Soulwake.Game.Inventory;
using Soulwake.Game.Skills;
using Soulwake.Game.Stats;

namespace Soulwake.Game.SaveLoad
{
    /// <summary>
    /// Scene-facing save/load controller with keyboard shortcuts.
    /// Saves on quit automatically.
    /// </summary>
    public class PlayerProgressSaveController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private PlayerGoldWallet playerGoldWallet;
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private PlayerSkillBook playerSkillBook;
        [SerializeField] private string playerTag = "Player";

        [Header("Hotkeys")]
        [SerializeField] private KeyCode saveKey = KeyCode.F5;
        [SerializeField] private KeyCode loadKey = KeyCode.F9;

        [Header("Options")]
        [SerializeField] private bool autoLoadOnStart = true;
        [SerializeField] private bool autoSaveOnApplicationQuit = true;
        [SerializeField] private bool showMessagesInTextFeed = true;

        private void Start()
        {
            ResolvePlayerReferences();

            if (autoLoadOnStart)
            {
                LoadProgress();
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(saveKey))
            {
                SaveProgress();
            }

            if (Input.GetKeyDown(loadKey))
            {
                LoadProgress();
            }
        }

        private void OnApplicationQuit()
        {
            if (!autoSaveOnApplicationQuit)
            {
                return;
            }

            SaveProgress();
        }

        [ContextMenu("Save Progress")]
        public void SaveProgress()
        {
            ResolvePlayerReferences();
            if (!PlayerProgressSaveSystem.TrySave(playerStats, playerGoldWallet, playerInventory, playerSkillBook, out string error))
            {
                Publish($"Save failed: {error}");
                return;
            }

            Publish("Progress saved.");
        }

        [ContextMenu("Load Progress")]
        public void LoadProgress()
        {
            ResolvePlayerReferences();
            if (!PlayerProgressSaveSystem.TryLoad(out PlayerProgressSaveData data, out string loadError))
            {
                Publish($"Load skipped: {loadError}");
                return;
            }

            if (!PlayerProgressSaveSystem.TryApplyLoadedData(
                    data,
                    playerStats,
                    playerGoldWallet,
                    playerInventory,
                    playerSkillBook,
                    out string applyError))
            {
                Publish($"Load failed: {applyError}");
                return;
            }

            Publish("Progress loaded.");
        }

        private void ResolvePlayerReferences()
        {
            if (playerStats != null && playerGoldWallet != null && playerInventory != null && playerSkillBook != null)
            {
                return;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
            if (playerObject == null)
            {
                return;
            }

            if (playerStats == null)
            {
                playerStats = playerObject.GetComponent<PlayerStats>();
            }

            if (playerGoldWallet == null)
            {
                playerGoldWallet = playerObject.GetComponent<PlayerGoldWallet>();
            }

            if (playerInventory == null)
            {
                playerInventory = playerObject.GetComponent<PlayerInventory>();
            }

            if (playerSkillBook == null)
            {
                playerSkillBook = playerObject.GetComponent<PlayerSkillBook>();
            }
        }

        private void Publish(string message)
        {
            if (!showMessagesInTextFeed)
            {
                return;
            }

            GameplayTextEvents.Raise(message);
        }
    }
}
