using GeneralGame;
using Maze;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemoryGame
{
    public class MemoryGameDifficultyManager : GameDifficultyManager<MemoryGameDifficultyModifierResult>
    {
        public static MemoryGameDifficultyManager Instance { get; private set; }

        [Title("Max Values")]
        [SerializeField]
        private int _maxSizeModifier = 6;

        [SerializeField]
        private int _maxBombsInGame = 5;

        [SerializeField]
        private int _maxCardsToSawp = 6;

        [SerializeField]
        private int _minNumberOfGuessesModifier = -3;

        [SerializeField]
        private int _maxNumberOfGuessesModifier = 4;

        [ShowInInspector, ReadOnly]
        public int BombsInGame {  get; private set; }

        [ShowInInspector, ReadOnly]
        public int CardsToSwap { get; private set; }

        [ShowInInspector, ReadOnly]
        public int GridSizeModifier { get; private set; }

        [ShowInInspector, ReadOnly]
        public int NumberOfGuessesModifier { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {

        }

        public override void ProvideDifficultyModifierResult(MemoryGameDifficultyModifierResult result)
        {
            ChangeGridSizeModifierValue(result.GridSizeModifier);
            ChangeNumberOfBombsInGame(result.BombsInGame);
            ChangeNumberOfCardsToSwap(result.NumberOfCardsToSwap);
            ChangeNumberOfGuessesModifier(result.NumberOfGuesses);
        }

        private void ChangeGridSizeModifierValue(int mazeSizeModifierValue)
        {
            GridSizeModifier = Mathf.Clamp(GridSizeModifier + mazeSizeModifierValue, 0, _maxSizeModifier);
        }

        private void ChangeNumberOfBombsInGame(int bombs)
        {
            BombsInGame = Mathf.Clamp(BombsInGame + bombs, 0, _maxBombsInGame);
        }

        private void ChangeNumberOfCardsToSwap(int cardsToSwap)
        {
            CardsToSwap = Mathf.Clamp(CardsToSwap + cardsToSwap, 0, _maxCardsToSawp);
        }

        private void ChangeNumberOfGuessesModifier(int guesses)
        {
            NumberOfGuessesModifier = Mathf.Clamp(NumberOfGuessesModifier + guesses, _minNumberOfGuessesModifier, _maxNumberOfGuessesModifier);
        }
    }
}
