using System;
using TopDownRPG.Infrastructure;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace TopDownRPG.Gameplay
{
    public class PlayerInputReader : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string playerMapName = "Player";
        [SerializeField] private string uiMapName = "UI";
        [SerializeField] private string moveActionName = "Move";
        [SerializeField] private string sprintActionName = "Sprint";
        [SerializeField] private string pauseActionName = "Cancel";
        [SerializeField] private string primaryAbilityMapName = "Player";
        [SerializeField] private string primaryAbilityActionName = "Attack";
        [SerializeField] private string secondaryAbilityMapName = "UI";
        [SerializeField] private string secondaryAbilityActionName = "RightClick";
        [SerializeField] private string mobilityAbilityMapName = "Player";
        [SerializeField] private string mobilityAbilityActionName = "Jump";

        private InputAction moveAction;
        private InputAction sprintAction;
        private InputAction pauseAction;
        private InputAction primaryAbilityAction;
        private InputAction secondaryAbilityAction;
        private InputAction mobilityAbilityAction;
        private IGameLogger logger;
        private bool isBound;

        public event Action PausePressed;
        public event Action<PlayerAbilitySlotId> AbilitySlotPressed;

        public Vector2 Move => moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        public bool SprintHeld => sprintAction?.IsPressed() == true;


        [Inject]
        public void Construct(IGameLogger logger)
        {
            this.logger = logger;
        }

        private void OnValidate()
        {
            playerMapName = playerMapName?.Trim();
            uiMapName = uiMapName?.Trim();
            moveActionName = moveActionName?.Trim();
            sprintActionName = sprintActionName?.Trim();
            pauseActionName = pauseActionName?.Trim();
            primaryAbilityMapName = primaryAbilityMapName?.Trim();
            primaryAbilityActionName = primaryAbilityActionName?.Trim();
            secondaryAbilityMapName = secondaryAbilityMapName?.Trim();
            secondaryAbilityActionName = secondaryAbilityActionName?.Trim();
            mobilityAbilityMapName = mobilityAbilityMapName?.Trim();
            mobilityAbilityActionName = mobilityAbilityActionName?.Trim();
        }

        private void Start()
        {
            BindActions();
        }

        private void OnEnable()
        {
            if (!isBound)
            {
                return;
            }

            inputActions.Enable();
            pauseAction.performed += OnPausePerformed;
            primaryAbilityAction.performed += OnPrimaryAbilityPerformed;
            secondaryAbilityAction.performed += OnSecondaryAbilityPerformed;
            mobilityAbilityAction.performed += OnMobilityAbilityPerformed;
        }

        private void OnDisable()
        {
            if (isBound)
            {
                pauseAction.performed -= OnPausePerformed;
                primaryAbilityAction.performed -= OnPrimaryAbilityPerformed;
                secondaryAbilityAction.performed -= OnSecondaryAbilityPerformed;
                mobilityAbilityAction.performed -= OnMobilityAbilityPerformed;
            }

            inputActions?.Disable();
        }

        private void BindActions()
        {
            if (inputActions == null)
            {
                LogError($"{nameof(PlayerInputReader)} has no input actions assigned.");
                return;
            }

            if (string.IsNullOrWhiteSpace(playerMapName) ||
                string.IsNullOrWhiteSpace(uiMapName) ||
                string.IsNullOrWhiteSpace(moveActionName) ||
                string.IsNullOrWhiteSpace(sprintActionName) ||
                string.IsNullOrWhiteSpace(pauseActionName) ||
                string.IsNullOrWhiteSpace(primaryAbilityMapName) ||
                string.IsNullOrWhiteSpace(primaryAbilityActionName) ||
                string.IsNullOrWhiteSpace(secondaryAbilityMapName) ||
                string.IsNullOrWhiteSpace(secondaryAbilityActionName) ||
                string.IsNullOrWhiteSpace(mobilityAbilityMapName) ||
                string.IsNullOrWhiteSpace(mobilityAbilityActionName))
            {
                LogError($"{nameof(PlayerInputReader)} has an empty input action map or action name.");
                return;
            }

            var playerMap = inputActions.FindActionMap(playerMapName, false);
            if (playerMap == null)
            {
                LogError($"{nameof(PlayerInputReader)} could not find input action map '{playerMapName}'.");
                return;
            }

            var uiMap = inputActions.FindActionMap(uiMapName, false);
            if (uiMap == null)
            {
                LogError($"{nameof(PlayerInputReader)} could not find input action map '{uiMapName}'.");
                return;
            }

            moveAction = playerMap.FindAction(moveActionName, false);
            sprintAction = playerMap.FindAction(sprintActionName, false);
            pauseAction = uiMap.FindAction(pauseActionName, false);
            primaryAbilityAction = FindAction(primaryAbilityMapName, primaryAbilityActionName);
            secondaryAbilityAction = FindAction(secondaryAbilityMapName, secondaryAbilityActionName);
            mobilityAbilityAction = FindAction(mobilityAbilityMapName, mobilityAbilityActionName);

            if (moveAction == null ||
                sprintAction == null ||
                pauseAction == null ||
                primaryAbilityAction == null ||
                secondaryAbilityAction == null ||
                mobilityAbilityAction == null)
            {
                LogError($"{nameof(PlayerInputReader)} could not bind one or more input actions.");
                return;
            }

            isBound = true;
            inputActions.Enable();
            pauseAction.performed += OnPausePerformed;
            primaryAbilityAction.performed += OnPrimaryAbilityPerformed;
            secondaryAbilityAction.performed += OnSecondaryAbilityPerformed;
            mobilityAbilityAction.performed += OnMobilityAbilityPerformed;
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            PausePressed?.Invoke();
        }

        private void OnPrimaryAbilityPerformed(InputAction.CallbackContext context)
        {
            AbilitySlotPressed?.Invoke(PlayerAbilitySlotId.Primary);
        }

        private void OnSecondaryAbilityPerformed(InputAction.CallbackContext context)
        {
            AbilitySlotPressed?.Invoke(PlayerAbilitySlotId.Secondary);
        }

        private void OnMobilityAbilityPerformed(InputAction.CallbackContext context)
        {
            AbilitySlotPressed?.Invoke(PlayerAbilitySlotId.Mobility);
        }

        private InputAction FindAction(string mapName, string actionName)
        {
            var actionMap = inputActions.FindActionMap(mapName, false);
            if (actionMap == null)
            {
                LogError($"{nameof(PlayerInputReader)} could not find input action map '{mapName}'.");
                return null;
            }

            var action = actionMap.FindAction(actionName, false);
            if (action == null)
            {
                LogError($"{nameof(PlayerInputReader)} could not find input action '{actionName}' in map '{mapName}'.");
            }

            return action;
        }

        private void LogError(string message)
        {
            if (logger != null)
            {
                logger.Error(message);
                return;
            }

            Debug.LogError(message, this);
        }
    }
}
