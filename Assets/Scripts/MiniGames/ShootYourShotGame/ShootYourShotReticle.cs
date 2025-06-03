using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

namespace ShootYourShotGame
{
    public class ShootYourShotReticle : MiniGameGameObject<ShootYourShotGameSolver, ShootYourShotGameGenerator>
    {
        protected override void Start()
        {
            base.Start();

            if (ensure(_gameSolver != null, "Game Solver is null"))
            {
                _gameSolver.OnCustomTimerEnd += StartMovingReticle;
            }
        }

        private void OnDestroy()
        {
            if (ensure(_gameSolver != null, "Game Solver is null"))
            {
                _gameSolver.OnCustomTimerEnd -= StartMovingReticle;
            }
        }

        protected override void OnGameGenerated()
        {
            base.OnGameGenerated();

            transform.position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f)).ChangeAxis(ExtensionMethods.VectorAxis.Z, -5);
        }

        private void StartMovingReticle()
        {
            StartCoroutine(LerpReticleToEdgeOfScreen());
        }

        private IEnumerator LerpReticleToEdgeOfScreen()
        {
            float timeToMove = _gameSolver.GameData.TimeAllowedToShootTarget;
            float time = 0;
            Vector3 startPos = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f)).ChangeAxis(ExtensionMethods.VectorAxis.Z, -5);
            Vector3 endPos = Camera.main.ViewportToWorldPoint(new Vector3(1, .5f)).ChangeAxis(ExtensionMethods.VectorAxis.Z, -5);

            while (time < timeToMove)
            {
                transform.position = Vector3.Lerp(startPos, endPos, time / timeToMove);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = endPos;
        }
    }
}
