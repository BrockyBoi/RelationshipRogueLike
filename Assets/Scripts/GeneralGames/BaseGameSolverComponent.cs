using CustomUI;
using GeneralGame.Generation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame
{
    public enum EGameStage
    {
        PreCountdown,
        DuringCountdown,
        InGame,
        GameFinished
    }

    public abstract class BaseGameSolverComponent : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        protected float _totalPenaltyTime = 0f;

        [SerializeField]
        protected float _timeToCompleteGame = 10f;
        protected float _countDownTimeGiven = 3f;

        protected float _timeLeftToFinish = 0;

        public bool WonPreviousGame { get; protected set; }

        public EGameStage CurrentGameState { get; private set; }

        protected Coroutine _gameStartCountdownCoroutine;

        public System.Action OnStartGameCountdownBegin;
        public System.Action OnStartGameCountdownLeft;
        public System.Action OnGameStart;
        public System.Action OnGameStop;

        public static System.Action OnAnyGamePresented;

        public System.Action<float> OnCountdownValueChange;
        public System.Action<float> OnMainTimerValueChange;

        public System.Action OnGameCompleted;
        public System.Action OnGameFailed;

        public System.Action<EGameStage> OnStageChange;

        #region Countdown
        protected void StartCountDown()
        {
            if (IsStage(EGameStage.PreCountdown))
            {
                OnStartGameCountdownBegin?.Invoke();
                _gameStartCountdownCoroutine = StartCoroutine(RunCoundownToBeginPlay());
            }
        }

        private IEnumerator RunCoundownToBeginPlay()
        {
            if (IsStage(EGameStage.InGame))
            {
                yield break;
            }

            SetGameStage(EGameStage.DuringCountdown);
            float countdownTime = _countDownTimeGiven;
            while (countdownTime > 0f)
            {
                countdownTime -= Time.deltaTime;
                OnCountdownValueChange(countdownTime);
                yield return null;
            }

            SetGameStage(EGameStage.InGame);

            yield return RunGameTimer();
        }

        protected void StopCountdown()
        {
            OnStartGameCountdownLeft?.Invoke();
            SetGameStage(EGameStage.PreCountdown);
            StopCoroutine(_gameStartCountdownCoroutine);
        }
        #endregion

        #region During Game
        protected virtual void StartGame()
        {
            OnGameStart?.Invoke();
            SetGameStage(EGameStage.InGame);
        }

        private IEnumerator RunGameTimer()
        {
            StartGame();

            _timeLeftToFinish = _timeToCompleteGame;
            _totalPenaltyTime = 0f;
            float countDownTimeWithPenalties = _timeLeftToFinish;
            while (countDownTimeWithPenalties > 0f && IsStage(EGameStage.InGame))
            {
                _timeLeftToFinish -= Time.deltaTime;
                countDownTimeWithPenalties = _timeLeftToFinish - _totalPenaltyTime;

                OnMainTimerValueChange?.Invoke(countDownTimeWithPenalties);
                yield return null;
            }

            FailGame();
        }
        protected float GetPercentageOfTimeLeftToCompleteGame()
        {
            return (_timeLeftToFinish - _totalPenaltyTime) / _timeToCompleteGame;
        }

        protected virtual void EndGame()
        {
            OnGameStop?.Invoke();
            SetGameStage(EGameStage.GameFinished);
            StopAllCoroutines();

            ApplyEndGameResults();
        }

        protected abstract void ApplyEndGameResults();
        #endregion

        #region EGameStage
        public bool IsStage(EGameStage stage)
        {
            return CurrentGameState == stage;
        }

        public void SetGameStage(EGameStage stage)
        {
            if (CurrentGameState != stage)
            {
                CurrentGameState = stage;

                OnStageChange?.Invoke(stage);
            }
        }
        #endregion

        #region Game Completion
        protected void FailGame()
        {
            WonPreviousGame = false;
            OnGameFailed?.Invoke();
            EndGame();
        }

        protected void CompletedGame()
        {
            WonPreviousGame = true;
            OnGameCompleted?.Invoke();
            EndGame();
        }
        #endregion

        protected abstract GameUI GetGameUIInstance();
    }
}
