using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class EndlessRunnerCoin : EndlessRunnerCollectable
    {
        protected override void OnItemCollected()
        {
            EndlessRunnerSolver.Instance.CollectObject();

            base.OnItemCollected();
        }
    }
}
