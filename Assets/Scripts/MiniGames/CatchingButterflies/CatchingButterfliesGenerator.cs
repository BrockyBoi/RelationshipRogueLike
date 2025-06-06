using UnityEngine;
using System.Collections;
using GeneralGame.Generation;

namespace CatchingButterflies
{
    public class CatchingButterfliesGenerator : DialogueCreatedGameGenerator<CatchingButterfliesSolver, CatchingButterfliesGenerationData, CatchingButterfliesCompletionResult>
    {
        public static CatchingButterfliesGenerator Instance { get; private set; }

        protected override CatchingButterfliesSolver GameSolverComponent { get { return CatchingButterfliesSolver.Instance; } }


        [SerializeField]
        private Butterfly _butterflyPrefab;

        [SerializeField]
        private Color[] _potentialColors;
        
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

        public void StartSpawningButterflies()
        {
            StartCoroutine(SpawnButterfliesOverTime(_gameData.ButterfliesToSpawn, _gameData.GameDuration));
        }

        private IEnumerator SpawnButterfliesOverTime(int amountToSpawn, float duration)
        {
            float timeElapsed = 0;
            float timeInterval = duration / amountToSpawn;
            float nextSpawnTime = 0;
            while (timeElapsed < duration && GameSolverComponent.IsStage(GeneralGame.EGameStage.InGame))
            {
                timeElapsed += Time.deltaTime;

                if (timeElapsed >= nextSpawnTime)
                {
                    Butterfly butterfly = Instantiate(_butterflyPrefab, transform.position, Quaternion.identity);
                    butterfly.Initialize(_potentialColors.GetRandomElement());
                    nextSpawnTime = timeElapsed + timeInterval;
                }
                yield return null;
            }
        }
    }
}
