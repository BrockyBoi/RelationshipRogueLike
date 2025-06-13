using UnityEngine;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;

namespace EndlessRunner
{
    public class EndlessRunnerUI : GameUI<EndlessRunnerGenerator, EndlessRunnerSolver>
    {
        [SerializeField, Required]
        private TextMeshProUGUI _coinCountText;
        protected override EndlessRunnerGenerator GameGenerator { get { return EndlessRunnerGenerator.Instance; } }
        protected override EndlessRunnerSolver GameSolver { get { return EndlessRunnerSolver.Instance; } }

        protected override void OnGameStart()
        {
            base.OnGameStart();

            GameSolver.OnCollectableCountChange += OnCountChange;
        }

        protected override void OnGameEnd()
        {
            GameSolver.OnCollectableCountChange -= OnCountChange;

            base.OnGameEnd();
        }

        private void OnCountChange(int newCount)
        {
            _coinCountText.text = "Coins: " + newCount.ToString() + " / " + GameSolver.CollectablesNeeded;
        }
    }
}
