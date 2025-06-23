using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using System;
using Sirenix.OdinInspector;

namespace CatchingButterflies
{
    [Serializable]
    public class CatchingButterfliesGenerationData : GameGenerationData<CatchingButterfliesCompletionResult>
    {
        [FoldoutGroup("@FoldoutGroupName"), Range(10, 1000)]
        public int ButterfliesToSpawn = 10;

        [FoldoutGroup("@FoldoutGroupName"), Range(10, 1000)]
        public int ButterfliesNeededToCatch = 10;

        [FoldoutGroup("@FoldoutGroupName"), Range(0f, 1f)]
        public float PercentageOfSpecialButterflies = 0;

        [FoldoutGroup("@FoldoutGroupName"), Range(.5f, 1.5f)]
        public float ButterflySpeedMultiplier = 1f;
    }
}
