using CatchingButterflies;
using Dialogue.UI;
using GeneralGame.Generation;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

namespace GeneralGame
{
    public abstract class GameSolverComponent<GenerationData, CompletionResultType> : BaseGameSolverComponent where GenerationData : GameGenerationData<CompletionResultType> where CompletionResultType : GameCompletionResult, new()
    {
        protected List<CompletionResultType> _gameCompletionResults;

        private Coroutine _highlightIndexCoroutine;

        protected GenerationData _gameData;
        public GenerationData GameData { get { return _gameData; } }

        [SerializeField]
        protected bool _startGameTimerOnInitialize = true;

        #region Completion Results
        private void SetGameCompletionResults(List<CompletionResultType> gameCompletionResults)
        {
            if (ensure(gameCompletionResults != null && gameCompletionResults.Count > 0, "There are no game completion results"))
            {
                _gameCompletionResults = gameCompletionResults;

                List<GameCompletionResult> results = new List<GameCompletionResult>(gameCompletionResults);
                PotentialPlayerDialogueUI.Instance.AddDialogueObjects(results);
            }
        }

        public void GeneratorInitializeSolver(List<CompletionResultType> gameCompletionResults, GenerationData data)
        {
            SetGameStage(EGameStage.PreCountdown);
            MiniGameControllersManager.Instance.SetCurrentGameType(_gameType);
            SetGameCompletionResults(gameCompletionResults);
            SetGenerationGameData(data);
        }

        protected virtual void SetGenerationGameData(GenerationData generationData)
        {
            if (ensure(generationData != null, "There is no generation data"))
            {
                _gameData = generationData;

                SetTimeToCompleteGame(generationData.GameDuration);

                if (_startGameTimerOnInitialize)
                {
                    StartGameTimer();
                }
            }
        }

        protected void UpdatePotentialPlayerDialogueUI()
        {
            PotentialPlayerDialogueUI.Instance.HighlightResult(GetCurrentPotentialDialogueIndex(), GetCurrentPotentialDialoguePercentage());
        }

        public int GetGameCompletionResultIndexByTimeRemaining()
        {
            return GetRoundedAndClampedResultIndex(GetPercentageOfTimeLeftToCompleteGame());
        }

        public float GetCurrentPotentialDialoguePercentageByTimeRemaining()
        {
            return GetPercentageOfResultIndex(GetPercentageOfTimeLeftToCompleteGame());
        }

        public float GetCurrentPotentialDialoguePercentageByGameHealthRemaining(int currentHealth, int maxHealth)
        {
            return ensure(maxHealth > 0, "Max health must be greater than 0") ? GetPercentageOfResultIndex(currentHealth / (float)maxHealth) : 0f;
        }

        public float GetPercentageOfResultIndex(float totalPercentage)
        {
            if (!ensure(_gameCompletionResults.Count > 0, "There are no game completion results"))
            {
                return 0f;
            }

            float count = _gameCompletionResults.Count;
            int index = GetCurrentPotentialDialogueIndex();
            float maxPercentage = (count - index) / count;
            if (!ensure(totalPercentage <= maxPercentage, "The total percentage can't be greater than the new max percentage: Total Percentage: " + totalPercentage + " vs : Max Percentage: " + maxPercentage))
            {
                return 0f;
            }
            float minPercentage = Mathf.Clamp((count - index - 1) / count, 0, 1);
            float modifiedMax = maxPercentage - minPercentage;

            if (!Mathf.Approximately(minPercentage, 0) && !ensure(totalPercentage >= minPercentage, "The total percentage should not be less than the modified min percentage: Total Percentage: " + totalPercentage + " vs. min percentage: " + minPercentage))
            {
                return 0f;
            }

            float modifiedPercentage = (totalPercentage - minPercentage) / modifiedMax;
            if (!ensure(modifiedPercentage <= 1, "The modified percentage cannot go over 100%: Modified Percentage: " + modifiedPercentage))
            {
                return 0f;
            }

            return modifiedPercentage;
        }

        public int GetGameCompletionResultIndexByHealthRemaining(int currentHealth, int maxHealth)
        {
            return GetRoundedAndClampedResultIndex((currentHealth / (float)maxHealth));
        }

        private int GetRoundedAndClampedResultIndex(float currentPercentage)
        {
            if (!ensure(_gameCompletionResults != null && _gameCompletionResults.Count != 0, "There are no completion results"))
            {
                return 0;
            }

            // 0 - 33%   [0]
            // 33 - 67%  [1]
            // 67 - 100% [2]
            // If 50%, do .5 * 3 = 1.5 = 1
            // If 75%, do .75 * 3 = 2.25 = 2
            // If 100%, do 1 * 3 = 2
            // If 99%, do .99 * 3 = 2.97
            // If 10%, do .1 * 3 = .3 = 0

            currentPercentage = 1 - currentPercentage;
            int roundedValue = Mathf.FloorToInt(_gameCompletionResults.Count * currentPercentage);
            return Mathf.Clamp(roundedValue, 0, _gameCompletionResults.Count - 1);
        }

        public float GetCurrentPotentialDialoguePercentageByPointsNeededToScore(int currentScore, int maxScore)
        {
            return GetPercentageOfResultIndex(currentScore / (float)maxScore);
        }

        public int GetGameCompletionResultIndexByPointsNeededToScore(int currentPoints, int maxPoints)
        {
            return GetRoundedAndClampedResultIndex(currentPoints / (float)maxPoints);
        }

        protected override void ApplyEndGameResults()
        {
            base.ApplyEndGameResults();

            CompletionResultType result = _gameCompletionResults[GetCurrentPotentialDialogueIndex()];
            result.ApplyEffects();
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