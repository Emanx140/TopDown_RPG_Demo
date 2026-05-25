using TopDownRPG.Core;

namespace TopDownRPG.Gameplay
{
    public sealed class EnemyIdleState : IState
    {
        private readonly EnemyAiController controller;

        public EnemyIdleState(EnemyAiController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            controller.StopMoving();
        }

        public void Tick(float deltaSeconds)
        {
        }

        public void Exit()
        {
        }
    }
}
