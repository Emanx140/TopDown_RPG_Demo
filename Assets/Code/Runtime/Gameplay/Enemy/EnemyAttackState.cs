using TopDownRPG.Core;

namespace TopDownRPG.Gameplay
{
    public sealed class EnemyAttackState : IState
    {
        private readonly EnemyAiController controller;

        public EnemyAttackState(EnemyAiController controller)
        {
            this.controller = controller;
        }

        public void Enter()
        {
            controller.StopMoving();
        }

        public void Tick(float deltaSeconds)
        {
            controller.FaceTarget(deltaSeconds);
            controller.TryAttackTarget();
        }

        public void Exit()
        {
        }
    }
}
