using System;
using TopDownRPG.Infrastructure;
using UnityEngine;
using VContainer;

namespace TopDownRPG.Gameplay
{
    public sealed class PlayerAnimationController : CharacterAnimationController
    {
        [SerializeField] private string moveSpeedParameter = "MoveSpeed";
        [SerializeField] private string isMovingParameter = "IsMoving";
        [SerializeField] private string isSprintingParameter = "IsSprinting";
        [SerializeField, Min(0f)] private float movementDeadZone = 0.01f;

        private IPlayerInput input;
        private IGameLogger logger;
        private int moveSpeedHash;
        private int isMovingHash;
        private int isSprintingHash;
        private bool hasMoveSpeedParameter;
        private bool hasIsMovingParameter;
        private bool hasIsSprintingParameter;

        [Inject]
        public void Construct(IPlayerInput input, IGameLogger logger)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void Awake()
        {
            base.Awake();
            CacheAnimatorParameters();
        }

        protected override void Update()
        {
            base.Update();

            if (input == null || Animator == null)
            {
                return;
            }

            var moveAmount = Mathf.Clamp01(input.Move.magnitude);
            var isMoving = moveAmount > movementDeadZone;
            var isSprinting = isMoving && input.SprintHeld;

            if (hasMoveSpeedParameter)
            {
                SetAnimatorFloat(moveSpeedHash, moveAmount);
            }

            if (hasIsMovingParameter)
            {
                SetAnimatorBool(isMovingHash, isMoving);
            }

            if (hasIsSprintingParameter)
            {
                SetAnimatorBool(isSprintingHash, isSprinting);
            }
        }

        private void OnValidate()
        {
            moveSpeedParameter = moveSpeedParameter?.Trim();
            isMovingParameter = isMovingParameter?.Trim();
            isSprintingParameter = isSprintingParameter?.Trim();
        }

        private void CacheAnimatorParameters()
        {
            if (Animator == null)
            {
                logger?.Warning($"{nameof(PlayerAnimationController)} has no Animator assigned.");
                return;
            }

            if (HasEmptyParameterName())
            {
                logger?.Warning($"{nameof(PlayerAnimationController)} has one or more empty Animator parameter names.");
                return;
            }

            moveSpeedHash = Animator.StringToHash(moveSpeedParameter);
            isMovingHash = Animator.StringToHash(isMovingParameter);
            isSprintingHash = Animator.StringToHash(isSprintingParameter);

            hasMoveSpeedParameter = HasParameter(moveSpeedHash, AnimatorControllerParameterType.Float);
            hasIsMovingParameter = HasParameter(isMovingHash, AnimatorControllerParameterType.Bool);
            hasIsSprintingParameter = HasParameter(isSprintingHash, AnimatorControllerParameterType.Bool);
        }

        private bool HasEmptyParameterName()
        {
            return string.IsNullOrWhiteSpace(moveSpeedParameter) ||
                   string.IsNullOrWhiteSpace(isMovingParameter) ||
                   string.IsNullOrWhiteSpace(isSprintingParameter);
        }
    }
}
