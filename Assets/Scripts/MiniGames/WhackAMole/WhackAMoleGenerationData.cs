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
        [FoldoutGroup("@FoldoutGroupName")]
        public float TimeObjectsStayInPlay = .75f;

        [FoldoutGroup("@FoldoutGroupName")]
        public float TimeBetweenSpawns = .75f;

        [FoldoutGroup("@FoldoutGroupName")]
        public bool HasDistractionObjects = false;

        [FoldoutGroup("@FoldoutGroupName")]
        public int StartingHealth = 10;

        [FoldoutGroup("@FoldoutGroupName")]
        public int HealthLostPerEnemy = 1;

        [FoldoutGroup("@FoldoutGroupName")]
        public int HealthLostPerDistraction = 2;
    }
}
