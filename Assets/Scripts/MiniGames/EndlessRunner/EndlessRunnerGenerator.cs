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
        private EndlessRunnerCoin _coinPrefab;

        [SerializeField, Required]
        private EndlessRunnerObstacleObject _obstaclePrefab;

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

        protected override void GenerateGameAssets()
        {
            base.GenerateGameAssets();

            Camera.main.transform.position = Vector3.zero.ChangeAxis(ExtensionMethods.EVectorAxis.Z, -30);
            Camera.main.orthographicSize = 5f;
        }

        public void StartSpawningObjects()
        {
            StartCoroutine(SpawnCoinsOverTime(_gameData.CoinsToSpawn, _gameData.GameDuration - 5, _coinPrefab));
            StartCoroutine(SpawnObstaclesOverTime(_gameData.ObstaclesToSpawn, _gameData.GameDuration - 5, _obstaclePrefab));
        }

        private IEnumerator SpawnCoinsOverTime(int amountToSpawn, float duration, EndlessRunnerCollectable prefab)
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
                        EndlessRunnerCollectable collectable = Instantiate(prefab, spawnLoc, Quaternion.identity);
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
