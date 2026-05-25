using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace TopDownRPG.Gameplay
{
    public sealed class EnemyFactory : MonoBehaviour, IEnemyFactory
    {
        [Serializable]
        private sealed class EnemyPrefabEntry
        {
            [field: SerializeField] public string Id { get; private set; }
            [field: SerializeField] public EnemyAiController Prefab { get; private set; }
        }

        [SerializeField] private List<EnemyPrefabEntry> enemyPrefabs = new List<EnemyPrefabEntry>();

        private IObjectResolver resolver;
        private PlayerController player;

        [Inject]
        public void Construct(IObjectResolver resolver, PlayerController player)
        {
            this.resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            this.player = player ?? throw new ArgumentNullException(nameof(player));
        }

        public EnemyAiController Spawn(string enemyId, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var prefab = FindPrefab(enemyId);
            if (prefab == null)
            {
                throw new InvalidOperationException($"Enemy prefab '{enemyId}' is not registered in {nameof(EnemyFactory)}.");
            }

            var enemy = resolver.Instantiate(prefab, position, rotation, parent);
            AssignPlayerTarget(enemy);
            return enemy;
        }

        private EnemyAiController FindPrefab(string enemyId)
        {
            foreach (var entry in enemyPrefabs)
            {
                if (entry == null || entry.Prefab == null)
                {
                    continue;
                }

                if (string.Equals(entry.Id, enemyId, StringComparison.Ordinal))
                {
                    return entry.Prefab;
                }
            }

            return null;
        }

        private void AssignPlayerTarget(EnemyAiController enemy)
        {
            if (enemy == null || player == null || !player.TryGetComponent<CharacterStats>(out var playerStats))
            {
                return;
            }

            enemy.AssignTarget(player.transform, playerStats);
        }
    }
}
