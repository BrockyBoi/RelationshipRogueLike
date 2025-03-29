using CustomUI;
using GeneralGame;
using MemoryGame.Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame
{
    public class MemoryGameSolverComponent : GameSolverComponent<MemoryGameCompletionResult>
    {
        public static MemoryGameSolverComponent Instance { get; private set; }

        private MemoryGameCard _currentlySelectedCard;

        private bool _isShowingResults = false;

        [SerializeField]
        private int _defaultGuessesAllowed = 5;
        private int _totalGuesses = 0;
        public int TotalGuessesAllowed { get; private set; }

        private EMemoryType _memoryTypeToSearchFor;

        public System.Action OnGuessMade;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            MemoryGameGenerator.Instance.ListenToOnGameGenerated(OnCardsGenerated);
            MemoryGameCard.OnCardClicked += SelectCard;
        }

        private void OnDestroy()
        {
            MemoryGameCard.OnCardClicked -= SelectCard;
        }

        protected override void EndGame()
        {
            base.EndGame();
            _totalGuesses = 0;
        }

        private void OnCardsGenerated()
        {
            _totalGuesses = 0;
            TotalGuessesAllowed = _defaultGuessesAllowed + MemoryGameDifficultyManager.Instance.NumberOfGuessesModifier;
            do
            {
                _memoryTypeToSearchFor = MemoryGameGenerator.Instance.GetRandomGridElement().MemoryType;
            }
            while (_memoryTypeToSearchFor == EMemoryType.Bomb);

            StartGame();
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

                    if (card.MemoryType == _memoryTypeToSearchFor)
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
                _totalGuesses++;

                if (_totalGuesses >= MemoryGameDifficultyManager.Instance.NumberOfGuessesModifier)
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
            MemoryGameCompletionResult result = GetGameCompletionResultToApplyBySucceeding();
            result.ApplyEffects();
        }

        protected override GameUI GetGameUIInstance()
        {
            throw new System.NotImplementedException();
        }
    }
}
