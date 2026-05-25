using System;

namespace TopDownRPG.Core
{
    public sealed class StateTransition
    {
        private readonly Func<bool> condition;

        public StateTransition(Type fromStateType, Type toStateType, Func<bool> condition)
        {
            if (fromStateType == null)
            {
                throw new ArgumentNullException(nameof(fromStateType));
            }

            if (toStateType == null)
            {
                throw new ArgumentNullException(nameof(toStateType));
            }

            if (!typeof(IState).IsAssignableFrom(fromStateType))
            {
                throw new ArgumentException($"State type must implement {nameof(IState)}.", nameof(fromStateType));
            }

            if (!typeof(IState).IsAssignableFrom(toStateType))
            {
                throw new ArgumentException($"State type must implement {nameof(IState)}.", nameof(toStateType));
            }

            FromStateType = fromStateType;
            ToStateType = toStateType;
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
        }

        public Type FromStateType { get; }
        public Type ToStateType { get; }

        public bool CanTransition()
        {
            return condition.Invoke();
        }
    }
}
