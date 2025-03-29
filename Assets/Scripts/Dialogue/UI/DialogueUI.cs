using CustomUI;
using GeneralGame.Generation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue.UI
{
    public class DialogueUI : GameUI
    {
        public static DialogueUI Instance { get; private set; }

        [SerializeField]
        private TextMeshProUGUI _dialogueText;

        [SerializeField]
        private TextMeshProUGUI _characterNameText;

        [SerializeField]
        private Image _characterImage;

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
            _characterImage.sprite = dialogueObject.GetCharacterSprite();
        }
    }
}
