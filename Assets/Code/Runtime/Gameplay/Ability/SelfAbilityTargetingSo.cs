using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "Self Targeting", menuName = "Top Down RPG/Gameplay/Ability Targeting/Self", order = 0)]
    public sealed class SelfAbilityTargetingSo : AbilityTargetingSo
    {
        public override void CollectTargets(AbilityTargetingContext context, List<IDamageable> results)
        {
            TryAddTarget(results, context.Source);
        }
    }
}
