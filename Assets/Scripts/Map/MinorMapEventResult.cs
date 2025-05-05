using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MainPlayer;

namespace Map
{
    public enum EResultModifierTypes
    {
        CurrentHealth,
        MaxHealth,
        PuzzleTimeModifier,
        PuzzlePunishmentsModifier,
    }

    [Serializable]
    public class MinorMapEventResult
    {
        [SerializeField, Title("Positive Outcome")]
        private string _positiveOutcomeDescription;

        public string PositiveOutsomeDescription {  get { return _positiveOutcomeDescription; } }

        [SerializeField]
        private List<ResultModifier> _positiveModifers = new List<ResultModifier>();

        [SerializeField, Title("Negative Outcome")]
        private string _negativeOutcomeDescription;

        public string NegativeOutcomeDescription {  get { return _negativeOutcomeDescription; } }

        [SerializeField]
        private List<ResultModifier> _negativeModifers = new List<ResultModifier>();

        public void ApplyAllModifiers(bool isPositiveOutcome)
        {
            List<ResultModifier> modifierList = isPositiveOutcome ? _positiveModifers : _negativeModifers;
            foreach (ResultModifier modifier in modifierList)
            {
                ApplyModifier(modifier);
            }
        }

        private void ApplyModifier(ResultModifier modifier)
        {
            switch(modifier.ModifierType)
            {
                case EResultModifierTypes.CurrentHealth:
                    {
                        Player.Instance.HealthComponent.ChangeHealth(Mathf.RoundToInt(modifier.AdditiveValue));
                        break;
                    }
                case EResultModifierTypes.MaxHealth:
                    {
                        Player.Instance.HealthComponent.ChangeMaxHealth(Mathf.RoundToInt(modifier.AdditiveValue));
                        break;
                    }
                case EResultModifierTypes.PuzzleTimeModifier:
                    {
                        Player.Instance.PlayerModifiersComponent.ChangeModifierValue(EDifficultyModiferType.PuzzleTimeModifier, new ModifierValue(modifier.AdditiveValue, modifier.MutliplicativeValue));
                        break;
                    }
                case EResultModifierTypes.PuzzlePunishmentsModifier:
                    {
                        Player.Instance.PlayerModifiersComponent.ChangeModifierValue(EDifficultyModiferType.PuzzlePunishmentsModifier, new ModifierValue(modifier.AdditiveValue, modifier.MutliplicativeValue));
                        break;
                    }
            }
        }
    }

    public struct ResultModifier
    {
        public EResultModifierTypes ModifierType;
        public float AdditiveValue;
        public float MutliplicativeValue;
    }
}
