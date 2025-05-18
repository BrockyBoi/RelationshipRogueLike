using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    public Vector2Int PositionInGrid { get; private set; }

    public virtual void SetPositionInGrid(Vector2Int positionInGrid)
    {
        PositionInGrid = positionInGrid;
    }
}
