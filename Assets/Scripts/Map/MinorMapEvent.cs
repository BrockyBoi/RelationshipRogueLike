using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MinorMapEventData", order = 1)]
    public class MinorMapEvent : SerializedScriptableObject
    {
        [TextArea]
        public string EventDescriptionText;

        public List<MinorMapEventResultChoice> MinorMapEventResultChoices;
    }
}
