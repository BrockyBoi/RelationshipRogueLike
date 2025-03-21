using Maze.Generation;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;

namespace Maze
{
    public class MazeSolverComponent : MonoBehaviour
    {
        public static MazeSolverComponent Instance { get; private set; }

        [SerializeField]
        private float _penaltyOnWallHit = 1.5f;

        [ShowInInspector, ReadOnly]
        private float _totalPenaltyTime = 0f;

        [SerializeField]
        private float _timeToSolveMaze = 10f;
        private float _countDownTimeGiven = 3f;

        private float _timeLeftToFinish = 0;

        private bool _hasStartCountdownBegun = false;
        private bool _hasGameStarted = false;
        public bool HasGameFinished { get; private set; }

        private Coroutine _gameStartCountdownCoroutine;

        private MazeNode _startNode;
        private MazeNode _endNode;

        public System.Action OnStartGameCountdownBegin;
        public System.Action OnStartGameCountdownLeft;
        public System.Action OnGameStart;
        public System.Action OnGameStop;

        public System.Action<float> OnCountdownValueChange;
        public System.Action<float> OnMainTimerValueChange;

        public System.Action OnWallHit;
        public System.Action OnMazeSolved;
        public System.Action OnMazeFailed;

        List<MazeCompletionResult> _mazeCompletionResults;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            MazeGenerator.Instance.ListenToOnMazeGenerated(OnMazeGenerated);
        }

        private void OnMazeGenerated()
        {
            HasGameFinished = false;
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
            if (!_hasGameStarted && !HasGameFinished && !_hasStartCountdownBegun)
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

        public void SetMazeCompletionResults(List<MazeCompletionResult> mazeCompletionResults)
        {
            _mazeCompletionResults = mazeCompletionResults;
        }

        private IEnumerator StartCoundownToBeginPlay()
        {
            if (_hasGameStarted)
            {
                yield break;
            }

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

            yield return StartSolvingMazeTimer();
        }

        private IEnumerator StartSolvingMazeTimer()
        {
            OnGameStart?.Invoke();

            _timeLeftToFinish = _timeToSolveMaze;
            _totalPenaltyTime = 0f;
            float countDownTimeWithPenalties = _timeLeftToFinish;
            while (countDownTimeWithPenalties > 0f && _hasGameStarted && !HasGameFinished)
            {
                _timeLeftToFinish -= Time.deltaTime;
                countDownTimeWithPenalties = _timeLeftToFinish - _totalPenaltyTime;

                OnMainTimerValueChange?.Invoke(countDownTimeWithPenalties);
                yield return null;
            }

            FailedMaze();
        }

        private void FailedMaze()
        {
            OnMazeFailed?.Invoke();
            OnMazeGameplayOver();
        }

        private void CompletedMaze()
        {
            OnMazeSolved?.Invoke();
            OnMazeGameplayOver();
        }

        private void OnMazeGameplayOver()
        {
            PlayerMazeSolverObject.Instance.gameObject.SetActive(true);
            HasGameFinished = true;
            StopAllCoroutines();
            OnGameStop?.Invoke();
            _hasGameStarted = false;
            _hasStartCountdownBegun = false;

            MazeDifficultyManager.Instance.ProvideDifficultyModifierResult(GetMazeCompletionResultToApply().DifficultyModifierResult);
        }

        public void EnteredExitZone()
        {
            if (_hasGameStarted && !HasGameFinished)
            {
                CompletedMaze();
            }
        }

        public void HitMazeWall()
        {
            _totalPenaltyTime += _penaltyOnWallHit;
            OnWallHit?.Invoke();
        }

        private float GetPercentageOfTimeLeftToSolveMaze()
        {
            return 1f - ((_timeLeftToFinish - _totalPenaltyTime) / _timeToSolveMaze);
        }

        public MazeCompletionResult GetMazeCompletionResultToApply()
        {
            if (_mazeCompletionResults.Count == 0)
            {
                Debug.LogError("There are no maze completion results");
                return new MazeCompletionResult();
            }

            float percentageTimeLeftToSolveMaze = GetPercentageOfTimeLeftToSolveMaze();
            // Ex player solved while only taking 25% of time, so value is 75%
            int indexDesired = 0;
            float percentagePerResult = 1f / _mazeCompletionResults.Count;
            // Ex there are only 3 results, so value is 33%
            do
            {
                indexDesired++;
            }
            while (indexDesired * percentagePerResult > percentageTimeLeftToSolveMaze);

            return _mazeCompletionResults[indexDesired - 1];
        }
    }
}
