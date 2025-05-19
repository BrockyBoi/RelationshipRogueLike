using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using System;

namespace CatchingButterflies
{
    [Serializable]
    public class CatchingButterfliesGenerationData : GameGenerationData<CatchingButterfliesCompletionResult>
    {
        [Range(10, 1000)]
        public int ButterfliesToSpawn;

        [Range(10, 1000)]
        public int ButterfliesNeededToCatch;

        [Range(0f, 1f)]
        public float PercentageOfSpecialButterflies;

        public float GameDuration = 15f;
    }
}
