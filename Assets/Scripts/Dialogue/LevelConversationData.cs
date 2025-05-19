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
    [Title("Conversations")]
    public Conversation ConversationToRun;
    public Conversation ConversationOnPlayerDeath;

    [Title("Background")]
    public Sprite BackgroundImage;
}
