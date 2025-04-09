using Characters;
using MainPlayer;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

using GeneralGame.Generation;
using MemoryGame.Dialogue;
using MemoryGame.Generation;
using MemoryGame;

namespace Dialogue
{
    public enum EDialogueObjectType
    {
        StandardDialogue,
        SpawnMaze,
        SpawnMemoryGame,
        EndConversation,
        LinkNewConversation
    }

    [Serializable]
    public class DialogueObject
    {
        public EDialogueObjectType DialogueObjectType;

        [ShowIf("DialogueObjectType", EDialogueObjectType.StandardDialogue)]
        public List<StandardDialogueObject> StandardDialogueObjects;

        [ShowIf("DialogueObjectType", EDialogueObjectType.SpawnMaze)]
        public MazeGeneratorData MazeSpawnerData;

        [ShowIf("DialogueObjectType", EDialogueObjectType.SpawnMemoryGame)]
        public MemoryGameGeneratorData MemoryGameSpawnerData;

        [ShowIf("DialogueObjectType", EDialogueObjectType.LinkNewConversation), HideLabel]
        public NewConversationObject LinkedConverssationObject;

        [ShowIf("DialogueObjectType", EDialogueObjectType.EndConversation), HideLabel]
        public EndConversationObject EndConversationObject;
    }

    [Serializable]
    public class SentimentDialogueDictionary : UnitySerializedDictionary<ECharacterSentiment, DialogueObject> { }

    [Serializable]
    public class DialogueData
    {
        [HorizontalGroup]
        public CharacterData CharacterData;
        [HorizontalGroup]
        public ECharacterSentiment CharacterSentiment = ECharacterSentiment.Neutral;

        [HideLabel]
        public CustomDialogueOptions CustomDialogueOptions;
        public bool IsCharacterMainPlayer;
    }

    [Serializable]
    public class StandardDialogueObject
    {
        [FoldoutGroup("@DialogueEditorDisplayString")]
        [SerializeField, FoldoutGroup("@DialogueEditorDisplayString")]
        private DialogueData _dialogueData;

        [SerializeField, TextArea(2, 4), FoldoutGroup("@DialogueEditorDisplayString")]
        private string _standardDialogue;

        public CharacterData CharacterData { get { return _dialogueData.CharacterData; } }
        public CustomDialogueOptions CustomDialogue { get { return _dialogueData.CustomDialogueOptions; } }

        private string DialogueEditorDisplayString { get { return CharacterData.CharacterName + " " + _dialogueData.CharacterSentiment.ToString().ToLower() + " dialogue"; } }

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

            if (HasCharacterData() && CustomDialogue.UsesSentimentSystem && CharacterData != null && CharacterData.SentimentPortraits != null && CharacterData.SentimentPortraits.Count > 0)
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
    public class EndConversationObject
    {
        public List<StandardDialogueObject> FinalDialogue;
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
        [HorizontalGroup("Row1")]
        public bool UsesCustomName;
        [ShowIf("UsesCustomName"), HorizontalGroup("Row1")]
        public string CustomName;

        [HorizontalGroup("Row2")]
        public bool UsesCustomSprite;
        [ShowIf("UsesCustomSprite"), HorizontalGroup("Row2")]
        public Sprite CustomSprite;

        [HorizontalGroup]
        public bool UsesSentimentSystem;
        [ShowIf("UsesSentimentSystem")]
        public SentimentDialogueDictionary SentimentDialogues;

        public bool PlayerHasSentiment()
        {
            ECharacterSentiment sentiment = Player.Instance.HealthComponent.GetCharacterSentiment();
            return UsesSentimentSystem && SentimentDialogues.ContainsKey(sentiment);
        }

        public DialogueObject GetSentimentDialogue()
        {
            ECharacterSentiment sentiment = Player.Instance.HealthComponent.GetCharacterSentiment();
            return PlayerHasSentiment() ? SentimentDialogues[sentiment] : null;
        }
    }
}
