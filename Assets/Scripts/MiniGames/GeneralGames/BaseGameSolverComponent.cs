using CustomUI;
using GeneralGame.Generation;
using MainPlayer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

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

        [SerializeField]
        protected EGameType _gameType;

        public bool WonPreviousGame { get; protected set; }

        public EGameStage CurrentGameState { get; private set; }

        protected Coroutine _gameStartCountdownCoroutine;

        public System.Action OnStartGameCountdownBegin;
        public System.Action OnStartGameCountdownEnd;
        public System.Action OnGameStart;
        public System.Action OnGameStop;

        public static System.Action OnAnyGamePresented;

        public System.Action<float> OnCountdownValueChange;
        public System.Action<float> OnMainTimerValueChange;
        public System.Action OnMainTimerEnd;

        public System.Action OnGameWin;
        public System.Action OnGameFailed;

        public System.Action<EGameStage> OnStageChange;

        protected virtual void Awake()
        {
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void Update()
        {
            if (IsStage(EGameStage.InGame) && Input.GetKeyDown(KeyCode.Tab))
            {
                CompleteGame();
            }
        }

        protected virtual void OnDisable()
        {

        }

        #region Countdown
        protected virtual void StartCountDown()
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
                OnCountdownValueChange?.Invoke(countdownTime);
                yield return null;
            }

            StartGameTimer();
        }

        protected void StopCountdown()
        {
            OnStartGameCountdownEnd?.Invoke();
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

        protected virtual void StartGameTimer()
        {
            StartGame();
            StartCoroutine(RunGameTimer());
        }

        protected IEnumerator RunGameTimer()
        {
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

        protected virtual void MainTimerValueChanged(float newTime)
        {

        }

        public void AddPenaltyTime(float penalty)
        {
            _totalPenaltyTime += penalty;
        }

        protected float GetPercentageOfTimeLeftToCompleteGame()
        {
            return ensure(_timeToCompleteGame > 0, "Time to complete game must be greater than 0") ? (_timeLeftToFinish - _totalPenaltyTime + _bonusTimeGained) / _timeToCompleteGame : 0f;
        }

        protected virtual void EndGame()
        {
            OnGameStop?.Invoke();
            SetGameStage(EGameStage.GameFinished);
            StopAllCoroutines();

            ApplyEndGameResults();
        }

        protected virtual void ApplyEndGameResults()
        {

        }
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

        protected void CompleteGame()
        {
            WonPreviousGame = true;
            OnGameWin?.Invoke();
            EndGame();
        }
        #endregion

        public void SetTimeToCompleteGame(float time)
        {
            _timeToCompleteGame = time;
            _timeLeftToFinish = time;
        }

        public abstract int GetCurrentPotentialDialogueIndex();
        public abstract float GetCurrentPotentialDialoguePercentage();

        public virtual bool CanPlayGame()
        {
            return CurrentGameState == EGameStage.InGame && !PauseController.Instance.IsPaused;
        }
    }
}
