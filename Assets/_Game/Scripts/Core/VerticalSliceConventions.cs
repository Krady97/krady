namespace Soulwake.Game.Core
{
    /// <summary>
    /// Centralized constants for scene conventions in the vertical slice.
    /// Keeps tags/IDs consistent across systems.
    /// </summary>
    public static class VerticalSliceConventions
    {
        public static class Tags
        {
            public const string Player = "Player";
            public const string Enemy = "Enemy";
            public const string SpecialSoul = "SpecialSoul";
        }

        public static class EnemyIds
        {
            public const string Default = "enemy_default";
            public const string NormalEnemyDefault = "enemy_normal_01";
            public const string UniqueEnemyDefault = "enemy_unique_01";
            public const string UniqueSoulDefault = UniqueEnemyDefault;
        }

        // Convenience aliases for existing serialized defaults.
        public const string PlayerTag = Tags.Player;
        public const string EnemyIdDefault = EnemyIds.Default;
        public const string DefaultNormalEnemyId = EnemyIds.NormalEnemyDefault;
        public const string DefaultUniqueEnemyId = EnemyIds.UniqueEnemyDefault;
        public const string DefaultUniqueSoulId = EnemyIds.UniqueSoulDefault;
    }
}
