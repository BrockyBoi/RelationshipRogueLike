using GeneralGame;
using GeneralGame.Results;
using MemoryGame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    public abstract class GameGridGenerator<GameSolverClass, GenerationData, CompletionResultType, GridObjectType> : DialogueCreatedGameGenerator<GameSolverClass, GenerationData, CompletionResultType>
        where GameSolverClass : GameSolverComponent<GenerationData, CompletionResultType> where GenerationData : GridGameGenerationData<CompletionResultType> where CompletionResultType : GameCompletionResult, new() where GridObjectType : GridObject
    {
        [SerializeField, AssetsOnly, Required]
        protected GridObjectType _objectPrefab;

        protected GridObjectType[,] _objectGrid;

        public int GridWidth { get { return _objectGrid != null & _objectGrid.Length > 0 ? _objectGrid.GetLength(0) : 0; } }
        public int GridHeight { get { return _objectGrid != null & _objectGrid.Length > 0 ? _objectGrid.GetLength(1) : 0; } }
        public int TotalElementsCount { get { return _objectGrid.Length; } }

        [SerializeField]
        protected float _spaceBetweenGridObjects = 5f;

        protected virtual void Start()
        {
            GameSolverComponent.OnGameStop += OnGameEnd;
        }

        protected virtual void OnDisable()
        {
            GameSolverComponent.OnGameStop -= OnGameEnd;
        }

        protected virtual void CreateGrid(Vector2Int gridSize)
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
                    GridObjectType gridObject = Instantiate<GridObjectType>(_objectPrefab, new Vector3(xPos * _spaceBetweenGridObjects, yPos * _spaceBetweenGridObjects, 0), _objectPrefab.transform.localRotation, parentObject.transform);
                    gridObject.SetPositionInGrid(new Vector2Int(xPos, yPos));
                    _objectGrid[xPos, yPos] = gridObject;
                }
            }

            Vector3 finalLoc = new Vector3((GridWidth - 1) * _spaceBetweenGridObjects, (GridHeight - 1) * _spaceBetweenGridObjects, 0);
            parentObject.transform.position = finalLoc;

            _hasGeneratedGame = true;
        }

        protected override void InitializeCameraPosition()
        {
            float cameraWidth = (GridWidth + 1) * _spaceBetweenGridObjects;
            float cameraHeight = (GridHeight + 1) * _spaceBetweenGridObjects;

            Vector3 finalLoc = new Vector3((GridWidth - 1) * _spaceBetweenGridObjects, (GridHeight - 1) * _spaceBetweenGridObjects, 0);

            // https://www.youtube.com/watch?v=3xXlnSetHPM
            if (cameraHeight >= cameraWidth)
            {
                Camera.main.orthographicSize = (GridHeight + 2) * _spaceBetweenGridObjects / 2;
            }
            else
            {
                Camera.main.orthographicSize = ((GridWidth + 5) * _spaceBetweenGridObjects) * Screen.height / Screen.width / 2;
            }

            Camera.main.transform.position = (finalLoc * 1.5f).ChangeAxis(ExtensionMethods.EVectorAxis.Z, -30) + (Vector3.right * finalLoc.x / 2);
        }

        protected virtual void OnGameEnd()
        {
            DestroyGrid();
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

        protected override void GenerateGameAssets()
        {
            base.GenerateGameAssets();

            CreateGrid(_gameData.GridSize);
        }

        public GridObjectType GetGridObject(Vector2Int objectPosition)
        {
            return _objectGrid[objectPosition.x, objectPosition.y];
        }

        public GridObjectType[,] GetGrid()
        {
            return _objectGrid;
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
