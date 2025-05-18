using MainPlayer;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze.Gameplay
{
    public enum EMazePickupType
    {
        ExtraTime,
        ExtraHealth,
        GeneralKey,
        NumberedKey
    }

    public abstract class MazePickupObject : MonoBehaviour
    {
        [SerializeField]
        private EMazePickupType _pickupType;

        [SerializeField, Required]
        private Collider _collider;

        [SerializeField, Required]
        private SpriteRenderer _spriteRenderer;

        protected void OnTriggerEnter(Collider other)
        {
            OnPickup();
            Destroy(gameObject);
        }

        public void ActivateObject()
        {
            _collider.enabled = true;
            _spriteRenderer.enabled = true;
        }

        public void DeactivateObject()
        {
            _collider.enabled = false;
            _spriteRenderer.enabled = false;
        }

        protected abstract void OnPickup();
    }
}
