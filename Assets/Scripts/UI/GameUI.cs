using CustomUI;
using Dialogue.UI;
using GeneralGame;
using GeneralGame.Generation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GameUI<GameGeneratorClass, GameSolverClass> : BaseGameUI where GameGeneratorClass : BaseGameGenerator where GameSolverClass : BaseGameSolverComponent
{
    protected abstract GameGeneratorClass GameGenerator { get; }
    protected abstract GameSolverClass GameSolver { get; }

    [SerializeField]
    protected TextMeshProUGUI _timerText;

    protected virtual void Start()
    {
        if (GameGenerator)
        {
            GameGenerator.ListenToOnGameGenerated(OnGameGenerated);
        }

        if (GameSolver)
        {
            GameSolver.OnGameStart += OnGameStart;
            GameSolver.OnStartGameCountdownBegin += OnGameCountdownStart;
            GameSolver.OnStartGameCountdownEnd += OnGameCountdownStop;
            GameSolver.OnGameStop += OnGameEnd;
            GameSolver.OnGameWin += OnGameWon;
            GameSolver.OnGameFailed += OnGameFailed;
            GameSolver.OnMainTimerValueChange += OnGameTimerValueChange;
            GameSolver.OnCountdownValueChange += OnCountdownTimerValueChange;
        }

        DialogueUI dialogueUI = DialogueUI.Instance;
        if (dialogueUI)
        {
            dialogueUI.OnShowUI += HideUI;
        }

        HideUI();
    }

    protected virtual void OnDestroy()
    {
        if (GameGenerator)
        {
            GameGenerator.UnlistenToOnGameGenerated(OnGameGenerated);
        }

        if (GameSolver)
        {
            GameSolver.OnGameStart -= OnGameStart;
            GameSolver.OnStartGameCountdownBegin -= OnGameCountdownStart;
            GameSolver.OnStartGameCountdownEnd -= OnGameCountdownStop;
            GameSolver.OnGameStop -= OnGameEnd;
            GameSolver.OnGameWin -= OnGameWon;
            GameSolver.OnGameFailed -= OnGameFailed;
            GameSolver.OnMainTimerValueChange -= OnGameTimerValueChange;
            GameSolver.OnCountdownValueChange -= OnCountdownTimerValueChange;
        }

        DialogueUI dialogueUI = DialogueUI.Instance;
        if (dialogueUI)
        {
            dialogueUI.OnShowUI -= HideUI;
        }
    }

    protected virtual void OnGameStart()
    {

    }

    protected virtual void OnGameCountdownStart()
    {

    }

    protected virtual void OnGameCountdownStop()
    {
        
    }

    protected virtual void OnGameEnd()
    {
        ClearTimerText();
        StopAllCoroutines();
    }

    protected virtual void OnGameGenerated()
    {
        ShowUI();
    }

    protected virtual void OnGameWon()
    {

    }

    protected virtual void OnGameFailed()
    {

    }

    protected void ClearTimerText()
    {
        _timerText.text = string.Empty;
    }

    protected virtual void OnCountdownTimerValueChange(float value)
    {
        ShowUI();
        _timerText.text = "CountDown : " + value.ToString("F2");
    }

    protected virtual void OnGameTimerValueChange(float value)
    {
        _timerText.text = "Time Left : " + value.ToString("F2");
    }
}
