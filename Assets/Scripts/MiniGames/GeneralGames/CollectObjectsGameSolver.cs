using GeneralGame;
using GeneralGame.Generation;
using GeneralGame.Results;

public abstract class CollectObjectsGameSolver<GenerationData, CompletionResultType> : GameSolverComponent<GenerationData, CompletionResultType> where GenerationData : GameGenerationData<CompletionResultType> where CompletionResultType : GameCompletionResult, new()
{
    protected int _collectablesCaught = 0;

    public abstract int CollectablesNeeded { get; }
    public System.Action<int> OnCollectableCountChange;

    protected override void Start()
    {
        base.Start();

        _collectablesCaught = 0;

        OnMainTimerEnd += FailGame;
    }

    protected virtual void OnDestroy()
    {
        OnMainTimerEnd -= FailGame;
    }

    protected override void StartGame()
    {
        base.StartGame();

        _collectablesCaught = 0;
    }

    public void CollectObject()
    {
        _collectablesCaught++;

        OnCollectableCountChange?.Invoke(_collectablesCaught);

        if (_collectablesCaught >= CollectablesNeeded)
        {
            CompletedGame();
        }
    }

    public override void SetGenerationGameData(GenerationData generationData)
    {
        base.SetGenerationGameData(generationData);

    }

    public override int GetCurrentPotentialDialogueIndex()
    {
        return GetGameCompletionResultIndexByPointsNeededToScore(_collectablesCaught, CollectablesNeeded);
    }

    public override float GetCurrentPotentialDialoguePercentage()
    {
        return GetCurrentPotentialDialoguePercentageByPointsNeededToScore(_collectablesCaught, CollectablesNeeded);
    }
}
