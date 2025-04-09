using Dialogue;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    [Serializable]
    public class MazeCompletionResult : GameCompletionResult
    {
        [FoldoutGroup("@GetResultTitleString")]
        public MazeDifficultyModifierResult DifficultyModifierResult;

        [FoldoutGroup("@GetResultTitleString")]
        public List<DialogueObject> MazeDialogueResponses;

        public override void ApplyEffects()
        {
            base.ApplyEffects();

            DifficultyModifierResult.ApplyEffect();
        }
    }

    [Serializable]
    public class MazeDifficultyModifierResult : GameResult
    {
        [Range(-4, 4)]
        public int MazeSizeModifier;

        [Range(-.25f, .25f)]
        public float MazeShakeModifier;

        [Range(-.25f, .25f)]
        public float MazeRotateModifier;

        public override void ApplyEffect()
        {
            MazeDifficultyManager.Instance.ProvideDifficultyModifierResult(this);
        }
    }
}
