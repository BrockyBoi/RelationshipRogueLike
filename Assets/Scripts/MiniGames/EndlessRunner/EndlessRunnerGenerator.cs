using UnityEngine;
using System.Collections;
using GeneralGame.Generation;

namespace EndlessRunner
{
    public class EndlessRunnerGenerator : DialogueCreatedGameGenerator<EndlessRunnerSolver, EndlessRunnerGenerationData, EndlessRunnerCompletionResult>
    {

        public static EndlessRunnerGenerator Instance { get; private set; }

        protected override EndlessRunnerSolver GameSolverComponent => throw new System.NotImplementedException();

        private void Awake()
        {
            Instance = this;
        }
    }
}
