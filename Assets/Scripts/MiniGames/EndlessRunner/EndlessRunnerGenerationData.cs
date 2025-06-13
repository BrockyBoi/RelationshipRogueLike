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
        [FoldoutGroup("Game Data"), Range(1f, 10f)]
        public float ObjectMoveSpeed = 5;

        [FoldoutGroup("Game Data"), Range(10, 250)]
        public int CoinsToCollect = 50;

        [FoldoutGroup("Game Data"), Range(10, 250)]
        public int CoinPatternsToSpawn = 10;

        [FoldoutGroup("Game Data")]
        public bool ShouldSpawnExtraObjects = false;

        [FoldoutGroup("Game Data"), ShowIf("@ShouldSpawnExtraObjects")]
        public GameObject ExtraObjectToSpawn;

        [FoldoutGroup("Game Data"), ShowIf("@ShouldSpawnExtraObjects")]
        public float TimeToSpawnExtraObject = 5;
    }
}
