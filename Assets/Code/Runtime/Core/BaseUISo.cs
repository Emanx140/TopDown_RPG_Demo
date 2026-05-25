using UnityEngine;
using UnityEngine.Localization;

namespace TopDownRPG.Gameplay
{
    public abstract class BaseUISo : ScriptableObject
    {
        [SerializeField] private LocalizedString displayName;
        [SerializeField] private Sprite icon;
    }
}