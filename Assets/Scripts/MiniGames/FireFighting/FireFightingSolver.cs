using UnityEngine;
using System.Collections;
using GeneralGame;

namespace FireFighting
{
    public class FireFightingSolver : GameSolverComponent<FireFightingGenerationData, FireFightingCompletionResult>
    {
        public static FireFightingSolver Instance { get; private set; }

        private FireFightingWindow[,] _windows;

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByTimeRemaining();
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            return GetCurrentPotentialDialoguePercentageByTimeRemaining();
        }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            OnMainTimerEnd += FailGame;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            OnMainTimerEnd -= FailGame;
        }

        protected override void StartGame()
        {
            base.StartGame();

            _windows = FireFightingGenerator.Instance.GetGrid();
        }

        protected override void Update()
        {
            base.Update();

            if (CanPlayGame() && _windows != null)
            {
                bool stillHasAFire = false;
                foreach (FireFightingWindow window in _windows)
                {
                    if (window != null && !window.FirePutOut)
                    {
                        stillHasAFire = true;
                        window.IncreaseFireLevel(_gameData.FireIncreasePerSecond * Time.deltaTime);
                    }
                }

                Camera.main.transform.position = Camera.main.transform.position + Vector3.up * _gameData.AmountToMoveUpPerSecond * Time.deltaTime;

                if (!stillHasAFire)
                {
                    CompleteGame();
                }
            }
        }

        protected override void MainTimerValueChanged(float newTime)
        {
            UpdatePotentialPlayerDialogueUI();
        }
    }
}
