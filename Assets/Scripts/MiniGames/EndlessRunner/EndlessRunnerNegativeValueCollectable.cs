using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class EndlessRunnerNegativeValueCollectable : EndlessRunnerCollectable
    {
        [SerializeField]
        private int _collectablesToRemove = 0;

        protected override void OnItemCollected()
        {
            base.OnItemCollected();

            EndlessRunnerSolver.Instance.ModifyCollectablesCount(_collectablesToRemove);
        }
    }
}
