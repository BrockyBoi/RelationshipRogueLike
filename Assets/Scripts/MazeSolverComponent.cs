using Maze.Generation;
using System.Collections;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Maze
{
    public class MazeSolverComponent : MonoBehaviour
    {
        public static MazeSolverComponent Instance { get; private set; }

        private bool _mouseHasReachedStartNode = false;

        [SerializeField]
        private float _penaltyOnWallHit = 1.5f;

        private float _totalPenaltyTime = 0f;

        [SerializeField]
        private float _timeToSolveMaze = 10f;
        private float _countDownTimeGiven = 3f;

        private bool _hasStartCountdownBegun = false;
        private bool _hasGameStarted = false;
        private bool _hasGameFinished = false;

        private Coroutine _gameStartCountdownCoroutine;

        private MazeNode _startNode;
        private MazeNode _endNode;

        public System.Action OnStartGameCountdownBegin;
        public System.Action OnStartGameCountdownLeft;
        public System.Action OnGameStart;

        public System.Action<float> OnCountdownValueChange;
        public System.Action<float> OnMainTimerValueChange;

        public System.Action OnWallHit;
        public System.Action OnMazeSolved;
        public System.Action OnMazeFailed;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            MazeGenerator.Instance.OnMazeGenerated += OnMazeGenerated;
        }

        private void OnMazeGenerated()
        {
            _startNode = MazeGenerator.Instance.StartNode;
            _endNode = MazeGenerator.Instance.EndNode;

            if ( _startNode != null )
            {
                _startNode.OnCursorEntered += EnterStartZone;
                _startNode.OnCursorExited += ExitStartZone;
            }

            if ( _endNode != null )
            {
                _endNode.OnCursorEntered += EnteredExitZone;
            }
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
            if (!_hasGameStarted && !_hasGameFinished && !_hasStartCountdownBegun)
            {
                OnStartGameCountdownBegin?.Invoke();
                _gameStartCountdownCoroutine = StartCoroutine(StartCoundownToBeginPlay());
            }
        }

        public void ExitStartZone()
        {
            if (_hasStartCountdownBegun && !_hasGameStarted)
            {
                OnStartGameCountdownLeft?.Invoke();
                _hasStartCountdownBegun = false;
                StopCoroutine(_gameStartCountdownCoroutine);
            }
        }

        private IEnumerator StartCoundownToBeginPlay()
        {
            if (_hasGameStarted)
            {
                yield break;
            }
            Debug.Log("Starting initial countdown");
            _hasStartCountdownBegun = true;
            float countdownTime = _countDownTimeGiven;
            while (countdownTime > 0f)
            {
                countdownTime -= Time.deltaTime;
                OnCountdownValueChange(countdownTime);
                yield return null;
            }

            _hasStartCountdownBegun = false;
            _hasGameStarted = true;

            Debug.Log("Finished initial countdown");
            yield return StartSolvingMazeTimer();
        }

        private IEnumerator StartSolvingMazeTimer()
        {
            OnGameStart?.Invoke();

            float countDownTime = _timeToSolveMaze;
            float countDownTimeWithPenalties = countDownTime;
            while (countDownTimeWithPenalties > 0f && _hasGameStarted && !_hasGameFinished)
            {
                countDownTime -= Time.deltaTime;
                countDownTimeWithPenalties = countDownTime - _totalPenaltyTime;

                OnMainTimerValueChange?.Invoke(countDownTimeWithPenalties);
                yield return null;
            }

            _hasGameFinished = true;
            OnMazeFailed?.Invoke();
        }

        public void EnteredExitZone()
        {
            if (_hasGameStarted && !_hasGameFinished)
                {
                _hasGameFinished = true;
                OnMazeSolved?.Invoke();
                StopAllCoroutines();
            }
        }

        public void HitMazeWall()
        {
            _totalPenaltyTime += _penaltyOnWallHit;
            OnWallHit?.Invoke();
        }
    }
}
