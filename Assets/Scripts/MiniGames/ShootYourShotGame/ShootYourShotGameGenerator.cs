using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using CatchingButterflies;

namespace ShootYourShotGame
{
    public class ShootYourShotGameGenerator : DialogueCreatedGameGenerator<ShootYourShotGameSolver, ShootYourShotGameGenerationData, ShootYourShotGameCompletionResult>
    {

        public static ShootYourShotGameGenerator Instance { get; private set; }

        protected override ShootYourShotGameSolver GameSolverComponent { get { return ShootYourShotGameSolver.Instance; } }

        private void Awake()
        {
            Instance = this;
        }
    }
}
