using GeneralGame.Generation;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    [Serializable]
    public class WhackAMoleGenerationData : GameGenerationData<WhackAMoleCompletionResult>
    {
        [FoldoutGroup("Game Data")]
        public float TimeToPlay = 15f;

        [FoldoutGroup("Game Data")]
        public float TimeObjectsStayInPlay = .75f;

        [FoldoutGroup("Game Data")]
        public float TimeBetweenSpawns = .75f;

        [FoldoutGroup("Game Data")]
        public bool HasDistractionObjects = false;

        [FoldoutGroup("Game Data")]
        public int StartingHealth = 10;

        [FoldoutGroup("Game Data")]
        public int HealthLostPerEnemy = 1;

        [FoldoutGroup("Game Data")]
        public int HealthLostPerDistraction = 2;
    }
}
