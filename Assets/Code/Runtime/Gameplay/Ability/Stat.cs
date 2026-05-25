using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace TopDownRPG.Gameplay
{
    public enum StatModifierType
    {
        Additive,
        Percent
    }

    [Serializable]
    public class StatModifier
    {
        [SerializeField] private float value;
        [SerializeField] private StatModifierType type;

        public StatModifier(float value, StatModifierType type, ActiveAbilityEffect source = null)
        {
            this.value = value;
            this.type = type;
            Source = source;
        }

        public float Value => value;
        public StatModifierType Type => type;
        public ActiveAbilityEffect Source { get; }
    }

    [Serializable]
    public class Stat 
    {
        [FormerlySerializedAs("value")]
        [SerializeField] private float baseValue;
        [SerializeField] private Vector2 minMaxValue;

        private readonly List<StatModifier> modifiers = new List<StatModifier>();

        public Stat(float value, Vector2 minMaxValue)
        {
            this.minMaxValue = minMaxValue;
            SetStatVal(value);
        }

        public float Value => CalculateValue();
        public float BaseValue => baseValue;
        public Vector2 MinMaxValue => minMaxValue;
        public float MinValue => minMaxValue.x;
        public float MaxValue => minMaxValue.y;

        public event Action<float> OnValueChanged;

        public void SetStatVal(float newValue)
        {
            var previousValue = Value;
            baseValue = Mathf.Clamp(newValue, MinValue, MaxValue);

            NotifyIfValueChanged(previousValue);
        }

        public void AddModifier(StatModifier modifier)
        {
            if (modifier == null)
            {
                return;
            }

            var previousValue = Value;
            modifiers.Add(modifier);
            NotifyIfValueChanged(previousValue);
        }

        public void RemoveModifier(StatModifier modifier)
        {
            if (modifier == null)
            {
                return;
            }

            var previousValue = Value;
            modifiers.Remove(modifier);
            NotifyIfValueChanged(previousValue);
        }

        public bool RemoveModifier(ActiveAbilityEffect effect, StatModifierType type, float value)
        {
            for (var index = modifiers.Count - 1; index >= 0; index--)
            {
                var modifier = modifiers[index];
                if (modifier == null ||
                    modifier.Type != type ||
                    !Mathf.Approximately(modifier.Value, value) ||
                    modifier.Source != effect)
                {
                    continue;
                }

                var previousValue = Value;
                modifiers.RemoveAt(index);
                NotifyIfValueChanged(previousValue);
                return true;
            }

            return false;
        }

        public void Add(Stat stat)
        {
            ThrowIfNull(stat);
            SetStatVal(baseValue + stat.Value);
        }

        public void Subtract(Stat stat)
        {
            ThrowIfNull(stat);
            SetStatVal(baseValue - stat.Value);
        }

        public void Multiply(Stat stat)
        {
            ThrowIfNull(stat);
            SetStatVal(baseValue * stat.Value);
        }

        public void Divide(Stat stat)
        {
            ThrowIfNull(stat);

            if (Mathf.Approximately(stat.Value, 0f))
            {
                throw new DivideByZeroException(nameof(stat));
            }

            SetStatVal(baseValue / stat.Value);
        }

        private float CalculateValue()
        {
            var additiveValue = 0f;
            var percentValue = 0f;

            foreach (var modifier in modifiers)
            {
                if (modifier == null)
                {
                    continue;
                }

                if (modifier.Type == StatModifierType.Additive)
                {
                    additiveValue += modifier.Value;
                    continue;
                }

                percentValue += modifier.Value;
            }

            return Mathf.Clamp((baseValue + additiveValue) * (1f + percentValue), MinValue, MaxValue);
        }

        private void NotifyIfValueChanged(float previousValue)
        {
            var currentValue = Value;
            if (Mathf.Approximately(previousValue, currentValue))
            {
                return;
            }

            OnValueChanged?.Invoke(currentValue);
        }

        private static void ThrowIfNull(Stat stat)
        {
            if (stat == null)
            {
                throw new ArgumentNullException(nameof(stat));
            }
        }
    }
}
