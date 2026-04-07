using System.Text;
using UnityEngine;
using Soulwake.Game.Inventory;
using Soulwake.Game.Skills;
using Soulwake.Game.Stats;
using Soulwake.Game.Souls;
using Soulwake.Game.SoulRealm;
using Soulwake.Game.Loot;
using Soulwake.Game.UI;
using Soulwake.Game.Core;

namespace Soulwake.Game.Bootstrap
{
    /// <summary>
    /// Lightweight runtime validator for the playable vertical slice setup.
    /// Prints warnings for missing core scene wiring.
    /// </summary>
    public class VerticalSliceSceneValidator : MonoBehaviour
    {
        [SerializeField] private GameObject playerObject;
        [SerializeField] private SoulDropService soulDropService;
        [SerializeField] private LootDropService lootDropService;
        [SerializeField] private SoulRealmManager soulRealmManager;
        [SerializeField] private SimpleGameplayTextFeed gameplayTextFeed;
        [SerializeField] private PlayerHudTextPanel hudPanel;

        private void Start()
        {
            Validate();
        }

        [ContextMenu("Validate Vertical Slice Setup")]
        public void Validate()
        {
            StringBuilder sb = new StringBuilder();

            if (playerObject == null)
            {
                sb.AppendLine("- Player object is not assigned.");
            }
            else
            {
                ValidatePlayer(playerObject, sb);
            }

            if (soulDropService == null)
            {
                sb.AppendLine("- SoulDropService reference is missing.");
            }

            if (lootDropService == null)
            {
                sb.AppendLine("- LootDropService reference is missing.");
            }

            if (soulRealmManager == null)
            {
                sb.AppendLine("- SoulRealmManager reference is missing.");
            }

            if (gameplayTextFeed == null)
            {
                sb.AppendLine("- SimpleGameplayTextFeed reference is missing.");
            }

            if (hudPanel == null)
            {
                sb.AppendLine("- PlayerHudTextPanel reference is missing.");
            }

            if (sb.Length == 0)
            {
                Debug.Log("Soulwake: Vertical slice validation passed.");
            }
            else
            {
                Debug.LogWarning($"Soulwake: Vertical slice validation warnings:\n{sb}");
            }
        }

        private static void ValidatePlayer(GameObject player, StringBuilder sb)
        {
            if (!player.CompareTag(VerticalSliceConventions.PlayerTag))
            {
                sb.AppendLine($"- Player object should be tagged as '{VerticalSliceConventions.PlayerTag}'.");
            }

            if (player.GetComponent<PlayerStats>() == null)
            {
                sb.AppendLine("- PlayerStats is missing on Player.");
            }

            if (player.GetComponent<PlayerGoldWallet>() == null)
            {
                sb.AppendLine("- PlayerGoldWallet is missing on Player.");
            }

            if (player.GetComponent<PlayerInventory>() == null)
            {
                sb.AppendLine("- PlayerInventory is missing on Player.");
            }

            if (player.GetComponent<PlayerSkillBook>() == null)
            {
                sb.AppendLine("- PlayerSkillBook is missing on Player.");
            }

            if (player.GetComponent<PlayerSoulInteractor>() == null)
            {
                sb.AppendLine("- PlayerSoulInteractor is missing on Player.");
            }
        }
    }
}
