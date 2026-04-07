using UnityEngine;
using Soulwake.Game.Player;
using Soulwake.Game.Core;

namespace Soulwake.Game.Souls
{
    /// <summary>
    /// Keeps player-to-soul interaction logic separate from movement/attack.
    /// Handles interaction prompt + F key action for special souls.
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerSoulInteractor : MonoBehaviour
    {
        [Header("Detection")]
        [SerializeField] private float interactionRadius = 1.25f;
        [SerializeField] private LayerMask specialSoulLayer;
        [SerializeField] private int maxDetectionHits = 8;

        [Header("Debug")]
        [SerializeField] private bool logPromptChanges;

        private readonly Collider2D[] overlapBuffer = new Collider2D[16];
        private PlayerInputReader inputReader;
        private ISoulInteractable currentInteractable;
        private string lastPromptText;

        public ISoulInteractable CurrentInteractable => currentInteractable;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            ResolveCurrentInteractable();

            if (currentInteractable == null)
            {
                return;
            }

            if (!currentInteractable.CanInteract)
            {
                return;
            }

            if (inputReader.InteractPressedThisFrame)
            {
                currentInteractable.Interact();
            }
        }

        private void ResolveCurrentInteractable()
        {
            int hitCount = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                interactionRadius,
                overlapBuffer,
                specialSoulLayer);

            float bestDistance = float.MaxValue;
            ISoulInteractable best = null;
            int checks = Mathf.Min(hitCount, maxDetectionHits);

            for (int i = 0; i < checks; i++)
            {
                Collider2D hit = overlapBuffer[i];
                if (hit == null)
                {
                    continue;
                }

                ISoulInteractable interactable = hit.GetComponentInParent<ISoulInteractable>();
                if (interactable == null || !interactable.CanInteract)
                {
                    continue;
                }

                float distance = ((Vector2)hit.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = interactable;
                }
            }

            currentInteractable = best;
            string prompt = currentInteractable != null ? currentInteractable.PromptText : string.Empty;
            if (logPromptChanges && prompt != lastPromptText)
            {
                if (string.IsNullOrEmpty(prompt))
                {
                    GameplayTextEvents.Raise(string.Empty);
                    Debug.Log("Soulwake: Special soul prompt hidden.");
                }
                else
                {
                    GameplayTextEvents.Raise(prompt);
                    Debug.Log($"Soulwake: {prompt}");
                }
                lastPromptText = prompt;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.4f, 0.8f, 1f, 0.7f);
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
