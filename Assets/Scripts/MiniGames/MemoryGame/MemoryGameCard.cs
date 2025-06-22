using MemoryGame.Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame
{
    [System.Flags]
    public enum EMemoryType
    {
        AnniversaryDate = 1 << 1,
        BestFriendName = 1 << 2,
        MomName = 1 << 3,
        DadName = 1 << 4,
        DogName = 1 << 5,
        BirthdayDate = 1 << 6,
        FavoriteFood = 1 << 7,
        FavoriteFlower = 1 << 8,
        FavoriteColor = 1 << 9,
        SchoolDegree = 1 << 10,
        FavoriteMovie = 1 << 11,
        MiddleName = 1 << 12,
        Bomb = 1 << 13,
        ALL = AnniversaryDate | BestFriendName | MomName | DadName | DogName | BirthdayDate | FavoriteFood | FavoriteFlower | FavoriteColor
            | FavoriteColor | SchoolDegree| FavoriteMovie | MiddleName
    }

    public class MemoryGameCard : GridObject
    {
        public EMemoryType MemoryType {  get; private set; }

        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private Image _memoryImage;

        public static System.Action<MemoryGameCard> OnCardClicked;

        private void Start()
        {
            HideCard();
        }

        public void SetMemoryType(EMemoryType memoryType)
        {
            MemoryType = memoryType;

            MemoryGameDialoguePromptData data = MemoryGameDialoguePromptsManager.Instance.GetMemoryGameDialoguePromptData(MemoryType);
            if (data && _memoryImage)
            {
                _memoryImage.sprite = data.MemorySprite;
            }
        }

        public void OnMouseDown()
        {
            if (MemoryGameSolverComponent.Instance.CanPlayGame())
            {
                OnCardClicked?.Invoke(this);
            }
        }

        public void ShowCard()
        {
            _text.text = MemoryType.ToString();
        }

        public void HideCard()
        {
            _text.text = string.Empty;
        }

        public void CollectCard()
        {
            gameObject.SetActive(false);
        }
    }
}
