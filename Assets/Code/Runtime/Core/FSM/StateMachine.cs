using System;
using System.Collections.Generic;

namespace TopDownRPG.Core
{
    public sealed class StateMachine
    {
        private readonly Dictionary<Type, IState> states = new Dictionary<Type, IState>();
        private readonly List<StateTransition> transitions = new List<StateTransition>();

        private IState currentState;

        public Type CurrentStateType => currentState?.GetType();

        public void AddState(IState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            states[state.GetType()] = state;
        }

        public void AddTransition(StateTransition transition)
        {
            if (transition == null)
            {
                throw new ArgumentNullException(nameof(transition));
            }

            transitions.Add(transition);
        }

        public void ChangeState<TState>() where TState : IState
        {
            ChangeState(typeof(TState));
        }

        public void ChangeState(Type stateType)
        {
            if (stateType == null)
            {
                throw new ArgumentNullException(nameof(stateType));
            }

            if (currentState != null && currentState.GetType() == stateType)
            {
                return;
            }

            if (!states.TryGetValue(stateType, out var nextState))
            {
                throw new InvalidOperationException($"State '{stateType.Name}' is not registered.");
            }

            currentState?.Exit();
            currentState = nextState;
            currentState.Enter();
        }

        public void Tick(float deltaSeconds)
        {
            currentState?.Tick(deltaSeconds);
            TickTransitions();
        }

        private void TickTransitions()
        {
            if (currentState == null)
            {
                return;
            }

            foreach (var transition in transitions)
            {
                if (transition.FromStateType != currentState.GetType() || !transition.CanTransition())
                {
                    continue;
                }

                ChangeState(transition.ToStateType);
                return;
            }
        }
    }
}
