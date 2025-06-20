using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapSceneManager : MonoBehaviour
    {
        public static MapSceneManager Instance { get; private set; }

        private HashSet<string> _alreadySelectedObjects = new HashSet<string>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DisableAlreadyUsedMapObjects();
        }

        public void SelectMapObject(SelectableMapObject selectableMapObject)
        {
            _alreadySelectedObjects.Add(selectableMapObject.gameObject.name);
        }

        public void DisableAlreadyUsedMapObjects()
        {
            SelectableMapObject[] selectableObjectsInScene = FindObjectsOfType<SelectableMapObject>();
            foreach (SelectableMapObject mapObject in selectableObjectsInScene)
            {
                if (_alreadySelectedObjects.Contains(mapObject.gameObject.name) || !mapObject.IsButtonAvailable(LevelDataManager.Instance.LevelsCompleted))
                {
                    mapObject.DisableObject();
                    continue;
                }
            }
        }
    }
}
