using System;
using TopDownRPG.Core;

namespace TopDownRPG.Gameplay
{
    public class ActiveAbilityEffect : IDisposable
    {
        private readonly IAbilityEffect effect;
        private readonly IDamageable source;
        private readonly IDamageable target;
        private readonly Timer durationTimer;

        public ActiveAbilityEffect(IAbilityEffect effect, IDamageable source, IDamageable target)
        {
            this.effect = effect ?? throw new ArgumentNullException(nameof(effect));
            this.source = source ?? throw new ArgumentNullException(nameof(source));
            this.target = target ?? throw new ArgumentNullException(nameof(target));

            durationTimer = new Timer(effect.DurationSeconds, effect.TickEverySeconds);
            durationTimer.Ticked += ApplyTick;
            durationTimer.Completed += Complete;
        }

        public bool IsActive { get; private set; }
        public bool IsPermanent => durationTimer.IsPermanent;
        public float RemainingSeconds => durationTimer.RemainingSeconds;

        public void Start()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            effect.Apply(this, source, target);

            if (durationTimer.IsInstant)
            {
                Complete();
            }
        }

        public void Tick(float deltaSeconds)
        {
            if (!IsActive)
            {
                return;
            }

            durationTimer.Tick(deltaSeconds);
        }

        public void Dispose()
        {
            if (!IsActive)
            {
                return;
            }

            Complete();
        }

        private void ApplyTick()
        {
            if (!effect.AppliesOnTick)
            {
                return;
            }

            effect.Apply(this, source, target);
        }

        private void Complete()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            durationTimer.Ticked -= ApplyTick;
            durationTimer.Completed -= Complete;

            if (!durationTimer.IsInstant && !durationTimer.IsPermanent)
            {
                effect.Remove(this, source, target);
            }
        }
    }
}
