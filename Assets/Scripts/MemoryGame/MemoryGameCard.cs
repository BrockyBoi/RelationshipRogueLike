using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        public static System.Action<MemoryGameCard> OnCardClicked;

        public void SetMemoryType(EMemoryType memoryType)
        {
            MemoryType = memoryType;
        }

        public void OnMouseDown()
        {
            OnCardClicked?.Invoke(this);
            Debug.Log("Clicked");
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
