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

    private void Awake()
    {
        Instance = this; 
    }
}
