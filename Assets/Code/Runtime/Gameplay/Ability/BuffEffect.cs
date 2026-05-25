using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "Stat Modifier Effect", menuName = "Ability Effects/Stat Modifier", order = 1)]
    public sealed class BuffEffect : AbilityEffectSo
    {
        [SerializeField] private StatSo targetStat;
        [SerializeField] private StatModifierType modifierType = StatModifierType.Additive;
        [Tooltip("Additive uses raw stat points. Percent uses 0.2 for +20% and -0.2 for -20%.")]
        [SerializeField] private float value;

        public override bool AppliesOnTick => false;

        public override void Apply(ActiveAbilityEffect effect, IDamageable source, IDamageable target)
        {
            if (TryGetTargetStat(target, out var stat))
            {
                stat.AddModifier( new StatModifier(value, modifierType, effect));
            }
        }

        public override void Remove(ActiveAbilityEffect effect, IDamageable source, IDamageable target)
        {
            if (TryGetTargetStat(target, out var stat))
            {
                stat.RemoveModifier(effect, modifierType, value);
            }
        }

        private bool TryGetTargetStat(IDamageable target, out Stat stat)
        {
            if (target == null || targetStat == null)
            {
                stat = null;
                return false;
            }

            return target.TryGetStat(targetStat, out stat);
        }
    }
}
