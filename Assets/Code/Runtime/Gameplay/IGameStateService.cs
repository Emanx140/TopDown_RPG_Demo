using System;

namespace TopDownRPG.Gameplay
{
    public interface IGameStateService
    {
        event Action<GameState> StateChanged;

        GameState CurrentState { get; }
        bool IsPaused { get; }

        void SetState(GameState state);
        void TogglePause();
    }
}
