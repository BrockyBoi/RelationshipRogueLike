using Maze.Generation;
using UnityEngine;

namespace Maze
{
    public class PlayerMazeSolverObject : MiniGameGameObject<MazeSolverComponent, MazeGenerator>
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

        protected override void Start()
        {
            base.Start();

            _camera = Camera.main;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 currentLoc = transform.position;
            float distanceToMove = _moveSpeed * Time.deltaTime;
            Vector3 nextPos = Vector3.MoveTowards(transform.position, mousePos.ChangeAxis(ExtensionMethods.EVectorAxis.Z, _objectHeight), distanceToMove);
            Vector3 dir = nextPos - currentLoc;

            Debug.DrawLine(currentLoc, currentLoc + dir * distanceToMove * 1.25f);
            RaycastHit hit;
            bool hitMazeWall = Physics.Raycast(currentLoc, dir, out hit, distanceToMove * 1.25f, 1 << LayerMask.NameToLayer("MazeWall"));
            _isBlockedFromMoving = hitMazeWall;
            if (!_isBlockedFromMoving)
            {
                transform.position = nextPos;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (Time.time > _lastTimeHitWall + _gracePeriodBetweenWallHits)
            {
                MazeSolverComponent.Instance.HitMazeWall();
                _lastTimeHitWall = Time.time;
            }
        }

        protected override void OnObjectEnabled()
        {
            base.OnObjectEnabled();
            transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
