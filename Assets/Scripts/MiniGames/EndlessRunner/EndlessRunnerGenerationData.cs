using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using System;
using Sirenix.OdinInspector;

namespace EndlessRunner
{
    [Serializable]
    public class EndlessRunnerGenerationData : GameGenerationData<EndlessRunnerCompletionResult>
    {
        [FoldoutGroup("@FoldoutGroupName"), Range(1f, 10f)]
        public float ObjectMoveSpeed = 5;

        [FoldoutGroup("@FoldoutGroupName"), Range(10, 250)]
        public int CoinsToCollect = 50;

        [FoldoutGroup("@FoldoutGroupName"), Range(10, 30)]
        public int CoinPatternsToSpawn = 10;

        [FoldoutGroup("@FoldoutGroupName")]
        public bool ShouldSpawnExtraObjects = false;

        [FoldoutGroup("@FoldoutGroupName"), ShowIf("@ShouldSpawnExtraObjects")]
        public GameObject ExtraObjectToSpawn;

        [FoldoutGroup("@FoldoutGroupName"), ShowIf("@ShouldSpawnExtraObjects")]
        public float TimeToSpawnExtraObject = 5;
    }
}
