using CustomUI;
using GeneralGame;
using Sirenix.OdinInspector;
using System.Collections;
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

        public int CurrentHealth { get { return _currentHealth; } }

        private int _spawnsWithoutDistractions = 0;
        private int _spawnsBetweenDistractions = 0;

        private WhackAMoleHole _currentlyHighlightedHole;

        public System.Action OnTakeDamage;

        private void Awake()
        {
            Instance = this;
        }

        protected override void Start()
        {
            base.Start();

            OnMainTimerEnd += EndGame;
        }

        protected override void StartGame()
        {
            base.StartGame();

            StartCoroutine(RunSpawningLogic());
        }

        protected void OnDestroy()
        {
            OnMainTimerEnd -= EndGame;
        }

        protected override void Update()
        {
            base.Update();

            //HandlePlayerInput();
        }

        private void HandlePlayerInput()
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

                        if (holeToHighlight != _currentlyHighlightedHole)
                        {
                            _currentlyHighlightedHole = holeToHighlight;

                            _currentlyHighlightedHole.Highlight();
                        }
                      
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

        private IEnumerator RunSpawningLogic()
        {
            if (_appearingObjectPrefabs.Count == 0)
            {
                Debug.LogError("Nothing to spawn");
                yield break;
            }

            while (IsStage(EGameStage.InGame))
            {
                yield return new WaitForSeconds(GameData.TimeBetweenSpawns);

                WhackAMoleHole randomHole = WhackAMoleGenerator.Instance.GetRandomUnoccupiedHole();
                if (randomHole)
                {
                    WhackAMoleAppearingObject appearingObject = Instantiate(GetObjectToSpawn(), randomHole.transform.position + Vector3.back, Quaternion.identity);
                    if (appearingObject)
                    {
                        randomHole.SetObjectInHole(appearingObject);
                        appearingObject.SetHoleExistingIn(randomHole);
                        ListenToAppearingObjectEvents(appearingObject);
                    }
                }
            }
        }

        private void ListenToAppearingObjectEvents(WhackAMoleAppearingObject appearingObject)
        {
            if (appearingObject)
            {
                appearingObject.OnAppearingObjectDestroyed += OnAppearingObjectDestroyed;
                appearingObject.OnAppearingObjectFailedToBeDestroyed += OnAppearingObjectDisappearedFromTime;
            }
        }

        private void UnlistenToApeparingObjectEvents(WhackAMoleAppearingObject appearingObject)
        {
            if (appearingObject)
            {
                appearingObject.OnAppearingObjectDestroyed -= OnAppearingObjectDestroyed;
                appearingObject.OnAppearingObjectFailedToBeDestroyed -= OnAppearingObjectDisappearedFromTime;
            }
        }

        private WhackAMoleAppearingObject GetObjectToSpawn()
        {
            if (GameData.HasDistractionObjects && ++_spawnsWithoutDistractions == _spawnsBetweenDistractions)
            {
                _spawnsBetweenDistractions = Random.Range(1, 5);
                return _distactionObjectPrefabs.GetRandomElement();
            }

            return _appearingObjectPrefabs.GetRandomElement();
        }

        private void OnAppearingObjectDestroyed(WhackAMoleAppearingObject appearingObject)
        {
            if (appearingObject && appearingObject.IsDistraction)
            {
                _currentHealth -= _gameData.HealthLostPerDistraction;
                CheckDeath();

                UnlistenToApeparingObjectEvents(appearingObject);

                OnTakeDamage?.Invoke();
            }
        }

        private void OnAppearingObjectDisappearedFromTime(WhackAMoleAppearingObject appearingObject)
        {
            if (appearingObject && !appearingObject.IsDistraction)
            {
                _currentHealth -= _gameData.HealthLostPerEnemy;
                CheckDeath();

                UnlistenToApeparingObjectEvents(appearingObject);

                OnTakeDamage?.Invoke();
            }
        }

        private void CheckDeath()
        {
            if (_currentHealth <= 0)
            {
                EndGame();
            }
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionResultIndexByHealthRemaining(_currentHealth, _startingHealth);
        }

        public override void SetGenerationGameData(WhackAMoleGenerationData generationData)
        {
            base.SetGenerationGameData(generationData);
            _startingHealth = _currentHealth = generationData.StartingHealth;
            if (generationData.HasDistractionObjects)
            {
                _spawnsBetweenDistractions = Random.Range(1, 7);
            }
        }

        public WhackAMoleCompletionResult GetResultByHealthRemaining()
        {
            return _gameCompletionResults[GetGameCompletionResultIndexByHealthRemaining(_currentHealth, _startingHealth)];
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            return GetCurrentPotentialDialoguePercentageByGameHealthRemaining(_currentHealth, _startingHealth);
        }
    }
}

