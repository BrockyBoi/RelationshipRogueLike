using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using System;

namespace EndlessRunner
{
    [Serializable]
    public class EndlessRunnerGenerationData : GameGenerationData<EndlessRunnerCompletionResult>
    {
        public float ObjectMoveSpeed = 5;
        public int CoinsToCollect = 50;
    }
}
