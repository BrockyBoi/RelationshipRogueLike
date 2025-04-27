using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhackAMole
{
    public class WhackAMoleAppearingObject : MonoBehaviour
    {
        public System.Action<bool> OnAppearingObjectDestroyed;

        [SerializeField]
        private bool _isDistraction = false;

        public void HitObject()
        {
            OnAppearingObjectDestroyed?.Invoke(_isDistraction);
        }
    }
}