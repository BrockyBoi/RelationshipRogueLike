using Dialogue.UI;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame
{
    public abstract class GameSolverComponent<CompletionResultType> : BaseGameSolverComponent where CompletionResultType : GameCompletionResult
    {
        protected List<CompletionResultType> _gameCompletionResults;

        private Coroutine _highlightIndexCoroutine;

        #region Completion Results
        public void SetGameCompletionResults(List<CompletionResultType> gameCompletionResults)
        {
            _gameCompletionResults = gameCompletionResults;

            List<GameCompletionResult> results = new List<GameCompletionResult>(gameCompletionResults);
            PotentialPlayerDialogueUI.Instance.AddDialogueObjects(results);
        }

        protected override void ApplyEndGameResults()
        {
            PotentialPlayerDialogueUI.Instance.DestroyAllDialogueOptions();
        }

        protected override void StartGame()
        {
            base.StartGame();
            _highlightIndexCoroutine = StartCoroutine(HighlightCurrentGameResultIndex());
        }

        private IEnumerator HighlightCurrentGameResultIndex()
        {
            while (IsStage(EGameStage.InGame))
            {
                PotentialPlayerDialogueUI.Instance.HighlightResult(GetGameCompletionResultIndexByTimeRemaining());
                yield return null;
            }
        }

        public int GetGameCompletionResultIndexByTimeRemaining()
        {
            if (_gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            float percentageTimeLeftToSolveMaze = GetPercentageOfTimeLeftToCompleteGame();
            // Ex player solved while only taking 25% of time, so value is 75%
            int index = Mathf.FloorToInt(_gameCompletionResults.Count * percentageTimeLeftToSolveMaze);
            // Ex there are only 3 results, so value is 33%

            return _gameCompletionResults.Count - index - 1;
        }

        public CompletionResultType GetGameCompletionResultToApplyByTimeRemaining()
        {
            return _gameCompletionResults[GetGameCompletionResultIndexByTimeRemaining()];
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