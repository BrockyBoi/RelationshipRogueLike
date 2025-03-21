using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Maze
{
    public enum EMazeCompletionResultType
    {
        DialogueResponse,
        HealthResult,
        DifficultyResult
    }

    [Serializable]
    public class MazeCompletionResult
    {
        [EnumToggleButtons]
        public EMazeCompletionResultType DialogueResultType;

        [ShowIf("DialogueResultType", EMazeCompletionResultType.DialogueResponse)]
        public DialogueResponseResult DialogueResponse;

        [ShowIf("DialogueResultType", EMazeCompletionResultType.HealthResult)]
        public DialogueHealthResult HealthResult;

        [ShowIf("DialogueResultType", EMazeCompletionResultType.DifficultyResult)]
        public DifficultyModifierResult DifficultyModifierResult;
    }

    [Serializable]
    public class DialogueResponseResult
    {
        public string CharacterName;
        [TextArea(2,10)]
        public string DialogueResponse;
    }

    [Serializable]
    public class DialogueHealthResult
    {
        [Range(-10, 10)]
        public int HealthAmountToChange = 0;
    }

    [Serializable]
    public class DifficultyModifierResult
    {
        [Range(-4, 4)]
        public int MazeSizeModifier;

        [Range(-.25f, .25f)]
        public float MazeShakeModifier;

        [Range(-.25f, .25f)]
        public float MazeRotateModifier;
    }
}
