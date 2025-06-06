using UnityEngine;
using System.Collections;
using GeneralGame.Generation;

namespace FireFighting
{
    public class FireFightingGenerator : GameGridGenerator<FireFightingSolver, FireFightingGenerationData, FireFightingCompletionResult, FireFightingWindow>
    {
        public static FireFightingGenerator Instance { get; private set; }
        protected override FireFightingSolver GameSolverComponent { get { return FireFightingSolver.Instance; } }

        protected override int GetDifficultySizeModifier()
        {
            return 0;
        }

        protected override GameObject GetGridParentObject()
        {
            return ParentObjectsManager.Instance.FireFightingWindowParent;
        }

        private void Awake()
        {
            Instance = this;
        }

        protected override void CreateGrid(Vector2Int gridSize)
        {
            base.CreateGrid(gridSize);
            foreach (FireFightingWindow window in _objectGrid)
            {
                window.IncreaseFireLevel(Random.Range(1, _gameData.MaxStartingFireLevel));
            }
        }
    }
}
