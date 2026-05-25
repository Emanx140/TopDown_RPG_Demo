namespace TopDownRPG.Gameplay
{
    public interface IAbility
    {
        bool IsActive { get; }

        bool Apply();
        void Tick(float deltaSeconds);
    }
}
