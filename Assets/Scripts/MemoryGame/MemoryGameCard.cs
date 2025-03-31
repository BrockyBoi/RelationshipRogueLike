using MemoryGame.Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryGame
{
    public enum EMemoryType
    {
        AnniversaryDate,
        BestFriendName,
        MomName,
        DadName,
        DogName,
        BirthdayDate,
        FavoriteFood,
        FavoriteFlower,
        FavoriteColor,
        SchoolDegree,
        FavoriteMovie,
        MiddleName,
        Bomb,
    }

    public class MemoryGameCard : GridObject
    {
        public EMemoryType MemoryType {  get; private set; }

        [SerializeField]
        TextMeshProUGUI _text;

        [SerializeField]
        Image _memoryImage;

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
            OnCardClicked?.Invoke(this);
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
