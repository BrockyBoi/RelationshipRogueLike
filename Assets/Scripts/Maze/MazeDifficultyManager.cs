using GeneralGame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeDifficultyManager : GameDifficultyManager<MazeDifficultyModifierResult>
    {
        public static MazeDifficultyManager Instance { get; private set; }

        [Title("Max Values")]
        [SerializeField]
        private int _maxSizeModifier = 6;

        [SerializeField, Range(0, .5f)]
        private float _maxRotation = .15f;

        [SerializeField, Range(0, .5f)]
        private float _maxShakeIntensity = .15f;

        [Title("Properties")]
        [ShowInInspector]
        public bool ShouldShake { get; private set; }

        [ShowInInspector, ShowIf("ShouldShake")]
        public float ShakeIntensity { get; private set; }

        public Vector2 ShakeOffsetPosition { get; private set; }

        [ShowInInspector]
        public bool ShouldRotate { get; private set; }

        [ShowInInspector, ShowIf("ShouldRotate")]
        public float RotateSpeed { get; private set; }

        [ShowInInspector, ReadOnly]
        public int MazeSizeModifier { get; private set; }

        public System.Action<Vector2> OnShakeOffsetPositionChanged;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (ShouldShake)
            {
                ShakeOffsetPosition = Random.insideUnitCircle * ShakeIntensity;
                OnShakeOffsetPositionChanged?.Invoke(ShakeOffsetPosition);
            }
        }

        public override void ProvideDifficultyModifierResult(MazeDifficultyModifierResult result)
        {
            ChangeRotationRate(result.MazeRotateModifier);
            ChangeShakeIntensity(result.MazeShakeModifier);
            ChangeMazeSizeModifierValue(result.MazeSizeModifier);
        }

        private void ChangeRotationRate(float rotationRateChange)
        {
            RotateSpeed = Mathf.Clamp(RotateSpeed + rotationRateChange, 0.0f, _maxRotation);
            ShouldRotate = RotateSpeed > 0.0f;
        }

        public void ChangeShakeIntensity(float shakeIntensityChange)
        {
            ShakeIntensity = Mathf.Clamp(ShakeIntensity + shakeIntensityChange, 0.0f, _maxShakeIntensity);
            ShouldShake = ShakeIntensity > 0.0f;
        }

        private void ChangeMazeSizeModifierValue(int mazeSizeModifierValue)
        {
            MazeSizeModifier = Mathf.Clamp(MazeSizeModifier + mazeSizeModifierValue, 0, _maxSizeModifier);
        }
    }
}
