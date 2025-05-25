using UnityEngine;
using System.Collections;
using GeneralGame;
using CustomUI;

namespace EndlessRunner
{
    public class EndlessRunnerSolver : CollectObjectsGameSolver<EndlessRunnerGenerationData, EndlessRunnerCompletionResult>
    {
        private void Awake()
        {
            Instance = this;
        }

        public static EndlessRunnerSolver Instance { get; private set; }

        public override int CollectablesNeeded { get { return _gameData.CoinsToCollect; } }
    }
}
