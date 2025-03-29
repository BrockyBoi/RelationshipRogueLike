using CustomUI;
using GeneralGame;
using Maze.UI;
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
           BaseGameSolverComponent.OnAnyGamePresented += HideUI;
        }

        private void OnDisable()
        {
            BaseGameSolverComponent.OnAnyGamePresented -= HideUI;
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
