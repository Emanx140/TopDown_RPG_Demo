using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public interface IEnemyFactory
    {
        EnemyAiController Spawn(string enemyId, Vector3 position, Quaternion rotation, Transform parent = null);
    }
}
