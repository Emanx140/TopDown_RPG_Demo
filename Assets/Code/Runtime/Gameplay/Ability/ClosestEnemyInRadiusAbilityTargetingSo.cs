using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "Closest Enemy In Radius Targeting", menuName = "Top Down RPG/Gameplay/Ability Targeting/Closest Enemy In Radius", order = 1)]
    public sealed class ClosestEnemyInRadiusAbilityTargetingSo : RadiusAbilityTargetingSo
    {
        private const int MaxTargets = 32;
        private readonly Collider[] candidates = new Collider[MaxTargets];

        public override void CollectTargets(AbilityTargetingContext context, List<IDamageable> results)
        {
            IDamageable closestTarget = null;
            var closestDistanceSqr = float.MaxValue;
            var count = CollectCandidates(context, candidates);

            for (var index = 0; index < count; index++)
            {
                if (!TryGetDamageable(candidates[index], out var candidate) || candidate == context.Source)
                {
                    continue;
                }

                var candidatePosition = candidates[index].transform.position;
                var distanceSqr = (candidatePosition - context.Origin).sqrMagnitude;
                if (distanceSqr >= closestDistanceSqr)
                {
                    continue;
                }

                closestDistanceSqr = distanceSqr;
                closestTarget = candidate;
            }

            TryAddTarget(results, closestTarget);
        }
    }
}
