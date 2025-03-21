using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maze.Generation
{
    public class MazeGenerator : MonoBehaviour
    {
        public static MazeGenerator Instance {  get; private set; }

        [SerializeField]
        private MazeNode _nodePrefab;

        [SerializeField]
        private Vector2Int _mazeSize;

        private MazeNode[,] _maze;

        [SerializeField]
        float _mazeCreationSpeed = .5f;

        public MazeNode StartNode { get; private set; }
        public MazeNode EndNode { get; private set; }

        private bool _hasGeneratedMaze = false;
        public System.Action OnMazeGenerated;
        public Vector2 OffsetPos { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void DestroyMaze()
        {
            for (int yPos = 0; yPos < _mazeSize.y; yPos++)
            {
                for (int xPos = 0; xPos < _mazeSize.x; xPos++)
                {
                    Destroy(_maze[xPos, yPos].gameObject);
                }
            }

            _maze = new MazeNode[0,0];
            StartNode = null;
            EndNode = null;
        }

        private IEnumerator BuildMazeOverTime()
        {
            for (int yPos = 0; yPos < _mazeSize.y; yPos++)
            {
                for (int xPos = 0; xPos < _mazeSize.x; xPos++)
                {
                    MazeNode mazeNode = Instantiate<MazeNode>(_nodePrefab, new Vector3(xPos, 0, yPos), Quaternion.identity);
                    mazeNode.SetPositionInMaze(new Vector2Int(xPos, yPos));
                    _maze[xPos, yPos] = mazeNode;
                }
            }

            MazeNode firstNode = _maze[0, 0];
            firstNode.ClearAllWalls();
            List<MazeNode> currentPath = new List<MazeNode>();
            int completedNodes = 0;

            currentPath.Add(firstNode);
            firstNode.VisitNode();
            MazeNode currentNode = firstNode;
            while (completedNodes < _maze.Length)
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

                    yield return new WaitForSeconds(_mazeCreationSpeed);

                }
                else if (currentPath.Count > 0)
                {
                    currentNode.CompleteNode();
                    currentNode = currentPath.Last();
                    currentPath.RemoveAt(currentPath.Count - 1);

                    yield return null;
                }
                else
                {
                    break;
                }    
            }
        }

        public void BuildMaze(MazeSpawnData data, List<MazeCompletionResult> mazeCompletionResults)
        {
            MazeSolverComponent.Instance.SetMazeCompletionResults(mazeCompletionResults);

            int difficultySizeModifier = MazeDifficultyManager.Instance.MazeSizeModifier;
            _maze = new MazeNode[data.MazeSize.x + difficultySizeModifier, data.MazeSize.y + difficultySizeModifier];

            GameObject parentObject = ParentObjectsManager.Instance.MazeNodesParent;
            parentObject.transform.position = new Vector3(_mazeSize.x / 2, 0, _mazeSize.y / 2);
            for (int yPos = 0; yPos < _mazeSize.y; yPos++)
            {
                for (int xPos = 0; xPos < _mazeSize.x; xPos++)
                {
                    MazeNode mazeNode = Instantiate<MazeNode>(_nodePrefab, new Vector3(xPos, 0, yPos), Quaternion.identity, parentObject.transform);
                    mazeNode.SetPositionInMaze(new Vector2Int(xPos, yPos));
                    _maze[xPos, yPos] = mazeNode;
                }
            }

            MazeNode firstNode = _maze[0, 0];
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
            while (completedNodes < _maze.Length)
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

            _hasGeneratedMaze = true;
            OnMazeGenerated?.Invoke();
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

            if (nodePos.x + 1 < _mazeSize.x)
            {
                MazeNode rightNode = GetMazeNode(new Vector2Int(nodePos.x + 1, nodePos.y));
                if (rightNode != null && rightNode.NodeState == desiredState)
                {
                    neighborNodes.Add(rightNode);
                }
            }

            if (nodePos.x - 1 >= 0)
            {
                MazeNode leftNode = GetMazeNode(new Vector2Int(nodePos.x - 1, nodePos.y));
                if (leftNode != null && leftNode.NodeState == desiredState)
                {
                    neighborNodes.Add(leftNode);
                }
            }

            if (nodePos.y + 1 < _mazeSize.y)
            {
                MazeNode frontNode = GetMazeNode(new Vector2Int(nodePos.x, nodePos.y + 1));
                if (frontNode != null && frontNode.NodeState == desiredState)
                {
                    neighborNodes.Add(frontNode);
                }
            }

            if (nodePos.y - 1 >= 0)
            {
                MazeNode backNode = GetMazeNode(new Vector2Int(nodePos.x, nodePos.y - 1));
                if (backNode != null && backNode.NodeState == desiredState)
                {
                    neighborNodes.Add(backNode);
                }
            }

            return neighborNodes;
        }

        public MazeNode GetMazeNode(Vector2Int nodePosition)
        {
            return _maze[nodePosition.x, nodePosition.y];
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

        public void ListenToOnMazeGenerated(System.Action action)
        {
            if (!_hasGeneratedMaze)
            {
                OnMazeGenerated += action;
            }
            else
            {
                action?.Invoke();
            }
        }
    }
}

