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
        private Image _backgroundImage;

        [SerializeField]
        private TextMeshProUGUI _dialogueText;

        [SerializeField]
        private Image _healthImage;

        [SerializeField]
        private TextMeshProUGUI _healthChangeText;

        private GameCompletionResult _result;

        private Vector3 _startScale = Vector3.one;

        private void Start()
        {
            _startScale = transform.localScale;
        }

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
            transform.localScale = _startScale * 1.25f;

        }

        public void StopHighlightingObject()
        {
            _backgroundImage.color = Color.white;
            transform.localScale = _startScale;
        }

        public void RemoveResult()
        {
            Vector3 endPos = transform.position + Vector3.right * 1000;
            GlobalFunctions.LerpObjectToLocation(this, gameObject, endPos, 2);
        }
    }
}
