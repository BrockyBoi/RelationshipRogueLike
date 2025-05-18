using Dialogue;
using Map;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelConversationData
{
    [SerializeField]
    public Conversation ConversationToRun;

    [SerializeField]
    public Conversation ConversationOnPlayerDeath;
}
