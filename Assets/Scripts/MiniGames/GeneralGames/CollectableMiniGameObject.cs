using GeneralGame;
using GeneralGame.Generation;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

public abstract class CollectableMiniGameObject<GameSolverComponent, GameGeneratorComponent> : MiniGameGameObject<GameSolverComponent, GameGeneratorComponent> where GameSolverComponent : BaseGameSolverComponent where GameGeneratorComponent : BaseGameGenerator
{
    protected float _minX, _maxX, _minY, _maxY;

    protected bool _wasCollected = false;

    protected virtual void Awake()
    {
        _minX = Camera.main.ViewportToWorldPoint(new Vector3(-.3f, 0)).x;
        _maxX = Camera.main.ViewportToWorldPoint(new Vector3(1.3f, 0)).x;

        _minY = Camera.main.ViewportToWorldPoint(new Vector3(0, -.3f)).y;
        _maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.3f)).y;
    }

    protected virtual void Update()
    {
        if (ensure(_gameSolver != null, "Game solver is not set") && _gameSolver.CanPlayGame())
        {
            MoveObject();

            if (IsOutOfBounds())
            {
                OnItemLeaveBounds();
            }
        }
    }

    protected abstract void MoveObject();
    public abstract void SpawnInRandomLocation();

    protected override void OnGameStop()
    {
        Destroy(gameObject);
    }

    protected virtual void OnItemLeaveBounds()
    {
        Destroy(gameObject);
    }

    public void CollectItem()
    {
        if (!_wasCollected)
        {
            _wasCollected = true;
            OnItemCollected();
        }
    }

    protected virtual void OnItemCollected()
    {
        Destroy(gameObject);
    }

    protected bool IsOutOfBounds()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        return x < _minX || x > _maxX || y < _minY || y > _maxY;
    }
}
