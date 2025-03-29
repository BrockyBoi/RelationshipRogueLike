using GeneralGame;
using GeneralGame.Results;
using MemoryGame;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    public abstract class GameGridGenerator<GenerationData, CompletionResultType, GridObjectType> : DialogueCreatedGameGenerator<GenerationData, CompletionResultType> 
        where GenerationData : GameGenerationData where CompletionResultType : GameCompletionResult where GridObjectType : GridObject
    {
        [SerializeField]
        protected GridObjectType _objectPrefab;

        protected GridObjectType[,] _objectGrid;

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
            int mazeLength = _objectGrid.GetLength(0);

            GameObject parentObject = GetGridParentObject();
            for (int yPos = 0; yPos < _objectGrid.GetLength(1); yPos++)
            {
                for (int xPos = 0; xPos < _objectGrid.GetLength(0); xPos++)
                {
                    GridObjectType gridObject = Instantiate<GridObjectType>(_objectPrefab, new Vector3(xPos * _spaceBetweenGridObjects, 0, yPos * _spaceBetweenGridObjects), Quaternion.identity, parentObject.transform);
                    gridObject.SetPositionInMaze(new Vector2Int(xPos, yPos));
                    _objectGrid[xPos, yPos] = gridObject;
                }
            }

            parentObject.transform.position = new Vector3(mazeLength * _spaceBetweenGridObjects, 0, mazeLength * _spaceBetweenGridObjects);

            Camera.main.orthographicSize = _objectGrid.GetLength(0) * _cameraSizeMultiplier;
            Camera.main.transform.position = GetGridParentObject().transform.position.ChangeAxis(ExtensionMethods.VectorAxis.Y, 10);

            _hasGeneratedGame = true;

            GiveResultsToSolver(results);
            GameGenerated();
        }

        public virtual void DestroyGrid()
        {
            for (int yPos = 0; yPos < _objectGrid.GetLength(1); yPos++)
            {
                for (int xPos = 0; xPos < _objectGrid.GetLength(0); xPos++)
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
            int x = Random.Range(0, _objectGrid.GetLength(0));
            int y = Random.Range(0, _objectGrid.GetLength(1));

            return _objectGrid[x, y];
        }
    }
}
