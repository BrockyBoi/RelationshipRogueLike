using Characters;
using MainPlayer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

using Maze;
using MemoryGame.Dialogue;
using MemoryGame.Generation;
using MemoryGame;

using WhackAMole;

using CatchingButterflies;
using UnityEditor;
using EndlessRunner;
using ShootYourShotGame;
using FireFighting;

namespace Dialogue
{
    public enum EDialogueObjectType
    {
        StandardDialogue,
        SentimentDialogue,
        SpawnMaze,
        SpawnMemoryGame,
        EndConversation,
        LinkNewConversation,
        SpawnWhackAMole,
        SpawnButterflyCatching,
        SpawnEndlessRunner,
        SpawnShootYourShot,
        SpawnFireFighting,
    }

    [Serializable]
    public class DialogueObject
    {
        [FoldoutGroup("@Description")]
        public string Description = "Default Description";

        [FoldoutGroup("@Description")]
        public EDialogueObjectType DialogueObjectType;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.StandardDialogue)]
        public List<StandardDialogueObject> StandardDialogueObjects;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SentimentDialogue)]
        public SentimentDialogueObject SentimentDialogueObject;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SpawnMaze)]
        public MazeGeneratorData MazeSpawnerData;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SpawnMemoryGame)]
        public MemoryGameGeneratorData MemoryGameSpawnerData;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SpawnWhackAMole)]
        public WhackAMoleGenerationData WhackAMoleGameSpawnerData;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SpawnButterflyCatching)]
        public CatchingButterfliesGenerationData CatchingButterflyGameSpawnerData;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SpawnEndlessRunner)]
        public EndlessRunnerGenerationData EndlessRunnerSpawnerData;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SpawnShootYourShot)]
        public ShootYourShotGameGenerationData ShootYourShotSpawnerData;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.SpawnFireFighting)]
        public FireFightingGenerationData FireFightingSpawnerData;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.LinkNewConversation), HideLabel]
        public NewConversationObject LinkedConverssationObject;

        [FoldoutGroup("@Description"), ShowIf("DialogueObjectType", EDialogueObjectType.EndConversation), HideLabel]
        public EndConversationObject EndConversationObject;
    }

    [Serializable]
    public class SentimentDialogueDictionary : UnitySerializedDictionary<ECharacterSentiment, BranchingDialogueObject> { }

    [Serializable]
    public class DialogueData
    {
        [HorizontalGroup, Required]
        public CharacterData CharacterData;
        [HorizontalGroup]
        public ECharacterSentiment CharacterSentiment = ECharacterSentiment.Neutral;

        [HideLabel]
        public CustomDialogueOptions CustomDialogueOptions;
    }

    [Serializable]
    public class StandardDialogueObject
    {
        [FoldoutGroup("@DialogueEditorDisplayString")]
        [SerializeField, FoldoutGroup("@DialogueEditorDisplayString"), HideLabel]
        private DialogueData _dialogueData;

        [SerializeField, TextArea(2, 4), FoldoutGroup("@DialogueEditorDisplayString")]
        private string _standardDialogue;

        public CharacterData CharacterData { get { return _dialogueData.CharacterData; } }
        public CustomDialogueOptions CustomDialogue { get { return _dialogueData.CustomDialogueOptions; } }

        private string DialogueEditorDisplayString { get { return CharacterData ? CharacterData.CharacterName + " " + _dialogueData.CharacterSentiment.ToString().ToLower() + " dialogue" + " '" + _standardDialogue.Substring(0, Mathf.Min(_standardDialogue.Length, 50)) + "'" : string.Empty; } }

        public static StandardDialogueObject EmptyDialogueObject
        {
            get { return new StandardDialogueObject(null, null, string.Empty); }
        }

        public StandardDialogueObject(CharacterData data, CustomDialogueOptions options, string dialogue)
        {
            if (!data)
            {
                Debug.LogError("Character Data is null");
            }

            if (options == null)
            {
                Debug.LogError("Custom Dialogue Options is null");
            }

            _dialogueData = new DialogueData();

            _dialogueData.CharacterData = data;
            _dialogueData.CustomDialogueOptions = options;
            _standardDialogue = dialogue;
        }

        public void OverrideDialogueString(string dialouge)
        {
            _standardDialogue = dialouge;
        }

        private bool HasCharacterData()
        {
            bool hasCharacterData = _dialogueData.CharacterData != null;
            if (!hasCharacterData)
            {
                Debug.LogError("Character data is null");
            }

            return hasCharacterData;
        }

        public string GetCharacterName()
        {
            if (HasCharacterData() && !_dialogueData.CustomDialogueOptions.UsesCustomName)
            {
                return _dialogueData.CharacterData.CharacterName;
            }

            return _dialogueData.CustomDialogueOptions.CustomName;
        }

        public Sprite GetCharacterSprite()
        {
            if (CustomDialogue.UsesCustomSprite)
            {
                return CustomDialogue.CustomSprite;
            }

            if (HasCharacterData() && CustomDialogue.UseCurrentPlayerHealthForSentimentSprite && CharacterData != null && CharacterData.SentimentPortraits != null && CharacterData.SentimentPortraits.Count > 0)
            {
                ECharacterSentiment sentiment = GetPlayerHealthComponent().GetCharacterSentiment();
                return CharacterData.SentimentPortraits.ContainsKey(sentiment) ? CharacterData.SentimentPortraits[sentiment] : CharacterData.DefaultSprite;
            }

            if (HasCharacterData() && _dialogueData.CharacterData.SentimentPortraits.ContainsKey(_dialogueData.CharacterSentiment))
            {
                return _dialogueData.CharacterData.SentimentPortraits[_dialogueData.CharacterSentiment];
            }

            return CharacterData.DefaultSprite;
        }

        public string GetDialogueString()
        {
            return _standardDialogue;
        }

        private HealthComponent GetPlayerHealthComponent()
        {
            return Player.Instance.HealthComponent;
        }
    }

    [Serializable]
    public class SentimentDialogueObject
    {
        public SentimentDialogueDictionary SentimentDialogue;
    }

    [Serializable]
    public class EndConversationObject
    {

    }

    [Serializable]
    public class NewConversationObject
    {
        public Conversation NewConversation;
    }

    [Serializable]
    public abstract class GameRelatedDialogue
    {
        [SerializeField]
        protected bool _hasOpeningDialogue;
        public bool HasOpeningDialogue { get { return _hasOpeningDialogue; } }

        [SerializeField]
        protected bool _hasClosingDialogue;
        public bool HasClosingDialogue { get { return _hasClosingDialogue; } }

        [SerializeField, ShowIf("_hasOpeningDialogue")]
        protected StandardDialogueObject _defaultOpeningDialogueObject;
        [SerializeField, ShowIf("_hasClosingDialogue")]
        protected StandardDialogueObject _defaultClosingDialogueObject;

        [SerializeField]
        protected DialogueData _dialogueData;

        public virtual StandardDialogueObject GetGameCreationDialogueObject() { return StandardDialogueObject.EmptyDialogueObject; }
        public virtual StandardDialogueObject GetGameClosingDialogueObject() { return StandardDialogueObject.EmptyDialogueObject; }
    }

    [Serializable]
    public class MemoryGameRelatedDialogue : GameRelatedDialogue
    {
        public override StandardDialogueObject GetGameCreationDialogueObject()
        {
            if (MemoryGameSolverComponent.Instance.IsLookingForSingleMemoryType)
            {
                EMemoryType memoryType = MemoryGameGenerator.Instance.MemoryTypeToSearchFor;
                MemoryGameDialoguePromptData data = MemoryGameDialoguePromptsManager.Instance.GetMemoryGameDialoguePromptData(memoryType);
                if (!data)
                {
                    Debug.LogError("No data for " + memoryType);
                }
                StandardDialogueObject dialogue = new StandardDialogueObject(_dialogueData.CharacterData, _dialogueData.CustomDialogueOptions, data.QuestionPrompts.GetRandomElement());

                return dialogue;
            }
            else
            {
                return _defaultOpeningDialogueObject;
            }
        }

        public override StandardDialogueObject GetGameClosingDialogueObject()
        {
            if (MemoryGameSolverComponent.Instance.IsLookingForSingleMemoryType)
            {
                EMemoryType memoryType = MemoryGameGenerator.Instance.MemoryTypeToSearchFor;
                MemoryGameDialoguePromptData data = MemoryGameDialoguePromptsManager.Instance.GetMemoryGameDialoguePromptData(memoryType);

                bool wonGame = MemoryGameSolverComponent.Instance.WonPreviousGame;
                StandardDialogueObject dialogue = new StandardDialogueObject(_dialogueData.CharacterData, _dialogueData.CustomDialogueOptions, wonGame ? data.CorrectAnswers.GetRandomElement() : data.IncorrectAnswers.GetRandomElement());

                return dialogue;
            }
            else
            {
                return _defaultClosingDialogueObject;
            }
        }
    }

    [Serializable]
    public class CustomDialogueOptions
    {
        public bool UsesCustomName;
        [ShowIf("UsesCustomName")]
        public string CustomName;

        [HideIf("UseCurrentPlayerHealthForSentimentSprite")]
        public bool UsesCustomSprite;
        [ShowIf("UsesCustomSprite")]
        public Sprite CustomSprite;
        public bool IsThinking;

        public bool UseCurrentPlayerHealthForSentimentSprite;
    }

    [Serializable]
    public class BranchingDialogueObject
    {
        public bool OnlyUsesDialogue = true;
        public List<StandardDialogueObject> DialogueObjects;
        [HideIf("OnlyUsesDialogue")]
        public Conversation NewConversation;
    }
}
