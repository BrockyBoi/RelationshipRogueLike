using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class EndlessRunnerObstacleObject : EndlessRunnerCollectable
    {
        [SerializeField]
        private float _penaltyTimeOnHit = 1f;

        protected override void OnItemCollected()
        {
            EndlessRunnerSolver.Instance.AddPenaltyTime(_penaltyTimeOnHit);
        }
    }
}
