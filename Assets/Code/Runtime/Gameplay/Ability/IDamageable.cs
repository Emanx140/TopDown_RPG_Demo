namespace TopDownRPG.Gameplay
{
    public interface IDamageable
    {
        bool TryGetStat(StatSo statDefinition, out Stat stat);
        void ApplyDamage(StatSo targetStat, Stat damage);
        bool ReceiveAbility(IAbility runtimeAbility);
    }
}
