using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "All In Radius Targeting", menuName = "Top Down RPG/Gameplay/Ability Targeting/All In Radius", order = 2)]
    public sealed class AllInRadiusAbilityTargetingSo : RadiusAbilityTargetingSo
    {
        private const int MaxTargets = 32;
        private readonly Collider[] candidates = new Collider[MaxTargets];

        public override void CollectTargets(AbilityTargetingContext context, List<IDamageable> results)
        {
            var count = CollectCandidates(context, candidates);

            for (var index = 0; index < count; index++)
            {
                if (TryGetDamageable(candidates[index], out var candidate) && candidate != context.Source)
                {
                    TryAddTarget(results, candidate);
                }
            }
        }
    }
}
