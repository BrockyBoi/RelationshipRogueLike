using UnityEngine;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;

namespace ShootYourShotGame
{
    public class ShootYourShotGameUI : GameUI<ShootYourShotGameGenerator, ShootYourShotGameSolver>
    {
        protected override ShootYourShotGameGenerator GameGenerator { get { return ShootYourShotGameGenerator.Instance; } }
        protected override ShootYourShotGameSolver GameSolver { get { return ShootYourShotGameSolver.Instance; } }

        protected override void OnGameStart()
        {
            base.OnGameStart();

            GameSolver.OnCustomTimerValueSet += SetCustomTimerText;
            GameSolver.OnCustomTimerEnd += OnCustomTimerEnd;
        }

        protected override void OnGameEnd()
        {
            base.OnGameEnd();

            _timerText.text = string.Empty;
            GameSolver.OnCustomTimerValueSet -= SetCustomTimerText;
            GameSolver.OnCustomTimerEnd -= OnCustomTimerEnd;
        }

        private void SetCustomTimerText(float timerValue)
        {
            _timerText.text = ((int)timerValue).ToString();
        }

        protected override void OnGameTimerValueChange(float value)
        {
            base.OnGameTimerValueChange(value);
            _timerText.text = "Shoot";
        }

        private void OnCustomTimerEnd()
        {
            _timerText.text = "Shoot";
        }
    }
}
