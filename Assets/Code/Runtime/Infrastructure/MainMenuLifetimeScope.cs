using TopDownRPG.UI;
using VContainer;
using VContainer.Unity;

namespace TopDownRPG.Infrastructure
{
    public sealed class MainMenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MainMenuController>();
        }
    }
}
