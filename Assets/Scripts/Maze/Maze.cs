using Maze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeObject : MonoBehaviour
    {
        public static MazeObject Instance { get; private set; }
        private MazeNode[,] _maze;

        private void Awake()
        {
            Instance = this;
        }

        public void SetMazeSize(Vector2Int size)
        {
            _maze = new MazeNode[size.x, size.y];
        }

        public void SetMazeNode(MazeNode node, Vector2Int nodeIndex)
        {
            _maze[nodeIndex.x, nodeIndex.y] = node;
        }
    }
}
