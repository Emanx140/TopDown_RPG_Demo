using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public readonly struct PlayerMovementFrame
    {
        public PlayerMovementFrame(Vector3 velocity, Vector3? facingDirection)
        {
            Velocity = velocity;
            FacingDirection = facingDirection;
        }

        public Vector3 Velocity { get; }
        public Vector3? FacingDirection { get; }
    }
}
