using Maze;
using MemoryGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    [Serializable]
    public class GameGenerationData
    {

    }

    [Serializable]
    public class MazeGeneratorData : GameGenerationData
    {
        public Vector2Int GridSize;
        public List<MazeCompletionResult> MazeCompletionResults;
    }

    [Serializable]
    public class MemoryGameGeneratorData : GameGenerationData
    {
        public Vector2Int GridSize;
        public List<MemoryGameCompletionResult> MemoryGameCompletionResults;
    }
}
