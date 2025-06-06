using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObjectsManager : MonoBehaviour
{
    public static ParentObjectsManager Instance { get; private set; }

    [SerializeField]
    private GameObject _mazeNodesParent;
    public GameObject MazeNodesParent { get { return _mazeNodesParent;} }

    [SerializeField]
    private GameObject _cardGameParent;
    public GameObject CardGameParent {  get { return _cardGameParent; } }

    [SerializeField]
    private GameObject _fireFightingWindowParent;
    public GameObject FireFightingWindowParent { get { return _fireFightingWindowParent; } }

    private void Awake()
    {
        Instance = this; 
    }
}
