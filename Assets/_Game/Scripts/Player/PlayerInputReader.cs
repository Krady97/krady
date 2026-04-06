using UnityEngine;

namespace Krady.Game.Player
{
    /// <summary>
    /// Centralized keyboard input for the player.
    /// Keeping this separate allows other systems (combat/interact)
    /// to read from one source without coupling to Unity input calls.
    /// </summary>
    public class PlayerInputReader : MonoBehaviour
    {
        [Header("Movement Keys")]
        [SerializeField] private KeyCode moveUpKey = KeyCode.W;
        [SerializeField] private KeyCode moveDownKey = KeyCode.S;
        [SerializeField] private KeyCode moveLeftKey = KeyCode.A;
        [SerializeField] private KeyCode moveRightKey = KeyCode.D;

        [Header("Action Keys")]
        [SerializeField] private KeyCode attackKey = KeyCode.Space;
        [SerializeField] private KeyCode interactKey = KeyCode.F;

        public Vector2 Move { get; private set; }
        public bool AttackPressedThisFrame { get; private set; }
        public bool InteractPressedThisFrame { get; private set; }

        private void Update()
        {
            float x = 0f;
            float y = 0f;

            if (Input.GetKey(moveLeftKey))
            {
                x -= 1f;
            }
            if (Input.GetKey(moveRightKey))
            {
                x += 1f;
            }
            if (Input.GetKey(moveDownKey))
            {
                y -= 1f;
            }
            if (Input.GetKey(moveUpKey))
            {
                y += 1f;
            }

            Move = new Vector2(x, y).normalized;
            AttackPressedThisFrame = Input.GetKeyDown(attackKey);
            InteractPressedThisFrame = Input.GetKeyDown(interactKey);
        }
    }
}
