using System;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public interface IPlayerInput
    {
        event Action PausePressed;
        event Action<PlayerAbilitySlotId> AbilitySlotPressed;

        Vector2 Move { get; }
        bool SprintHeld { get; }
    }
}
