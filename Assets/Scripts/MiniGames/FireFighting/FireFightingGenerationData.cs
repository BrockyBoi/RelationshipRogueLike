using System;
using UnityEngine;
using System.Collections;
using GeneralGame.Generation;

namespace FireFighting
{
    [Serializable]
    public class FireFightingGenerationData : GameGenerationData<FireFightingCompletionResult>
    {
        [Range(1f, 10f)]
        public float FireIncreasePerSecond;

        [Range(1, 100)]
        public float MaxStartingFireLevel;
    }
}
