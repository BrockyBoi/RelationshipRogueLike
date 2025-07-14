using CustomUI;
using GeneralGame;
using GeneralGame.Results;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static GlobalFunctions;

namespace Dialogue.UI
{
    [RequireComponent(typeof(VerticalAlignmentOptions))]
    public class PotentialPlayerDialogueUI : BaseGameUI
    {
        public static PotentialPlayerDialogueUI Instance { get; private set; }

        [SerializeField, Title("UI")]
        private Canvas _canvas;

        [SerializeField]
        private VerticalLayoutGroup _verticalLayoutGroup;

        [SerializeField, AssetsOnly]
        private PotentialPlayerDialogueUIObject _potentialPlayerDialoguePrefab;

        private BaseGameSolverComponent _currentGameSolver;

        private List<PotentialPlayerDialogueUIObject> _dialogueObjects;

        private int _currentlyHighlightedIndex = 0;

        [SerializeField]
        private float _timeToMoveFromResultToDialogue = 1.5f;

        [SerializeField, Title("Show UI Timing")]
        private float _timeBetweenShowingUIElements = .35f;

        [SerializeField]
        private float _timeForDialogueUIToAppear = 1.5f;
        public float TimeToMoveFromResultToDialogue { get {  return _timeToMoveFromResultToDialogue;} }

        [SerializeField, Title("Audio")]
        private AudioClip _dialogueObjectChangeClip;

        private void Awake()
        {
            Instance = this;
            _dialogueObjects = new List<PotentialPlayerDialogueUIObject>();
            _currentlyHighlightedIndex = -1;

            Application.targetFrameRate = 120;
        }

        public void AddDialogueObjects(List<GameCompletionResult> completionResults, BaseGameSolverComponent currentGameSolver)
        {
            //_verticalLayoutGroup.enabled = true;


            foreach (GameCompletionResult completionResult in completionResults)
            {
                PotentialPlayerDialogueUIObject dialogueUI = Instantiate(_potentialPlayerDialoguePrefab, gameObject.transform);
                dialogueUI.SetGameCompletionResult(completionResult);
                dialogueUI.StopHighlightingObject();
                _dialogueObjects.Add(dialogueUI);

                dialogueUI.Show(false);
            }

            _currentGameSolver = currentGameSolver;


            StartCoroutine(AddPotentialDialogueObjectsToUI());

            ShowUI();
        }

        private IEnumerator AddPotentialDialogueObjectsToUI()
        {
            yield return new WaitForSeconds(.05f);

            _verticalLayoutGroup.enabled = false;

            foreach (PotentialPlayerDialogueUIObject ui in _dialogueObjects)
            {
                ui.AddResultToUI(_timeForDialogueUIToAppear);
                ui.Show(true);

                yield return new WaitForSeconds(_timeBetweenShowingUIElements);
            }

            yield return new WaitForSeconds(1.5f);
            _currentGameSolver.OnUIInitialized();
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

            _verticalLayoutGroup.enabled = true;
        }

        public void HighlightResult(int indexResult, float percentage)
        {
            //_verticalLayoutGroup.enabled = false;

            if (!ensure(_dialogueObjects.IsValidIndex(indexResult), indexResult + " is not a valid index with a count of " + _dialogueObjects.Count + " dialogue objects"))
            {
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

                        if (MiniGameControllersManager.Instance.GetCurrentGameSolver().IsStage(EGameStage.InGame))
                        {
                            oldResult.RemoveResultFromUI();
                            AudioManager.Instance.PlaySoundEffect(_dialogueObjectChangeClip);
                        }
                    }
                }

                _dialogueObjects[indexResult].HighlightObject();

                _currentlyHighlightedIndex = indexResult;
            }

            if (_dialogueObjects.IsValidIndex(_currentlyHighlightedIndex))
            {
                _dialogueObjects[_currentlyHighlightedIndex].SetProgressPercentage(percentage);
            }
        }


    }
}
