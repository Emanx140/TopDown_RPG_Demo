using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "All In Arc Targeting", menuName = "Top Down RPG/Gameplay/Ability Targeting/All In Arc", order = 3)]
    public sealed class AllInArcAbilityTargetingSo : RadiusAbilityTargetingSo
    {
        private const int MaxTargets = 32;

        [SerializeField, Range(0f, 360f)] private float angleDegrees = 90f;

        private readonly Collider[] candidates = new Collider[MaxTargets];

        public override void CollectTargets(AbilityTargetingContext context, List<IDamageable> results)
        {
            var count = CollectCandidates(context, candidates);
            var forward = Vector3.ProjectOnPlane(context.Forward, Vector3.up);

            if (forward.sqrMagnitude <= Mathf.Epsilon)
            {
                forward = Vector3.forward;
            }

            forward.Normalize();
            var halfAngle = angleDegrees * 0.5f;

            for (var index = 0; index < count; index++)
            {
                if (!TryGetDamageable(candidates[index], out var candidate) || candidate == context.Source)
                {
                    continue;
                }

                var toTarget = Vector3.ProjectOnPlane(candidates[index].transform.position - context.Origin, Vector3.up);
                if (toTarget.sqrMagnitude <= Mathf.Epsilon || Vector3.Angle(forward, toTarget) <= halfAngle)
                {
                    TryAddTarget(results, candidate);
                }
            }
        }
    }
}
