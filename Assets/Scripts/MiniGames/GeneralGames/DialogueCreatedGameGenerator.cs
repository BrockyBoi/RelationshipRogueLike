using GeneralGame.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

using static GlobalFunctions;

namespace GeneralGame.Generation
{
    public abstract class DialogueCreatedGameGenerator<GameSolver, GenerationData, CompletionResult> : BaseGameGenerator where GameSolver : GameSolverComponent<GenerationData, CompletionResult> where GenerationData : GameGenerationData<CompletionResult> where CompletionResult : GameCompletionResult, new()
    {
        protected GenerationData _gameData;
        public GenerationData GameData { get { return _gameData; } }

        protected abstract GameSolver GameSolverComponent { get; }

        public void GenerateGame(GenerationData generationData)
        {
            _gameData = generationData;

            GenerateGameAssets();
            InitializeCameraPosition();
            if (ensure(GameSolverComponent != null, "Game Solver is null") && ensure(generationData != null, "Generation data is null"))
            {
                GameSolverComponent.GeneratorInitializeSolver(generationData.GameCompletionResults, _gameData);
            }

            OnGameGenerated?.Invoke();
            OnAnyGameGenerated?.Invoke();
        }

        protected virtual void GenerateGameAssets()
        {

        }

        protected virtual void InitializeCameraPosition()
        {
            Camera.main.transform.position = Vector3.zero.ChangeAxis(ExtensionMethods.EVectorAxis.Z, -30);
            Camera.main.orthographicSize = 5f;
        }
    }
}
