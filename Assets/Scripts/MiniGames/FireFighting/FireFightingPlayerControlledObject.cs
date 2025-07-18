using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireFighting
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FireFightingPlayerControlledObject : MiniGameGameObject<FireFightingSolver, FireFightingGenerator>
    {
        private Camera _camera;

        [SerializeField]
        private float _moveSpeed = 10;

        private float _currentMoveSpeedSlowdownMultiplier = 1f;

        [SerializeField]
        private float _moveSpeedSlowdownAtMaxModifier = .5f;

        [SerializeField, Range(1, 1000)]
        private float _maxWaterLevel = 10;
        private float _currentWaterLevel = 1;

        [SerializeField, Range(1, 5)]
        private float _scaleMultiplier = 2.5f;

        [SerializeField]
        private float _objectHeight = .5f;

        Vector3 _startingScale = Vector3.one;

        protected override void Start()
        {
            _camera = Camera.main;
            _startingScale = transform.localScale;

            base.Start();
        }

        private void Update()
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            float distanceToMove = _moveSpeed * Time.deltaTime * _currentMoveSpeedSlowdownMultiplier;
            Vector3 nextPos = Vector3.MoveTowards(transform.position, mousePos.ChangeAxis(ExtensionMethods.EVectorAxis.Z, _objectHeight), distanceToMove);

            transform.position = nextPos;

            float waterPressureChange = Input.GetAxis("Vertical");
            if (waterPressureChange != 0)
            {
                ChangeWaterPressure(waterPressureChange * .5f);
            }
        }

        private void ChangeWaterPressure(float pressureChange)
        {
            _currentWaterLevel = Mathf.Clamp(_currentWaterLevel + pressureChange, 1f, _maxWaterLevel);
            _currentMoveSpeedSlowdownMultiplier = Mathf.Lerp(_moveSpeedSlowdownAtMaxModifier, 1f,  1 - (_currentWaterLevel / _maxWaterLevel));
            transform.localScale = _startingScale * (1 + (1 - _currentMoveSpeedSlowdownMultiplier) * _scaleMultiplier);
        }

        protected override void OnObjectEnabled()
        {
            base.OnObjectEnabled();
            transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            FireFightingWindow window = collision.gameObject.GetComponent<FireFightingWindow>();
            if (window != null)
            {
                window.DecreaseFireLevel(_currentWaterLevel * Time.deltaTime);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            FireFightingWindow window = collision.gameObject.GetComponent<FireFightingWindow>();
            if (window != null)
            {
                window.DecreaseFireLevel(_currentWaterLevel * Time.deltaTime);
            }
        }
    }
}