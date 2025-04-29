using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    public class WhackAMoleAppearingObject : MonoBehaviour
    {
        public System.Action<WhackAMoleAppearingObject> OnAppearingObjectDestroyed;
        public System.Action<WhackAMoleAppearingObject> OnAppearingObjectFailedToBeDestroyed;

        private WhackAMoleHole _holeExisitngIn;

        [SerializeField]
        private bool _isDistraction = false;

        public bool IsDistraction { get { return _isDistraction; } }

        public void SetHoleExistingIn(WhackAMoleHole holeExisitngIn)
        {
            _holeExisitngIn = holeExisitngIn;
            StartCoroutine(RunDisappearTimer());
        }

        public void HitObject()
        {
            OnAppearingObjectDestroyed?.Invoke(this);

            DestroyAppearingObject();
        }

        private void DestroyAppearingObject()
        {
            if (_holeExisitngIn)
            {
                _holeExisitngIn.ObjectLeftHole();
            }

            StopAllCoroutines();
            Destroy(gameObject);
        }

        private IEnumerator RunDisappearTimer()
        {
            yield return new WaitForSeconds(WhackAMoleGenerator.Instance.GameData.TimeObjectsStayInPlay);

            OnAppearingObjectFailedToBeDestroyed?.Invoke(this);
            DestroyAppearingObject();
        }
    }
}