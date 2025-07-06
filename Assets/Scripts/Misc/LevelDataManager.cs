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
    Fight,
    None
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

    private ELevel _currentLevel = ELevel.None;

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

    public LevelConversationData GetLevelConversationData(ELevel level = ELevel.None)
    {
        ELevel levelToCheck = level == ELevel.None ? _currentLevel : level;
        return ensure(_levelDatas.ContainsKey(levelToCheck), "Does not have data for " + levelToCheck + " level") ? _levelDatas[levelToCheck] : null;
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
