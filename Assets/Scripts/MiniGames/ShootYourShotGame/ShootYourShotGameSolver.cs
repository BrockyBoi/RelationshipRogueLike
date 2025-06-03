using UnityEngine;
using System.Collections;
using GeneralGame;

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

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByTimeRemaining();
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            return GetPercentageOfTimeLeftToCompleteGame();
        }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        protected override void StartGame()
        {
            base.StartGame();
            _timeLeftToFinish = 0;
            _bullseye.SetActive(true);
        }

        protected override void EndGame()
        {
            base.EndGame();

            _bullseye.SetActive(false);
        }

        protected override void StartGameTimer()
        {
            StartGame();
            StartCoroutine(StartCustomTimer());
        }

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
            StartCoroutine(RunGameTimer());

            yield return new WaitForSeconds(_gameData.TimeAllowedToShootTarget);

            FailGame();
        }

        protected override void Update()
        {
            base.Update();

            if (CanPlayGame() && Input.GetKeyDown(KeyCode.Space))
            {
                AttemptShootTarget();
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
    }
}
