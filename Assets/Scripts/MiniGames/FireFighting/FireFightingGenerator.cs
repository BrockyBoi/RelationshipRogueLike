using UnityEngine;
using System.Collections;
using GeneralGame.Generation;

using static GlobalFunctions;

namespace FireFighting
{
    public class FireFightingGenerator : GameGridGenerator<FireFightingSolver, FireFightingGenerationData, FireFightingCompletionResult, FireFightingWindow>
    {
        public static FireFightingGenerator Instance { get; private set; }
        protected override FireFightingSolver GameSolverComponent { get { return FireFightingSolver.Instance; } }

        [SerializeField]
        private int _windowRowsToShowAtStart = 3;

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
                if (ensure(window != null, "Window is null"))
                {
                    window.IncreaseFireLevel(Random.Range(1, _gameData.MaxStartingFireLevel));
                }
            }
        }

        protected override void InitializeCameraPosition()
        {
            float cameraWidth = (GridWidth + 1) * _spaceBetweenGridObjects;
            float cameraHeight = (_windowRowsToShowAtStart - 1) * _spaceBetweenGridObjects;

            Vector3 finalLoc = new Vector3((GridWidth - 1) * _spaceBetweenGridObjects, _windowRowsToShowAtStart * _spaceBetweenGridObjects, 0);

            // https://www.youtube.com/watch?v=3xXlnSetHPM
            if (cameraHeight >= cameraWidth)
            {
                Camera.main.orthographicSize = (_windowRowsToShowAtStart + 2) * _spaceBetweenGridObjects / 2;
            }
            else
            {
                Camera.main.orthographicSize = ((GridWidth + 5) * _spaceBetweenGridObjects) * Screen.height / Screen.width / 2;
            }

            Camera.main.transform.position = (finalLoc * 1.5f).ChangeAxis(ExtensionMethods.EVectorAxis.Z, -30) + (Vector3.right * finalLoc.x / 2);

            GameObject parentObject = GetGridParentObject();
            parentObject.transform.position = finalLoc;
        }
    }
}
