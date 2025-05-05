using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                _modiferValues.Add(modiferType, new ModifierValue());
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

        public void ModifyValue(EDifficultyModiferType type, out float modifiedValue)
        {
            modifiedValue = 0;

            if (_modiferValues.ContainsKey(type))
            {
                modifiedValue += _modiferValues[type].AdditiveValue;
                modifiedValue *= _modiferValues[type].MultiplicativeValue;
            }
        }
    }
}
