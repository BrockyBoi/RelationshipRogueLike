using Dialogue;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static GlobalFunctions;

namespace Map
{
    public enum EMapObjectType
    {
        MinorEvent,
        StartNewScene
    }

    public class SelectableMapObject : MonoBehaviour
    {
        [SerializeField]
        private EMapObjectType _mapObjectType;

        [SerializeField, ShowIf("@_mapObjectType", EMapObjectType.MinorEvent)]
        private MinorMapEvent _minorMapEvent;

        [SerializeField, ShowIf("@_mapObjectType", EMapObjectType.StartNewScene)]
        private LevelConversationData _levelConversationData;

        [SerializeField, Required]
        private Button _button;

        private void Start()
        {
            if (ensure(_button != null, "Button has not been set on SelectableMapObject"))
            {
                _button.onClick.AddListener(SelectMapObject);
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(SelectMapObject);
        }

        public void SelectMapObject()
        {
            MapSceneManager.Instance.SelectMapObject(this);
            switch (_mapObjectType)
            {
                case EMapObjectType.MinorEvent:
                    {
                        MinorMapEventUI.Instance.SetCurrentMapEvent(_minorMapEvent);
                        break;
                    }
                case EMapObjectType.StartNewScene:
                    {
                        ConversationManager.Instance.SetConversationsForLevel(_levelConversationData);
                        GameSceneManager.Instance.LoadGameLevel();
                        break;
                    }
            }

            _button.gameObject.SetActive(false);
        }
    }

}
