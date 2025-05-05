using GeneralGame.Results;
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
        private Slider _backgroundSlider;

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
            _backgroundSlider.gameObject.SetActive(false);
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

        public void SetProgressPercentage(float progressPercentage)
        {
            _backgroundSlider.value = progressPercentage;
        }

        public void HighlightObject()
        {
            _backgroundSlider.gameObject.SetActive(true);

            _backgroundImage.color = Color.yellow;
            transform.localScale = _startScale * 1.25f;
        }

        public void StopHighlightingObject()
        {
            _backgroundSlider.gameObject.SetActive(false);
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
