using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public abstract class RadiusAbilityTargetingSo : AbilityTargetingSo
    {
        [SerializeField, Min(0f)] private float radius = 2f;
        [SerializeField] private LayerMask targetLayers = ~0;
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        public float Radius => radius;
        protected LayerMask TargetLayers => targetLayers;

        protected int CollectCandidates(AbilityTargetingContext context, Collider[] buffer)
        {
            if (radius <= 0f)
            {
                return 0;
            }

            return Physics.OverlapSphereNonAlloc(context.Origin, radius, buffer, targetLayers, triggerInteraction);
        }
    }
}
