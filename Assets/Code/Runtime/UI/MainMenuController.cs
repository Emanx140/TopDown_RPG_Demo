using System;
using Cysharp.Threading.Tasks;
using TopDownRPG.Infrastructure;
using TopDownRPG.Scenes;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace TopDownRPG.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private SceneReference levelScene;
        [SerializeField] private SceneReference overlayScene;
        [SerializeField] private string playButtonName = "play-button";

        private Button playButton;
        private IGameLogger logger;
        private ISceneLoader sceneLoader;
        private bool isLoading;

        [Inject]
        public void Construct(IGameLogger logger, ISceneLoader sceneLoader)
        {
            this.logger = logger;
            this.sceneLoader = sceneLoader;
        }

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        private void OnValidate()
        {
            playButtonName = playButtonName?.Trim();
        }

        private void Awake()
        {

            if (document == null)
            {
                document = GetComponent<UIDocument>();
            }
        }

        private void OnEnable()
        {
            BindButtons();
        }

        private void OnDisable()
        {
            if (playButton != null)
            {
                playButton.clicked -= OnPlayClicked;
            }
        }

        private void BindButtons()
        {
            if (document == null)
            {
                LogError($"{nameof(MainMenuController)} requires a {nameof(UIDocument)}.");
                return;
            }

            playButton = document.rootVisualElement.Q<Button>(playButtonName);
            if (playButton == null)
            {
                playButton = document.rootVisualElement.Q<VisualElement>(playButtonName)?.Q<Button>();
            }

            if (playButton == null)
            {
                LogError($"Could not find main menu play button named '{playButtonName}'.");
                return;
            }

            playButton.clicked -= OnPlayClicked;
            playButton.clicked += OnPlayClicked;
        }

        private void OnPlayClicked()
        {
            LoadGameAsync().Forget();
        }

        private async UniTaskVoid LoadGameAsync()
        {
            if (isLoading)
            {
                LogWarning("Main menu play ignored because a scene load is already running.");
                return;
            }

            if (sceneLoader == null)
            {
                LogError($"{nameof(MainMenuController)} requires an injected {nameof(ISceneLoader)}.");
                return;
            }

            if (!IsValid(levelScene))
            {
                LogError($"{nameof(MainMenuController)} has no valid level scene assigned.");
                return;
            }

            isLoading = true;
            SetPlayButtonEnabled(false);

            try
            {
                await sceneLoader.LoadAsync(levelScene);

                if (IsValid(overlayScene))
                {
                    await sceneLoader.LoadAdditiveAsync(overlayScene);
                }
            }
            catch (Exception exception)
            {
                LogError($"Failed to start game from main menu: {exception.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void SetPlayButtonEnabled(bool isEnabled)
        {
            playButton?.SetEnabled(isEnabled);
        }

        private static bool IsValid(SceneReference scene)
        {
            return scene != null && scene.IsValid;
        }

        private void LogWarning(string message)
        {
            if (logger != null)
            {
                logger.Warning(message);
                return;
            }

            Debug.LogWarning(message, this);
        }

        private void LogError(string message)
        {
            if (logger != null)
            {
                logger.Error(message);
                return;
            }

            Debug.LogError(message, this);
        }
    }
}
