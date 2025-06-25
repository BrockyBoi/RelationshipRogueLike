using UnityEngine;
using System.Collections;
using GeneralGame;
using System.Linq;

namespace ShootYourShotGame
{
    public class ShootYourShotGameSolver : GameSolverComponent<ShootYourShotGameGenerationData, ShootYourShotGameCompletionResult>
    {
        public static ShootYourShotGameSolver Instance { get; private set; }
        public System.Action<float> OnCustomTimerValueSet;
        public System.Action OnCustomTimerEnd;

        private bool _hasCustomTimerEnded = false;

        [SerializeField]
        private GameObject _bullseye;

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            _startGameTimerOnInitialize = false;
        }

        protected override void MainTimerValueChanged(float newTime)
        {
            UpdatePotentialPlayerDialogueUI();
        }

        protected override void StartCountDown()
        {
            OnStartGameCountdownBegin?.Invoke();
            _bullseye.SetActive(true);
            StartCoroutine(StartCustomTimer());
        }

        protected override void EndGame()
        {
            base.EndGame();

            _bullseye.SetActive(false);
        }

        protected override void SetGenerationGameData(ShootYourShotGameGenerationData generationData)
        {
            base.SetGenerationGameData(generationData);
            StartCountDown();
        }

        //protected override void StartGameTimer()
        //{
        //    SetGameStage(EGameStage.DuringCountdown);
        //}

        private IEnumerator StartCustomTimer()
        {
            float timeLeft = 3;
            OnCustomTimerValueSet?.Invoke(timeLeft);
            if (_gameData.IsStandardCountdown)
            {
                while (timeLeft > 0)
                {
                    yield return new WaitForSeconds(1);
                    timeLeft--;
                    OnCustomTimerValueSet?.Invoke(timeLeft);
                }
            }
            else
            {
                yield return new WaitForSeconds(_gameData.TimeBetween3And2);
                OnCustomTimerValueSet?.Invoke(2);

                yield return new WaitForSeconds(_gameData.TimeBetween2And1);
                OnCustomTimerValueSet?.Invoke(1);

                yield return new WaitForSeconds(_gameData.TimeBetween1And0);
            }

            _hasCustomTimerEnded = true;
            OnCustomTimerEnd?.Invoke();

              yield return RunAimingTimer();
        }

        private IEnumerator RunAimingTimer()
        {
            SetTimeToCompleteGame(_gameData.TimeAllowedToShootTarget);
            StartGameTimer();

            yield return new WaitForSeconds(_gameData.TimeAllowedToShootTarget);

            FailGame();
        }

        protected override void Update()
        {
            if (CanPlayGame())
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    AttemptShootTarget();
                }

                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    CompleteGame();
                }
            }
        }

        private void AttemptShootTarget()
        {
            if (!_hasCustomTimerEnded)
            {
                FailGame();
            }
            else
            {
                CompleteGame();
            }
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return IsStage(EGameStage.InGame) || IsStage(EGameStage.GameFinished) ? GetGameCompletionResultIndexByTimeRemaining() : _gameCompletionResults.Count - 1;
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            return GetCurrentPotentialDialoguePercentageByTimeRemaining();
        }
    }
}
