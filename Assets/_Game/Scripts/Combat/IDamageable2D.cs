namespace Soulwake.Game.Combat
{
    public interface IDamageable2D
    {
        bool IsDead { get; }
        void ApplyDamage(int amount);
    }
}
