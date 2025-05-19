using Dialogue.UI;
using GeneralGame.Generation;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame
{
    public abstract class GameSolverComponent<GenerationData, CompletionResultType> : BaseGameSolverComponent where GenerationData : GameGenerationData<CompletionResultType> where CompletionResultType : GameCompletionResult, new()
    {
        protected List<CompletionResultType> _gameCompletionResults;

        private Coroutine _highlightIndexCoroutine;

        protected GenerationData _gameData;
        public GenerationData GameData { get { return _gameData; } }

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

        public virtual void SetGenerationGameData(GenerationData generationData)
        {
            _gameData = generationData;
        }

        private IEnumerator HighlightCurrentGameResultIndex()
        {
            while (IsStage(EGameStage.InGame))
            {
                PotentialPlayerDialogueUI.Instance.HighlightResult(GetCurrentPotentialDialogueIndex(), GetCurrentPotentialDialoguePercentage());
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

        public float GetCurrentPotentialDialoguePercentageByTimeRemaining()
        {
            if (_gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            float percentageTimeLeftToSolveMaze = GetPercentageOfTimeLeftToCompleteGame();

            float count = _gameCompletionResults.Count;
            int currentIndex = GetCurrentPotentialDialogueIndex();
            float maxPercentage = (count - currentIndex) / count;
            float minPercentage = Mathf.Clamp((count - currentIndex - 1) / count, 0, count - 1);
            float modifiedMax = maxPercentage - minPercentage;


            return (percentageTimeLeftToSolveMaze - minPercentage) / modifiedMax;
        }

        public float GetCurrentPotentialDialoguePercentageByGameHealthRemaining(int currentHealth, int maxHealth)
        {
            if (_gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            float percentageHealthRemaining = currentHealth / (float)maxHealth;

            float count = _gameCompletionResults.Count;
            int currentIndex = GetGameCompletionResultIndexByHealthRemaining(currentHealth, maxHealth);
            float maxPercentage = (count - currentIndex) / count;
            float minPercentage = Mathf.Clamp((count - currentIndex - 1) / count, 0, count - 1);
            float modifiedMax = maxPercentage - minPercentage;


            return (percentageHealthRemaining - minPercentage) / modifiedMax;
        }

        public int GetGameCompletionResultIndexByHealthRemaining(int currentHealth, int maxHealth)
        {
            if (_gameCompletionResults == null || _gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            float healthPercentage = 1f - (currentHealth / (float)maxHealth);
            return Mathf.Clamp(Mathf.RoundToInt(_gameCompletionResults.Count * healthPercentage), 0, _gameCompletionResults.Count - 1);
        }

        public float GetCurrentPotentialDialoguePercentageByPointsNeededToScore(int currentScore, int maxScore)
        {
            if (_gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            float percentageScoreGained = currentScore / (float)maxScore;

            float count = _gameCompletionResults.Count;
            int currentIndex = GetGameCompletionResultIndexByPointsNeededToScore(currentScore, maxScore);
            float maxPercentage = (count - currentIndex) / count;
            float minPercentage = Mathf.Clamp((count - currentIndex - 1) / count, 0, count - 1);
            float modifiedMax = maxPercentage - minPercentage;


            return (percentageScoreGained - minPercentage) / modifiedMax;
        }

        public int GetGameCompletionResultIndexByPointsNeededToScore(int currentPoints, int maxPoints)
        {
            if (_gameCompletionResults == null || _gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            float scorePercentage = currentPoints / (float)maxPoints;
            return Mathf.Clamp(Mathf.RoundToInt(_gameCompletionResults.Count * scorePercentage), 0, _gameCompletionResults.Count - 1);
        }

        public CompletionResultType GetCurrentCompletionResult()
        {
            return _gameCompletionResults.IsValidIndex(GetCurrentPotentialDialogueIndex()) ? _gameCompletionResults[GetCurrentPotentialDialogueIndex()] : default(CompletionResultType);
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