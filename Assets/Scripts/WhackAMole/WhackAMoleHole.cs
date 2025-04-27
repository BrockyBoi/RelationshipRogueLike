using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    public class WhackAMoleHole : MonoBehaviour
    {
        private WhackAMoleAppearingObject _objectInHole;

        [SerializeField, Required]
        private SpriteRenderer _spriteRenderer;
        public void SetObjectInHole(WhackAMoleAppearingObject appearingObject)
        {
            _objectInHole = appearingObject;
        }

        public void Highlight()
        {
            _spriteRenderer.color = Color.white;
        }

        public void StopHighlighting()
        {
            _spriteRenderer.color = Color.black;
        }

        public void ObjectLeftHole()
        {
            if (_objectInHole)
            {
                Destroy(_objectInHole.gameObject);
            }
        }

        public void PlayerHitHole()
        {
            if (_objectInHole)
            {
                _objectInHole.HitObject();

                Destroy(_objectInHole.gameObject); 
            }
        }
    }
}
