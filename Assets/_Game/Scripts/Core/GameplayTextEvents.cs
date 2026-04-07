using System;

namespace Soulwake.Game.Core
{
    /// <summary>
    /// Tiny text event bus for vertical-slice gameplay feedback.
    /// Keeps gameplay systems decoupled from concrete UI objects.
    /// </summary>
    public static class GameplayTextEvents
    {
        public static event Action<string> MessageRaised;

        public static void Raise(string message)
        {
            MessageRaised?.Invoke(message);
        }
    }
}
