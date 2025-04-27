using GeneralGame.Generation;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Maze
{
    [Serializable]
    public class MazeGeneratorData : GameGenerationData<MazeCompletionResult>
    {
        [FoldoutGroup("@FoldoutGroupName")]
        public Vector2Int GridSize = new Vector2Int(4,4);

        [FoldoutGroup("@FoldoutGroupName"), Range(0, .2f), DisableIf("@RotationSpeed > 0")]
        public float ShakeIntensity = 0;

        [FoldoutGroup("@FoldoutGroupName"), Range(0, .2f)]
        public float RotationSpeed = 0;

        [FoldoutGroup("@FoldoutGroupName"), Range(5, 120)]
        public float TimeToSolveMaze = 10;

        [FoldoutGroup("@FoldoutGroupName")]
        public bool NeedsKeys = false;

        [FoldoutGroup("@FoldoutGroupName"), ShowIf("NeedsKeys")]
        public bool NeedsNumberedKeys = false;

        [FoldoutGroup("@FoldoutGroupName"), ShowIf("NeedsKeys")]
        public int KeysNeeded = 0;

        [FoldoutGroup("@FoldoutGroupName")]
        public bool IsMazeFake = false;

        [FoldoutGroup("@FoldoutGroupName"), ShowIf("IsMazeFake")]
        public float FakeMazeTime = 2f;

        [FoldoutGroup("@FoldoutGroupName")]
        public bool ForceDifficultySettings;

        private string FoldoutGroupName { get { return  (ShakeIntensity > 0 ? "Shaking " : string.Empty) + (RotationSpeed > 0 ? "Rotating " : string.Empty) + 
                                                        (IsMazeFake ? "Fake " : string.Empty) + GridSize.x.ToString() + " x " + GridSize.y.ToString() + " Grid" + 
                                                        (NeedsKeys && KeysNeeded > 0 ? " With " + KeysNeeded.ToString() + " Keys" : string.Empty) +
                                                        (ForceDifficultySettings ? " With Set Difficulty Settings" : string.Empty); } }
    }
}
