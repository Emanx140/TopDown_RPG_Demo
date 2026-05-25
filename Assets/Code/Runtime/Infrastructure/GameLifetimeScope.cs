using TopDownRPG.Audio;
using TopDownRPG.Gameplay;
using TopDownRPG.Scenes;
using VContainer;
using VContainer.Unity;
using UnityEngine;

namespace TopDownRPG.Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] private ScreenFadeTransition sceneTransition;
        [SerializeField] private AudioService audioService;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IGameLogger, UnityGameLogger>(Lifetime.Singleton);

            if (sceneTransition == null)
            {
                sceneTransition = GetComponentInChildren<ScreenFadeTransition>(true);
            }

            if (sceneTransition != null)
            {
                builder.RegisterComponent(sceneTransition).As<ISceneTransition>();
            }
            else
            {
                Debug.LogWarning($"{nameof(GameLifetimeScope)} has no scene transition assigned. Scene loads will run without fades.");
                builder.Register<NoOpSceneTransition>(Lifetime.Singleton).As<ISceneTransition>();
            }

            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Singleton);

            if (audioService == null)
            {
                audioService = GetComponentInChildren<AudioService>(true);
            }

            if (audioService != null)
            {
                builder.RegisterComponent(audioService).As<IAudioService>();
            }

            builder.RegisterComponentInHierarchy<Bootstrapper>();
            builder.RegisterComponentInHierarchy<PlayerInputReader>().AsImplementedInterfaces();
        }
    }
}
