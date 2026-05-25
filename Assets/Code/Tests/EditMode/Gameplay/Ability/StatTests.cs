using NUnit.Framework;
using TopDownRPG.Gameplay;
using UnityEngine;

namespace TopDownRPG.Tests.Gameplay.Ability
{
    public sealed class StatTests
    {
        [Test]
        public void Constructor_ClampsBaseValueToRange()
        {
            var stat = new Stat(15f, new Vector2(0f, 10f));

            Assert.That(stat.BaseValue, Is.EqualTo(10f));
            Assert.That(stat.Value, Is.EqualTo(10f));
        }

        [Test]
        public void AddModifier_WithAdditiveModifier_IncreasesValue()
        {
            var stat = new Stat(10f, new Vector2(0f, 100f));

            stat.AddModifier(new StatModifier(5f, StatModifierType.Additive));

            Assert.That(stat.Value, Is.EqualTo(15f));
        }

        [Test]
        public void AddModifier_WithPercentModifier_ScalesValue()
        {
            var stat = new Stat(10f, new Vector2(0f, 100f));

            stat.AddModifier(new StatModifier(0.5f, StatModifierType.Percent));

            Assert.That(stat.Value, Is.EqualTo(15f));
        }

        [Test]
        public void RemoveModifier_RemovesOnlyMatchingModifier()
        {
            var stat = new Stat(10f, new Vector2(0f, 100f));
            var additiveModifier = new StatModifier(5f, StatModifierType.Additive);
            var percentModifier = new StatModifier(0.5f, StatModifierType.Percent);
            stat.AddModifier(additiveModifier);
            stat.AddModifier(percentModifier);

            stat.RemoveModifier(additiveModifier);

            Assert.That(stat.Value, Is.EqualTo(15f));
        }

        [Test]
        public void SetStatVal_NotifiesWhenValueChanges()
        {
            var changedValue = 0f;
            var stat = new Stat(10f, new Vector2(0f, 100f));
            stat.OnValueChanged += value => changedValue = value;

            stat.SetStatVal(20f);

            Assert.That(changedValue, Is.EqualTo(20f));
        }
    }
}
