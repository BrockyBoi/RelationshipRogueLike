using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class EndlessRunnerCollectable : CollectableMiniGameObject<EndlessRunnerSolver, EndlessRunnerGenerator>
    {
        public override void SpawnInRandomLocation()
        {
            transform.position = new Vector3(_maxX * .95f, Random.Range( _minY * .5f , _maxY * .9f), 0);
        }

        protected override void MoveObject()
        {
            float speed = Time.deltaTime * _gameSolver.GameData.ObjectMoveSpeed;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + Vector3.left * speed, speed);

        }
    }
}
