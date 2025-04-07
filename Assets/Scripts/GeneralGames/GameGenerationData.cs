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
        public Vector2Int GridSize;
        public float ShakeIntensity = 0;
        public float RotationSpeed = 0;
    }

    [Serializable]
    public class MemoryGameGeneratorData : GameGenerationData<MemoryGameCompletionResult>
    {
        public MemoryGameRelatedDialogue MemoryGameRelatedDialogue;

        [SerializeField]
        private bool _searchForSingleMemoryType;

        [ShowIf("_searchForSingleMemoryType")]
        public bool _forceMemoryTypeToSearchFor;
        [ShowIf("_forceMemoryTypeToSearchFor")]
        public EMemoryType _forcedMemoryTypeToSearchFor;

        public bool _showLimitedMemoryTypesAvailable;
        [ShowIf("_showLimitedMemoryTypesAvailable")]
        public EMemoryType _allowedMemoryTypes;

        public Vector2Int GridSize;

        public void GenerateMemoryGameData()
        {
            EMemoryType memoryType = _forceMemoryTypeToSearchFor ? _forcedMemoryTypeToSearchFor : EMemoryType.Bomb;
            if (_searchForSingleMemoryType && memoryType == EMemoryType.Bomb)
            {
                do
                {
                    memoryType = GlobalFunctions.RandomEnumValue<EMemoryType>();
                }
                while (memoryType == EMemoryType.Bomb || (_allowedMemoryTypes & memoryType) == 0 || memoryType == EMemoryType.ALL);
            }

            MemoryGameSolverComponent.Instance.SetIsLookingForSingleMemoryType(_searchForSingleMemoryType);
            MemoryGameGenerator.Instance.SetMemoryTypeToSearchFor(memoryType, _allowedMemoryTypes);
        }
    }
}
