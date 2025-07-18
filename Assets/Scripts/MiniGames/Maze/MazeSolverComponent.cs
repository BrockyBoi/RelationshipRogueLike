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

        private int _keysToCollect = 0;
        public int KeysToCollect {  get { return _keysToCollect; } }

        private bool IsFakeGame { get { return !Mathf.Approximately(_fakeMazeTime, 0) && _fakeMazeTime > 0; } }

        [SerializeField]
        private AudioClip _unlockGateAudioClip;

        [SerializeField]
        private AudioClip _collectKeyAudioClip;

        public System.Action OnWallHit;
        public System.Action OnKeyCollected;
        public System.Action OnMazeSolved;
        public System.Action OnMazeFailed;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
            _startGameTimerOnInitialize = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (MazeGenerator.Instance != null)
            {
                MazeGenerator.Instance.ListenToOnMazePathGenerated(OnMazePathGenerated);
            }
            OnMainTimerEnd += FailGame;
        }

        private void OnMazePathGenerated()
        {
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

        protected override void OnDisable()
        {
            base.OnDisable();
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
            if (IsStage(EGameStage.InGame) && _keysToCollect == 0)
            {
                CompleteGame();
            }
        }

        protected override void StartGame()
        {
            base.StartGame();

            if (!Mathf.Approximately(_fakeMazeTime, 0))
            {
                MainPlayer.Player.Instance.HealthComponent.SetInvincibility(true);
                Invoke("OnFakeTimerEnd", _fakeMazeTime);
            }
        }

        protected override void EndGame()
        {
            base.EndGame();

            MainPlayer.Player.Instance.HealthComponent.SetInvincibility(false);
        }

        protected override void MainTimerValueChanged(float newTime)
        {
            UpdatePotentialPlayerDialogueUI();
        }

        public void CollectKey()
        {
            _keysToCollect--;
            OnKeyCollected?.Invoke();

            AudioManager.Instance.PlaySoundEffect(_collectKeyAudioClip);

            if (KeysToCollect <= 0)
            {
                AudioManager.Instance.PlaySoundEffect(_unlockGateAudioClip);
            }
        }

        public void OnTimePickupCollected(float timeAdded)
        {
            _bonusTimeGained += timeAdded;
        }

        private void OnFakeTimerEnd()
        {
            CompleteGame();
        }

        public void HitMazeWall()
        {
            AddPenaltyTime(_penaltyOnWallHit);
            OnWallHit?.Invoke();
        }

        public void GainBonusTime(float bonusTimeGained)
        {
            _bonusTimeGained += bonusTimeGained;
        }

        protected override void ApplyEndGameResults()
        {
            base.ApplyEndGameResults();

            Cursor.visible = true;
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByTimeRemaining();
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            return GetCurrentPotentialDialoguePercentageByTimeRemaining();
        }

        protected override void SetGenerationGameData(MazeGeneratorData generationData)
        {
            base.SetGenerationGameData(generationData);
            _keysToCollect = generationData.NeedsKeys ? generationData.KeysNeeded : 0;
        }
    }
}
