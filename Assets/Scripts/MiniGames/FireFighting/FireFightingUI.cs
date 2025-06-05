using UnityEngine;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;

namespace FireFighting
{
public class FireFightingUI : GameUI<FireFightingGenerator, FireFightingSolver>
{
 
protected override FireFightingGenerator GameGenerator { get { return FireFightingGenerator.Instance; } }
protected override FireFightingSolver GameSolver { get { return FireFightingSolver.Instance; } }
}
}
