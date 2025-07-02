using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MainPlayer;
using System.Linq;

using static GlobalFunctions;

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
    public class MinorMapEventResultChoice
    {
        [SerializeField]
        public bool _resultIsToCloseUI;

        public bool ResultIsToCloseUI { get { return _resultIsToCloseUI; } }

        [SerializeField, TextArea]
        private string _choiceDescription;

        public string ChoiceDescription { get { return _choiceDescription; } }

        [SerializeField, HideIf("ResultIsToCloseUI")]
        private List<MinorMapEventResult> _potentialResults = new List<MinorMapEventResult>();

        public MinorMapEventResult GetRandomResult()
        {
            float randomValue = UnityEngine.Random.value;
            MinorMapEventResult randomResult = null;
            foreach (MinorMapEventResult result in _potentialResults)
            {
                if (result != null && result.PercentChanceForOutcome < randomValue)
                {
                    randomResult = result;
                    break;
                }
            }

            if (randomResult == null)
            {
                randomResult = _potentialResults.Last();
            }

            return randomResult;
        }
    }

    [Serializable]
    public class MinorMapEventResult
    {
        [TextArea]
        public string OutcomeDescription;

        public List<ResultModifier> OutcomeModifers = new List<ResultModifier>();

        [Range(0f, 1f)]
        public float PercentChanceForOutcome = .5f;

        public void ApplyAllModifiers()
        {
            foreach (ResultModifier modifier in OutcomeModifers)
            {
                ApplyModifier(modifier);
            }
        }

        private void ApplyModifier(ResultModifier modifier)
        {
            if (ensure(Player.Instance != null, "Player is not in scene"))
            {
                switch (modifier.ModifierType)
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
                            Player.Instance.PlayerModifiersComponent.ChangeModifierValue(EDifficultyModiferType.PuzzleTimeModifier, new ModifierValue(modifier.AdditiveValue, modifier.MultiplicativeValue));
                            break;
                        }
                    case EResultModifierTypes.PuzzlePunishmentsModifier:
                        {
                            Player.Instance.PlayerModifiersComponent.ChangeModifierValue(EDifficultyModiferType.PuzzlePunishmentsModifier, new ModifierValue(modifier.AdditiveValue, modifier.MultiplicativeValue));
                            break;
                        }
                }
            }
        }
    }

    [Serializable]
    public struct ResultModifier
    {
        public EResultModifierTypes ModifierType;
        public float AdditiveValue;
        public float MultiplicativeValue;

        public string GetDisplayString()
        {
            string displayString = string.Empty;
            switch (ModifierType)
            {
                case EResultModifierTypes.CurrentHealth:
                    {
                        int amount = Mathf.FloorToInt(Mathf.Abs(AdditiveValue));
                        displayString = AdditiveValue > 0 ? "You gain " + amount + " health": "You lose " + amount + " health";
                        break;
                    }
                    case EResultModifierTypes.MaxHealth:
                    {
                        int amount = Mathf.FloorToInt(Mathf.Abs(AdditiveValue));
                        displayString = AdditiveValue > 0 ? "You gain " + amount + " max health" : "You lose " + amount + " max health";
                        break;
                    }
                case EResultModifierTypes.PuzzleTimeModifier:
                    {
                        if (AdditiveValue != 0)
                        {
                            displayString = "Puzzle times have been " + (AdditiveValue > 0 ? "increased " : "decreased ") + " by " + Mathf.Abs(AdditiveValue) + " seconds";
                        }

                        if (MultiplicativeValue != 0)
                        {
                            if (displayString != string.Empty)
                            {
                                displayString += "\n";
                            }

                            displayString += "Puzzle times have been multiplied by " + MultiplicativeValue;
                        }
                        break;
                    }
                case EResultModifierTypes.PuzzlePunishmentsModifier:
                    {
                        if (AdditiveValue != 0)
                        {
                            displayString = "Puzzle punishments have been modified by " + Mathf.Abs(AdditiveValue);
                        }

                        if (MultiplicativeValue != 0)
                        {
                            if (displayString != string.Empty)
                            {
                                displayString += "\n";
                            }

                            displayString += "Puzzle punishments have been multiplied by " + MultiplicativeValue;
                        }
                        break;
                    }
                default:
                    {
                        Debug.LogWarning("No string created for " + ModifierType);
                        break;
                    }
            }

            return displayString;
        }
    }
}
