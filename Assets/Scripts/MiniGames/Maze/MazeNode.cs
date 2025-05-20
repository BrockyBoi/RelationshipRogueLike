using Maze.Generation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public enum EMazeDirection
    {
        None, Upward, Downward, Left, Right, COUNT
    }

    public enum ENodeState
    {
        None, Visited, Completed
    }

    public class MazeNode : GridObject
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

        public ENodeState NodeState { get; private set; }
        private Vector3 _startPos = Vector3.zero;

        public System.Action OnCursorEntered;
        public System.Action OnCursorExited;
        public System.Action OnCursorCollidedWithWall;

        private void Start()
        {
            MazeSolverComponent.Instance.OnGameStart += OnGameStart;
            MazeSolverComponent.Instance.OnKeyCollected += OnKeyCollected;

            _difficultyManager = MazeDifficultyManager.Instance;

            _difficultyManager.OnShakeOffsetPositionChanged += OnShakeOffsetChanged;

            _startPos = transform.position;

        }

        private void Update()
        {
            if (MazeSolverComponent.Instance.CanPlayGame())
            {
                if (_difficultyManager.ShouldRotate)
                {
                    float speed = _difficultyManager.RotateSpeed;
                    transform.RotateAround(transform.parent.localPosition * 1.5f, Vector3.up, speed);
                }
            }
        }

        private void OnDestroy()
        {
            MazeSolverComponent.Instance.OnKeyCollected -= OnKeyCollected;
            MazeSolverComponent.Instance.OnGameStart -= OnGameStart;
            _difficultyManager.OnShakeOffsetPositionChanged -= OnShakeOffsetChanged;
        }

        private void OnShakeOffsetChanged(Vector2 offset)
        {
            if (_difficultyManager.ShouldShake && MazeSolverComponent.Instance.IsStage(GeneralGame.EGameStage.InGame))
            {
                transform.position = _startPos + new Vector3(offset.x, 0, offset.y);
            }
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

        public void ClearWall(EMazeDirection direction)
        {
            GameObject wall = null;
            switch (direction)
            {
                case EMazeDirection.Upward:
                    wall = _frontWall;
                    break;
                case EMazeDirection.Downward:
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

        [Button]
        public void ShowWalls()
        {
            ShowWalls(true);
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

            if (IsEndNode)
            {
                SetFloorColor(MazeSolverComponent.Instance.KeysNeeded > 0 ? Color.red : Color.green);
            }
            else if (IsStartNode)
            {
                SetFloorColor(Color.black);
            }
        }

        private void OnKeyCollected()
        {
            if (IsEndNode && MazeSolverComponent.Instance.KeysNeeded == 0)
            {
                SetFloorColor(Color.green);
            }
        }

        public void ClearAllWalls()
        {
            ClearWall(EMazeDirection.Upward);
            ClearWall(EMazeDirection.Downward);
            ClearWall(EMazeDirection.Left);
            ClearWall(EMazeDirection.Right);
        }

        public void SetAsStartNode()
        {
            _boxCollider.gameObject.SetActive(true);
            IsStartNode = true;
            SetFloorColor(Color.green);
            _boxCollider.enabled = true;
        }

        public void SetAsEndNode()
        {
            _boxCollider.gameObject.SetActive(true);
            IsEndNode = true;
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

        private void SetFloorColor(Color color)
        {
            _floor.GetComponent<MeshRenderer>().material.color = color;
        }
    }
}
