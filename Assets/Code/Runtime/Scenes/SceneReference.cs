#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TopDownRPG.Scenes
{
    [CreateAssetMenu(menuName = "TopDown RPG/Scenes/Scene Reference", fileName = "SceneReference")]
    public class SceneReference : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;
#endif
        [SerializeField] private string sceneName;

        public string SceneName => sceneName;
        public bool IsValid => !string.IsNullOrWhiteSpace(sceneName);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (sceneAsset != null)
            {
                sceneName = sceneAsset.name;
            }
            else if (sceneName != null)
            {
                sceneName = sceneName.Trim();
            }
        }
#endif
    }
}
