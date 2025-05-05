using Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        [SerializeField]
        private Conversation _conversationToRun;

        [SerializeField]
        private Conversation _conversationOnPlayerDeath;

        private void OnMouseDown()
        {
            switch (_mapObjectType)
            {
                case EMapObjectType.MinorEvent:
                case EMapObjectType.StartNewScene:
                    {
                        ConversationManager.Instance.SetConversationsForLevel(_conversationToRun, _conversationOnPlayerDeath);
                        break;
                    }
            }
        }
    }

}
