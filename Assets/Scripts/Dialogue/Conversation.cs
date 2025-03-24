using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ConversationObject", order = 1)]
    public class Conversation : SerializedScriptableObject
    {
        public List<DialogueObject> DialogueObjects;
    }
}
