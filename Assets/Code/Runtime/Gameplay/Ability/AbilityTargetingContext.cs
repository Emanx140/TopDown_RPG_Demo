using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public readonly struct AbilityTargetingContext
    {
        public AbilityTargetingContext(
            IDamageable source,
            Vector3 origin,
            Vector3 forward)
        {
            Source = source;
            Origin = origin;
            Forward = forward;
        }

        public IDamageable Source { get; }
        public Vector3 Origin { get; }
        public Vector3 Forward { get; }
    }
}
