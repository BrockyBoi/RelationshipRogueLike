using System;
using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using Sirenix.OdinInspector;

namespace ShootYourShotGame
{
    [Serializable]
    public class ShootYourShotGameGenerationData : GameGenerationData<ShootYourShotGameCompletionResult>
    {
        [FoldoutGroup("Game Data")]
        public bool IsStandardCountdown = false;

        [FoldoutGroup("Game Data"), HideIf("@IsStandardCountdown"), Range(.1f, 10f)]
        public float TimeBetween3And2 = 1;
        [FoldoutGroup("Game Data"), HideIf("@IsStandardCountdown"), Range(.1f, 10f)]
        public float TimeBetween2And1 = 1;
        [FoldoutGroup("Game Data"), HideIf("@IsStandardCountdown"), Range(.1f, 10f)]
        public float TimeBetween1And0 = 1;

        [FoldoutGroup("Game Data"), Range(.5f, 2f)]
        public float TimeAllowedToShootTarget = 1;
    }
}
