using Dialogue;
using GeneralGame.Results;
using Maze;
using MemoryGame;
using MemoryGame.Generation;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    public abstract class BaseGameGenerationData
    {

    }

    [Serializable]
    public abstract class GameGenerationData<GameResult> : BaseGameGenerationData where GameResult : GameCompletionResult
    {
        public List<GameResult> GameCompletionResults;
    }
}
