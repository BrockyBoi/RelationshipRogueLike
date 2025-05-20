using GeneralGame.Generation;
using Maze.Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maze.Generation
{
    public class MazeGenerator : GameGridGenerator<MazeSolverComponent, MazeGeneratorData, MazeCompletionResult, MazeNode>
    {
        public static MazeGenerator Instance {  get; private set; }

        public MazeNode StartNode { get; private set; }
        public MazeNode EndNode { get; private set; }

        protected override MazeSolverComponent GameSolverComponent { get { return MazeSolverComponent.Instance; } }

        bool _needsKeys = false;
        int _keysNeeded = 0;

        bool _hasGeneratedMazePath = false;
        private System.Action OnMazePathGenerated;

        [SerializeField]
        private KeyPickupObject _keyPrefab;

        private List<KeyPickupObject> _keysInGame = new List<KeyPickupObject>();

        private void Awake()
        {
            Instance = this;
        }

        protected void Start()
        {
            MazeSolverComponent.Instance.OnGameStart += OnGameStart;
            MazeSolverComponent.Instance.OnGameStop += OnGameEnd;
        }

        private void OnDestroy()
        {
            MazeSolverComponent.Instance.OnGameStart -= OnGameStart;
        }

        private void OnGameStart()
        {
            foreach (KeyPickupObject key in _keysInGame)
            {
                key.ActivateObject();
            }
        }

        private void OnGameEnd()
        {
            _keysInGame.Clear();
        }

        public override void DestroyGrid()
        {
            base.DestroyGrid();

            StartNode = null;
            EndNode = null;
        }

        protected override void CreateGrid(Vector2Int gridSize)
        {
            base.CreateGrid(gridSize);

            CreateMazePath();
        }

        #region Maze Generation
        private void CreateMazePath()
        {
            List<MazeNode> potentialStartNodes = new List<MazeNode>();
            GeneratePathThroughMazeNodes(potentialStartNodes);

            MazeNode bestNode = GetBestStartNode(potentialStartNodes);

            potentialStartNodes.Remove(bestNode);

            GenerateKeysInMaze(potentialStartNodes);

            StartNode = bestNode;
            StartNode.SetAsStartNode();

            _hasGeneratedMazePath = true;
            OnMazePathGenerated?.Invoke();
        }

        private void GeneratePathThroughMazeNodes(List<MazeNode> potentialStartNodes)
        {
            MazeNode firstNode = _objectGrid[0, 0];
            firstNode.ClearAllWalls();
            firstNode.SetAsEndNode();
            EndNode = firstNode;

            firstNode.VisitNode();
            MazeNode currentNode = firstNode;

            List<MazeNode> currentPath = new List<MazeNode>();

            currentPath.Add(firstNode);
            int completedNodes = 0;
            bool wasLastNodeValid = false;
            while (completedNodes < _objectGrid.Length)
            {
                List<MazeNode> neighborNodes = GetNeighborNodes(currentNode, ENodeState.None);
                if (neighborNodes.Count > 0)
                {
                    MazeNode nextNode = neighborNodes.GetRandomElement();
                    if (nextNode.NodeState == ENodeState.None)
                    {
                        nextNode.VisitNode();
                        ClearWalls(currentNode, nextNode, GetDirectionMovingIn(currentNode.PositionInGrid, nextNode.PositionInGrid));
                    }

                    currentPath.Add(nextNode);
                    currentNode = nextNode;

                    wasLastNodeValid = true;
                }
                else if (currentPath.Count > 0)
                {
                    if (wasLastNodeValid)
                    {
                        potentialStartNodes.Add(currentNode);
                    }
                    currentNode.CompleteNode();
                    currentNode = currentPath.Last();
                    currentPath.RemoveAt(currentPath.Count - 1);
                    wasLastNodeValid = false;
                }
                else
                {
                    break;
                }
            }
        }

        private MazeNode GetBestStartNode(List<MazeNode> potentialStartNodes)
        {
            float maxDist = -1;
            MazeNode bestNode = null;
            foreach (MazeNode node in potentialStartNodes)
            {
                float distance = Vector3.Distance(node.transform.position, EndNode.transform.position);
                if (distance > maxDist)
                {
                    maxDist = distance;
                    bestNode = node;
                }
            }

            return bestNode;
        }

        private void GenerateKeysInMaze(List<MazeNode> potentialStartNodes)
        {
            int startPotentialStartNodesCount = potentialStartNodes.Count;
            int keysToSpawn = _keysNeeded;
            if (_needsKeys && _keysNeeded > 0)
            {
                for (int i = 0; i < _keysNeeded; i++)
                {
                    MazeNode node = potentialStartNodes.GetRandomElement();
                    if (node)
                    {
                        potentialStartNodes.Remove(node);

                        KeyPickupObject keyPickupObject = Instantiate(_keyPrefab, node.transform.position, Quaternion.identity, node.transform);
                        if (keyPickupObject)
                        {
                            keyPickupObject.transform.localPosition = keyPickupObject.transform.localPosition.ChangeAxis(ExtensionMethods.VectorAxis.Y, .5f);
                            _keysInGame.Add(keyPickupObject);
                            keyPickupObject.DeactivateObject();
                            keysToSpawn--;

                            if (potentialStartNodes.Count == 0 && keysToSpawn > 0)
                            {
                                _keysNeeded = startPotentialStartNodesCount;
                                break;
                            }

                            if (keysToSpawn == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private List<MazeNode> GetNeighborNodes(MazeNode currentNode, ENodeState desiredState)
        {
            List<MazeNode> neighborNodes = new List<MazeNode>();
            Vector2Int nodePos = currentNode.PositionInGrid;

            if (nodePos.x + 1 < _objectGrid.GetLength(0))
            {
                MazeNode rightNode = GetGridObject(new Vector2Int(nodePos.x + 1, nodePos.y));
                if (rightNode != null && rightNode.NodeState == desiredState)
                {
                    neighborNodes.Add(rightNode);
                }
            }

            if (nodePos.x - 1 >= 0)
            {
                MazeNode leftNode = GetGridObject(new Vector2Int(nodePos.x - 1, nodePos.y));
                if (leftNode != null && leftNode.NodeState == desiredState)
                {
                    neighborNodes.Add(leftNode);
                }
            }

            if (nodePos.y + 1 < _objectGrid.GetLength(1))
            {
                MazeNode aboveNode = GetGridObject(new Vector2Int(nodePos.x, nodePos.y + 1));
                if (aboveNode != null && aboveNode.NodeState == desiredState)
                {
                    neighborNodes.Add(aboveNode);
                }
            }

            if (nodePos.y - 1 >= 0)
            {
                MazeNode belowNode = GetGridObject(new Vector2Int(nodePos.x, nodePos.y - 1));
                if (belowNode != null && belowNode.NodeState == desiredState)
                {
                    neighborNodes.Add(belowNode);
                }
            }

            return neighborNodes;
        }

        private static EMazeDirection GetDirectionMovingIn(Vector2Int from, Vector2Int to)
        {
            if (from.x > to.x)
            {
                return EMazeDirection.Left;
            }

            if (from.x < to.x)
            {
                return EMazeDirection.Right;
            }

            if (from.y > to.y)
            {
                return EMazeDirection.Downward;
            }

            if (from.y < to.y)
            {
                return EMazeDirection.Upward;
            }

            return EMazeDirection.None;
        }
        #endregion

        public void ListenToOnMazePathGenerated(System.Action action)
        {
            if (!_hasGeneratedMazePath)
            {
                OnMazePathGenerated += action;
            }
            else
            {
                action?.Invoke();
            }
        }

        public void UnlistenToMazePathGenerated(System.Action action)
        {
            OnMazePathGenerated -= action;
        }

        private void ClearWalls(MazeNode from, MazeNode to, EMazeDirection directionMoving)
        {
            if (from == null)
            {
                return;
            }

            from.ClearWall(directionMoving);
            switch (directionMoving)
            {
                case EMazeDirection.Left:
                    to.ClearWall(EMazeDirection.Right);
                    break;
                case EMazeDirection.Right:
                    to.ClearWall(EMazeDirection.Left);
                    break;
                case EMazeDirection.Upward:
                    to.ClearWall(EMazeDirection.Downward);
                    break;
                case EMazeDirection.Downward:
                    to.ClearWall(EMazeDirection.Upward);
                    break;
                case EMazeDirection.None:
                    break;
            }
        }

        protected override int GetDifficultySizeModifier()
        {
            return MazeDifficultyManager.Instance.MazeSizeModifier;
        }

        protected override GameObject GetGridParentObject()
        {
            return ParentObjectsManager.Instance.MazeNodesParent;
        }

        public override void GenerateGame(MazeGeneratorData generationData)
        {
            base.GenerateGame(generationData);
            SetGameGenerationData(generationData);
            if (generationData.RotationSpeed > 0)
            {
                MazeDifficultyManager.Instance.InitializeRotationRate(generationData.RotationSpeed, generationData.ForceDifficultySettings);
                MazeDifficultyManager.Instance.InitializeShakeIntensityRate(0, true);
            }
            else
            {
                MazeDifficultyManager.Instance.InitializeRotationRate(0, true);
                MazeDifficultyManager.Instance.InitializeShakeIntensityRate(generationData.ShakeIntensity, generationData.ForceDifficultySettings);
            }

            GameSolverComponent.SetTimeToCompleteGame(generationData.TimeToSolveMaze);
            GameSolverComponent.SetFakeTime(generationData.IsMazeFake ? generationData.FakeMazeTime : 0);

            _needsKeys = generationData.NeedsKeys;
            _keysNeeded = _needsKeys ? generationData.KeysNeeded : 0;
            CreateGrid(generationData.GridSize);
        }
    }
}

