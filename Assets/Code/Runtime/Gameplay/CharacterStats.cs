using System.Collections.Generic;
using UnityEngine;

namespace TopDownRPG.Gameplay
{
    public class CharacterStats : MonoBehaviour, IDamageable
    {
        [SerializeField] private StatSo healthStat;
        [SerializeField] private List<StatSo> stats = new List<StatSo>();

        private readonly List<IAbility> activeAbilities = new List<IAbility>();
        
        private readonly Dictionary<StatSo, Stat> runtimeStats = new Dictionary<StatSo, Stat>();

        public bool TryGetStat(StatSo statDefinition, out Stat stat)
        {
            if (statDefinition == null)
            {
                stat = null;
                return false;
            }

            EnsureInitialized();
            return runtimeStats.TryGetValue(statDefinition, out stat);
        }

        private void TickAbilities(float deltaSeconds)
        {
            for (var index = activeAbilities.Count - 1; index >= 0; index--)
            {
                var ability = activeAbilities[index];
                ability.Tick(deltaSeconds);

                if (!ability.IsActive)
                {
                    activeAbilities.RemoveAt(index);
                }
            }
        }
        
        private void Update()
        {
            TickAbilities(Time.deltaTime);
        }
        
        public bool ReceiveAbility(IAbility runtimeAbility)
        {
            if (runtimeAbility == null || !runtimeAbility.Apply())
            {
                return false;
            }

            if (runtimeAbility.IsActive)
            {
                activeAbilities.Add(runtimeAbility);
            }

            return true;
        }

        public void ApplyDamage(StatSo targetStatDefinition, Stat damage)
        {
            if (targetStatDefinition == null || damage == null || !TryGetStat(targetStatDefinition, out var targetStat))
            {
                return;
            }
            targetStat.Subtract(damage);
        }

        private void Awake()
        {
            InitializeStats();
        }

        private void OnValidate()
        {
            stats.RemoveAll(stat => stat == null);

            if (healthStat != null && !stats.Contains(healthStat))
            {
                stats.Insert(0, healthStat);
            }
        }

        private void EnsureInitialized()
        {
            if (runtimeStats.Count == 0)
            {
                InitializeStats();
            }
        }

        private void InitializeStats()
        {
            runtimeStats.Clear();

            foreach (var statDefinition in stats)
            {
                if (statDefinition == null || runtimeStats.ContainsKey(statDefinition))
                {
                    continue;
                }

                runtimeStats.Add(statDefinition, statDefinition.CreateStat());
            }
        }
    }
}
