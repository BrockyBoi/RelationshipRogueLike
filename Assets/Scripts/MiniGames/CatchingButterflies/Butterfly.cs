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
        Loop,
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

        [SerializeField, Title("Variances")]
        private float _potentialMoveSpeedVariance = 1.25f;

        [SerializeField]
        private float _potentialWaveAmplitudeVariance = 1.25f;

        private float _waveAmplitude = 0;

        private float _randomDirectionMultiplier = 1;

        private float minX, maxX, minY, maxY;


        private void Start()
        {
            CatchingButterfliesSolver.Instance.OnGameStop += OnGameEnd;
            _waveAmplitude = Random.Range(1 / _potentialWaveAmplitudeVariance, 1 * _potentialWaveAmplitudeVariance);
            _randomDirectionMultiplier = Random.value > .5f ? 1 : -1;
        }

        private void OnDestroy()
        {
            CatchingButterfliesSolver.Instance.OnGameStop -= OnGameEnd;
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        private void PickRandomDirectionToMove()
        {
            transform.position = new Vector3(Random.Range(minX * .95f, maxX * .35f), Random.value > .5f ? minY * .95f : maxY * .95f, 0);
            Vector3 randomPointInWorld = GlobalFunctions.GetRandomWorldPosOnScreen(.25f, .45f, .5f, .5f);
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
            if (randomValue > .75f)
            {
                _movementType = EButterflyMovementType.Loop;
            }
            else if (randomValue > .5f)
            {
                _movementType = EButterflyMovementType.Cos;
            }
            else if (randomValue > .25f)
            {
                _movementType = EButterflyMovementType.Sin;
            }
            else
            {
                _movementType = EButterflyMovementType.Straight;
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
                        extraMovement = new Vector3(0, Mathf.Sin(Time.time * _randomDirectionMultiplier) * _waveAmplitude, 0);
                        break;
                    case EButterflyMovementType.Cos:
                        extraMovement = new Vector3(Mathf.Cos(Time.time * _randomDirectionMultiplier) * _waveAmplitude, 0, 0f);
                        break;
                    case EButterflyMovementType.Loop:
                        extraMovement = new Vector3(Mathf.Cos(-Time.time * _randomDirectionMultiplier) * _waveAmplitude, Mathf.Sin(Time.time * _randomDirectionMultiplier) * _potentialWaveAmplitudeVariance, 0f);
                        break;
                    case EButterflyMovementType.FleeMouse:
                        break;
                    default:
                        extraMovement = Vector3.zero;
                        break;
                }

                float speed = Time.deltaTime * _moveSpeed * CatchingButterfliesSolver.Instance.GameData.ButterflySpeedMultiplier;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + _directionToMove + extraMovement, speed).ChangeAxis(ExtensionMethods.VectorAxis.Z, 0);

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

        private void OnGameEnd()
        {
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
