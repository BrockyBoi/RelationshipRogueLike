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

                    if (card.MemoryType == MemoryTypeToSearchFor)
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
            base.ApplyEndGameResults();

            MemoryGameCompletionResult result = GetGameCompletionResultToApplyBySucceeding();
            result.ApplyEffects();
        }

        public bool AlreadyPlayedForMemoryType(EMemoryType memoryType)
        {
            return _memoryTypesSearchedForPreviously.Contains(memoryType);
        }

        protected override GameUI GetGameUIInstance()
        {
            throw new System.NotImplementedException();
        }
    }
}
