using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GeneralGame;
using GeneralGame.Generation;
using Sirenix.OdinInspector;

namespace WhackAMole
{
    public class WhackAMoleGenerator : DialogueCreatedGameGenerator<WhackAMoleSolver, WhackAMoleGenerationData, WhackAMoleCompletionResult>
    {
        public static WhackAMoleGenerator Instance { get; private set; }

        [SerializeField, Required, AssetsOnly]
        private WhackAMoleHole _holePrefab;

        private WhackAMoleHole[] _holes = new WhackAMoleHole[8];

        protected override WhackAMoleSolver GameSolverComponent { get { return WhackAMoleSolver.Instance; } }

        [SerializeField]
        private float _spawnDistanceFromCenter = 3;

        private void Awake()
        {
            Instance = this;
        }

        protected override void GenerateGameAssets()
        {
            base.GenerateGameAssets();
            for (int i = 0; i < _holes.Length; i++)
            {
                float degrees = i * (360 / _holes.Length) * Mathf.Deg2Rad;
                Vector3 finalPos = new Vector3(Mathf.Cos(degrees) * _spawnDistanceFromCenter, Mathf.Sin(degrees) * _spawnDistanceFromCenter, 0);
                _holes[i] = Instantiate(_holePrefab, finalPos, Quaternion.identity);
            }

            Camera.main.transform.position = Vector3.zero.ChangeAxis(ExtensionMethods.EVectorAxis.Z, -30) + (Vector3.right * _spawnDistanceFromCenter);
            Camera.main.orthographicSize = 1.5f * _spawnDistanceFromCenter;

            GameSolverComponent.OnGameStop += DeleteGameObjects;
        }

        public WhackAMoleHole GetHoleNearestToAngle(float angle)
        {
            int bestIndex = -1;
            float distance = float.MaxValue;
            for (int i = 0; i < _holes.Length; i++)
            {
                float degrees = i * (360 / _holes.Length);
                float difference = Mathf.Abs(angle - degrees);
                if (difference < distance)
                {
                    distance = difference;
                    bestIndex = i;
                    
                    // Check both 0 and 360 degrees for first item
                    if (i == 0)
                    {
                        degrees = 360;
                        difference = Mathf.Abs(angle - degrees);
                        if (difference < distance)
                        {
                            distance = difference;
                            bestIndex = i;
                        }
                    }
                }
            }

            return _holes[bestIndex];
        }

        public WhackAMoleHole GetRandomUnoccupiedHole()
        {
            WhackAMoleHole unoccupiedHole = null;
            int maxCounter = 0;
            do
            {
                unoccupiedHole = _holes.GetRandomElement();
                if (maxCounter > 50)
                {
                    Debug.LogError("In loop too long");
                    break;
                }
            }
            while (unoccupiedHole && unoccupiedHole.HasObjectInHole);

            return unoccupiedHole;
        }

        private void DeleteGameObjects()
        {
            for (int i = 0; i < _holes.Length; i++)
            {
                Destroy(_holes[i].gameObject);
            }

            GameSolverComponent.OnGameStop -= DeleteGameObjects;
        }
    }
}
