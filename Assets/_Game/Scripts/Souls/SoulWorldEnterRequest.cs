using System;
using UnityEngine;

namespace Soulwake.Game.Souls
{
    /// <summary>
    /// Simple bridge event for "enter soul world" requests.
    /// Soul realm implementation will subscribe in the next step.
    /// </summary>
    public static class SoulWorldEnterRequest
    {
        public static event Action<string, Vector2> Requested;

        public static void Raise(string enemyId, Vector2 soulWorldPosition)
        {
            Requested?.Invoke(enemyId, soulWorldPosition);
        }
    }
}
