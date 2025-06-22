using CustomUI;
using GeneralGame;
using MemoryGame.Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

namespace MemoryGame
{
    public class MemoryGameSolverComponent : GameSolverComponent<MemoryGameGeneratorData, MemoryGameCompletionResult>
    {
        public static MemoryGameSolverComponent Instance { get; private set; }

        private MemoryGameCard _currentlySelectedCard;

        private bool _isShowingResults = false;

        private int _cardsCollected = 0;

        public bool IsLookingForSingleMemoryType { get; private set; }
        public int GuessesLeft { get; private set; }
        public int TotalGuessesAllowed { get { return _gameData.NumberOfGuesses + MemoryGameDifficultyManager.Instance.NumberOfGuessesModifier; } }

        public EMemoryType MemoryTypeToSearchFor { get { return MemoryGameGenerator.Instance.MemoryTypeToSearchFor; } }

        private HashSet<EMemoryType>_memoryTypesSearchedForPreviously;

        public System.Action OnGuessMade;

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
            _memoryTypesSearchedForPreviously = new HashSet<EMemoryType>();
        }

        protected override void StartGame()
        {
            base.StartGame();

            MemoryGameGenerator.Instance.ListenToOnCardValuesSet(OnCardValuesSet);
            MemoryGameCard.OnCardClicked += SelectCard;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (MemoryGameGenerator.Instance != null)
            {
                MemoryGameGenerator.Instance.UnlistenToOnCardValuesSet(OnCardValuesSet);
            }
            MemoryGameCard.OnCardClicked -= SelectCard;
        }

        protected override void EndGame()
        {
            base.EndGame();
            GuessesLeft = 0;
        }

        private void OnCardValuesSet()
        {
            _cardsCollected = 0;
            GuessesLeft = TotalGuessesAllowed;
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
                        CompleteGame();
                    }
                    else if (!IsLookingForSingleMemoryType && _cardsCollected == MemoryGameGenerator.Instance.TotalElementsCount)
                    {
                        CompleteGame();
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

            if (GuessesLeft > 0)
            {
                UpdatePotentialPlayerDialogueUI();
            }
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
            if (!ensure(_gameCompletionResults != null && _gameCompletionResults.Count != 0, "There are no completion results"))
            {
                return 0;
            }

            if (!ensure(TotalGuessesAllowed > 0, "Total guesses allowed must be greater than 0"))
            {
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

        public override int GetCurrentPotentialDialogueIndex()
        {
            return GetGameCompletionIndexBasedOnGuessesLeft();
        }
    }
}
