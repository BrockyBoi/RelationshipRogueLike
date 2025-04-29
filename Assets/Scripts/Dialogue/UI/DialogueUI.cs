using CustomUI;
using GeneralGame.Generation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI
{
    public class DialogueUI : BaseGameUI
    {
        public static DialogueUI Instance { get; private set; }

        [SerializeField]
        private Image _textBackgroundImage;

        [SerializeField]
        private TextMeshProUGUI _dialogueText;

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

        private void Start()
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
            _dialogueText.text = dialogueObject.GetDialogueString();
            _characterNameText.text = dialogueObject.GetCharacterName();

            bool isMainCharacter = dialogueObject.CharacterData.IsMainCharacter;
            _playableCharacterImage.sprite = isMainCharacter ? dialogueObject.GetCharacterSprite() : null;
            _NPCImage.sprite = !isMainCharacter ? dialogueObject.GetCharacterSprite() : null;

            _playableCharacterImage.enabled = isMainCharacter;
            _NPCImage.enabled = !isMainCharacter;
        }

        public void GetMoveFromGameResultToConversationData(out Vector3 finalLocation, out RectTransform backgroundTransform)
        {
            finalLocation = Vector3.zero;
            backgroundTransform = null;
            if (_textBackgroundImage)
            {
                finalLocation = _textBackgroundImage.transform.position;
                backgroundTransform = _textBackgroundImage.rectTransform;
            }
        }
    }
}
