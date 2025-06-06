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

        public virtual void GenerateGame(GenerationData generationData)
        {
            if (ensure(GameSolverComponent != null, "Game Solver is null") && ensure(generationData != null, "Generation data is null"))
            {
                GameSolverComponent.SetGameCompletionResults(generationData.GameCompletionResults);
                GameGenerated();
                SetGameGenerationData(generationData);
            }
        }

        protected void SetGameGenerationData(GenerationData data)
        {
            _gameData = data;
            GameSolverComponent.SetGenerationGameData(data);
        }
    }
}
