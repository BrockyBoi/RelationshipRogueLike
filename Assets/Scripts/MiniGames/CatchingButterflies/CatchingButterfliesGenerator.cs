using UnityEngine;
using System.Collections;
using GeneralGame.Generation;

namespace CatchingButterflies
{
    public class CatchingButterfliesGenerator : DialogueCreatedGameGenerator<CatchingButterfliesSolver, CatchingButterfliesGenerationData, CatchingButterfliesCompletionResult>
    {

        public static CatchingButterfliesGenerator Instance { get; private set; }

        protected override CatchingButterfliesSolver GameSolverComponent { get { return CatchingButterfliesSolver.Instance; } }

        private void Awake()
        {
            Instance = this;
        }
    }
}
