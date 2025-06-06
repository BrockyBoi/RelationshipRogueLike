using UnityEngine;

namespace CustomUI
{
    public abstract class BaseGameUI : MonoBehaviour
    {
        [SerializeField]
        protected Canvas _uiCanvas;

        public System.Action OnShowUI;
        public System.Action OnHideUI;
        protected void ShowUI()
        {
            _uiCanvas.enabled = true;
            OnShowUI?.Invoke();
        }

        protected void HideUI()
        {
            _uiCanvas.enabled = false;
            OnHideUI?.Invoke();
        }
    }
}
