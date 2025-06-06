using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeneralGame.Results;
using System;
using Dialogue;
using Sirenix.OdinInspector;

namespace MemoryGame
{
    [Serializable]
    public class MemoryGameCompletionResult : GameCompletionResult
    {
        [FoldoutGroup("@GetResultTitleString")]
        public bool HasResponseBasedOnSpecificMemoryType;

        [FoldoutGroup("@GetResultTitleString"), ShowIf("HasResponseBasedOnSpecificMemoryType")]
        public MemoryGameRelatedDialogue GameRelatedDialogue;

        [FoldoutGroup("@GetResultTitleString")]
        public MemoryGameDifficultyModifierResult DifficultyModifierResult;

        public override void ApplyEffects()
        {
            base.ApplyEffects();

            DifficultyModifierResult.ApplyEffect();
        }
    }

    [Serializable]
    public class MemoryGameDifficultyModifierResult : GameResult
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
