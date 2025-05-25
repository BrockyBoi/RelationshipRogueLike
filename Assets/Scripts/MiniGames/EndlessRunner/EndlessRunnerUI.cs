using UnityEngine;
using System.Collections;
using TMPro;
using Sirenix.OdinInspector;

namespace EndlessRunner
{
public class EndlessRunnerUI : GameUI<EndlessRunnerGenerator, EndlessRunnerSolver>
{
 
protected override EndlessRunnerGenerator GameGenerator { get { return EndlessRunnerGenerator.Instance; } }
protected override EndlessRunnerSolver GameSolver { get { return EndlessRunnerSolver.Instance; } }
}
}
