using GeneralGame;
using GeneralGame.Generation;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MemoryGame.Generation
{
    public class MemoryGameGenerator : GameGridGenerator<MemoryGameSolverComponent, MemoryGameGeneratorData, MemoryGameCompletionResult, MemoryGameCard>
    {
        public static MemoryGameGenerator Instance {  get; private set; }

        public EMemoryType MemoryTypeToSearchFor { get; private set; }

        protected override MemoryGameSolverComponent GameSolverComponent { get { return MemoryGameSolverComponent.Instance; } }

        private EMemoryType _allowedMemoryTypes;

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

        protected override void CreateGrid(Vector2Int gridSize)
        {
            if (gridSize.x * gridSize.y % 2 != 0)
            {
                Debug.LogError("Cannot spawn an odd number of cards");
                return;
            }

            base.CreateGrid(gridSize);
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

            if (canHaveBombs)
            {
                SetNumberOfCardsToValue(bombsInGame, EMemoryType.Bomb, ref tempMemoryGameCardsList);
            }

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
            bool isSearchingForSingleMemoryType = MemoryGameSolverComponent.Instance.IsLookingForSingleMemoryType;
            List<EMemoryType> avaialableMemoryTypes = GlobalFunctions.EnumToList(EMemoryType.ALL, EMemoryType.Bomb);
            if (isSearchingForSingleMemoryType)
            {
                avaialableMemoryTypes.Remove(MemoryTypeToSearchFor);
            }

            while (cards.Count > 0)
            {
                EMemoryType memoryType;
                int increments = 0;
                do
                {
                    memoryType = avaialableMemoryTypes.GetRandomElement();
                    if (++increments >= 100)
                    {
                        Debug.LogError("Spent too much time in SetAllCardValues");
                        break;
                    }

                }
                while (MemoryGameSolverComponent.Instance.AlreadyPlayedForMemoryType(memoryType) ||
                !_allowedMemoryTypes.Has(memoryType));

                if (increments >= 100)
                {
                    break;
                }

                for (int i = 0; i < 2; i++)
                {
                    MemoryGameCard card = cards.GetRandomElement();
                    if (card)
                    {
                        card.SetMemoryType(memoryType);
                        cards.Remove(card);
                    }
                    else
                    {
                        Debug.LogError("Ran out of cards");
                    }    
                }

                avaialableMemoryTypes.Remove(memoryType);
            }
        }

        public void SetMemoryTypeToSearchFor(EMemoryType memoryType, EMemoryType allowedMemoryTypes)
        {
            MemoryTypeToSearchFor = memoryType;
            _allowedMemoryTypes = allowedMemoryTypes;
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
            MemoryGameSolverComponent.Instance.SetGameStage(EGameStage.InGameInputPrevented);
            for (int i = 0; i < timesToSwap; i++)
            {
                SwapRandomCards();
                yield return new WaitForSeconds(_timeBetweenSwaps);
            }
            MemoryGameSolverComponent.Instance.SetGameStage(EGameStage.InGame);
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

            GlobalFunctions.LerpObjectToLocation(card1, card1.gameObject, card2StartPos, .75f);
            GlobalFunctions.LerpObjectToLocation(card2,card2.gameObject, card1StartPos, .75f);
        }

        public override void GenerateGame(MemoryGameGeneratorData generationData)
        {
            base.GenerateGame(generationData);
            MemoryGameSolverComponent.Instance.SetBaseNumberOfGuesses(generationData.NumberOfGuesses);
            CreateGrid(generationData.GridSize);
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
