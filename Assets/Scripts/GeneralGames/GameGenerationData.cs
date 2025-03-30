using Dialogue;
using GeneralGame.Results;
using Maze;
using MemoryGame;
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

    [Serializable]
    public class MazeGeneratorData : GameGenerationData<MazeCompletionResult>
    {
        public Vector2Int GridSize;
    }

    [Serializable]
    public class MemoryGameGeneratorData : GameGenerationData<MemoryGameCompletionResult>
    {
        public MemoryGameRelatedDialogue OpeningDialogue;
        public Vector2Int GridSize;
    }
}
