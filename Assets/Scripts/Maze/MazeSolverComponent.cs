using Maze.Generation;
using GeneralGame;
using UnityEngine;

namespace Maze
{
    public class MazeSolverComponent : MazeSolverComponentLogic
    {
        public static MazeSolverComponent Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }

    public class MazeSolverComponentLogic : GameSolverComponent<MazeCompletionResult>
    {

        [SerializeField]
        private float _penaltyOnWallHit = 1.5f;

        [SerializeField]
        private float _timeToSolveMaze = 10f;

        private MazeNode _startNode;
        private MazeNode _endNode;

        public System.Action OnWallHit;
        public System.Action OnMazeSolved;
        public System.Action OnMazeFailed;

        private void Start()
        {
            MazeGenerator.Instance.ListenToOnMazeGenerated(OnMazeGenerated);
        }

        private void OnMazeGenerated()
        {
            _startNode = MazeGenerator.Instance.StartNode;
            _endNode = MazeGenerator.Instance.EndNode;

            if (_startNode != null)
            {
                _startNode.OnCursorEntered += EnterStartZone;
                _startNode.OnCursorExited += ExitStartZone;
            }

            if (_endNode != null)
            {
                _endNode.OnCursorEntered += EnteredExitZone;
            }

            PlayerMazeSolverObject.Instance.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (_startNode != null)
            {
                _startNode.OnCursorEntered -= EnterStartZone;
                _startNode.OnCursorExited -= ExitStartZone;
            }

            if (_endNode != null)
            {
                _endNode.OnCursorEntered -= EnteredExitZone;
            }
        }

        public void EnterStartZone()
        {
            if (IsStage(EGameStage.PreCountdown))
            {
                StartCountDown();
            }
        }

        public void ExitStartZone()
        {
            if (IsStage(EGameStage.DuringCountdown))
            {
                StopCountdown();
            }
        }

        protected override void EndGame()
        {
            base.EndGame();

            PlayerMazeSolverObject.Instance.gameObject.SetActive(true);

        }

        public void EnteredExitZone()
        {
            if (IsStage(EGameStage.InGame))
            {
                CompletedGame();
            }
        }

        public void HitMazeWall()
        {
            _totalPenaltyTime += _penaltyOnWallHit;
            OnWallHit?.Invoke();
        }
    }
}
