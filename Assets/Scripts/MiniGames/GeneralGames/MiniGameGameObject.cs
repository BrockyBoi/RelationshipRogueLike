using GeneralGame;
using GeneralGame.Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

public abstract class MiniGameGameObject<GameSolverComponent, GameGeneratorComponent> : MonoBehaviour where GameSolverComponent : BaseGameSolverComponent where GameGeneratorComponent : BaseGameGenerator
{
    protected GameSolverComponent _gameSolver;
    protected GameGeneratorComponent _gameGenerator;

    [SerializeField]
    protected EGameType _gameType;

    [SerializeField]
    private bool _disableOnInitialize = false;

    protected virtual void Start()
    {
        BaseGameGenerator generator;
        BaseGameSolverComponent solver;
        MiniGameControllersManager.Instance.GetBothControllers(out solver, out generator, _gameType);
        if (ensure(solver != null && generator != null, "Either solver or generator class is null"))
        {
            if (ensure(solver is GameSolverComponent, "Cannot get " + typeof(GameSolverComponent) + " from " + gameObject.name + " with game type " + _gameType))
            {
                _gameSolver = (GameSolverComponent)solver;
                _gameSolver.OnGameStop += OnGameStop;
                _gameSolver.OnStageChange += OnGameStateChanged;
            }

            if (ensure(generator is GameGeneratorComponent, "Cannot get " + typeof(GameGeneratorComponent) + " from " + gameObject.name + " with game type " + _gameType))
            {
                _gameGenerator = (GameGeneratorComponent)generator;
            }
        }

        if (_disableOnInitialize)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (_gameSolver != null)
        {
            _gameSolver.OnGameStop -= OnGameStop;
            _gameSolver.OnStageChange -= OnGameStateChanged;
        }
    }

    protected virtual void OnGameStateChanged(EGameStage stage)
    {
        switch(stage)
        {
            case EGameStage.PreCountdown:
                OnObjectEnabled(); 
                break;
            default:
                break;
        }
    }

    protected virtual void OnObjectEnabled()
    {
        gameObject.SetActive(true);
    }

    protected virtual void OnGameStop()
    {
        gameObject.SetActive(false);
    }
}
