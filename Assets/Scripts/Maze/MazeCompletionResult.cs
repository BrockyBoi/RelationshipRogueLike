using GeneralGame.Results;
using System;
using UnityEngine;

namespace Maze
{
    [Serializable]
    public class MazeCompletionResult : GameCompletionResult
    {
        public MazeDifficultyModifierResult DifficultyModifierResult;

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
