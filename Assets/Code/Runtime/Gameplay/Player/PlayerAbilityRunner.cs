using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace TopDownRPG.Gameplay
{
    [RequireComponent(typeof(CharacterStats))]
    public class PlayerAbilityRunner : AbilityRunner
    {
        [Serializable]
        private class AbilitySlot
        {
            [field: SerializeField] public PlayerAbilitySlotId Id { get; private set; }
            [field: SerializeField] public AbilitySo Ability { get; private set; }
        }

        [SerializeField] private CharacterStats sourceStats;
        [SerializeField] private List<AbilitySlot> slots = new List<AbilitySlot>();

        private IPlayerInput input;
        private bool isSubscribed;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            this.input = input ?? throw new ArgumentNullException(nameof(input));
            Subscribe();
        }

        protected override void Awake()
        {
            base.Awake();

            if (sourceStats == null)
            {
                sourceStats = GetComponent<CharacterStats>();
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void TryUseSlot(PlayerAbilitySlotId slotId)
        {
            var slot = FindSlot(slotId);
            if (slot == null)
            {
                return;
            }

            //TODO: IDamageable target; Later make target manager
            TryUseAbility(slot.Ability, sourceStats, null);
        }

        private AbilitySlot FindSlot(PlayerAbilitySlotId slotId)
        {
            foreach (var slot in slots)
            {
                if (slot != null && slot.Id == slotId)
                {
                    return slot;
                }
            }

            return null;
        }

        private void Subscribe()
        {
            if (input == null || isSubscribed || !isActiveAndEnabled)
            {
                return;
            }

            isSubscribed = true;
            input.AbilitySlotPressed += TryUseSlot;
        }

        private void Unsubscribe()
        {
            if (input == null || !isSubscribed)
            {
                return;
            }

            isSubscribed = false;
            input.AbilitySlotPressed -= TryUseSlot;
        }
    }
}
