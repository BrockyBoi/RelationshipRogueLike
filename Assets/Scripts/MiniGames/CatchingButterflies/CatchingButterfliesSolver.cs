using UnityEngine;
using System.Collections;
using GeneralGame;
using CustomUI;
using WhackAMole;

namespace CatchingButterflies
{
    public class CatchingButterfliesSolver : CollectObjectsGameSolver<CatchingButterfliesGenerationData, CatchingButterfliesCompletionResult>
    {
        public static CatchingButterfliesSolver Instance { get; private set; }

        public override int CollectablesNeeded { get { return _gameData.ButterfliesNeededToCatch; } }

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        protected override void StartGame()
        {
            base.StartGame();

            CatchingButterfliesGenerator.Instance.StartSpawningButterflies();
        }
    }
}
