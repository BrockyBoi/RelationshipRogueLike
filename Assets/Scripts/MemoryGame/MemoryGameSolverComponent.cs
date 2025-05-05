using CustomUI;
using GeneralGame;
using MemoryGame.Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame
{
    public class MemoryGameSolverComponent : GameSolverComponent<MemoryGameGeneratorData, MemoryGameCompletionResult>
    {
        public static MemoryGameSolverComponent Instance { get; private set; }

        private MemoryGameCard _currentlySelectedCard;

        private bool _isShowingResults = false;

        [SerializeField]
        private int _defaultGuessesAllowed = 5;

        private int _cardsCollected = 0;

        public bool IsLookingForSingleMemoryType { get; private set; }
        public int GuessesLeft { get; private set; }
        public int TotalGuessesAllowed { get; private set; }

        public EMemoryType MemoryTypeToSearchFor { get { return MemoryGameGenerator.Instance.MemoryTypeToSearchFor; } }

        private HashSet<EMemoryType>_memoryTypesSearchedForPreviously;

        public System.Action OnGuessMade;

        private void Awake()
        {
            Instance = this;
            _memoryTypesSearchedForPreviously = new HashSet<EMemoryType>();
        }

        protected override void Start()
        {
            base.Start();
            MemoryGameGenerator.Instance.ListenToOnCardValuesSet(OnCardValuesSet);
            MemoryGameCard.OnCardClicked += SelectCard;
        }

        private void OnDestroy()
        {
            MemoryGameGenerator.Instance.UnlistenToOnGameGenerated(OnCardValuesSet);
            MemoryGameCard.OnCardClicked -= SelectCard;
        }

        protected override void EndGame()
        {
            base.EndGame();
            GuessesLeft = 0;
        }

        private void OnCardValuesSet()
        {
            SetGameStage(EGameStage.PreCountdown);
            _cardsCollected = 0;
            TotalGuessesAllowed = _defaultGuessesAllowed + MemoryGameDifficultyManager.Instance.NumberOfGuessesModifier;
            GuessesLeft = TotalGuessesAllowed;
            StartGame();

            _memoryTypesSearchedForPreviously.Add(MemoryTypeToSearchFor);
        }

        public void SelectCard(MemoryGameCard card)
        {
            if (IsStage(EGameStage.InGame) && card != null && !_isShowingResults) 
            {
                if (_currentlySelectedCard == null)
                {
                    _currentlySelectedCard = card;
                    card.ShowCard();
                }
                else if (card != _currentlySelectedCard)
                {
                    StartCoroutine(ShowCardResults(card));
                }
            }
        }

        private IEnumerator ShowCardResults(MemoryGameCard card)
        {
            _isShowingResults = true;
            card.ShowCard();

            if (card.MemoryType == _currentlySelectedCard.MemoryType)
            {
                if (card.MemoryType == EMemoryType.Bomb)
                {
                    FailGame();
                }
                else
                {
                    yield return new WaitForSeconds(1f);

                    card.CollectCard();
                    _currentlySelectedCard.CollectCard();
                    _cardsCollected += 2;

                    if (IsLookingForSingleMemoryType && card.MemoryType == MemoryTypeToSearchFor)
                    {
                        CompletedGame();
                    }
                    else if (!IsLookingForSingleMemoryType && _cardsCollected == MemoryGameGenerator.Instance.TotalElementsCount)
                    {
                        CompletedGame();
                    }
                    else
                    {
                        OnGuessMade?.Invoke();
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(1f);
                _currentlySelectedCard.HideCard();
                card.HideCard();
                GuessesLeft--;

                if (GuessesLeft <= 0)
                {
                    FailGame();
                }

                OnGuessMade?.Invoke();
            }

            _currentlySelectedCard = null;
            _isShowingResults = false;
        }

        protected override void ApplyEndGameResults()
        {
            MemoryGameCompletionResult result = IsLookingForSingleMemoryType ? GetGameCompletionResultToApplyBySucceeding() : GetGameCompletionResultToApplyByGuessesLeft();
            result.ApplyEffects();
        }

        public bool AlreadyPlayedForMemoryType(EMemoryType memoryType)
        {
            return _memoryTypesSearchedForPreviously.Contains(memoryType);
        }

        public void SetIsLookingForSingleMemoryType(bool isLookingForSingleMemoryType)
        {
            IsLookingForSingleMemoryType = isLookingForSingleMemoryType;
        }

        public int GetGameCompletionIndexBasedOnGuessesLeft()
        {
            if (_gameCompletionResults == null || _gameCompletionResults.Count == 0)
            {
                Debug.LogError("There are no completion results");
                return 0;
            }

            float percentageOfGuessesUsed = 1f - (GuessesLeft / (float)TotalGuessesAllowed);
            return Mathf.Clamp(Mathf.FloorToInt(_gameCompletionResults.Count * percentageOfGuessesUsed), 0, _gameCompletionResults.Count - 1);
        }

        public override float GetCurrentPotentialDialoguePercentage()
        {
            return GetCurrentPotentialDialoguePercentageByGameHealthRemaining(GuessesLeft, TotalGuessesAllowed);
        }

        public MemoryGameCompletionResult GetGameCompletionResultToApplyByGuessesLeft()
        {
            return _gameCompletionResults[GetGameCompletionIndexBasedOnGuessesLeft()];
        }

        public void SetBaseNumberOfGuesses(int baseNumberOfGuesses)
        {
            _defaultGuessesAllowed = baseNumberOfGuesses;
        }

        protected override BaseGameUI GetGameUIInstance()
        {
            throw new System.NotImplementedException();
        }

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionIndexBasedOnGuessesLeft();
        }
        public override void SetGenerationGameData(MemoryGameGeneratorData generationData)
        {
            base.SetGenerationGameData(generationData);
        }
    }
}
