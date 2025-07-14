using GeneralGame.Results;
using System.Collections;
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
        private Vector2 _startSizeDelta;

        [SerializeField]
        private Vector3 _endUIPos;

        [SerializeField]
        private Vector2 _endSizeDelta;

        [SerializeField]
        private Vector3 _endScale;

        private void Start()
        {
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
            //_backgroundSlider.gameObject.SetActive(true);

            //_backgroundImage.color = Color.yellow;
            transform.localScale = _startScale * 1.25f;
        }

        public void StopHighlightingObject()
        {
            //_backgroundSlider.gameObject.SetActive(false);
            //_backgroundImage.color = Color.white;
            transform.localScale = _startScale;
        }

        public void Show(bool shouldShow)
        {
            _backgroundImage.enabled = shouldShow;
            _backgroundSlider.enabled = shouldShow;
            _dialogueText.enabled = shouldShow;
            _healthChangeText.enabled = shouldShow;
            _healthImage.enabled = shouldShow;
        }

        public void AddResultToUI(float timeToShow)
        {
            Vector3 endPos = transform.position;
            transform.position = transform.position + Vector3.right * 1000;
            GlobalFunctions.LerpObjectToLocation(this, gameObject, endPos, timeToShow);
        }

        public void RemoveResultFromUI()
        {
            Vector3 endPos = transform.position + Vector3.right * 1000;
            GlobalFunctions.LerpObjectToLocation(this, gameObject, endPos, 2f);
        }

        public void MoveToDialogueUILocation()
        {
            StartCoroutine(MoveToDialogueUILocationCoroutine());
        }

        private IEnumerator MoveToDialogueUILocationCoroutine()
        {
            Vector3 startPos = transform.position;
            float time = 0;
            float timeToTake = PotentialPlayerDialogueUI.Instance.TimeToMoveFromResultToDialogue;
            RectTransform rect = GetComponent<RectTransform>();
            _startSizeDelta = rect.sizeDelta;
            _startScale = transform.localScale;

            _backgroundSlider.gameObject.SetActive(false);
            _backgroundImage.gameObject.SetActive(true);

            while (time < timeToTake)
            {
                Vector3 pos = Vector3.Lerp(startPos, _endUIPos, time / timeToTake);
                transform.position = pos;

                Vector3 sizeDelta = Vector3.Lerp(_startSizeDelta, _endSizeDelta, time / timeToTake);
                rect.sizeDelta = sizeDelta;

                Vector3 standardScale = Vector3.Lerp(_startScale, _endScale, time / timeToTake);
                transform.localScale = standardScale;
                time += Time.deltaTime;

                yield return null;
            }
        }
    }
}
