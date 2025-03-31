using GeneralGame;
using GeneralGame.Results;
using MemoryGame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    public abstract class GameGridGenerator<GenerationData, CompletionResultType, GridObjectType> : DialogueCreatedGameGenerator<GenerationData, CompletionResultType> 
        where GenerationData : BaseGameGenerationData where CompletionResultType : GameCompletionResult where GridObjectType : GridObject
    {
        [SerializeField, AssetsOnly]
        protected GridObjectType _objectPrefab;

        protected GridObjectType[,] _objectGrid;

        public int GridWidth { get { return _objectGrid != null & _objectGrid.Length > 0 ? _objectGrid.GetLength(0) : 0; } }
        public int GridHeight { get { return _objectGrid != null & _objectGrid.Length > 0 ? _objectGrid.GetLength(1) : 0; } }

        [SerializeField]
        private float _spaceBetweenGridObjects = 5f;

        [SerializeField]
        private float _cameraSizeMultiplier = 1f;

        protected virtual void CreateGrid(Vector2Int gridSize, List<CompletionResultType> results)
        {
            if (gridSize.x == 0 || gridSize.y == 0)
            {
                Debug.LogError("Invalid grid size");
                return;
            }

            int difficultySizeModifier = GetDifficultySizeModifier();
            _objectGrid = new GridObjectType[gridSize.x + difficultySizeModifier, gridSize.y + difficultySizeModifier];

            GameObject parentObject = GetGridParentObject();
            parentObject.transform.position = Vector3.zero;
            for (int yPos = 0; yPos < GridHeight; yPos++)
            {
                for (int xPos = 0; xPos < GridWidth; xPos++)
                {
                    GridObjectType gridObject = Instantiate<GridObjectType>(_objectPrefab, new Vector3(xPos * _spaceBetweenGridObjects, 0, yPos * _spaceBetweenGridObjects), Quaternion.identity, parentObject.transform);
                    gridObject.SetPositionInMaze(new Vector2Int(xPos, yPos));
                    _objectGrid[xPos, yPos] = gridObject;
                }
            }

            Vector3 finalLoc = new Vector3((GridWidth - 1) * _spaceBetweenGridObjects, 0, (GridHeight - 1) * _spaceBetweenGridObjects);
            parentObject.transform.position = finalLoc;

            Camera.main.orthographicSize = _objectGrid.Length * _cameraSizeMultiplier;
            Camera.main.transform.position = (finalLoc * 1.5f).ChangeAxis(ExtensionMethods.VectorAxis.Y, 10);

            _hasGeneratedGame = true;

            GiveResultsToSolver(results);
            GameGenerated();
        }

        public virtual void DestroyGrid()
        {
            for (int yPos = 0; yPos < GridHeight; yPos++)
            {
                for (int xPos = 0; xPos < GridWidth; xPos++)
                {
                    Destroy(_objectGrid[xPos, yPos].gameObject);
                }
            }

            _objectGrid = new GridObjectType[0, 0];
        }

        public GridObjectType GetGridObject(Vector2Int objectPosition)
        {
            return _objectGrid[objectPosition.x, objectPosition.y];
        }

        protected abstract int GetDifficultySizeModifier();

        protected abstract GameObject GetGridParentObject();

        public GridObjectType GetRandomGridElement()
        {
            int x = Random.Range(0, GridWidth);
            int y = Random.Range(0, GridHeight);

            return _objectGrid[x, y];
        }
    }
}
