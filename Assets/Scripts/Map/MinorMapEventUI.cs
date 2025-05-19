using CustomUI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static GlobalFunctions;

namespace Map
{
    public class MinorMapEventUI : BaseGameUI
    {
        public static MinorMapEventUI Instance { get; private set; }

        [SerializeField]
        private TextMeshProUGUI _descriptionText;

        [SerializeField, Required]
        private VerticalLayoutGroup _verticalLayoutGroup;

        [SerializeField, Required]
        private MapEventUIChoiceObject _choiceUIPrefab;

        [SerializeField]
        private MinorMapEventResultChoice _closeUIChoice;

        private List<MapEventUIChoiceObject> _choices = new List<MapEventUIChoiceObject>();

        private MinorMapEvent _currentMapEvent;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            HideUI();
        }

        private void SetDescriptionText(string descriptionText)
        {
            _descriptionText.text = descriptionText;
        }

        public void SetChoiceResult(MinorMapEventResult result)
        {
            DestroyChoiceObjects();

            if (ensure(result != null, "No result given"))
            {
                SetDescriptionText(result.OutcomeDescription);
                SpawnChoiceObject(_closeUIChoice);
            }
        }

        public void SetCurrentMapEvent(MinorMapEvent mapEvent)
        {
            ShowUI();

            _currentMapEvent = mapEvent;

            SetDescriptionText(mapEvent.EventDescriptionText);

            foreach (MinorMapEventResultChoice choice in mapEvent.MinorMapEventResultChoices)
            {
                SpawnChoiceObject(choice);
            }
        }

        public void SelectCloseUI()
        {
            DestroyChoiceObjects();
            HideUI();
        }

        private void DestroyChoiceObjects()
        {
            for (int i = _choices.Count - 1; i >= 0; i--)
            {
                Destroy(_choices[i].gameObject);
            }

            _choices.Clear();
        }

        private void SpawnChoiceObject(MinorMapEventResultChoice choice)
        {
            MapEventUIChoiceObject choiceUI = Instantiate(_choiceUIPrefab, _verticalLayoutGroup.transform);
            choiceUI.SetEventResultChoice(choice);
            _choices.Add(choiceUI);
        }
    }
}
