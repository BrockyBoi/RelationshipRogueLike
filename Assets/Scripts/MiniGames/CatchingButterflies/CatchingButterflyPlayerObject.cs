using Maze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatchingButterflies
{
    public class CatchingButterflyPlayerObject : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 5f;

        [SerializeField]
        private float _objectHeight = 0f;

        private Camera _camera;

        void Start()
        {
            _camera = Camera.main;

            CatchingButterfliesGenerator.Instance.ListenToOnGameGenerated(OnGameCreated);
            CatchingButterfliesSolver.Instance.OnGameStop += OnGameEnd;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            CatchingButterfliesGenerator.Instance.UnlistenToOnGameGenerated(OnGameCreated);
            CatchingButterfliesSolver.Instance.OnGameCompleted -= OnGameEnd;
        }

        void Update()
        {
            if (CatchingButterfliesSolver.Instance.CanPlayGame())
            {
                Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 currentLoc = transform.position;
                float distanceToMove = _moveSpeed * Time.deltaTime;
                Vector3 nextPos = Vector3.MoveTowards(currentLoc, mousePos.ChangeAxis(ExtensionMethods.VectorAxis.Y, _objectHeight), distanceToMove);

                transform.position = nextPos;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Butterfly butterfly = collision.gameObject.GetComponent<Butterfly>();
            if (butterfly)
            {
                butterfly.CaptureButterfly();
            }
        }

        private void OnGameCreated()
        {
            gameObject.SetActive(true);
            transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnGameEnd()
        {

        }
    }
}
