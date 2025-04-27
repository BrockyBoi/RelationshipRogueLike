using GeneralGame.Results;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace GeneralGame.Generation
{
    public abstract class DialogueCreatedGameGenerator<GameSolver, GenerationData, CompletionResult> : BaseGameGenerator where GameSolver : GameSolverComponent<GenerationData, CompletionResult> where GenerationData : GameGenerationData<CompletionResult> where CompletionResult : GameCompletionResult, new()
    {
        protected GenerationData _gameData;
        public GenerationData GameData { get { return _gameData; } }

        protected abstract GameSolver GameSolverComponent { get; }

        public virtual void GenerateGame(GenerationData generationData)
        {
            GameSolverComponent.SetGameCompletionResults(generationData.GameCompletionResults);
        }
        protected void SetGameGenerationData(GenerationData data)
        {
            _gameData = data;
            GameSolverComponent.SetGenerationGameData(data);
        }
    }
}
