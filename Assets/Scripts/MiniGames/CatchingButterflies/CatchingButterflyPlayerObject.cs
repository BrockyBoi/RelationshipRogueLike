using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatchingButterflies
{
    public class CatchingButterflyPlayerObject : MiniGameGameObject<CatchingButterfliesSolver, CatchingButterfliesGenerator>
    {
        [SerializeField]
        private float _moveSpeed = 5f;

        [SerializeField]
        private float _objectHeight = 0f;

        private Camera _camera;

        protected override void Start()
        {
            base.Start();

            _camera = Camera.main;

            gameObject.SetActive(false);
        }

        void Update()
        {
            Vector3 mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 currentLoc = transform.position;
            float distanceToMove = _moveSpeed * Time.deltaTime;
            Vector3 nextPos = Vector3.MoveTowards(currentLoc, mousePos.ChangeAxis(ExtensionMethods.EVectorAxis.Z, _objectHeight), distanceToMove);

            transform.position = nextPos;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Butterfly butterfly = collision.gameObject.GetComponent<Butterfly>();
            if (butterfly)
            {
                butterfly.CollectItem();
            }
        }

        protected override void OnObjectEnabled()
        {
            base.OnObjectEnabled();
            transform.position = _camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
