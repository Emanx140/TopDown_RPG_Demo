using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [RequireComponent(typeof(CharacterStats))]
    public sealed class EnemyAbilityRunner : AbilityRunner
    {
        [SerializeField] private CharacterStats sourceStats;
        [SerializeField] private List<AbilitySo> abilities = new List<AbilitySo>();

        protected override void Awake()
        {
            base.Awake();

            if (sourceStats == null)
            {
                sourceStats = GetComponent<CharacterStats>();
            }
        }

        public bool TryUseAnyReadyAbility(IDamageable target)
        {
            if (target == null || sourceStats == null)
            {
                return false;
            }

            foreach (var ability in abilities)
            {
                if (ability != null && !IsOnCooldown(ability))
                {
                    return TryUseAbility(ability, sourceStats, target);
                }
            }

            return false;
        }
    }
}
