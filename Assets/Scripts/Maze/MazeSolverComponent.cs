using Maze.Generation;
using GeneralGame;
using UnityEngine;
using CustomUI;
using Maze.UI;

namespace Maze
{
    public class MazeSolverComponent : GameSolverComponent<MazeCompletionResult>
    {
        public static MazeSolverComponent Instance { get; private set; }

        [SerializeField]
        private float _penaltyOnWallHit = 1.5f;

        private MazeNode _startNode;
        private MazeNode _endNode;

        public System.Action OnWallHit;
        public System.Action OnMazeSolved;
        public System.Action OnMazeFailed;
        private void Awake()
        {
            Instance = this;
        }

        protected override void Start()
        {
            base.Start();
            MazeGenerator.Instance.ListenToOnMazePathGenerated(OnMazePathGenerated);
        }

        private void OnMazePathGenerated()
        {
            SetGameStage(EGameStage.PreCountdown);
            _startNode = MazeGenerator.Instance.StartNode;
            _endNode = MazeGenerator.Instance.EndNode;

            if (_startNode == null || _endNode == null)
            {
                Debug.LogError("Either start or end node is null in maze");
                return;
            }

            if (_startNode != null)
            {
                _startNode.OnCursorEntered += EnterStartZone;
                _startNode.OnCursorExited += ExitStartZone;
            }

            if (_endNode != null)
            {
                _endNode.OnCursorEntered += EnteredExitZone;
            }
        }

        private void OnDisable()
        {
            MazeGenerator.Instance.UnlistenToMazePathGenerated(OnMazePathGenerated);

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

        private void EnterStartZone()
        {
            if (IsStage(EGameStage.PreCountdown))
            {
                StartCountDown();
            }
        }

        private void ExitStartZone()
        {
            if (IsStage(EGameStage.DuringCountdown))
            {
                StopCountdown();
            }
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

        protected override void ApplyEndGameResults()
        {
            base.ApplyEndGameResults();
            MazeCompletionResult result = GetGameCompletionResultToApplyByTimeRemaining();
            result.ApplyEffects();
        }

        protected override GameUI GetGameUIInstance()
        {
            return MazeSolverUI.Instance;
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByTimeRemaining();
        }
    }
}
