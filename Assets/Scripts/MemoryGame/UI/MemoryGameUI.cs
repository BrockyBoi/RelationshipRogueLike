using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using MemoryGame;
using CustomUI;

namespace MemoryGame.UI
{
    public class MemoryGameUI : GameUI
    {
        public static MemoryGameUI Instance { get; private set; }

        [SerializeField]
        TextMeshProUGUI _memorySearchingForText;

        [SerializeField]
        TextMeshProUGUI _guessesLeftText;

        private void Awake()
        {
            Instance = this;
            HideUI();
        }

        private void Start()
        {
            MemoryGameSolverComponent solver = MemoryGameSolverComponent.Instance;
            if (solver != null)
            {
                solver.OnGuessMade += OnGuessMade;
                solver.OnGameStart += OnGameStart;
                solver.OnGameStop += OnGameEnd;
            }
        }

        private void OnDestroy()
        {
            MemoryGameSolverComponent solver = MemoryGameSolverComponent.Instance;
            if (solver != null)
            {
                solver.OnGuessMade -= OnGuessMade;
                solver.OnGameStart -= OnGameStart;
            }
        }

        private void OnGameStart()
        {
            EMemoryType memoryType = MemoryGameSolverComponent.Instance.MemoryTypeToSearchFor;
            bool isSearchingForSingleMemoryType = MemoryGameSolverComponent.Instance.IsLookingForSingleMemoryType;
            _memorySearchingForText.text = isSearchingForSingleMemoryType ? "Memory Searching For: " + memoryType.ToString() : string.Empty;
            OnGuessMade();
            ShowUI();
        }

        private void OnGameEnd()
        {
            HideUI();
        }

        private void OnGuessMade()
        {
            int guessesLeft = MemoryGameSolverComponent.Instance.GuessesLeft;
            _guessesLeftText.text = "Guesses left: " + guessesLeft.ToString();
        }
    }
}
