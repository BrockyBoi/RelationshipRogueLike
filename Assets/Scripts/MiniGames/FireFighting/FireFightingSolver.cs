using UnityEngine;
using System.Collections;
using GeneralGame;

namespace FireFighting
{
    public class FireFightingSolver : GameSolverComponent<FireFightingGenerationData, FireFightingCompletionResult>
    {

        public static FireFightingSolver Instance { get; private set; }

        public override int GetCurrentPotentialDialogueIndex()
        {
            throw new System.NotImplementedException();
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            throw new System.NotImplementedException();
        }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }
    }
}
