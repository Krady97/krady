using System;
using UnityEngine;
using Soulwake.Game.Core;

namespace Soulwake.Game.Souls
{
    /// <summary>
    /// Special soul that requires explicit player interaction (F key).
    /// Emits a request event for the upcoming soul realm system.
    /// </summary>
    public class SpecialSoulInteractable : MonoBehaviour, ISoulInteractable
    {
        [Header("Prompt")]
        [SerializeField] private string promptText = "Press F to enter Soul Realm";

        [Header("Payload")]
        [SerializeField] private string sourceEnemyId = "unique_enemy_01";
        [SerializeField] private bool consumeOnInteract = true;

        public static event Action<SpecialSoulInteractable> AnySpecialSoulInteracted;

        public bool CanInteract => isActiveAndEnabled;
        public string PromptText => promptText;
        public string SourceEnemyId => sourceEnemyId;

        public void Interact()
        {
            if (!CanInteract)
            {
                return;
            }

            GameplayTextEvents.Raise($"{sourceEnemyId} soul resonates... Entering realm.");
            SoulWorldEnterRequest.Raise(sourceEnemyId, transform.position);
            AnySpecialSoulInteracted?.Invoke(this);

            if (consumeOnInteract)
            {
                Destroy(gameObject);
            }
        }
    }
}
