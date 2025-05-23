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
        public int ButterfliesToSpawn = 10;

        [Range(10, 1000)]
        public int ButterfliesNeededToCatch = 10;

        [Range(0f, 1f)]
        public float PercentageOfSpecialButterflies = 0;

        [Range(.5f, 1.5f)]
        public float ButterflySpeedMultiplier = 1f;

        [Range(0, 120f)]
        public float GameDuration = 15f;
    }
}
