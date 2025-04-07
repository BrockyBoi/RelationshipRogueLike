using MainPlayer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [Serializable]
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
        public SentimentSpriteDictionary SentimentPortraits;
    }

    public class SentimentSpriteDictionary : UnitySerializedDictionary<ECharacterSentiment, Sprite> { }
}
