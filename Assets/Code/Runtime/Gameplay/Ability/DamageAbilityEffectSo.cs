using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "Damage Effect", menuName = "Top Down RPG/Gameplay/Ability Effects/Damage", order = 0)]
    public sealed class DamageAbilityEffectSo : AbilityEffectSo
    {
        [SerializeField] private StatSo reduceStat;
        [SerializeField] private StatSo damageStat;

        public override void Apply(ActiveAbilityEffect effect, IDamageable source, IDamageable target)
        {
            if (source != null && target != null && source.TryGetStat(damageStat, out var damage))
            {
                target.ApplyDamage(reduceStat, damage);
            }
        }
    }
}
