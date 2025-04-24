using GeneralGame.Generation;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Maze
{
    [Serializable]
    public class MazeGeneratorData : GameGenerationData<MazeCompletionResult>
    {
        [FoldoutGroup("MazeData")]
        public Vector2Int GridSize = new Vector2Int(4,4);

        [FoldoutGroup("MazeData")]
        public float ShakeIntensity = 0;

        [FoldoutGroup("MazeData")]
        public float RotationSpeed = 0;

        [FoldoutGroup("MazeData")]
        public float TimeToSolveMaze = 10;

        [FoldoutGroup("MazeData")]
        public bool NeedsKeys = false;

        [FoldoutGroup("MazeData"), ShowIf("NeedsKeys")]
        public bool NeedsNumberedKeys = false;

        [FoldoutGroup("MazeData"), ShowIf("NeedsKeys")]
        public int KeysNeeded = 0;

        [FoldoutGroup("MazeData")]
        public bool IsMazeFake = false;

        [FoldoutGroup("MazeData"), ShowIf("IsMazeFake")]
        public float FakeMazeTime = 2f;
    }
}
