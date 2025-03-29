using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneralGame.Results;
using Maze;
using System;

namespace MemoryGame
{
    [Serializable]
    public class MemoryGameCompletionResult : GameCompletionResult
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
        public int GridSizeModifier;

        [Range(-4, 4)]
        public int NumberOfCardsToSwap;
        
        [Range(-4, 4)]
        public int BombsInGame;

        [Range(-4, 4)]
        public int NumberOfGuesses;

        public override void ApplyEffect()
        {
            MemoryGameDifficultyManager.Instance.ProvideDifficultyModifierResult(this);
        }
    }
}
