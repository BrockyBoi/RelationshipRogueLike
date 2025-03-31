using GeneralGame;
using GeneralGame.Generation;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame.Generation
{
    public class MemoryGameGenerator : GameGridGenerator<MemoryGameGeneratorData, MemoryGameCompletionResult, MemoryGameCard>
    {
        public static MemoryGameGenerator Instance {  get; private set; }

        public EMemoryType MemoryTypeToSearchFor { get; private set; }

        [SerializeField]
        private float _timeBetweenSwaps = .75f;

        private bool _hasCardValuesBeenSet = false;
        private System.Action OnCardValuesSet;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            MemoryGameSolverComponent.Instance.OnGuessMade += OnGuessMade;
        }

        private void OnDestroy()
        {
            MemoryGameSolverComponent.Instance.OnGuessMade -= OnGuessMade;
        }

        protected override void CreateGrid(Vector2Int gridSize, List<MemoryGameCompletionResult> results)
        {
            if (gridSize.x * gridSize.y % 2 != 0)
            {
                Debug.LogError("Cannot spawn an odd number of cards");
                return;
            }

            base.CreateGrid(gridSize, results);
            SetAllCardValues();
        }

        private void SetAllCardValues()
        {
            int bombsInGame = MemoryGameDifficultyManager.Instance.BombsInGame;
            bool canHaveBombs = bombsInGame > 1;
            List<MemoryGameCard> tempMemoryGameCardsList = new List<MemoryGameCard>();
            foreach (MemoryGameCard card in _objectGrid)
            {
                tempMemoryGameCardsList.Add(card);
            }

            SetNumberOfCardsToValue(bombsInGame, EMemoryType.Bomb, ref tempMemoryGameCardsList);
            SetNumberOfCardsToValue(2, MemoryTypeToSearchFor, ref tempMemoryGameCardsList);
            FillRandomValuesForCardsList(ref tempMemoryGameCardsList);

            OnCardValuesSet?.Invoke();
        }

        private void SetNumberOfCardsToValue(int numberOfCardsToSet, EMemoryType memoryType, ref List<MemoryGameCard> initializedCards)
        {
            for (int i = 0; i < numberOfCardsToSet; i++)
            {
                if (initializedCards.Count > 0)
                {
                    MemoryGameCard randomCard = initializedCards.GetRandomElement();
                    if (randomCard)
                    {
                        randomCard.SetMemoryType(memoryType);
                        initializedCards.Remove(randomCard);
                    }
                }
                else
                {
                    Debug.LogError("No valid cards to set");
                    break;
                }
            }
        }

        private void FillRandomValuesForCardsList(ref List<MemoryGameCard> cards)
        {
            Dictionary<EMemoryType, int> cardCountsPerMemoryType = new Dictionary<EMemoryType, int>();
            bool hasBombs = MemoryGameDifficultyManager.Instance.BombsInGame > 1;
            while (cards.Count > 0)
            {
                EMemoryType memoryType;
                int increments = 0;
                // Remove 2 since we remove one type for bombs and remove another type for the memory type we have to search for
                int possibleMemoryTypeCounts = (_objectGrid.Length / 2) - 1 - (hasBombs ? 1 : 0);
                do
                {
                    memoryType = GlobalFunctions.RandomEnumValue<EMemoryType>();
                    if (++increments >= 100)
                    {
                        Debug.LogError("Spent too much time in SetAllCardValues");
                        break;
                    }
                }
                while ((cardCountsPerMemoryType.ContainsKey(memoryType) && cardCountsPerMemoryType[memoryType] == 2) ||
                (!cardCountsPerMemoryType.ContainsKey(memoryType) && cardCountsPerMemoryType.Count >= possibleMemoryTypeCounts) ||
                memoryType == EMemoryType.Bomb ||
                MemoryGameSolverComponent.Instance.AlreadyPlayedForMemoryType(memoryType) ||
                memoryType == MemoryTypeToSearchFor);

                if (increments >= 100)
                {
                    break;
                }

                if (!cardCountsPerMemoryType.ContainsKey(memoryType))
                {
                    cardCountsPerMemoryType.Add(memoryType, 0);
                }
                cardCountsPerMemoryType[memoryType]++;

                MemoryGameCard card = cards.GetRandomElement();
                card.SetMemoryType(memoryType);
                cards.Remove(card);
            }
        }

        public void SetMemoryTypeToSearchFor(EMemoryType memoryType)
        {
            MemoryTypeToSearchFor = memoryType;
        }

        public void ListenToOnCardValuesSet(System.Action action)
        {
            if (!_hasCardValuesBeenSet)
            {
                OnCardValuesSet += action;
            }
            else
            {
                action?.Invoke();
            }
        }

        public void UnlistenToOnCardValuesSet(System.Action action)
        {
            OnCardValuesSet -= action;
        }

        protected override int GetDifficultySizeModifier()
        {
            return MemoryGameDifficultyManager.Instance.GridSizeModifier;
        }

        protected override GameObject GetGridParentObject()
        {
            return ParentObjectsManager.Instance.CardGameParent;
        }

        private void OnGuessMade()
        {
            int cardsToSwap = MemoryGameDifficultyManager.Instance.CardsToSwap;
            if (cardsToSwap > 0 && MemoryGameSolverComponent.Instance.IsStage(EGameStage.InGame))
            {
                StartCoroutine(SwapCards(cardsToSwap));
            }
        }

        private IEnumerator SwapCards(int timesToSwap)
        {
            for (int i = 0; i < timesToSwap; i++)
            {
                SwapRandomCards();
                yield return new WaitForSeconds(_timeBetweenSwaps);
            }
        }

        private void SwapRandomCards()
        {
            MemoryGameCard card1 = GetRandomGridElement();
            Vector3 card1StartPos = card1.transform.position;
            MemoryGameCard card2;
            do
            {
                card2 = GetRandomGridElement();
            } while (card1 == card2);
            Vector3 card2StartPos = card2.transform.position;

            GlobalFunctions.LerpObjectToLocation(card1.gameObject, card2StartPos, .75f);
            GlobalFunctions.LerpObjectToLocation(card2.gameObject, card1StartPos, .75f);
        }

        protected override void GiveResultsToSolver(List<MemoryGameCompletionResult> results)
        {
            MemoryGameSolverComponent.Instance.SetGameCompletionResults(results);
        }

        public override void GenerateGame(MemoryGameGeneratorData generationData)
        {
            CreateGrid(generationData.GridSize, generationData.GameCompletionResults);
        }

        [Button]
        public void DebugShowAllCards()
        {
            if (_objectGrid != null && _objectGrid.Length > 0)
            {
                foreach (MemoryGameCard card in _objectGrid)
                {
                    card.ShowCard();
                }
            }
        }

        [Button]
        public void DebugHideAllCards()
        {
            if (_objectGrid != null && _objectGrid.Length > 0)
            {
                foreach (MemoryGameCard card in _objectGrid)
                {
                    card.HideCard();
                }
            }
        }
    }
}
