using GeneralGame;
using GeneralGame.Generation;

using MemoryGame;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maze.Generation
{
    public class MazeGenerator : GameGridGenerator<MazeGeneratorData, MazeCompletionResult, MazeNode>
    {
        public static MazeGenerator Instance {  get; private set; }

        public MazeNode StartNode { get; private set; }
        public MazeNode EndNode { get; private set; }

        bool _hasGeneratedMazePath = false;
        private System.Action OnMazePathGenerated;

        private void Awake()
        {
            Instance = this;
        }

        public override void DestroyGrid()
        {
            base.DestroyGrid();

            StartNode = null;
            EndNode = null;
        }

        protected override void CreateGrid(Vector2Int gridSize, List<MazeCompletionResult> results)
        {
            base.CreateGrid(gridSize, results);

            CreateMazePath();
        }

        private void CreateMazePath()
        {
            MazeNode firstNode = _objectGrid[0, 0];
            firstNode.ClearAllWalls();
            firstNode.SetAsEndNode();
            EndNode = firstNode;
            List<MazeNode> currentPath = new List<MazeNode>();
            int completedNodes = 0;

            currentPath.Add(firstNode);
            firstNode.VisitNode();
            MazeNode currentNode = firstNode;

            List<MazeNode> potentialStartNodes = new List<MazeNode>();

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
                        ClearWalls(currentNode, nextNode, GetDirectionMovingIn(currentNode.PositionInMaze, nextNode.PositionInMaze));
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

            StartNode = bestNode;
            StartNode.SetAsStartNode();

            _hasGeneratedMazePath = true;
            OnMazePathGenerated?.Invoke();
        }

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

        private void ClearWalls(MazeNode previousNode, MazeNode currentNode, EMazeDirection directionMoving)
        {
            if (previousNode == null)
            {
                return;
            }

            previousNode.ClearWall(directionMoving);
            switch (directionMoving)
            {
                case EMazeDirection.Left:
                    currentNode.ClearWall(EMazeDirection.Right);
                    break;
                case EMazeDirection.Right:
                    currentNode.ClearWall(EMazeDirection.Left);
                    break;
                case EMazeDirection.Forward:
                    currentNode.ClearWall(EMazeDirection.Backward);
                    break;
                case EMazeDirection.Backward:
                    currentNode.ClearWall(EMazeDirection.Forward);
                    break;
                case EMazeDirection.None:
                    break;
            }
        }

        private List<MazeNode> GetNeighborNodes(MazeNode currentNode, ENodeState desiredState)
        {
            List<MazeNode> neighborNodes = new List<MazeNode>();
            Vector2Int nodePos = currentNode.PositionInMaze;

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
                MazeNode frontNode = GetGridObject(new Vector2Int(nodePos.x, nodePos.y + 1));
                if (frontNode != null && frontNode.NodeState == desiredState)
                {
                    neighborNodes.Add(frontNode);
                }
            }

            if (nodePos.y - 1 >= 0)
            {
                MazeNode backNode = GetGridObject(new Vector2Int(nodePos.x, nodePos.y - 1));
                if (backNode != null && backNode.NodeState == desiredState)
                {
                    neighborNodes.Add(backNode);
                }
            }

            return neighborNodes;
        }

        private static EMazeDirection GetDirectionMovingIn(Vector2Int node1, Vector2Int node2)
        {
            if (node1.x > node2.x)
            {
                return EMazeDirection.Left;
            }

            if (node1.x < node2.x)
            {
                return EMazeDirection.Right;
            }

            if (node1.y > node2.y)
            {
                return EMazeDirection.Backward;
            }

            if (node1.y < node2.y)
            {
                return EMazeDirection.Forward;
            }

            return EMazeDirection.None;
        }

        protected override int GetDifficultySizeModifier()
        {
            return MazeDifficultyManager.Instance.MazeSizeModifier;
        }

        protected override GameObject GetGridParentObject()
        {
            return ParentObjectsManager.Instance.MazeNodesParent;
        }

        protected override void GiveResultsToSolver(List<MazeCompletionResult> results)
        {
            MazeSolverComponent.Instance.SetGameCompletionResults(results);
        }

        public override void GenerateGame(MazeGeneratorData generationData)
        {
            CreateGrid(generationData.GridSize, generationData.GameCompletionResults);
        }

        //protected override GameSolverComponent<MazeCompletionResult> GetAssociatedGameSolver()
        //{
        //    return MazeSolverComponent.Instance;
        //}
    }
}

