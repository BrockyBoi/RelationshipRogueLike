using CustomUI;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField]
        private float _timeToMoveFromResultToDialogue = 1.5f;
        public float TimeToMoveFromResultToDialogue { get {  return _timeToMoveFromResultToDialogue;} }

        [SerializeField]
        private VerticalLayoutGroup _verticalLayoutGroup;

        private void Awake()
        {
            Instance = this;
            _dialogueObjects = new List<PotentialPlayerDialogueUIObject>();
            _currentlyHighlightedIndex = -1;
        }

        public void AddDialogueObjects(List<GameCompletionResult> completionResults)
        {
            //_verticalLayoutGroup.enabled = true;

            foreach (GameCompletionResult completionResult in completionResults)
            {
                PotentialPlayerDialogueUIObject dialogueUI = Instantiate(_potentialPlayerDialoguePrefab, gameObject.transform);
                dialogueUI.SetGameCompletionResult(completionResult);
                dialogueUI.StopHighlightingObject();
                _dialogueObjects.Add(dialogueUI);
            }

            //_verticalLayoutGroup.enabled = false;

            ShowUI();
        }

        public PotentialPlayerDialogueUIObject GetCurrentlyHighlightedPotentialPlayerDialogueUI()
        {
             return _dialogueObjects.IsValidIndex(_currentlyHighlightedIndex) ? _dialogueObjects[_currentlyHighlightedIndex] : null;
        }

        public void DestroyAllDialogueOptions()
        {
            for (int i = 0; i< _dialogueObjects.Count; i++)
            {
                Destroy(_dialogueObjects[i].gameObject);
            }

            _currentlyHighlightedIndex = -1;
            _dialogueObjects.Clear();
            HideUI();
        }

        public void HighlightResult(int indexResult)
        {
            //_verticalLayoutGroup.enabled = false;

            if (!_dialogueObjects.IsValidIndex(indexResult))
            {
                Debug.LogError(indexResult + " is not a valid index");
                return;
            }

            if (indexResult != _currentlyHighlightedIndex)
            {
                if (_dialogueObjects.IsValidIndex(_currentlyHighlightedIndex))
                {
                    PotentialPlayerDialogueUIObject oldResult = _dialogueObjects[_currentlyHighlightedIndex];
                    if (oldResult)
                    {
                        oldResult.StopHighlightingObject();
                        oldResult.RemoveResult();
                    }
                }

                _dialogueObjects[indexResult].HighlightObject();

                _currentlyHighlightedIndex = indexResult;
            }
        }
    }
}
