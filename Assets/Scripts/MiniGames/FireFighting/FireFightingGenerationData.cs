using System;
using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using Sirenix.OdinInspector;

namespace FireFighting
{
    [Serializable]
    public class FireFightingGenerationData : GridGameGenerationData<FireFightingCompletionResult>
    {
        [FoldoutGroup("@FoldoutGroupName"), Range(1f, 10f)]
        public float FireIncreasePerSecond = 1;

        [FoldoutGroup("@FoldoutGroupName"), Range(1, 100)]
        public float MaxStartingFireLevel = 1;
    }
}
