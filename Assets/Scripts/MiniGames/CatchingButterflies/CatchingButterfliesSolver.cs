using UnityEngine;
using System.Collections;
using GeneralGame;
using CustomUI;

namespace CatchingButterflies
{
    public class CatchingButterfliesSolver : GameSolverComponent<CatchingButterfliesGenerationData, CatchingButterfliesCompletionResult>
    {

        public static CatchingButterfliesSolver Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            throw new System.NotImplementedException();
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            throw new System.NotImplementedException();
        }

        protected override void ApplyEndGameResults()
        {
            throw new System.NotImplementedException();
        }

        protected override BaseGameUI GetGameUIInstance()
        {
            throw new System.NotImplementedException();
        }
    }
}
