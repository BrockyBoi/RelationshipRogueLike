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
        public StandardDialogueObject StandardDialogueObject;

        [ShowIf("DialogueObjectType", EDialogueObjectType.SpawnMaze)]
        public MazeSpawnerDialogue MazeSpawnerDialogue;
    }

    [Serializable]
    public class StandardDialogueObject
    {
        public string CharacterName;

        public bool IsCharacterMainPlayer;
        public bool UsesSentimentSystem;

        [ShowIf("UsesSentimentSystem")]
        public ECharacterSentiment CharacterSentiment;

        [ShowIf("@UsesSentimentSystem && CharacterSentiment == ECharacterSentiment.Happy"), TextArea(2, 4)]
        public string HappyDialogue;

        [ShowIf("@UsesSentimentSystem && CharacterSentiment == ECharacterSentiment.Neutral"), TextArea(2, 4)]
        public string NeutralDialogue;

        [ShowIf("@UsesSentimentSystem && CharacterSentiment == ECharacterSentiment.Annoyed"), TextArea(2, 4)]
        public string AnnoyedDialogue;

        [ShowIf("@UsesSentimentSystem && CharacterSentiment == ECharacterSentiment.FuckingPissed"), TextArea(2, 4)]
        public string FuckingPissedDialogue;

        [HideIf("UsesSentimentSystem"), TextArea(2, 4)]
        public string StandardDialogue;

        public string GetDialogueString(HealthComponent healthComponent)
        {
            if (UsesSentimentSystem && healthComponent != null)
            {
                switch (healthComponent.GetCharacterSentiment())
                {
                    case ECharacterSentiment.Happy:
                        return HappyDialogue;
                    case ECharacterSentiment.Neutral:
                        return NeutralDialogue;
                    case ECharacterSentiment.Annoyed:
                        return AnnoyedDialogue;
                    case ECharacterSentiment.FuckingPissed:
                        return FuckingPissedDialogue;
                }
            }
            else
            {
                return StandardDialogue;
            }

            Debug.LogError("Something went wrong and return empty string");
            return string.Empty;
        }
    }

    [Serializable]
    public class MazeSpawnerDialogue
    {
        public MazeSpawnData MazeSpawnData;
        public List<MazeCompletionResult> MazeCompletionResults;
    }
}
