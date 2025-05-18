using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

using static GlobalFunctions;

namespace Map
{
    public class MapEventUIChoiceObject : MonoBehaviour
    {
        [SerializeField, Required]
        private TextMeshProUGUI _descriptionText;

        private MinorMapEventResultChoice _eventChoice;

        public void SetEventResultChoice(MinorMapEventResultChoice eventChoice)
        {
            _eventChoice = eventChoice;
            _descriptionText.text = _eventChoice.ChoiceDescription;
        }

        public void SelectChoice()
        {
            if (ensure(_eventChoice != null, "Event choice is null"))
            {
                if (_eventChoice.ResultIsToCloseUI)
                {
                    MinorMapEventUI.Instance.SelectCloseUI();
                    return;
                }

                MinorMapEventResult result = _eventChoice.GetRandomResult();
                if (ensure(result != null, "Could not get map event result"))
                {
                    result.ApplyAllModifiers();
                    MinorMapEventUI.Instance.SetChoiceResult(result);
                }
            }
        }
    }
}