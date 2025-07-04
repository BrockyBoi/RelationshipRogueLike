using GeneralGame.Results;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    [Serializable]
    public abstract class BaseGameGenerationData
    {
        protected virtual string FoldoutGroupName { get { return "Game Data"; } }
    }

    [Serializable]
    public abstract class GameGenerationData<GameResult> : BaseGameGenerationData where GameResult : GameCompletionResult, new()
    {
        [InfoBox("There must be 2 or more results at all times", InfoMessageType.Error, "@!HasEnoughResults")]
        public List<GameResult> GameCompletionResults = new List<GameResult>() { new GameResult(), new GameResult() };

        [FoldoutGroup("@FoldoutGroupName"), Range(0, 120)]
        public float GameDuration = 15;

        private bool HasEnoughResults { get { return GameCompletionResults != null && GameCompletionResults.Count >= 2; } }
    }

    [Serializable]
    public abstract class GridGameGenerationData<GameResult> : GameGenerationData<GameResult> where GameResult : GameCompletionResult, new ()
    {
        [FoldoutGroup("@FoldoutGroupName")]
        public Vector2Int GridSize = new Vector2Int(4, 4);
    }
}
