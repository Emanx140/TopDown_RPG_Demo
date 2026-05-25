using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public abstract class AbilityEffectSo : BaseUISo, IAbilityEffect
    {
        [SerializeField, Min(0f)] private float durationSeconds;
        [SerializeField, Min(0f)] private float tickEverySeconds;

        public float DurationSeconds => durationSeconds;
        public float TickEverySeconds => tickEverySeconds;
        public virtual bool AppliesOnTick => tickEverySeconds > 0f;

        public abstract void Apply(ActiveAbilityEffect effect, IDamageable source, IDamageable target);

        public virtual void Remove(ActiveAbilityEffect effect,IDamageable source, IDamageable target)
        {
        }
    }
}
