using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public abstract class AbilityTargetingSo : ScriptableObject
    {
        public abstract void CollectTargets(AbilityTargetingContext context, List<IDamageable> results);

        protected static bool TryAddTarget(List<IDamageable> results, IDamageable target)
        {
            if (target == null || results.Contains(target))
            {
                return false;
            }

            results.Add(target);
            return true;
        }

        protected static bool TryGetDamageable(Collider collider, out IDamageable damageable)
        {
            if (collider == null)
            {
                damageable = null;
                return false;
            }

            damageable = collider.GetComponentInParent<IDamageable>();
            return damageable != null;
        }
    }
}
