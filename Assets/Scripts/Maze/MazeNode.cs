using Maze.Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public enum EMazeDirection
    {
        None, Forward, Backward, Left, Right, COUNT
    }

    public enum ENodeState
    {
        None, Visited, Completed
    }

    public class MazeNode : MonoBehaviour
    {
        [SerializeField]
        private GameObject _leftWall;

        [SerializeField]
        private GameObject _rightWall;

        [SerializeField]
        private GameObject _frontWall;

        [SerializeField]
        private GameObject _backWall;

        [SerializeField]
        private GameObject _floor;

        [SerializeField]
        private GameObject _roof;

        [SerializeField]
        private BoxCollider _boxCollider;

        public BoxCollider MouseTriggerCollider { get { return _boxCollider; } }

        private MazeDifficultyManager _difficultyManager;

        public bool IsStartNode { get; private set; }
        public bool IsEndNode { get; private set; }

        public Vector2Int PositionInMaze { get; private set; }

        public ENodeState NodeState { get; private set; }
        private Vector3 _startPos = Vector3.zero;

        public System.Action OnCursorEntered;
        public System.Action OnCursorExited;
        public System.Action OnCursorCollidedWithWall;

        private void Start()
        {
            _leftWall.GetComponent<MeshRenderer>().material.color = Color.red;
            _rightWall.GetComponent<MeshRenderer>().material.color = Color.red;
            _frontWall.GetComponent<MeshRenderer>().material.color = Color.red;
            _backWall.GetComponent<MeshRenderer>().material.color = Color.red;

            MazeSolverComponent.Instance.OnGameStart += OnGameStart;
            _difficultyManager = MazeDifficultyManager.Instance;
        }

        private void Update()
        {
            if (_difficultyManager.ShouldShake)
            {
                Vector2 offset = _difficultyManager.ShakeOffsetPosition;
                transform.position = _startPos + new Vector3(offset.x, 0, offset.y);
            }
            else if (_difficultyManager.ShouldRotate)
            {
                GameObject mazeNodes = ParentObjectsManager.Instance.MazeNodesParent;
                transform.RotateAround(mazeNodes.transform.position, Vector3.up, _difficultyManager.RotateSpeed);
            }
        }

        private void OnDestroy()
        {
            MazeSolverComponent.Instance.OnGameStart -= OnGameStart;
        }

        public void VisitNode()
        {
            NodeState = ENodeState.Visited;
            _roof.SetActive(false);
            ShowWalls(false);
        }

        public void CompleteNode()
        {
            NodeState = ENodeState.Completed;
        }

        public void SetPositionInMaze(Vector2Int positionInMaze)
        {
            PositionInMaze = positionInMaze;
            _startPos = transform.position;
        }

        public void ClearWall(EMazeDirection direction)
        {
            GameObject wall = null;
            switch (direction)
            {
                case EMazeDirection.Forward:
                    wall = _frontWall; ;
                    break;
                case EMazeDirection.Backward:
                    wall = _backWall;
                    break;
                case EMazeDirection.Left:
                    wall = _leftWall;
                    break;
                case EMazeDirection.Right:
                    wall = _rightWall;
                    break;
            }

            if (wall != null)
            {
                wall.SetActive(false);
                wall.GetComponent<Collider>().enabled = false;
            }
        }

        private void ShowWalls(bool shouldShow)
        {
            _leftWall.GetComponent<MeshRenderer>().enabled = shouldShow;
            _leftWall.GetComponent<Collider>().enabled = shouldShow;

            _rightWall.GetComponent<MeshRenderer>().enabled = shouldShow;
            _rightWall.GetComponent<Collider>().enabled = shouldShow;

            _frontWall.GetComponent<MeshRenderer>().enabled = shouldShow;
            _frontWall.GetComponent<Collider>().enabled = shouldShow;

            _backWall.GetComponent<MeshRenderer>().enabled = shouldShow;
            _backWall.GetComponent<Collider>().enabled = shouldShow;
        }

        private void OnGameStart()
        {
            ShowWalls(true);
        }

        public void ClearAllWalls()
        {
            ClearWall(EMazeDirection.Forward);
            ClearWall(EMazeDirection.Backward);
            ClearWall(EMazeDirection.Left);
            ClearWall(EMazeDirection.Right);
        }

        public void SetAsStartNode()
        {
            _boxCollider.gameObject.SetActive(true);
            IsStartNode = true;
            _floor.GetComponent<MeshRenderer>().material.color = Color.blue;
            _boxCollider.enabled = true;
        }

        public void SetAsEndNode()
        {
            _boxCollider.gameObject.SetActive(true);
            IsEndNode = true;
            _floor.GetComponent<MeshRenderer>().material.color = Color.yellow;
            _boxCollider.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsEndNode || IsStartNode)
            {
                OnCursorEntered?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsEndNode || IsStartNode)
            {
                OnCursorExited?.Invoke();
            }
        }
    }
}
