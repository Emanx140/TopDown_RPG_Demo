using TopDownRPG.Gameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TopDownRPG.Infrastructure
{
    public sealed class LevelLifetimeScope : LifetimeScope
    {
        [SerializeField] private EnemyFactory enemyFactory;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameStateController>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<PlayerController>();
            builder.RegisterComponentInHierarchy<PlayerAnimationController>();
            builder.RegisterComponentInHierarchy<PlayerAbilityRunner>();
            builder.RegisterComponentInHierarchy<AbilityPresenter>();

            if (enemyFactory == null)
            {
                enemyFactory = GetComponentInChildren<EnemyFactory>(true);
            }

            if (enemyFactory != null)
            {
                builder.RegisterComponent(enemyFactory).As<IEnemyFactory>();
            }
        }
    }
}
