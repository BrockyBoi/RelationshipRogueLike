using GeneralGame.Results;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    public abstract class DialogueCreatedGameGenerator<GenerationData, CompletionResult> : BaseGameGenerator where GenerationData : BaseGameGenerationData where CompletionResult : GameCompletionResult
    {
        public abstract void GenerateGame(GenerationData generationData);
        protected abstract void GiveResultsToSolver(List<CompletionResult> results);
    }
}
