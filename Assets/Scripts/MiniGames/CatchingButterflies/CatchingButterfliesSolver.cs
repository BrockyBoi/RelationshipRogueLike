using UnityEngine;
using System.Collections;
using GeneralGame;
using CustomUI;
using WhackAMole;

namespace CatchingButterflies
{
    public class CatchingButterfliesSolver : GameSolverComponent<CatchingButterfliesGenerationData, CatchingButterfliesCompletionResult>
    {

        public static CatchingButterfliesSolver Instance { get; private set; }

        private int _butterfliesCaught = 0;
        public int ButterfliesCaught { get { return _butterfliesCaught; } }
        public int ButterfliesNeededToCatch { get { return _gameData.ButterfliesNeededToCatch; } }

        public System.Action<int> OnButterflyCountChange;

        private void Awake()
        {
            Instance = this;
        }

        protected override void StartGame()
        {
            base.StartGame();
            _butterfliesCaught = 0;
            CatchingButterfliesGenerator.Instance.StartSpawningButterflies();

            OnMainTimerEnd += FailGame;
        }

        protected void OnDestroy()
        {
            OnMainTimerEnd -= FailGame;
        }

        public override void SetGenerationGameData(CatchingButterfliesGenerationData generationData)
        {
            base.SetGenerationGameData(generationData);

            SetTimeToCompleteGame(generationData.GameDuration - 3);
            StartGameTimer();
        }

        public void CatchButterfly()
        {
            _butterfliesCaught++;

            OnButterflyCountChange?.Invoke(_butterfliesCaught);

            if (_butterfliesCaught >= _gameData.ButterfliesNeededToCatch)
            {
                CompletedGame();
            }
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByPointsNeededToScore(_butterfliesCaught, _gameData.ButterfliesNeededToCatch);
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            return GetCurrentPotentialDialoguePercentageByPointsNeededToScore(_butterfliesCaught, _gameData.ButterfliesNeededToCatch);
        }

        protected override void ApplyEndGameResults()
        {
            CatchingButterfliesCompletionResult result = _gameCompletionResults[GetCurrentPotentialDialogueIndex()];
            result.ApplyEffects();
        }

        protected override BaseGameUI GetGameUIInstance()
        {
            throw new System.NotImplementedException();
        }
    }
}
