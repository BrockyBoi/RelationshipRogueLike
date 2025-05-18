using Dialogue;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBackgroundUI : MonoBehaviour
{
    [SerializeField, Required]
    private Image _backgroundImage;

    private void Start()
    {
        _backgroundImage.sprite = ConversationManager.Instance.ConversationData.BackgroundImage;
    }
}
