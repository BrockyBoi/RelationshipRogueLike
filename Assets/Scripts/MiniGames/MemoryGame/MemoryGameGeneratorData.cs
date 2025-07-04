using Dialogue;
using GeneralGame.Generation;
using MemoryGame.Generation;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

namespace MemoryGame
{
    [Serializable]
    public class MemoryGameGeneratorData : GridGameGenerationData<MemoryGameCompletionResult>
    {
        public MemoryGameRelatedDialogue MemoryGameRelatedDialogue;

        [SerializeField, FoldoutGroup("Memory Game Data")]
        private bool _searchForSingleMemoryType;

        [ShowIf("_searchForSingleMemoryType"), FoldoutGroup("@FoldoutGroupName")]
        public bool _forceMemoryTypeToSearchFor;
        [ShowIf("_forceMemoryTypeToSearchFor"), FoldoutGroup("@FoldoutGroupName")]
        public EMemoryType _forcedMemoryTypeToSearchFor;

        [SerializeField, FoldoutGroup("@FoldoutGroupName")]
        private bool _showLimitedMemoryTypesAvailable;
        [ShowIf("_showLimitedMemoryTypesAvailable"), FoldoutGroup("@FoldoutGroupName")]
        public EMemoryType _allowedMemoryTypes = EMemoryType.ALL;

        [FoldoutGroup("@FoldoutGroupName")]
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
                    memoryType = RandomEnumValue(EMemoryType.ALL, EMemoryType.Bomb);
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
