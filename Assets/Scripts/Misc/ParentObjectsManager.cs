using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObjectsManager : MonoBehaviour
{
    public static ParentObjectsManager Instance { get; private set; }

    [SerializeField]
    private GameObject _mazeNodesParent;
    public GameObject MazeNodesParent { get { return _mazeNodesParent;} }

    private void Awake()
    {
        Instance = this; 
    }
}
