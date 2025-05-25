using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CatchingButterflies
{
    public class CatchingButterfliesUI : GameUI<CatchingButterfliesGenerator, CatchingButterfliesSolver>
    {
        [SerializeField, Required]
        private TextMeshProUGUI _currentCatchCountText;
        protected override CatchingButterfliesGenerator GameGenerator { get { return CatchingButterfliesGenerator.Instance; } }

        protected override CatchingButterfliesSolver GameSolver { get { return CatchingButterfliesSolver.Instance; } }

        protected override void Start()
        {
            base.Start();

            GameSolver.OnCollectableCountChange += OnButterflyCountChange;
        }

        protected override void OnDestroy()
        {
            GameSolver.OnCollectableCountChange -= OnButterflyCountChange;

            base.OnDestroy();
        }

        protected override void OnGameStart()
        {
            base.OnGameStart();

            _currentCatchCountText.text = "Caught 0 out of " + GameSolver.CollectablesNeeded;
        }

        private void OnButterflyCountChange(int count)
        {
            _currentCatchCountText.text = "Caught " + count.ToString() + " out of " + GameSolver.CollectablesNeeded;
        }
    }
}
