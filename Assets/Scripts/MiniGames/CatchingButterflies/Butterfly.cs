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
    public class Butterfly : CollectableMiniGameObject<CatchingButterfliesSolver, CatchingButterfliesGenerator>
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

        protected override void Start()
        {
            base.Start();

            _waveAmplitude = Random.Range(1 / _potentialWaveAmplitudeVariance, 1 * _potentialWaveAmplitudeVariance);
            _randomDirectionMultiplier = Random.value > .5f ? 1 : -1;
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        public override void SpawnInRandomLocation()
        {
            transform.position = new Vector3(Random.Range(_minX * .95f, _maxX * .35f), Random.value > .5f ? _minY * .95f : _maxY * .95f, 0);
        }

        private void PickRandomDirectionToMove()
        {
            Vector3 randomPointInWorld = GlobalFunctions.GetRandomWorldPosOnScreen(.25f, .45f, .5f, .5f);
            _directionToMove = (randomPointInWorld - transform.position).ChangeAxis(ExtensionMethods.EVectorAxis.Z, 0);
            _directionToMove.Normalize();
        }

        public void Initialize(Color color)
        {
            _moveSpeed = _moveSpeed * Random.Range(_moveSpeed / _potentialMoveSpeedVariance, _moveSpeed * _potentialMoveSpeedVariance);

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
            SpawnInRandomLocation();
            PickRandomDirectionToMove();
        }

        protected override void OnItemCollected()
        {
            CatchingButterfliesSolver.Instance.CollectObject();

            base.OnItemCollected();
        }

        protected override void MoveObject()
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

            float speed = Time.deltaTime * _moveSpeed * _gameSolver.GameData.ButterflySpeedMultiplier;
            transform.position = Vector3.MoveTowards(transform.position, transform.position + _directionToMove + extraMovement, speed).ChangeAxis(ExtensionMethods.EVectorAxis.Z, 0);
        }
    }
}
