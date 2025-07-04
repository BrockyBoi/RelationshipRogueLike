using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

namespace MainPlayer
{
    public enum EDifficultyModiferType
    {
        PuzzleTimeModifier,
        PuzzlePunishmentsModifier,
    }

    public class PlayerModifiersComponent : MonoBehaviour
    {
        private Dictionary<EDifficultyModiferType, ModifierValue> _modiferValues = new Dictionary<EDifficultyModiferType, ModifierValue> ();

        public void ChangeModifierValue(EDifficultyModiferType modiferType, ModifierValue modifier)
        {
            if (!_modiferValues.ContainsKey(modiferType))
            {
                InitializeValue(modiferType);
            }

            _modiferValues[modiferType].AdditiveValue += modifier.AdditiveValue;
            _modiferValues[modiferType].MultiplicativeValue += modifier.MultiplicativeValue;
        }

        public float GetAdditiveModifierValue(EDifficultyModiferType type)
        {
            return _modiferValues.ContainsKey(type) ? _modiferValues[type].AdditiveValue : 0;
        }

        public float GetMultiplicativeValue(EDifficultyModiferType type)
        {
            return _modiferValues.ContainsKey(type) ? _modiferValues[type].MultiplicativeValue : 0;
        }

        public void ModifyValue(EDifficultyModiferType type, ref float modifiedValue)
        {
            if (!_modiferValues.ContainsKey(type))
            {
                InitializeValue(type);
            }

            modifiedValue += _modiferValues[type].AdditiveValue;
            modifiedValue *= _modiferValues[type].MultiplicativeValue;
        }

        private void InitializeValue(EDifficultyModiferType modifierType)
        {
            if (!_modiferValues.ContainsKey(modifierType))
            {
                _modiferValues.Add(modifierType, new ModifierValue());
            }
        }
    }
}
