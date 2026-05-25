using NUnit.Framework;
using TopDownRPG.Core;

namespace TopDownRPG.Tests.Core.FSM
{
    public sealed class StateMachineTests
    {
        [Test]
        public void ChangeState_EntersSelectedState()
        {
            var state = new TestState();
            var stateMachine = new StateMachine();
            stateMachine.AddState(state);

            stateMachine.ChangeState<TestState>();

            Assert.That(state.EnterCount, Is.EqualTo(1));
            Assert.That(stateMachine.CurrentStateType, Is.EqualTo(typeof(TestState)));
        }

        [Test]
        public void ChangeState_WhenChangingToAnotherState_ExitsPreviousState()
        {
            var firstState = new TestState();
            var secondState = new OtherTestState();
            var stateMachine = new StateMachine();
            stateMachine.AddState(firstState);
            stateMachine.AddState(secondState);

            stateMachine.ChangeState<TestState>();
            stateMachine.ChangeState<OtherTestState>();

            Assert.That(firstState.ExitCount, Is.EqualTo(1));
            Assert.That(secondState.EnterCount, Is.EqualTo(1));
        }

        [Test]
        public void Tick_ForwardsDeltaTimeToCurrentState()
        {
            var state = new TestState();
            var stateMachine = new StateMachine();
            stateMachine.AddState(state);
            stateMachine.ChangeState<TestState>();

            stateMachine.Tick(0.5f);

            Assert.That(state.TickCount, Is.EqualTo(1));
            Assert.That(state.LastDeltaSeconds, Is.EqualTo(0.5f));
        }

        [Test]
        public void Tick_WhenTransitionConditionPasses_ChangesState()
        {
            var firstState = new TestState();
            var secondState = new OtherTestState();
            var shouldTransition = false;
            var stateMachine = new StateMachine();
            stateMachine.AddState(firstState);
            stateMachine.AddState(secondState);
            stateMachine.AddTransition(new StateTransition(typeof(TestState), typeof(OtherTestState), () => shouldTransition));
            stateMachine.ChangeState<TestState>();

            shouldTransition = true;
            stateMachine.Tick(0.1f);

            Assert.That(stateMachine.CurrentStateType, Is.EqualTo(typeof(OtherTestState)));
            Assert.That(firstState.ExitCount, Is.EqualTo(1));
            Assert.That(secondState.EnterCount, Is.EqualTo(1));
        }

        private class TestState : IState
        {
            public int EnterCount { get; private set; }
            public int TickCount { get; private set; }
            public int ExitCount { get; private set; }
            public float LastDeltaSeconds { get; private set; }

            public void Enter()
            {
                EnterCount++;
            }

            public void Tick(float deltaSeconds)
            {
                TickCount++;
                LastDeltaSeconds = deltaSeconds;
            }

            public void Exit()
            {
                ExitCount++;
            }
        }

        private sealed class OtherTestState : TestState
        {
        }
    }
}
