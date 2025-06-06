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
    public enum EGameType
    {
        Maze,
        CatchingButterflies,
        EndlessRunner,
        Memory,
        WhackAMole,
        ShootYourShot,
        FireFighting
    }

    [Serializable]
    public class ControllersDictionary : UnitySerializedDictionary<EGameType, Controllers> { }

    public class MiniGameControllersManager : MonoBehaviour
    {
        private static MiniGameControllersManager _instance;
        public static MiniGameControllersManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    return GameObject.FindFirstObjectByType(typeof(MiniGameControllersManager)) as MiniGameControllersManager;
                }

                return _instance;
            }
        }

        [SerializeField]
        private ControllersDictionary _controllerReferences;

        private EGameType _currentGameType;
        public EGameType CurrentGameType { get { return _currentGameType; } }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void SetCurrentGameType(EGameType currentGameType)
        {
            _currentGameType = currentGameType;
        }

        public void GetBothControllers(out BaseGameSolverComponent solver, out BaseGameGenerator generator, EGameType gameType)
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

        public void GetBothControllers<Solver, Generator>(out Solver solver, out Generator generator, EGameType gameType) where Generator : BaseGameGenerator where Solver : BaseGameSolverComponent
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

        public BaseGameSolverComponent GetSolverComponent(EGameType gameType)
        {
            if (ensure(_controllerReferences.ContainsKey(gameType), gameType + " is not in the mini game controllers manager"))
            {
                return _controllerReferences[gameType].GameSolver;
            }

            return null;
        }

        public BaseGameSolverComponent GetCurrentGameSolver()
        {
            if (ensure(_controllerReferences.ContainsKey(CurrentGameType), CurrentGameType + " is not in the mini game controllers manager"))
            {
                return _controllerReferences[CurrentGameType].GameSolver;
            }

            return null;
        }

        public BaseGameGenerator GetGeneratorComponent(EGameType gameType)
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