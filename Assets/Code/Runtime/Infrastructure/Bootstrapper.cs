using System;
using TopDownRPG.Scenes;
using UnityEngine;
using VContainer;

namespace TopDownRPG.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private SceneReference initialScene;

        private static Bootstrapper instance;

        private IGameLogger logger;
        private ISceneLoader sceneLoader;
        private bool isDuplicate;

        [Inject]
        public void Construct(IGameLogger logger, ISceneLoader sceneLoader)
        {
            this.logger = logger;
            this.sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                isDuplicate = true;
                Destroy(transform.root.gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }

        private async void Start()
        {
            if (isDuplicate)
            {
                return;
            }

            logger.Log("App initialized.");

            if (initialScene == null || !initialScene.IsValid)
            {
                logger.Error($"{nameof(Bootstrapper)} has no valid initial scene assigned.");
                return;
            }

            try
            {
                await sceneLoader.LoadAsync(initialScene, destroyCancellationToken);
            }
            catch (OperationCanceledException) when (destroyCancellationToken.IsCancellationRequested)
            {
            }
            catch (Exception exception)
            {
                logger.Error($"Failed to load initial scene '{initialScene.SceneName}': {exception.Message}");
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }
    }
}
