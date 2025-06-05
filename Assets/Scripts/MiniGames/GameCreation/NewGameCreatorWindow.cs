using ShootYourShotGame;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NewGameCreatorWindow : EditorWindow
{
    string NewGameName = "Default Name";

    [MenuItem("Window/New Game Creator Window")]
    public static void ShowWindow()
    {
        GetWindow<NewGameCreatorWindow>("New Game Creator Window");
    }

    private void OnGUI()
    {
        NewGameName = EditorGUILayout.TextField("New Game Name", NewGameName);
        if (GUILayout.Button("Create Game Files"))
        {
            CreateNewGameFiles();

            AssetDatabase.Refresh();
        }
    }
    private void CreateNewGameFiles()
    {
        string folderPath = "Assets/Scripts/MiniGames/" + NewGameName;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            folderPath += "/";
            CreateGameGenerator(folderPath);
            CreateGameSolver(folderPath);
            CreateGenerationData(folderPath);
            CreateCompletionResult(folderPath);
            CreateGameUI(folderPath);
        }
    }

    private void CreateGameGenerator(string startingFileName)
    {
        string className = NewGameName + "Generator";
        string fileName = startingFileName + className;

        Debug.Log("Creating " + fileName + ".cs");
        using (StreamWriter outFile = new StreamWriter(fileName + ".cs"))
        {
            outFile.WriteLine("using UnityEngine;");
            outFile.WriteLine("using System.Collections;");
            outFile.WriteLine("using GeneralGame.Generation;");
            outFile.WriteLine("");
            outFile.WriteLine("namespace " + NewGameName);
            outFile.WriteLine("{");
            outFile.WriteLine("public class " + className + " : DialogueCreatedGameGenerator<" + NewGameName + "Solver, " + NewGameName + "GenerationData, " + NewGameName + "CompletionResult> \n{");
            outFile.WriteLine("public static " + className + " Instance { get; private set; }");
            outFile.WriteLine("protected override "+ NewGameName + "Solver GameSolverComponent { get { return " + NewGameName + "Solver" + ".Instance; } }");
            outFile.WriteLine(" ");
            outFile.WriteLine("private void Awake()\n{\nInstance = this;\n}");
            outFile.WriteLine("}");
            outFile.WriteLine("}");
        }
    }

    private void CreateGameSolver(string startingFileName)
    {
        string className = NewGameName + "Solver";
        string fileName = startingFileName + className;

        Debug.Log("Creating " + fileName + ".cs");
        using (StreamWriter outFile = new StreamWriter(fileName + ".cs"))
        {
            outFile.WriteLine("using UnityEngine;");
            outFile.WriteLine("using System.Collections;");
            outFile.WriteLine("using GeneralGame;");
            outFile.WriteLine("");
            outFile.WriteLine("namespace " + NewGameName);
            outFile.WriteLine("{");
            outFile.WriteLine("public class " + className + " : GameSolverComponent<" + NewGameName + "GenerationData, " + NewGameName + "CompletionResult> \n{");
            outFile.WriteLine(" ");
            outFile.WriteLine("public static " + className + " Instance { get; private set; }");
            outFile.WriteLine(" ");
            outFile.WriteLine("protected override void Awake()\n{\nbase.Awake();\nInstance = this;\n}");
            outFile.WriteLine("}");
            outFile.WriteLine("}");
        }
    }

    private void CreateGenerationData(string startingFileName)
    {
        string className = NewGameName + "GenerationData";
        string fileName = startingFileName + className;

        Debug.Log("Creating " + fileName + ".cs");
        using (StreamWriter outFile = new StreamWriter(fileName + ".cs"))
        {
            outFile.WriteLine("using System;");
            outFile.WriteLine("using UnityEngine;");
            outFile.WriteLine("using System.Collections;");
            outFile.WriteLine("using GeneralGame.Generation;");
            outFile.WriteLine("");
            outFile.WriteLine("namespace " + NewGameName);
            outFile.WriteLine("{");
            outFile.WriteLine("[Serializable]");
            outFile.WriteLine("public class " + className + " : GameGenerationData<" + NewGameName + "CompletionResult> \n{");
            outFile.WriteLine(" ");
            outFile.WriteLine("}");
            outFile.WriteLine("}");
        }
    }

    private void CreateCompletionResult(string startingFileName)
    {
        string className = NewGameName + "CompletionResult";
        string fileName = startingFileName + className;

        Debug.Log("Creating " + fileName + ".cs");
        using (StreamWriter outFile = new StreamWriter(fileName + ".cs"))
        {
            outFile.WriteLine("using System;");
            outFile.WriteLine("using UnityEngine;");
            outFile.WriteLine("using System.Collections;");
            outFile.WriteLine("using GeneralGame.Results;");
            outFile.WriteLine("");
            outFile.WriteLine("namespace " + NewGameName);
            outFile.WriteLine("{");
            outFile.WriteLine("[Serializable]");
            outFile.WriteLine("public class " + className + " : GameCompletionResult \n{");
            outFile.WriteLine(" ");
            outFile.WriteLine("}");
            outFile.WriteLine("}");
        }
    }

    private void CreateGameUI(string startingFileName)
    {
        string className = NewGameName + "UI";
        string generatorClass = NewGameName + "Generator";
        string solverClass = NewGameName + "Solver";
        string fileName = startingFileName + className;

        Debug.Log("Creating " + fileName + ".cs");
        using (StreamWriter outFile = new StreamWriter(fileName + ".cs"))
        {
            outFile.WriteLine("using UnityEngine;");
            outFile.WriteLine("using System.Collections;");
            outFile.WriteLine("using TMPro;");
            outFile.WriteLine("using Sirenix.OdinInspector;");
            outFile.WriteLine("");
            outFile.WriteLine("namespace " + NewGameName);
            outFile.WriteLine("{");
            outFile.WriteLine("public class " + className + " : GameUI<" + NewGameName + "Generator, " + NewGameName + "Solver>\n{");
            outFile.WriteLine(" ");
            outFile.WriteLine("protected override " + generatorClass + " GameGenerator { get { return " + generatorClass + ".Instance; } }");
            outFile.WriteLine("protected override " + solverClass + " GameSolver { get { return " + solverClass + ".Instance; } }");
            outFile.WriteLine("}");
            outFile.WriteLine("}");
        }
    }
}
