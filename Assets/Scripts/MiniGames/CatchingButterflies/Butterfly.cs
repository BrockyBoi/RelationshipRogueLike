using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatchingButterflies
{
    public enum EButterflyMovementType
    {
        Straight,
        Sin,
        Cos,
        FleeMouse
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Butterfly : MonoBehaviour
    {
        [SerializeField, Required]
        private SpriteRenderer _spriteRenderer;

        private EButterflyMovementType _movementType;

        private Vector3 _directionToMove;

        [SerializeField]
        private float _moveSpeed = 5;

        [SerializeField]
        private float _potentialMoveSpeedVariance = 1.25f;

        private float minX, maxX, minY, maxY;

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        private void PickRandomDirectionToMove()
        {
            transform.position = new Vector3(Random.Range(minX * .95f, maxX * .95f), Random.value > .5f ? minY * .95f : maxY * .95f, 0);
            Vector3 randomPointInWorld = GlobalFunctions.GetRandomWorldPosOnScreen(.25f, .75f, .5f, .5f);
            _directionToMove = (randomPointInWorld - transform.position).ChangeAxis(ExtensionMethods.VectorAxis.Z, 0);
            _directionToMove.Normalize();
        }

        public void Initialize(Color color)
        {
            _moveSpeed = _moveSpeed * Random.Range(_moveSpeed / _potentialMoveSpeedVariance, _moveSpeed * _potentialMoveSpeedVariance);

            var bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
            var topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 1));


            minX = Camera.main.ViewportToWorldPoint(new Vector3(-.3f, 0)).x;
            maxX = Camera.main.ViewportToWorldPoint(new Vector3(1.3f, 0)).x;

            minY = Camera.main.ViewportToWorldPoint(new Vector3(0, -.3f)).y;
            maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.3f)).y;


            float randomValue = Random.value;
            if (randomValue > .67f)
            {
                _movementType = EButterflyMovementType.Straight;
            }
            else if (randomValue > .33f)
            {
                _movementType = EButterflyMovementType.Cos;
            }
            else
            {
                _movementType = EButterflyMovementType.Sin;
            }

            SetColor(color);
            PickRandomDirectionToMove();
        }

        public void Update()
        {
            if (CatchingButterfliesSolver.Instance.CanPlayGame())
            {
                Vector3 extraMovement = Vector3.zero;
                switch (_movementType)
                {
                    case EButterflyMovementType.Sin:
                        extraMovement = new Vector3(0, Mathf.Sin(Time.time) * .75f, 0);
                        break;
                    case EButterflyMovementType.Cos:
                        extraMovement = new Vector3(Mathf.Cos(Time.time) * .75f, 0, 0f);
                        break;
                    case EButterflyMovementType.FleeMouse:
                        break;
                }

                transform.position = Vector3.MoveTowards(transform.position, transform.position + _directionToMove + extraMovement, Time.deltaTime * _moveSpeed).ChangeAxis(ExtensionMethods.VectorAxis.Z, 0);

                if (IsOutOfBounds())
                {
                    ButterflyEscaped();
                }
            }
        }

        private void ButterflyEscaped()
        {
            Destroy(gameObject);
        }

        public void CaptureButterfly()
        {
            CatchingButterfliesSolver.Instance.CatchButterfly();
            Destroy(gameObject);
        }

        private bool IsOutOfBounds()
        {
            float x = transform.position.x;
            float y = transform.position.y;

            return x < minX || x > maxX || y < minY || y > maxY;
        }
    }
}
