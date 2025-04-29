using CustomUI;
using System.Collections;
using UnityEngine;
using TMPro;
using Dialogue.UI;
using Maze.Generation;

namespace Maze.UI
{
    public class MazeSolverUI : GameUI<MazeGenerator, MazeSolverComponent>
    {
        public static MazeSolverUI Instance { get; private set; }

        protected override MazeGenerator GameGenerator { get { return MazeGenerator.Instance; } }

        protected override MazeSolverComponent GameSolver { get { return MazeSolverComponent.Instance; } }

        bool _runningWallHitAnim = false;

        [SerializeField]
        private float _wallHitAnimTime = .5f;

        private void Awake()
        {
            Instance = this;
            HideUI();
        }

        protected override void Start()
        {
            base.Start();

            GameSolver.OnStartGameCountdownLeft += HideUI;
            GameSolver.OnWallHit += OnWallHit;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            GameSolver.OnStartGameCountdownLeft -= HideUI;
            GameSolver.OnWallHit -= OnWallHit;
        }

        private void ResetTextVisuals()
        {
            _timerText.color = Color.white;
        }

        protected override void OnGameCompleted()
        {
            base.OnGameCompleted();

            StopAllCoroutines();
            ClearTimerText();
            ResetTextVisuals();
        }

        protected override void OnGameFailed()
        {
            base.OnGameFailed();

            StopAllCoroutines();
            ResetTextVisuals();
            ClearTimerText();
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
