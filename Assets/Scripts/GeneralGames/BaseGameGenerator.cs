using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralGame.Generation
{
    public abstract class BaseGameGenerator : MonoBehaviour
    {
        protected bool _hasGeneratedGame = false;

        private System.Action OnGameGenerated;
        public static System.Action OnAnyGameGenerated;

        public void ListenToOnGameGenerated(System.Action action)
        {
            if (!_hasGeneratedGame)
            {
                OnGameGenerated += action;
            }
            else
            {
                action?.Invoke();
            }
        }

        public void UnlistenToOnGameGenerated(System.Action action)
        {
            OnGameGenerated -= action;
        }

        protected virtual void GameGenerated()
        {
            OnGameGenerated?.Invoke();
            OnAnyGameGenerated?.Invoke();
        }
    }
}
