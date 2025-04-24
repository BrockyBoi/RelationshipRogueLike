using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze.Gameplay
{
    public class KeyPickupObject : MazePickupObject
    {
        protected override void OnPickup()
        {
            MazeSolverComponent.Instance.OnKeyCollected();
        }
    }
}
