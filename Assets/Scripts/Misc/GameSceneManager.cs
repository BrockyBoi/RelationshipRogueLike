using Dialogue;
using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance {  get; private set; }

    [SerializeField]
    private string _gameSceneName = "GameScene";

    [SerializeField]
    private string _mapSceneName = "MapScene";

    [SerializeField]
    private AudioClip _defaultMapSceneBackgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _gameSceneName)
        {
            ConversationManager.Instance.StartConversation();
        }
        else if (scene.name == _mapSceneName)
        {
            MapSceneManager.Instance.DisableAlreadyUsedMapObjects();
            AudioManager.Instance.PlayBackgroundMusic(_defaultMapSceneBackgroundMusic);
        }
    }

    public void LoadMapLevel()
    {
        SceneManager.LoadScene(_mapSceneName);
    }

    public void LoadGameLevel()
    {
        SceneManager.LoadScene(_gameSceneName);
    }
}
