using Dialogue;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static GlobalFunctions;

namespace Map
{
    public enum EMapObjectType
    {
        MinorEvent,
        StartNewScene
    }

    public class SelectableMapObject : MonoBehaviour
    {
        [SerializeField]
        private EMapObjectType _mapObjectType;
        public EMapObjectType MapObjectType { get { return _mapObjectType; } }

        [SerializeField, ShowIf("@_mapObjectType", EMapObjectType.MinorEvent)]
        private MinorMapEvent _minorMapEvent;

        [SerializeField, ShowIf("@_mapObjectType", EMapObjectType.StartNewScene)]
        private ELevel _levelToPlay;
        public ELevel LevelToPlay { get { return _levelToPlay; } }

        [SerializeField, Title("Level Requirements")]
        private List<ELevel> _allLevelsNeededToBeCompleted;

        [SerializeField]
        private List<ELevel> _possibleLevelsToUnlockThis;

        [SerializeField, Required]
        private Button _button;

        private void Start()
        {
            if (ensure(_button != null, "Button has not been set on SelectableMapObject"))
            {
                _button.onClick.AddListener(SelectMapObject);
            }
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(SelectMapObject);
        }

        public void SelectMapObject()
        {
            MapSceneManager.Instance.SelectMapObject(this);
            switch (_mapObjectType)
            {
                case EMapObjectType.MinorEvent:
                    {
                        MinorMapEventUI.Instance.SetCurrentMapEvent(_minorMapEvent);
                        break;
                    }
                case EMapObjectType.StartNewScene:
                    {
                        LevelDataManager.Instance.SetLevelToPlayOnLoad(_levelToPlay);
                        GameSceneManager.Instance.LoadGameLevel();
                        break;
                    }
            }

            DisableObject();
        }

        public void DisableObject()
        {
            gameObject.SetActive(false);
        }

        public bool IsButtonAvailable(HashSet<ELevel> completedLevels)
        {
            foreach (ELevel level in _allLevelsNeededToBeCompleted)
            {
                if (!completedLevels.Contains(level))
                {
                    return false;
                }
            }

            bool completedOptionalLevels = _possibleLevelsToUnlockThis.Count == 0;
            foreach (ELevel level in _possibleLevelsToUnlockThis)
            {
                if (completedLevels.Contains(level))
                {
                    return true;
                }
            }

            return completedOptionalLevels;
        }
    }

}
