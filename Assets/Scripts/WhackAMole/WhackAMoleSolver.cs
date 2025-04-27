using CustomUI;
using GeneralGame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    public class WhackAMoleSolver : GameSolverComponent<WhackAMoleGenerationData, WhackAMoleCompletionResult>
    {
        public static WhackAMoleSolver Instance { get; private set; }

        [SerializeField, AssetsOnly, Required]
        private List<WhackAMoleAppearingObject> _appearingObjectPrefabs;

        [SerializeField, AssetsOnly, Required]
        private List<WhackAMoleAppearingObject> _distactionObjectPrefabs;

        private int _startingHealth = 10;
        private int _currentHealth = 10;

        private WhackAMoleHole _currentlyHighlightedHole;

        private void Awake()
        {
            Instance = this;
        }

        protected override void StartGame()
        {
            base.StartGame();
        }

        protected void Update()
        {
            if (IsStage(EGameStage.InGame))
            {
                int xInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                int yInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
                if (xInput != 0 || yInput != 0)
                {
                    Vector3 inputVec = new Vector3(xInput, 0, yInput);
                    inputVec.Normalize();
                    float angle = Vector3.Angle(Vector3.right, inputVec);
                    if (yInput < 0)
                    {
                        angle += 180;

                        if (xInput != 0)
                        {
                            angle += xInput > 0 ? 90 : -90;
                        }
                    }

                    WhackAMoleHole holeToHighlight = WhackAMoleGenerator.Instance.GetHoleNearestToAngle(angle);
                    if (holeToHighlight)
                    {
                        if (_currentlyHighlightedHole && _currentlyHighlightedHole != holeToHighlight)
                        {
                            _currentlyHighlightedHole.StopHighlighting();
                        }
                        _currentlyHighlightedHole = holeToHighlight;
                        _currentlyHighlightedHole.Highlight();
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            _currentlyHighlightedHole.PlayerHitHole();
                        }
                    }
                    else
                    {
                        Debug.LogError("No hole was chosen");
                    }
                }
                else if (_currentlyHighlightedHole && xInput == 0 && yInput == 0)
                {
                    _currentlyHighlightedHole.StopHighlighting();
                    _currentlyHighlightedHole = null;
                }
            }
        }

        private void OnAppearingObjectDestroyed(bool isDistraction)
        {
            if (isDistraction)
            {
                _currentHealth -= _gameData.HealthLostPerDistraction;
                CheckDeath();
            }
        }

        private void OnAppearingObjectDisappearedFromTime(bool isDistraction)
        {
            if (!isDistraction)
            {
                _currentHealth -= _gameData.HealthLostPerEnemy;
                CheckDeath();
            }
        }

        private void CheckDeath()
        {
            if (_currentHealth <= 0)
            {
                EndGame();
            }
        }

        protected override GameUI GetGameUIInstance()
        {
            throw new System.NotImplementedException();
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByHealthRemaining();
        }

        public override void SetGenerationGameData(WhackAMoleGenerationData generationData)
        {
            base.SetGenerationGameData(generationData);
            _timeToCompleteGame = generationData.TimeToPlay;
            _startingHealth = _currentHealth = generationData.StartingHealth;
            StartGame();
        }

        protected override void ApplyEndGameResults()
        {
            WhackAMoleCompletionResult result = _gameCompletionResults[GetGameCompletionResultIndexByHealthRemaining()];
            result.ApplyEffects();
        }

        private int GetGameCompletionResultIndexByHealthRemaining()
        {
            if (_gameCompletionResults == null || _gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            int index = Mathf.Clamp(Mathf.FloorToInt(_gameCompletionResults.Count * (_currentHealth / _startingHealth)), 0, _gameCompletionResults.Count - 1);
            return _gameCompletionResults.Count - 1 - index;
        }

        public WhackAMoleCompletionResult GetResultByHealthRemaining()
        {
            return _gameCompletionResults[GetGameCompletionResultIndexByHealthRemaining()];
        }
    }
}

