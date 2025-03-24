using Characters;
using Dialogue;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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
        public List<StandardDialogueObject> DialogueResponses;

        [ShowIf("DialogueResultType", EMazeCompletionResultType.HealthResult)]
        public DialogueHealthResult HealthResult;

        [ShowIf("DialogueResultType", EMazeCompletionResultType.DifficultyResult)]
        public DifficultyModifierResult DifficultyModifierResult;

        public void ApplyEffects()
        {
            MazeDifficultyManager.Instance.ProvideDifficultyModifierResult(DifficultyModifierResult);

            int healthToChange = HealthResult.HealthAmountToChange;
            if (healthToChange != 0)
            {
                MainPlayer.Player.Instance.HealthComponent.ChangeHealth(healthToChange);
            }
        }
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
