using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UIElements;

namespace TopDownRPG.Scenes
{
    [RequireComponent(typeof(UIDocument))]
    public class ScreenFadeTransition : MonoBehaviour, ISceneTransition
    {
        [SerializeField, Min(0.01f)] private float fadeOutDuration = 0.2f;
        [SerializeField, Min(0.01f)] private float fadeInDuration = 0.25f;
        [SerializeField] private UIDocument document;
        [SerializeField] private string overlayElementName = "fade-overlay";

        private VisualElement overlay;
        private Tween fadeTween;
        private int fadeVersion;

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }

        private void OnEnable()
        {
            ResolveOverlay();
            SetAlpha(0f);
        }

        public UniTask FadeOutAsync(CancellationToken cancellationToken = default)
        {
            return FadeToAsync(1f, fadeOutDuration, cancellationToken);
        }

        public UniTask FadeInAsync(CancellationToken cancellationToken = default)
        {
            return FadeToAsync(0f, fadeInDuration, cancellationToken);
        }

        private void ResolveOverlay()
        {
            overlay = null;

            if (document == null)
            {
                document = GetComponent<UIDocument>();
            }

            if (document == null || document.rootVisualElement == null || string.IsNullOrWhiteSpace(overlayElementName))
            {
                Debug.LogWarning($"{nameof(ScreenFadeTransition)} could not resolve a fade overlay because its UI document or overlay name is missing.", this);
                return;
            }

            overlay = document.rootVisualElement.Q<VisualElement>(overlayElementName);

            if (overlay == null)
            {
                Debug.LogWarning($"{nameof(ScreenFadeTransition)} could not find a fade overlay named '{overlayElementName}'.", this);
            }
        }

        private async UniTask FadeToAsync(float targetAlpha, float duration, CancellationToken cancellationToken)
        {
            if (overlay == null)
            {
                ResolveOverlay();
            }

            if (overlay == null)
            {
                return;
            }

            var version = ++fadeVersion;
            if (fadeTween.isAlive)
            {
                fadeTween.Stop();
            }

            fadeTween = Tween.Alpha(
                overlay,
                overlay.resolvedStyle.opacity,
                targetAlpha,
                duration,
                Ease.InOutSine,
                useUnscaledTime: true);
            var tween = fadeTween;

            overlay.pickingMode = targetAlpha > 0f ? PickingMode.Position : PickingMode.Ignore;

            try
            {
                await UniTask.WaitUntil(tween, static tween => !tween.isAlive, PlayerLoopTiming.Update, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                tween.Stop();
                throw;
            }

            if (version == fadeVersion)
            {
                SetAlpha(targetAlpha);
            }
        }

        private void SetAlpha(float alpha)
        {
            if (overlay == null)
            {
                return;
            }

            overlay.style.opacity = alpha;
            overlay.pickingMode = alpha > 0f ? PickingMode.Position : PickingMode.Ignore;
        }
    }
}
