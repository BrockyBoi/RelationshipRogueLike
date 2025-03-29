using GeneralGame.Results;
using Maze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameDifficultyManager<CompletionGameResult> : MonoBehaviour where CompletionGameResult : GameResult
{
    public abstract void ProvideDifficultyModifierResult(CompletionGameResult result);
}
