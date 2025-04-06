using GeneralGame.Results;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI
{
    public class PotentialPlayerDialogueUIObject : MonoBehaviour
    {
        [SerializeField]
        Image _backgroundImage;

        [SerializeField]
        TextMeshProUGUI _dialogueText;

        [SerializeField]
        Image _healthImage;

        [SerializeField]
        TextMeshProUGUI _healthChangeText;

        private GameCompletionResult _result;

        public void SetGameCompletionResult(GameCompletionResult result)
        {
            _result = result;

            _dialogueText.text = result.PotentialPlayerDialogueDescription;

            bool isZeroHealthChange = result.HealthResult.HealthAmountToChange == 0;
            _healthImage.gameObject.SetActive(!isZeroHealthChange);
            _healthChangeText.gameObject.SetActive(!isZeroHealthChange);
            if (!isZeroHealthChange)
            {
                int healthChange = result.HealthResult.HealthAmountToChange;
                _healthChangeText.text = string.Empty;
                if (healthChange > 0)
                {
                    _healthChangeText.text += "+";
                }

                _healthChangeText.text += healthChange;
            }
        }

        public void HighlightObject()
        {
            _backgroundImage.color = Color.yellow;
        }

        public void StopHighlightingObject()
        {
            _backgroundImage.color = Color.white;
        }
    }
}
