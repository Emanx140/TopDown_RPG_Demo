using System.Collections.Generic;
using TopDownRPG.Core;
using UnityEngine;
using VContainer;

namespace TopDownRPG.Gameplay
{
    public abstract class AbilityRunner : MonoBehaviour
    {
        private class CooldownState
        {
            private readonly Timer timer;

            public CooldownState(AbilitySo ability)
            {
                Ability = ability;
                timer = new Timer(ability.CooldownSeconds);
            }

            public AbilitySo Ability { get; }
            public bool IsCompleted => timer.IsCompleted;
            public float RemainingSeconds => timer.RemainingSeconds;

            public void Tick(float deltaSeconds)
            {
                timer.Tick(deltaSeconds);
            }
        }

        private AbilityPresenter abilityPresenter;

        [SerializeField] private CharacterAnimationController animationController;

        private readonly List<CooldownState> cooldowns = new List<CooldownState>();
        private readonly List<IDamageable> targetBuffer = new List<IDamageable>();

        [Inject]
        public void ConstructAbilityPresenter(AbilityPresenter abilityPresenter)
        {
            this.abilityPresenter = abilityPresenter;
        }

        protected virtual void Awake()
        {
            if (animationController == null)
            {
                animationController = GetComponentInChildren<CharacterAnimationController>();
            }
        }

        protected virtual void Update()
        {
            TickCooldowns(Time.deltaTime);
        }

        protected bool TryUseAbility(AbilitySo ability, IDamageable source, IDamageable target)
        {
            if (ability == null || source == null || IsOnCooldown(ability))
            {
                return false;
            }

            CollectTargets(ability, source, target);

            foreach (var receiver in targetBuffer)
            {
                var runtimeAbility = new RuntimeAbility(ability, source, receiver);
                receiver.ReceiveAbility(runtimeAbility);
            }

            targetBuffer.Clear();
            StartCooldown(ability);
            animationController?.PlayAbility(ability);
            abilityPresenter?.PlayCast(ability, GetCastPosition(ability), GetCastRotation(ability));
            return true;
        }

        protected virtual Vector3 GetCastPosition(AbilitySo ability)
        {
            return transform.position;
        }

        protected virtual Quaternion GetCastRotation(AbilitySo ability)
        {
            return transform.rotation;
        }

        private void CollectTargets(AbilitySo ability, IDamageable source, IDamageable explicitTarget)
        {
            targetBuffer.Clear();

            if (ability.Targeting == null)
            {
                targetBuffer.Add(explicitTarget ?? source);
                return;
            }

            var context = new AbilityTargetingContext(
                source,
                GetCastPosition(ability),
                GetCastRotation(ability) * Vector3.forward);

            ability.Targeting.CollectTargets(context, targetBuffer);
        }

        private void TickCooldowns(float deltaSeconds)
        {
            for (var index = cooldowns.Count - 1; index >= 0; index--)
            {
                var cooldown = cooldowns[index];
                cooldown.Tick(deltaSeconds);

                if (cooldown.IsCompleted)
                {
                    cooldowns.RemoveAt(index);
                }
            }
        }

        public bool IsOnCooldown(AbilitySo ability)
        {
            return TryGetCooldownRemaining(ability, out _);
        }

        public bool TryGetCooldownRemaining(AbilitySo ability, out float remainingSeconds)
        {
            foreach (var cooldown in cooldowns)
            {
                if (cooldown.Ability == ability)
                {
                    remainingSeconds = cooldown.RemainingSeconds;
                    return true;
                }
            }

            remainingSeconds = 0f;
            return false;
        }

        private void StartCooldown(AbilitySo ability)
        {
            if (ability.CooldownSeconds <= 0f)
            {
                return;
            }

            cooldowns.Add(new CooldownState(ability));
        }
    }
}
