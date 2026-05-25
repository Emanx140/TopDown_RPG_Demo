using TopDownRPG.Core;

namespace TopDownRPG.Gameplay
{
    public sealed class EnemyChaseState : IState
    {
        private readonly EnemyAiController controller;

        public EnemyChaseState(EnemyAiController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            controller.ResumeMoving();
        }

        public void Tick(float deltaSeconds)
        {
            controller.ChaseTarget(deltaSeconds);
        }

        public void Exit()
        {
        }
    }
}
