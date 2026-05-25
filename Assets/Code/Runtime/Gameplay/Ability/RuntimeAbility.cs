using System;
using System.Collections.Generic;

namespace TopDownRPG.Gameplay
{
    public sealed class RuntimeAbility : IAbility
    {
        private readonly AbilitySo definition;
        private readonly IDamageable source;
        private readonly IDamageable target;
        private readonly List<ActiveAbilityEffect> activeEffects = new List<ActiveAbilityEffect>();

        public RuntimeAbility(AbilitySo definition, IDamageable source, IDamageable target)
        {
            this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.target = target;
        }

        public AbilitySo Definition => definition;
        public bool IsActive => activeEffects.Count > 0;

        public bool Apply()
        {
            foreach (var effect in definition.EffectAssets)
            {
                if (effect == null)
                {
                    continue;
                }

                var activeEffect = new ActiveAbilityEffect(effect, source, target);
                activeEffect.Start();

                if (activeEffect.IsActive)
                {
                    activeEffects.Add(activeEffect);
                }
            }

            return true;
        }

        public void Tick(float deltaSeconds)
        {
            for (var index = activeEffects.Count - 1; index >= 0; index--)
            {
                var activeEffect = activeEffects[index];
                activeEffect.Tick(deltaSeconds);

                if (!activeEffect.IsActive)
                {
                    activeEffects.RemoveAt(index);
                }
            }
        }
    }
}
