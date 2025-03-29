using GeneralGame;
using GeneralGame.Generation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame.Generation
{
    public class MemoryGameGenerator : GameGridGenerator<MemoryGameGeneratorData, MemoryGameCompletionResult, MemoryGameCard>
    {
        public static MemoryGameGenerator Instance {  get; private set; }

        [SerializeField]
        float _timeBetweenSwaps = .75f;

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
            // Maximum amount of types allowed
            // Example: If 16 cards, then we want 8 pairs.  Add 1 extra later for bombs
            int maxMemoryTypes = _objectGrid.Length / 2;
            int bombCount = 0;
            Dictionary<EMemoryType, int> usedTypes = new Dictionary<EMemoryType, int>();
            Func<Dictionary<EMemoryType, int>, bool, EMemoryType> getRandomMemoryTypeValue = (types, canBeBomb) =>
            {
                int possibleMemoryTypeCount = maxMemoryTypes + (canBeBomb ? 1 : 0);
                EMemoryType memoryType;
                bool isBomb;
                int increments = 0;
                do
                {
                    memoryType = GlobalFunctions.RandomEnumValue<EMemoryType>();
                    isBomb = memoryType == EMemoryType.Bomb;

                    if (++increments >= 100)
                    {
                        Debug.LogError("Spent too much time in SetAllCardValues");
                        break;
                    }
                }
                while ((!canBeBomb && isBomb) || (types.ContainsKey(memoryType) && types[memoryType] == 2) || (!types.ContainsKey(memoryType) && types.Count >= possibleMemoryTypeCount));

                if (!types.ContainsKey(memoryType))
                {
                    types.Add(memoryType, 0);
                }
                types[memoryType]++;
                return memoryType;
            };

            foreach (MemoryGameCard card in _objectGrid)
            {
                EMemoryType memoryType = getRandomMemoryTypeValue(usedTypes, bombCount < MemoryGameDifficultyManager.Instance.BombsInGame);
                if (memoryType == EMemoryType.Bomb)
                {
                    bombCount++;
                }

                card.SetMemoryType(memoryType);
                card.HideCard();
            }
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
            if (cardsToSwap > 0)
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
            CreateGrid(generationData.GridSize, generationData.MemoryGameCompletionResults);
        }
    }
}
