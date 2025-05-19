using CustomUI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapEventUI : BaseGameUI
{
    [SerializeField]
    TextMeshProUGUI _eventDescriptionText;


    void Start()
    {
        HideUI();
    }
}
