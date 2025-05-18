using Maze;
using Maze.Generation;
using UnityEngine;

namespace Maze
{
    public class PlayerMazeSolverObject : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 1f;

        [SerializeField]
        private float _objectHeight = .25f;

        [SerializeField]
        private float _distanceCheckToWalls = .5f;

        private bool _isBlockedFromMoving = false;

        private Camera _camera;

        [SerializeField]
        private float _gracePeriodBetweenWallHits = 1f;

        private float _lastTimeHitWall = 0f;


        private void OnCollisionEnter(Collision collision)
        {
            if (Time.time > _lastTimeHitWall + _gracePeriodBetweenWallHits)
            {
                MazeSolverComponent.Instance.HitMazeWall();
                _lastTimeHitWall = Time.time;
            }
        }

        private void Start()
        {
            _camera = Camera.main;
            MazeSolverComponent.Instance.OnGameStop += OnMazeGameEnd;
            MazeGenerator.Instance.ListenToOnMazePathGenerated(OnMazePathGenerated);
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            MazeSolverComponent.Instance.OnGameStop -= OnMazeGameEnd;
            MazeGenerator.Instance.UnlistenToMazePathGenerated(OnMazePathGenerated);
        }

        private void Update()
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 currentLoc = transform.position;
            float distanceToMove = _moveSpeed * Time.deltaTime;
            Vector3 nextPos = Vector3.MoveTowards(transform.position, mousePos.ChangeAxis(ExtensionMethods.VectorAxis.Y, _objectHeight), distanceToMove);
            Vector3 dir = nextPos - currentLoc;

            Debug.DrawLine(currentLoc, currentLoc + dir * _distanceCheckToWalls);
            RaycastHit hit;
            bool hitMazeWall = Physics.Raycast(currentLoc, dir, out hit, _distanceCheckToWalls, 1 << LayerMask.NameToLayer("MazeWall"));
            if (hitMazeWall)
            {
                //Debug.Log("Hit " +  hit.collider.name);
            }

            _isBlockedFromMoving = hitMazeWall;
            if (!_isBlockedFromMoving)
            {
                //Debug.Log("Is not blocked from moving");
                transform.position = nextPos;
            }
        }

        private void OnMazePathGenerated()
        {
            gameObject.SetActive(true);
            transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMazeGameEnd()
        {
            gameObject.SetActive(false);
        }
    }
}
