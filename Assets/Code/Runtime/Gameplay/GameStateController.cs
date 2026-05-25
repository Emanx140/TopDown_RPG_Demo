using System;
using TopDownRPG.Infrastructure;
using UnityEngine;
using VContainer.Unity;

namespace TopDownRPG.Gameplay
{
    public class GameStateController : IGameStateService, IInitializable, IDisposable
    {
        private readonly IPlayerInput playerInput;
        private readonly IGameLogger logger;

        public event Action<GameState> StateChanged;

        public GameState CurrentState { get; private set; } = GameState.Playing;
        public bool IsPaused => CurrentState == GameState.Paused;

        public GameStateController(IPlayerInput playerInput, IGameLogger logger)
        {
            this.playerInput = playerInput ?? throw new ArgumentNullException(nameof(playerInput));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            playerInput.PausePressed += TogglePause;
            ApplyState(CurrentState);
        }

        public void Dispose()
        {
            playerInput.PausePressed -= TogglePause;
            SetState(GameState.Playing);
        }

        public void TogglePause()
        {
            SetState(IsPaused ? GameState.Playing : GameState.Paused);
        }

        public void SetState(GameState state)
        {
            if (CurrentState == state)
            {
                return;
            }

            CurrentState = state;
            ApplyState(state);
            StateChanged?.Invoke(state);
            logger.Log($"Game state changed to {state}.");
        }

        private static void ApplyState(GameState state)
        {
            Time.timeScale = state == GameState.Paused ? 0f : 1f;
        }
    }
}
