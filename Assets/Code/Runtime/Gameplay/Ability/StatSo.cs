using UnityEngine;

namespace TopDownRPG.Gameplay
{
    [CreateAssetMenu(fileName = "Stat", menuName = "Top Down RPG/Gameplay/Stat", order = 0)]
    public class StatSo : BaseUISo
    {
        [SerializeField] private float initialValue = 1f;
        [SerializeField] private Vector2 minMaxValue = new Vector2(0f, 100f);

        public Stat CreateStat()
        {
            return new Stat(initialValue, minMaxValue);
        }
    }
}
