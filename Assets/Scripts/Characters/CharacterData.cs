using MainPlayer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public enum ECharacterSentiment
    {
        Happy,
        Neutral,
        Annoyed,
        FuckingPissed
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
    public class CharacterData : SerializedScriptableObject
    {
        public string CharacterName;
        public Sprite DefaultSprite;
        public Dictionary<ECharacterSentiment, Sprite> SentimentPortraits;
    }
}
