using Maze.Generation;
using GeneralGame;
using UnityEngine;
using CustomUI;
using Maze.UI;

namespace Maze
{
    public class MazeSolverComponent : GameSolverComponent<MazeGeneratorData, MazeCompletionResult>
    {
        public static MazeSolverComponent Instance { get; private set; }

        [SerializeField]
        private float _penaltyOnWallHit = 1.5f;

        private float _fakeMazeTime = 0f;

        private MazeNode _startNode;
        private MazeNode _endNode;

        private int _keysNeeded = 0;

        public int KeysNeeded {  get { return _keysNeeded; } }

        private bool IsFakeGame { get { return !Mathf.Approximately(_fakeMazeTime, 0) && _fakeMazeTime > 0; } }

        public System.Action OnWallHit;
        public System.Action OnKeyCollected;
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
            OnMainTimerEnd += FailGame;
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

            OnMainTimerEnd -= FailGame;

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

        public void SetFakeTime(float time)
        {
            _fakeMazeTime = time;
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
            if (IsStage(EGameStage.InGame) && _keysNeeded == 0)
            {
                CompletedGame();
            }
        }

        protected override void StartGame()
        {
            base.StartGame();

            if (!Mathf.Approximately(_fakeMazeTime, 0))
            {
                Invoke("OnFakeTimerEnd", _fakeMazeTime);
            }
        }

        public void CollectKey()
        {
            _keysNeeded--;
            Debug.Log("Keys left in map: " + _keysNeeded);
            OnKeyCollected?.Invoke();
        }

        public void OnTimePickupCollected(float timeAdded)
        {
            _bonusTimeGained += timeAdded;
        }

        private void OnFakeTimerEnd()
        {
            CompletedGame();
        }

        public void HitMazeWall()
        {
            _totalPenaltyTime += _penaltyOnWallHit;
            OnWallHit?.Invoke();
        }

        public void GainBonusTime(float bonusTimeGained)
        {
            _bonusTimeGained += bonusTimeGained;
        }

        protected override void ApplyEndGameResults()
        {
            if (!IsFakeGame)
            {
                MazeCompletionResult result = GetGameCompletionResultToApplyByTimeRemaining();
                result.ApplyEffects();
            }

            Cursor.visible = true;
        }

        protected override BaseGameUI GetGameUIInstance()
        {
            return MazeSolverUI.Instance;
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByTimeRemaining();
        }

        public override void SetGenerationGameData(MazeGeneratorData generationData)
        {
            base.SetGenerationGameData(generationData);
            _keysNeeded = generationData.NeedsKeys ? generationData.KeysNeeded : 0;
        }
    }
}
