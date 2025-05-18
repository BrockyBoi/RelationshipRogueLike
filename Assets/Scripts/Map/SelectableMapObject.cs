using Dialogue;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public void SelectMapObject()
        {
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
                        break;
                    }
            }
        }
    }

}
