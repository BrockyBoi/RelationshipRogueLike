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
        if (ensure(solver != null && generator != null))
        {
            if (ensure(solver is GameSolverComponent, "Cannot get " + typeof(GameSolverComponent) + " from " + gameObject.name + " with game type " + _gameType))
            {
                _gameSolver = (GameSolverComponent)solver;
                _gameSolver.OnGameStop += OnGameStop;
            }

            if (ensure(generator is GameGeneratorComponent, "Cannot get " + typeof(GameGeneratorComponent) + " from " + gameObject.name + " with game type " + _gameType))
            {
                _gameGenerator = (GameGeneratorComponent)generator;
                _gameGenerator.ListenToOnGameGenerated(OnGameGenerated);
            }
        }

        if (_disableOnInitialize)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (_gameGenerator != null)
        {
            _gameGenerator.UnlistenToOnGameGenerated(OnGameGenerated);
        }

        if (_gameSolver != null)
        {
            _gameSolver.OnGameStop -= OnGameStop;
        }
    }

    protected virtual void OnGameGenerated()
    {
        gameObject.SetActive(true);
    }

    protected virtual void OnGameStop()
    {
        gameObject.SetActive(false);
    }
}
