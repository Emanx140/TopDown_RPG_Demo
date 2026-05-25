using System;
using TopDownRPG.Infrastructure;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using VContainer;

namespace TopDownRPG.Gameplay
{
    public abstract class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField, Tooltip("Mask used when ability animations are allowed to play while the character is moving.")]
        private AvatarMask abilityOverlayMask;
        [SerializeField, Min(0f)] private float abilityFadeSeconds = 0.08f;

        private PlayableGraph graph;
        private AnimationLayerMixerPlayable layerMixer;
        private AnimatorControllerPlayable controllerPlayable;
        private AnimationClipPlayable abilityClipPlayable;
        private bool hasControllerPlayable;
        private bool hasAbilityClipPlayable;
        private int abilityLayerIndex;
        private float abilityRemainingSeconds;
        private float abilityFadeRemainingSeconds;
        private bool abilityUsesOverlayLayer;
        private IGameLogger logger;

        private const int BaseLayerIndex = 0;
        private const int FullBodyAbilityLayerIndex = 1;
        private const int OverlayAbilityLayerIndex = 2;

        protected Animator Animator => animator;

        [Inject]
        public void Construct(IGameLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        protected virtual void Awake()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            CreateAnimationGraph();
        }

        protected virtual void Update()
        {
            TickAbilityAnimation(Time.deltaTime);
        }

        protected virtual void OnDisable()
        {
            StopAbilityAnimation();
        }

        protected virtual void OnDestroy()
        {
            if (graph.IsValid())
            {
                graph.Destroy();
            }
        }

        public void PlayAbility(AbilitySo ability)
        {
            if (ability == null)
            {
                return;
            }

            PlayAbilityClip(ability.AnimationClip, ability.AllowWhileRunning);
        }
        

        protected void SetAnimatorFloat(int parameterHash, float value)
        {
            if (animator != null)
            {
                animator.SetFloat(parameterHash, value);
            }

            if (hasControllerPlayable)
            {
                controllerPlayable.SetFloat(parameterHash, value);
            }
        }

        protected void SetAnimatorBool(int parameterHash, bool value)
        {
            if (animator != null)
            {
                animator.SetBool(parameterHash, value);
            }

            if (hasControllerPlayable)
            {
                controllerPlayable.SetBool(parameterHash, value);
            }
        }

        protected bool HasParameter(int parameterHash, AnimatorControllerParameterType expectedType)
        {
            if (animator == null)
            {
                return false;
            }

            foreach (var parameter in animator.parameters)
            {
                if (parameter.nameHash == parameterHash && parameter.type == expectedType)
                {
                    return true;
                }
            }

            return false;
        }

        private void PlayAbilityClip(AnimationClip animationClip, bool allowWhileRunning)
        {
            if (animationClip == null || animator == null || !graph.IsValid())
            {
                return;
            }

            StopAbilityAnimation();

            abilityClipPlayable = AnimationClipPlayable.Create(graph, animationClip);
            abilityClipPlayable.SetApplyFootIK(false);
            abilityClipPlayable.SetTime(0d);
            abilityClipPlayable.SetSpeed(1d);
            abilityClipPlayable.SetDuration(animationClip.length);
            hasAbilityClipPlayable = true;

            abilityUsesOverlayLayer = allowWhileRunning;
            abilityLayerIndex = allowWhileRunning ? OverlayAbilityLayerIndex : FullBodyAbilityLayerIndex;
            graph.Connect(abilityClipPlayable, 0, layerMixer, abilityLayerIndex);
            abilityRemainingSeconds = animationClip.length;
            abilityFadeRemainingSeconds = abilityFadeSeconds;

            if (allowWhileRunning)
            {
                if (abilityOverlayMask == null)
                {
                    logger?.Warning($"{GetType().Name} is playing an overlay ability without an AvatarMask assigned.");
                }

                layerMixer.SetInputWeight(BaseLayerIndex, 1f);
                layerMixer.SetInputWeight(FullBodyAbilityLayerIndex, 0f);
                layerMixer.SetInputWeight(OverlayAbilityLayerIndex, 1f);
                return;
            }

            layerMixer.SetInputWeight(BaseLayerIndex, 0f);
            layerMixer.SetInputWeight(FullBodyAbilityLayerIndex, 1f);
            layerMixer.SetInputWeight(OverlayAbilityLayerIndex, 0f);
        }

        private void TickAbilityAnimation(float deltaSeconds)
        {
            if (!hasAbilityClipPlayable)
            {
                return;
            }

            abilityRemainingSeconds -= deltaSeconds;
            if (abilityRemainingSeconds > 0f)
            {
                return;
            }

            if (abilityFadeSeconds <= 0f)
            {
                StopAbilityAnimation();
                return;
            }

            abilityFadeRemainingSeconds -= deltaSeconds;
            var abilityWeight = Mathf.Clamp01(abilityFadeRemainingSeconds / abilityFadeSeconds);

            if (!abilityUsesOverlayLayer)
            {
                layerMixer.SetInputWeight(BaseLayerIndex, 1f - abilityWeight);
                layerMixer.SetInputWeight(FullBodyAbilityLayerIndex, abilityWeight);
            }
            else
            {
                layerMixer.SetInputWeight(OverlayAbilityLayerIndex, abilityWeight);
            }

            if (abilityFadeRemainingSeconds <= 0f)
            {
                StopAbilityAnimation();
            }
        }

        private void CreateAnimationGraph()
        {
            if (animator == null)
            {
                logger?.Warning($"{GetType().Name} has no Animator assigned.");
                return;
            }

            layerMixer = AnimationPlayableUtilities.PlayLayerMixer(animator, 3, out graph);
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            if (animator.runtimeAnimatorController != null)
            {
                controllerPlayable = AnimatorControllerPlayable.Create(graph, animator.runtimeAnimatorController);
                graph.Connect(controllerPlayable, 0, layerMixer, BaseLayerIndex);
                layerMixer.SetInputWeight(BaseLayerIndex, 1f);
                hasControllerPlayable = true;
            }
            else
            {
                logger?.Warning($"{GetType().Name} Animator has no RuntimeAnimatorController assigned.");
            }

            layerMixer.SetInputWeight(FullBodyAbilityLayerIndex, 0f);
            layerMixer.SetInputWeight(OverlayAbilityLayerIndex, 0f);

            if (abilityOverlayMask != null)
            {
                layerMixer.SetLayerMaskFromAvatarMask((uint)OverlayAbilityLayerIndex, abilityOverlayMask);
            }
        }

        private void StopAbilityAnimation()
        {
            if (!hasAbilityClipPlayable)
            {
                return;
            }

            if (abilityClipPlayable.IsValid())
            {
                graph.Disconnect(layerMixer, abilityLayerIndex);
                abilityClipPlayable.Destroy();
            }

            hasAbilityClipPlayable = false;
            abilityLayerIndex = 0;
            abilityRemainingSeconds = 0f;
            abilityFadeRemainingSeconds = 0f;
            layerMixer.SetInputWeight(BaseLayerIndex, 1f);
            layerMixer.SetInputWeight(FullBodyAbilityLayerIndex, 0f);
            layerMixer.SetInputWeight(OverlayAbilityLayerIndex, 0f);
        }
    }
}
