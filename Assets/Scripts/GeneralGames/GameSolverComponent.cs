using Dialogue.UI;
using GeneralGame.Generation;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame
{
    public abstract class GameSolverComponent<GameGenerator, CompletionResultType> : BaseGameSolverComponent where GameGenerator : BaseGameGenerator where CompletionResultType : GameCompletionResult
    {
        protected List<CompletionResultType> _gameCompletionResults;

        private Coroutine _highlightIndexCoroutine;

        protected abstract GameGenerator GameGeneratorInstance { get; }

        #region Completion Results
        public void SetGameCompletionResults(List<CompletionResultType> gameCompletionResults)
        {
            _gameCompletionResults = gameCompletionResults;

            List<GameCompletionResult> results = new List<GameCompletionResult>(gameCompletionResults);
            PotentialPlayerDialogueUI.Instance.AddDialogueObjects(results);
        }

        protected override void StartGame()
        {
            base.StartGame();
            _highlightIndexCoroutine = StartCoroutine(HighlightCurrentGameResultIndex());
        }

        protected override void ApplyEndGameResults()
        {
        }

        private IEnumerator HighlightCurrentGameResultIndex()
        {
            while (IsStage(EGameStage.InGame))
            {
                PotentialPlayerDialogueUI.Instance.HighlightResult(GetCurrentPotentialDialogueIndex());
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
            int index = Mathf.Clamp(Mathf.FloorToInt(_gameCompletionResults.Count * percentageTimeLeftToSolveMaze), 0, _gameCompletionResults.Count - 1);
            // Ex there are only 3 results, so value is 33%

            return _gameCompletionResults.Count - 1 - index;
        }

        public CompletionResultType GetGameCompletionResultToApplyByTimeRemaining()
        {
            int index = GetGameCompletionResultIndexByTimeRemaining();
            if (_gameCompletionResults.IsValidIndex(index))
            {
                return _gameCompletionResults[index];
            }
            else
            {
                Debug.LogError("Trying to use index: " + index + " in results.");
                return default(CompletionResultType);
            }
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