using CustomUI;
using GeneralGame.Generation;
using MainPlayer;
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
        InGameInputPrevented,
        InGame,
        GameFinished
    }

    public abstract class BaseGameSolverComponent : MonoBehaviour
    {
        [ShowInInspector, ReadOnly]
        protected float _totalPenaltyTime = 0f;
        protected float _bonusTimeGained = 0f;

        protected float _timeToCompleteGame = 10f;

        [SerializeField]
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
        public System.Action OnMainTimerEnd;

        public System.Action OnGameWin;
        public System.Action OnGameFailed;

        public System.Action<EGameStage> OnStageChange;

        protected virtual void Start()
        {
            
        }

        protected virtual void Update()
        {
            if (IsStage(EGameStage.InGame) && Input.GetKeyDown(KeyCode.Tab))
            {
                CompletedGame();
            }
        }

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

            StartGameTimer();
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
            _timeLeftToFinish = _timeToCompleteGame;
            _totalPenaltyTime = 0f;
            _bonusTimeGained = 0f;

            OnGameStart?.Invoke();
            SetGameStage(EGameStage.InGame);
        }

        protected void StartGameTimer()
        {
            StartCoroutine(RunGameTimer());
        }

        private IEnumerator RunGameTimer()
        {
            StartGame();

            float countDownTimeWithPenalties = _timeLeftToFinish;
            while (countDownTimeWithPenalties > 0f && IsStage(EGameStage.InGame))
            {
                _timeLeftToFinish -= Time.deltaTime;
                countDownTimeWithPenalties = _timeLeftToFinish - _totalPenaltyTime + _bonusTimeGained;

                OnMainTimerValueChange?.Invoke(countDownTimeWithPenalties);
                yield return null;
            }

            OnMainTimerEnd?.Invoke();
        }
        protected float GetPercentageOfTimeLeftToCompleteGame()
        {
            return (_timeLeftToFinish - _totalPenaltyTime + _bonusTimeGained) / _timeToCompleteGame;
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

            // If player fails to complete game they lose an extra health point
            Player.Instance.HealthComponent.ChangeHealth(-1);
            EndGame();
        }

        protected void CompletedGame()
        {
            WonPreviousGame = true;
            OnGameWin?.Invoke();
            EndGame();
        }
        #endregion

        public void SetTimeToCompleteGame(float time)
        {
            _timeToCompleteGame = time;
        }

        protected abstract BaseGameUI GetGameUIInstance();
        public abstract int GetCurrentPotentialDialogueIndex();
        public abstract float GetCurrentPotentialDialoguePercentage();

        public virtual bool CanPlayGame()
        {
            return CurrentGameState == EGameStage.InGame && !PauseController.Instance.IsPaused;
        }
    }
}
