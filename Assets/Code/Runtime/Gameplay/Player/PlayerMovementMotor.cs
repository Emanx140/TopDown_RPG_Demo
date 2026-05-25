using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public sealed class PlayerMovementMotor
    {
        private const float MoveEpsilon = 0.001f;

        private readonly float gravity;
        private readonly float groundedVelocity;

        private Vector3 verticalVelocity;

        public PlayerMovementMotor(float gravity, float groundedVelocity)
        {
            this.gravity = gravity;
            this.groundedVelocity = groundedVelocity;
        }

        public PlayerMovementFrame Tick(Vector2 moveInput, bool isSprinting, bool isGrounded, float moveSpeed, float sprintSpeed, float deltaTime)
        {
            var clampedInput = Vector2.ClampMagnitude(moveInput, 1f);
            var moveDirection = new Vector3(clampedInput.x, 0f, clampedInput.y);

            UpdateGravity(isGrounded, deltaTime);

            var normalizedMoveSpeed = Mathf.Max(0f, moveSpeed);
            var normalizedSprintSpeed = Mathf.Max(normalizedMoveSpeed, sprintSpeed);
            var speed = isSprinting ? normalizedSprintSpeed : normalizedMoveSpeed;
            var horizontalVelocity = moveDirection * speed;
            var facingDirection = GetFacingDirection(moveDirection);

            return new PlayerMovementFrame(horizontalVelocity + verticalVelocity, facingDirection);
        }

        private void UpdateGravity(bool isGrounded, float deltaTime)
        {
            if (isGrounded && verticalVelocity.y < 0f)
            {
                verticalVelocity.y = groundedVelocity;
                return;
            }

            verticalVelocity.y += gravity * deltaTime;
        }

        private static Vector3? GetFacingDirection(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude <= MoveEpsilon)
            {
                return null;
            }

            return moveDirection;
        }
    }
}
