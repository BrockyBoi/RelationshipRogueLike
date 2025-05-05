using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MinorMapEventData", order = 1)]
public class MinorMapEvent : SerializedScriptableObject
{
    [SerializeField]
    private string _eventDescriptionText;


}
