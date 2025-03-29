using CustomUI;
using System.Collections;
using UnityEngine;
using TMPro;
using Dialogue.UI;
using Maze.Generation;

namespace Maze.UI
{
    public class MazeSolverUI : GameUI
    {
        public static MazeSolverUI Instance { get; private set; }

        [SerializeField]
        TextMeshProUGUI _timerText;

        bool _runningWallHitAnim = false;

        [SerializeField]
        private float _wallHitAnimTime = .5f;

        private void Awake()
        {
            Instance = this;
            HideUI();
        }

        void Start()
        {
            MazeSolverComponent mazeSolver = MazeSolverComponent.Instance;
            if (mazeSolver != null)
            {
                mazeSolver.OnMazeFailed += OnMazeFailed;
                mazeSolver.OnMazeSolved += OnMazeSolved;
                mazeSolver.OnCountdownValueChange += OnCountdownTimerValueChange;
                mazeSolver.OnMainTimerValueChange += OnGameTimerValueChange;
                mazeSolver.OnStartGameCountdownLeft += HideUI;
                mazeSolver.OnWallHit += OnWallHit;
            }

            MazeGenerator mazeGenerator = MazeGenerator.Instance;
            if (mazeGenerator != null)
            {
                mazeGenerator.OnGridGenerated += ShowUI;
            }

            DialogueUI dialogueUI = DialogueUI.Instance;
            if (dialogueUI != null)
            {
                dialogueUI.OnShowUI += HideUI;
            }
        }

        private void OnDestroy()
        {
            MazeSolverComponent mazeSolver = MazeSolverComponent.Instance;
            if (mazeSolver != null)
            {
                mazeSolver.OnMazeFailed -= OnMazeFailed;
                mazeSolver.OnMazeSolved -= OnMazeSolved;
                mazeSolver.OnCountdownValueChange -= OnCountdownTimerValueChange;
                mazeSolver.OnMainTimerValueChange -= OnGameTimerValueChange;
                mazeSolver.OnStartGameCountdownLeft -= HideUI;
                mazeSolver.OnWallHit -= OnWallHit;
            }

            MazeGenerator mazeGenerator = MazeGenerator.Instance;
            if (mazeGenerator != null)
            {
                mazeGenerator.OnGridGenerated -= ShowUI;
            }

            DialogueUI dialogueUI = DialogueUI.Instance;
            if (dialogueUI != null)
            {
                dialogueUI.OnShowUI -= HideUI;
            }
        }

        private void ClearText()
        {
            _timerText.text = string.Empty;
        }

        private void ResetTextVisuals()
        {
            _timerText.color = Color.white;
        }

        private void OnMazeSolved()
        {
            _timerText.text = "Ta daaaaa";
            StopAllCoroutines();
            ResetTextVisuals();
        }

        private void OnMazeFailed()
        {
            _timerText.text = "You suck LOLOLOLOl";
            StopAllCoroutines();
            ResetTextVisuals();
        }

        private void OnCountdownTimerValueChange(float value)
        {
            ShowUI();
            _timerText.text = "CountDown : " + value.ToString("F2");
        }

        private void OnGameTimerValueChange(float value)
        {
            _timerText.text = "Time Left : " + value.ToString("F2"); ;
        }

        private void OnWallHit()
        {
            if (!_runningWallHitAnim)
            {
                StartCoroutine(WallHitTextAnim());
            }
        }

        private IEnumerator WallHitTextAnim()
        {
            if (!_runningWallHitAnim)
            {
                _runningWallHitAnim = true;
                float animTime = 0;
                float halfTime = _wallHitAnimTime / 2;
                while (animTime < halfTime)
                {
                    animTime += Time.deltaTime;
                    _timerText.color = Color.Lerp(Color.white, Color.red, animTime / halfTime);
                    yield return null;
                }

                while (animTime < _wallHitAnimTime)
                {
                    animTime += Time.deltaTime;
                    _timerText.color = Color.Lerp(Color.red, Color.white, (animTime - halfTime) / halfTime);
                    yield return null;
                }

                _timerText.color = Color.white;
                _runningWallHitAnim = false;
            }
        }
    }
}
