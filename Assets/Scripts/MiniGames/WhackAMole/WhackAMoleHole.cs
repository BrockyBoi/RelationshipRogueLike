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

        private bool _isHighlighted = false;
        public bool IsHighlighted { get { return _isHighlighted; } }
        public bool HasObjectInHole { get { return _objectInHole; } }
        public void SetObjectInHole(WhackAMoleAppearingObject appearingObject)
        {
            _objectInHole = appearingObject;
            _objectInHole.transform.SetParent(transform);
        }

        public void Highlight()
        {
            _isHighlighted = true;
            _spriteRenderer.color = Color.white;
        }

        public void StopHighlighting()
        {
            _isHighlighted = false;
            _spriteRenderer.color = Color.black;
        }

        public void ObjectLeftHole()
        {
            _objectInHole = null;
        }

        private void OnMouseDown()
        {
            if (WhackAMoleSolver.Instance.CanPlayGame())
            {
                PlayerHitHole();
            }
        }

        public void PlayerHitHole()
        {
            StartCoroutine(ShowHoleHit());

            if (_objectInHole)
            {
                _objectInHole.HitObject();
            }
        }

        private IEnumerator ShowHoleHit()
        {
            if (_objectInHole)
            {
                _spriteRenderer.color = _objectInHole.IsDistraction ? Color.red : Color.green;
            }
            else
            {
                _spriteRenderer.color = Color.white;
            }

            yield return new WaitForSeconds(.1f);

            _spriteRenderer.color = _isHighlighted ? Color.white : Color.black;
        }
    }
}
