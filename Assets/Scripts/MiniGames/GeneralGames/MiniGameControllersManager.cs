using Characters;
using Dialogue;
using GeneralGame;
using GeneralGame.Generation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalFunctions;

namespace GeneralGame
{
    public enum EGameTypes
    {
        Maze,
        CatchingButterflies,
        EndlessRunner,
        Memory,
        WhackAMole,
        ShootYourShot
    }

    [Serializable]
    public class ControllersDictionary : UnitySerializedDictionary<EGameTypes, Controllers> { }

    public class MiniGameControllersManager : MonoBehaviour
    {
        public static MiniGameControllersManager Instance { get; private set; }

        [SerializeField]
        private ControllersDictionary _controllerReferences;

        private EGameTypes _currentGameType;
        public EGameTypes CurrentGameType { get { return _currentGameType; } }

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

        public void SetCurrentGameType(EGameTypes currentGameType)
        {
            _currentGameType = currentGameType;
        }

        public void GetBothControllers(out BaseGameSolverComponent solver, out BaseGameGenerator generator, EGameTypes gameType)
        {
            solver = null;
            generator = null;
            if (ensure(_controllerReferences.ContainsKey(gameType), gameType + " is not in the mini game controllers manager"))
            {
                Controllers controllers = _controllerReferences[gameType];
                solver = controllers.GameSolver;
                generator = controllers.GameGenerator;
            }
        }

        public void GetBothControllers<Solver, Generator>(out Solver solver, out Generator generator, EGameTypes gameType) where Generator : BaseGameGenerator where Solver : BaseGameSolverComponent
        {
            solver = null;
            generator = null;
            if (ensure(_controllerReferences.ContainsKey(gameType), gameType + " is not in the mini game controllers manager"))
            {
                Controllers controllers = _controllerReferences[gameType];
                solver = (Solver)controllers.GameSolver;
                generator = (Generator)controllers.GameGenerator;
            }
        }

        public BaseGameSolverComponent GetSolverComponent(EGameTypes gameType)
        {
            if (ensure(_controllerReferences.ContainsKey(gameType), gameType + " is not in the mini game controllers manager"))
            {
                return _controllerReferences[gameType].GameSolver;
            }

            return null;
        }

        public BaseGameGenerator GetGeneratorComponent(EGameTypes gameType)
        {
            if (ensure(_controllerReferences.ContainsKey(gameType), gameType + " is not in the mini game controllers manager"))
            {
                return _controllerReferences[gameType].GameGenerator;
            }

            return null;
        }
    }

    [Serializable]
    public class Controllers
    {
        public BaseGameSolverComponent GameSolver;
        public BaseGameGenerator GameGenerator;
    }
}