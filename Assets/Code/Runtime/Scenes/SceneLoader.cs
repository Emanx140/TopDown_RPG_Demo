using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TopDownRPG.Infrastructure;
using UnityEngine.SceneManagement;

namespace TopDownRPG.Scenes
{
    public class SceneLoader : ISceneLoader
    {
        private readonly IGameLogger logger;
        private readonly ISceneTransition sceneTransition;
        private readonly HashSet<string> additiveScenes = new();

        public bool IsLoading { get; private set; }

        public SceneLoader(IGameLogger logger, ISceneTransition sceneTransition)
        {
            this.logger = logger;
            this.sceneTransition = sceneTransition;
        }

        public async UniTask LoadAsync(SceneReference scene, CancellationToken cancellationToken = default)
        {
            ValidateScene(scene);

            if (!TryBeginLoading())
            {
                return;
            }

            try
            {
                await LoadSingleWithTransitionAsync(scene, cancellationToken);
                additiveScenes.Clear();
            }
            finally
            {
                EndLoading();
            }
        }

        public async UniTask LoadAdditiveAsync(SceneReference scene, CancellationToken cancellationToken = default)
        {
            ValidateScene(scene);

            if (!TryBeginLoading())
            {
                return;
            }

            try
            {
                if (IsAdditiveLoaded(scene.SceneName))
                {
                    additiveScenes.Add(scene.SceneName);
                    return;
                }

                await LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive, cancellationToken);
                additiveScenes.Add(scene.SceneName);
            }
            finally
            {
                EndLoading();
            }
        }

        private bool IsAdditiveLoaded(string sceneName)
        {
            return additiveScenes.Contains(sceneName) || SceneManager.GetSceneByName(sceneName).isLoaded;
        }

        private async UniTask LoadSingleWithTransitionAsync(SceneReference scene, CancellationToken cancellationToken)
        {
            if (sceneTransition != null)
            {
                await sceneTransition.FadeOutAsync(cancellationToken);
            }

            await LoadSceneAsync(scene.SceneName, LoadSceneMode.Single, cancellationToken);

            if (sceneTransition != null)
            {
                await sceneTransition.FadeInAsync(cancellationToken);
            }
        }

        private static async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode, CancellationToken cancellationToken)
        {
            await SceneManager.LoadSceneAsync(sceneName, mode).ToUniTask(cancellationToken: cancellationToken);
        }

        private bool TryBeginLoading()
        {
            if (IsLoading)
            {
                logger.Warning("Scene load ignored because another scene operation is running.");
                return false;
            }

            IsLoading = true;
            return true;
        }

        private void EndLoading()
        {
            IsLoading = false;
        }

        private static void ValidateScene(SceneReference scene)
        {
            if (scene == null)
            {
                throw new ArgumentNullException(nameof(scene));
            }

            if (!scene.IsValid)
            {
                throw new ArgumentException("Scene reference must have a scene name.", nameof(scene));
            }
        }
    }
}
