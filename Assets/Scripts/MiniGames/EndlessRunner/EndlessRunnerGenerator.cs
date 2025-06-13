using UnityEngine;
using System.Collections;
using GeneralGame.Generation;
using CatchingButterflies;
using Sirenix.OdinInspector;

namespace EndlessRunner
{
    public class EndlessRunnerGenerator : DialogueCreatedGameGenerator<EndlessRunnerSolver, EndlessRunnerGenerationData, EndlessRunnerCompletionResult>
    {
        public static EndlessRunnerGenerator Instance { get; private set; }

        protected override EndlessRunnerSolver GameSolverComponent { get { return EndlessRunnerSolver.Instance; } }

        [Title("Prefabs"), SerializeField, Required]
        private GameObject[] _coinPatternPrefabs;

        [Title("Spawn Data"), SerializeField]
        private float _spawnX = 10;

        [SerializeField]
        private float _obstacleSpawnHeight = 0;

        [SerializeField]
        private float _baseCoinHeight = 2;

        private void Awake()
        {
            Instance = this;
        }

        protected override void InitializeCameraPosition()
        {
            base.InitializeCameraPosition();

            Camera.main.transform.position = (Vector3.up * 5).ChangeAxis(ExtensionMethods.EVectorAxis.Z, -30);
        }

        public void StartSpawningObjects()
        {
            StartCoroutine(SpawnCoinPatternsOverTime(_gameData.CoinPatternsToSpawn, _gameData.GameDuration - 5));
        }

        private IEnumerator SpawnCoinPatternsOverTime(int amountToSpawn, float duration)
        {
            if (amountToSpawn > 0)
            {
                float timeElapsed = 0;
                float timeInterval = duration / amountToSpawn;
                float nextSpawnTime = 0;
                while (timeElapsed < duration && GameSolverComponent.IsStage(GeneralGame.EGameStage.InGame))
                {
                    timeElapsed += Time.deltaTime;

                    if (timeElapsed >= nextSpawnTime)
                    {
                        Vector3 spawnLoc = new Vector3(_spawnX, _baseCoinHeight);
                        Instantiate(_coinPatternPrefabs.GetRandomElement(), spawnLoc, Quaternion.identity);
                        nextSpawnTime = timeElapsed + timeInterval;
                    }
                    yield return null;
                }
            }
        }

        private IEnumerator SpawnObstaclesOverTime(int amountToSpawn, float duration, EndlessRunnerObstacleObject prefab)
        {
            if (amountToSpawn > 0)
            {
                float timeElapsed = 0;
                float timeInterval = duration / amountToSpawn;
                float nextSpawnTime = 0;
                while (timeElapsed < duration && GameSolverComponent.IsStage(GeneralGame.EGameStage.InGame))
                {
                    timeElapsed += Time.deltaTime;

                    if (timeElapsed >= nextSpawnTime)
                    {
                        Vector3 spawnLoc = new Vector3(_spawnX, _obstacleSpawnHeight);
                        EndlessRunnerCollectable collectable = Instantiate(prefab, spawnLoc, Quaternion.identity);
                        nextSpawnTime = timeElapsed + timeInterval;
                    }
                    yield return null;
                }
            }
        }
    }
}
