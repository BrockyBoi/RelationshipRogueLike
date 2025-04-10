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

    [Serializable]
    public class MazeGeneratorData : GameGenerationData<MazeCompletionResult>
    {
        [FoldoutGroup("MazeData")]
        public Vector2Int GridSize;
        [FoldoutGroup("MazeData")]
        public float ShakeIntensity = 0;
        [FoldoutGroup("MazeData")]
        public float RotationSpeed = 0;
        [FoldoutGroup("MazeData")]
        public float TimeToSolveMaze = 10;
    }

    [Serializable]
    public class MemoryGameGeneratorData : GameGenerationData<MemoryGameCompletionResult>
    {
        public MemoryGameRelatedDialogue MemoryGameRelatedDialogue;

        [SerializeField, FoldoutGroup("Memory Game Data")]
        private bool _searchForSingleMemoryType;

        [ShowIf("_searchForSingleMemoryType"), FoldoutGroup("Memory Game Data")]
        public bool _forceMemoryTypeToSearchFor;
        [ShowIf("_forceMemoryTypeToSearchFor"), FoldoutGroup("Memory Game Data")]
        public EMemoryType _forcedMemoryTypeToSearchFor;

        [SerializeField, FoldoutGroup("Memory Game Data")]
        private bool _showLimitedMemoryTypesAvailable;
        [ShowIf("_showLimitedMemoryTypesAvailable"), FoldoutGroup("Memory Game Data")]
        public EMemoryType _allowedMemoryTypes = EMemoryType.ALL;

        [FoldoutGroup("Memory Game Data")]
        public Vector2Int GridSize;

        [FoldoutGroup("Memory Game Data")]
        public int NumberOfGuesses = 5;

        public void GenerateMemoryGameData()
        {
            if (!_showLimitedMemoryTypesAvailable || (_showLimitedMemoryTypesAvailable && _allowedMemoryTypes == 0))
            {
                _allowedMemoryTypes = EMemoryType.ALL;
            }

            EMemoryType memoryType = _forceMemoryTypeToSearchFor ? _forcedMemoryTypeToSearchFor : EMemoryType.Bomb;
            if (_searchForSingleMemoryType && memoryType == EMemoryType.Bomb)
            {
                int iterations = 0;
                do
                {
                    memoryType = GlobalFunctions.RandomEnumValue(EMemoryType.ALL, EMemoryType.Bomb);
                    if (iterations++ >= 100)
                    {
                        Debug.LogError("Stuck in while loop");
                        break;
                    }
                }
                while (!_allowedMemoryTypes.Has(memoryType));
            }

            MemoryGameSolverComponent.Instance.SetIsLookingForSingleMemoryType(_searchForSingleMemoryType);
            MemoryGameGenerator.Instance.SetMemoryTypeToSearchFor(memoryType, _allowedMemoryTypes);
        }
    }
}
