using UnityEngine;
using TMPro;
using Soulwake.Game.Core;

namespace Soulwake.Game.UI
{
    /// <summary>
    /// Minimal text feed used by the vertical slice.
    /// Last message overwrites previous message to keep setup simple.
    /// </summary>
    public class SimpleGameplayTextFeed : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private string defaultText = string.Empty;

        private void Awake()
        {
            if (messageText != null)
            {
                messageText.text = defaultText;
            }
        }

        private void OnEnable()
        {
            GameplayTextEvents.MessageRaised += ShowMessage;
        }

        private void OnDisable()
        {
            GameplayTextEvents.MessageRaised -= ShowMessage;
        }

        public void ShowMessage(string message)
        {
            if (messageText == null)
            {
                return;
            }

            messageText.text = message;
        }
    }
}
