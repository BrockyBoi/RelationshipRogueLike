using GeneralGame.Results;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace GeneralGame.Generation
{
    public abstract class BaseGameGenerationData
    {

    }

    [Serializable]
    public abstract class GameGenerationData<GameResult> : BaseGameGenerationData where GameResult : GameCompletionResult, new()
    {
        [InfoBox("There must be 2 or more results at all times", InfoMessageType.Error, "@!HasEnoughResults")]
        public List<GameResult> GameCompletionResults = new List<GameResult>() { new GameResult(), new GameResult() };

        private bool HasEnoughResults { get { return GameCompletionResults != null && GameCompletionResults.Count >= 2; } }
    }
}
