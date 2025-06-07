using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireFighting
{
    public class FireFightingWindow : GridObject
    {
        [SerializeField, Required]
        private SpriteRenderer _windowSpriteRenderer;

        [SerializeField, Required]
        private SpriteRenderer _fireSpriteRenderer;

        private float _fireLevel = 0;

        private bool _firePutOut = false;
        public bool FirePutOut { get { return _firePutOut; } }

        public void IncreaseFireLevel(float increase)
        {
            if (!FirePutOut)
            {
                SetFireLevel(_fireLevel + increase);
            }
        }

        public void DecreaseFireLevel(float decrease)
        {
            if (!FirePutOut)
            {
                SetFireLevel(_fireLevel - decrease);
            }
        }

        private void SetFireLevel(float level)
        {
            _fireLevel = Mathf.Clamp(level, 0, 100);
            _fireSpriteRenderer.color = _fireSpriteRenderer.color.ChangeColorAxis(ExtensionMethods.EColorAxis.A, _fireLevel / 100);

            if (Mathf.Approximately(_fireLevel, 0))
            {
                _firePutOut = true;
            }
        }
    }
}
