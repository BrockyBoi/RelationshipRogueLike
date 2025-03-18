using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Maze
{
    public class MazeSolverUI : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _timerText;

        bool _runningWallHitAnim = false;

        [SerializeField]
        private float _wallHitAnimTime = .5f;

        // Start is called before the first frame update
        void Start()
        {
            MazeSolverComponent mazeSolver = MazeSolverComponent.Instance;
            if (mazeSolver != null)
            {
                mazeSolver.OnMazeFailed += OnMazeFailed;
                mazeSolver.OnMazeSolved += OnMazeSolved;
                mazeSolver.OnCountdownValueChange += OnCountdownTimerValueChange;
                mazeSolver.OnMainTimerValueChange += OnGameTimerValueChange;
                mazeSolver.OnStartGameCountdownLeft += ClearText;
                mazeSolver.OnWallHit += OnWallHit;
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
                mazeSolver.OnStartGameCountdownLeft -= ClearText;
                mazeSolver.OnWallHit -= OnWallHit;
            }
        }

        // Update is called once per frame
        void Update()
        {

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
