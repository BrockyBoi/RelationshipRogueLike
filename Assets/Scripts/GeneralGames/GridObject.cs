using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    public Vector2Int PositionInMaze { get; private set; }

    public virtual void SetPositionInMaze(Vector2Int positionInMaze)
    {
        PositionInMaze = positionInMaze;
    }
}
