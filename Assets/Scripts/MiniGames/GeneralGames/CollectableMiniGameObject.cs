using GeneralGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollectableMiniGameObject<GameSolverComponent> : MonoBehaviour where GameSolverComponent : BaseGameSolverComponent
{
    protected float _minX, _maxX, _minY, _maxY;

    protected abstract GameSolverComponent _gameSolver { get; }

    protected virtual void Start()
    {
        _gameSolver.OnGameStop += OnGameEnd;
        _minX = Camera.main.ViewportToWorldPoint(new Vector3(-.3f, 0)).x;
        _maxX = Camera.main.ViewportToWorldPoint(new Vector3(1.3f, 0)).x;

        _minY = Camera.main.ViewportToWorldPoint(new Vector3(0, -.3f)).y;
        _maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.3f)).y;
    }

    protected virtual void OnDestroy()
    {
        _gameSolver.OnGameStop -= OnGameEnd;
    }

    protected virtual void Update()
    {
        if (_gameSolver.CanPlayGame())
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

    protected virtual void OnGameEnd()
    {
        Destroy(gameObject);
    }

    protected virtual void OnItemLeaveBounds()
    {
        Destroy(gameObject);
    }

    public virtual void CollectItem()
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
