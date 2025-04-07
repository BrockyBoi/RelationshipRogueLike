using CustomUI;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dialogue.UI
{
    [RequireComponent(typeof(VerticalAlignmentOptions))]
    public class PotentialPlayerDialogueUI : GameUI
    {
        public static PotentialPlayerDialogueUI Instance { get; private set; }

        [SerializeField]
        private Canvas _canvas;

        [SerializeField, AssetsOnly]
        private PotentialPlayerDialogueUIObject _potentialPlayerDialoguePrefab;

        private List<PotentialPlayerDialogueUIObject> _dialogueObjects;

        private int _currentlyHighlightedIndex = 0;

        private void Awake()
        {
            Instance = this;
            _dialogueObjects = new List<PotentialPlayerDialogueUIObject>();
        }

        public void AddDialogueObjects(List<GameCompletionResult> completionResults)
        {
            foreach (GameCompletionResult completionResult in completionResults)
            {
                PotentialPlayerDialogueUIObject dialogueUI = Instantiate(_potentialPlayerDialoguePrefab, gameObject.transform);
                dialogueUI.SetGameCompletionResult(completionResult);
                dialogueUI.StopHighlightingObject();
                _dialogueObjects.Add(dialogueUI);
            }

            ShowUI();
            HighlightResult(0);
        }

        public void DestroyAllDialogueOptions()
        {
            for (int i = 0; i< _dialogueObjects.Count; i++)
            {
                Destroy(_dialogueObjects[i].gameObject);
            }

            _currentlyHighlightedIndex = 0;
            _dialogueObjects.Clear();
            HideUI();
        }

        public void HighlightResult(int indexResult)
        {
            if (indexResult != _currentlyHighlightedIndex && _dialogueObjects.IsValidIndex(indexResult))
            {
                _dialogueObjects[_currentlyHighlightedIndex].StopHighlightingObject();
                _dialogueObjects[indexResult].HighlightObject();

                _currentlyHighlightedIndex = indexResult;
            }
        }
    }
}
