using Characters;
using Maze;
using MainPlayer;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum EDialogueObjectType
    {
        StandardDialogue,
        SpawnMaze
    }

    [Serializable]
    public class DialogueObject
    {
        [EnumToggleButtons]
        public EDialogueObjectType DialogueObjectType;

        [ShowIf("DialogueObjectType", EDialogueObjectType.StandardDialogue)]
        public List<StandardDialogueObject> StandardDialogueObjects;

        [ShowIf("DialogueObjectType", EDialogueObjectType.SpawnMaze)]
        public MazeSpawnerDialogue MazeSpawnerDialogue;
    }

    [Serializable]
    public class SentimentDialogueDictionary : UnitySerializedDictionary<ECharacterSentiment, string> { }


    [Serializable]
    public class StandardDialogueObject
    {
        [SerializeField]
        private CharacterData _characterData;

        [SerializeField]
        private bool _usesCustomName;
        [ShowIf("_usesCustomName")]
        private string _customName;

        [SerializeField]
        private bool _usesCustomSprite;
        [SerializeField, ShowIf("_usesCustomSprite")]
        private Sprite _customSprite;

        [SerializeField]
        private bool _isCharacterMainPlayer;

        [SerializeField]
        private bool _usesSentimentSystem;

        [SerializeField, ShowIf("_usesSentimentSystem")]
        private SentimentDialogueDictionary _sentimentDialogues;

        [SerializeField, HideIf("_usesSentimentSystem"), TextArea(2, 4)]
        private string _standardDialogue;

        public string GetCharacterName()
        {
            if (!_usesCustomName)
            {
                return _characterData.CharacterName;
            }

            return _customName;
        }

        public Sprite GetCharacterSprite()
        {
            if (_usesCustomSprite)
            {
                return _customSprite;
            }

            if (_usesSentimentSystem && _characterData != null && _characterData.SentimentPortraits != null && _characterData.SentimentPortraits.Count > 0)
            {
                ECharacterSentiment sentiment = GetPlayerHealthComponent().GetCharacterSentiment();
                return _characterData.SentimentPortraits.ContainsKey(sentiment) ? _characterData.SentimentPortraits[sentiment] : _characterData.DefaultSprite;
            }

            return _characterData.DefaultSprite;
        }

        public string GetDialogueString()
        {
            if (_usesSentimentSystem)
            {
                return _sentimentDialogues[GetPlayerHealthComponent().GetCharacterSentiment()];
            }

            return _standardDialogue;
        }

        private HealthComponent GetPlayerHealthComponent()
        {
            return Player.Instance.HealthComponent;
        }
    }

    [Serializable]
    public class MazeSpawnerDialogue
    {
        public MazeSpawnData MazeSpawnData;
        public List<MazeCompletionResult> MazeCompletionResults;
    }
}
