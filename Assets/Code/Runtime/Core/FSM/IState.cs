namespace TopDownRPG.Core
{
    public interface IState
    {
        void Enter();
        void Tick(float deltaSeconds);
        void Exit();
    }
}
