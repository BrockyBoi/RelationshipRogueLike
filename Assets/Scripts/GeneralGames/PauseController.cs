using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    private bool _isPaused = false;
    public bool IsPaused { get { return _isPaused; } }


    [SerializeField, Required]
    private Canvas _pauseUICanvas;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _pauseUICanvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                UnPauseGame();
            }
            else
            {
                SetPause(true);
            }
        }
    }

    public void UnPauseGame()
    {
        SetPause(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetPause(bool isPaused)
    {
        _isPaused = isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        _pauseUICanvas.enabled = isPaused;
    }
}
