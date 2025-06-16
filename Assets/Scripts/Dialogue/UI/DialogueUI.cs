using CustomUI;
using GeneralGame.Generation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI
{
    public class DialogueUI : BaseGameUI
    {
        public static DialogueUI Instance { get; private set; }

        [SerializeField, Title("Standard Dialogue")]
        private Image _standardTextBackgroundImage;

        [SerializeField]
        private TextMeshProUGUI _standardDialogueText;

        [SerializeField, Title("Thinking Dialogue")]
        private Image _thinkingTextBackgroundImage;

        [SerializeField]
        private TextMeshProUGUI _thinkingDialogueText;

        [SerializeField]
        private TextMeshProUGUI _characterNameText;

        [SerializeField]
        private Image _playableCharacterImage;

        [SerializeField]
        private Image _NPCImage;

        private void Awake()
        {
            Instance = this;
            HideUI();
        }

        private void OnEnable()
        {
           BaseGameGenerator.OnAnyGameGenerated += HideUI;
        }

        private void OnDisable()
        {
            BaseGameGenerator.OnAnyGameGenerated -= HideUI;
        }

        public void ShowDialogue(StandardDialogueObject dialogueObject)
        {
            ShowUI();
            _standardDialogueText.text = dialogueObject.GetDialogueString();
            _thinkingDialogueText.text = dialogueObject.GetDialogueString();
            _characterNameText.text = dialogueObject.GetCharacterName();

            bool isMainCharacter = dialogueObject.CharacterData.IsMainCharacter;
            _playableCharacterImage.sprite = isMainCharacter ? dialogueObject.GetCharacterSprite() : null;
            _NPCImage.sprite = !isMainCharacter ? dialogueObject.GetCharacterSprite() : null;

            _playableCharacterImage.enabled = isMainCharacter;
            _NPCImage.enabled = !isMainCharacter;

            bool isThinking = dialogueObject.CustomDialogue.IsThinking;
            _standardTextBackgroundImage.gameObject.SetActive(!isThinking);
            _thinkingTextBackgroundImage.gameObject.SetActive(isThinking);
        }

        public void GetMoveFromGameResultToConversationData(out Vector3 finalLocation, out RectTransform backgroundTransform)
        {
            finalLocation = Vector3.zero;
            backgroundTransform = null;
            if (_standardTextBackgroundImage)
            {
                finalLocation = _standardTextBackgroundImage.transform.position;
                backgroundTransform = _standardTextBackgroundImage.rectTransform;
            }
        }
    }
}
