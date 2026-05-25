using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Top Down RPG/Gameplay/Ability", order = 0)]
    public class AbilitySo : BaseUISo
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private AnimationClip animationClip;
        [SerializeField] private bool allowWhileRunning = true;
        [SerializeField] private GameObject vfxPrefab;
        [SerializeField, Min(0f)] private float cooldownSeconds = 5f;
        [SerializeField] private AbilityTargetingSo targeting;
        [SerializeField] private List<AbilityEffectSo> effects = new List<AbilityEffectSo>();

        public AudioClip AudioClip => audioClip;
        public AnimationClip AnimationClip => animationClip;
        public bool AllowWhileRunning => allowWhileRunning;
        public GameObject VfxPrefab => vfxPrefab;
        public float CooldownSeconds => cooldownSeconds;
        public AbilityTargetingSo Targeting => targeting;
        public IReadOnlyList<AbilityEffectSo> EffectAssets => effects;
    }
}
