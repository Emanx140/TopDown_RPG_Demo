using System;
using UnityEngine;
using VContainer;

namespace TopDownRPG.Gameplay
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private CharacterStats characterStats;
        [SerializeField] private StatSo moveSpeedStat;
        [SerializeField] private StatSo sprintSpeedStat;
        [SerializeField, Min(0f)] private float rotationSpeed = 720f;
        [SerializeField] private float gravity = -30f;
        [SerializeField] private float groundedVelocity = -2f;

        private IPlayerInput input;
        private PlayerMovementMotor motor;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
        }

        private void Reset()
        {
            characterController = GetComponent<CharacterController>();
            characterStats = GetComponent<CharacterStats>();
        }

        private void Awake()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStats>();
            }

            motor = new PlayerMovementMotor(gravity, groundedVelocity);
        }

        private void Update()
        {
            if (input == null || characterController == null || motor == null)
            {
                return;
            }

            var frame = motor.Tick(
                input.Move,
                input.SprintHeld,
                characterController.isGrounded,
                GetStatValue(moveSpeedStat),
                GetStatValue(sprintSpeedStat),
                Time.deltaTime);
            ApplyRotation(frame.FacingDirection);
            characterController.Move(frame.Velocity * Time.deltaTime);
        }

        private float GetStatValue(StatSo statDefinition)
        {
            if (characterStats != null && characterStats.TryGetStat(statDefinition, out var stat))
            {
                return stat.Value;
            }

            throw new InvalidOperationException($"Stat {statDefinition.name} not found");
        }

        private void ApplyRotation(Vector3? facingDirection)
        {
            if (!facingDirection.HasValue)
            {
                return;
            }

            var targetRotation = Quaternion.LookRotation(facingDirection.Value, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }
}
