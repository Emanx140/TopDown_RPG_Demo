namespace TopDownRPG.Gameplay
{
    public interface IAbilityEffect
    {
        float DurationSeconds { get; }
        float TickEverySeconds { get; }
        bool AppliesOnTick { get; }

        void Remove(ActiveAbilityEffect effect, IDamageable source, IDamageable target);
        void Apply(ActiveAbilityEffect effect, IDamageable source, IDamageable target);
    }
}
