using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame
{
    public abstract class GameSolverComponent<CompletionResultType> : BaseGameSolverComponent where CompletionResultType : GameCompletionResult
    {
        List<CompletionResultType> _gameCompletionResults;

        #region Completion Results
        public void SetGameCompletionResults(List<CompletionResultType> gameCompletionResults)
        {
            _gameCompletionResults = gameCompletionResults;
        }

        public CompletionResultType GetGameCompletionResultToApplyByTimeRemaining()
        {
            if (_gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return default(CompletionResultType);
            }

            float percentageTimeLeftToSolveMaze = GetPercentageOfTimeLeftToCompleteGame();
            // Ex player solved while only taking 25% of time, so value is 75%
            int indexDesired = 0;
            float percentagePerResult = 1f / _gameCompletionResults.Count;
            // Ex there are only 3 results, so value is 33%
            do
            {
                indexDesired++;
            }
            while (_gameCompletionResults.IsValidIndex(indexDesired) && indexDesired * percentagePerResult > percentageTimeLeftToSolveMaze);

            return _gameCompletionResults[indexDesired - 1];
        }

        public CompletionResultType GetGameCompletionResultToApplyBySucceeding()
        {
            if (_gameCompletionResults == null || _gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return default(CompletionResultType);
            }

            if (_gameCompletionResults.Count > 2)
            {
                Debug.LogError("Can only have 2 results (Win or Lose)");
                return default(CompletionResultType);
            }

            return _gameCompletionResults[WonPreviousGame ? 0 : 1];
        }
        #endregion
    }
}