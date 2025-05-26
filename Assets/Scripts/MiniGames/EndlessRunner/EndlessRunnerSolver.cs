using UnityEngine;
using System.Collections;
using GeneralGame;
using CustomUI;
using Sirenix.OdinInspector;

namespace EndlessRunner
{
    public class EndlessRunnerSolver : CollectObjectsGameSolver<EndlessRunnerGenerationData, EndlessRunnerCompletionResult>
    {
        public static EndlessRunnerSolver Instance { get; private set; }

        public override int CollectablesNeeded { get { return _gameData.CoinsToCollect; } }

        [SerializeField, Required]
        private GameObject _groundReference;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        protected override void Start()
        {
            base.Start();

            _groundReference.SetActive(false);
        }

        protected override void StartGame()
        {
            base.StartGame();

            _groundReference.SetActive(true);
            EndlessRunnerGenerator.Instance.StartSpawningObjects();
        }

        protected override void EndGame()
        {
            base.EndGame();

            _groundReference.SetActive(false);
        }
    }
}
