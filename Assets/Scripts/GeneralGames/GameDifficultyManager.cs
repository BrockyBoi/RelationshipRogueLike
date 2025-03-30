using GeneralGame.Results;
using Maze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame
{
    public abstract class GameDifficultyManager<CompletionGameResult> : BaseGameDifficultyManager where CompletionGameResult : GameResult
    {
        public abstract void ProvideDifficultyModifierResult(CompletionGameResult result);
    }
}
