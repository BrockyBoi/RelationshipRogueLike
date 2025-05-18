using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using MemoryGame;
using CustomUI;
using MemoryGame.Generation;

namespace MemoryGame.UI
{
    public class MemoryGameUI : GameUI<MemoryGameGenerator, MemoryGameSolverComponent>
    {
        public static MemoryGameUI Instance { get; private set; }

        protected override MemoryGameGenerator GameGenerator { get { return MemoryGameGenerator.Instance; } }

        protected override MemoryGameSolverComponent GameSolver { get { return MemoryGameSolverComponent.Instance; } }

        [SerializeField]
        private TextMeshProUGUI _memorySearchingForText;

        [SerializeField]
        private TextMeshProUGUI _guessesLeftText;

        private void Awake()
        {
            Instance = this;
            HideUI();
        }

        protected override void Start()
        {
            base.Start();

            if (GameSolver)
            {
                GameSolver.OnGuessMade += OnGuessMade;
                GameSolver.OnGameStart += OnGameStart;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (GameSolver)
            {
                GameSolver.OnGuessMade -= OnGuessMade;
                GameSolver.OnGameStart -= OnGameStart;
            }
        }

        protected override void OnGameStart()
        {
            base.OnGameStart();

            EMemoryType memoryType = MemoryGameSolverComponent.Instance.MemoryTypeToSearchFor;
            bool isSearchingForSingleMemoryType = MemoryGameSolverComponent.Instance.IsLookingForSingleMemoryType;
            _memorySearchingForText.text = isSearchingForSingleMemoryType ? "Memory Searching For: " + memoryType.ToString() : string.Empty;
            OnGuessMade();
            ShowUI();
        }

        protected override void OnGameEnd()
        {
            base.OnGameEnd();

            HideUI();
        }

        private void OnGuessMade()
        {
            int guessesLeft = MemoryGameSolverComponent.Instance.GuessesLeft;
            _guessesLeftText.text = "Guesses left: " + guessesLeft.ToString();
        }
    }
}
