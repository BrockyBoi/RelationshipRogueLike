using MainPlayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

public enum ELevel
{
    FirstDate,
    Restaurant,
    MovieTheatre,
    LoveConfession,
    Fight
}

[Serializable]
public class LevelConversationDataDictionary : UnitySerializedDictionary<ELevel, LevelConversationData> { }

public class LevelDataManager : MonoBehaviour
{
    public static LevelDataManager Instance { get; private set; }

    [SerializeField]
    private LevelConversationDataDictionary _levelDatas;

    private HashSet<ELevel> _levelsCompleted;
    public HashSet<ELevel> LevelsCompleted { get { return _levelsCompleted; } }

    private ELevel _currentLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _levelsCompleted = new HashSet<ELevel>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public LevelConversationData GetLevelConversationData()
    {
        return ensure(_levelDatas.ContainsKey(_currentLevel), "Does not have data for " + _currentLevel + " level") ? _levelDatas[_currentLevel] : null;
    }

    public void SetLevelToPlayOnLoad(ELevel level)
    {
        _currentLevel = level;
    }

    public void CompleteCurrentLevel()
    {
        if (!Player.Instance.HealthComponent.IsDead)
        {
            _levelsCompleted.Add(_currentLevel);
        }
    }
}
