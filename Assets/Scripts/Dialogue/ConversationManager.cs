using Maze;
using Maze.Generation;
using MainPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue.UI;
using GeneralGame;

namespace Dialogue
{
    public class ConversationManager : MonoBehaviour
    {
        [SerializeField]
        private Conversation _conversationToRun;

        private void Start ()
        {
            StartCoroutine(RunConversation());
        }

        private IEnumerator RunConversation()
        {
            Player player = Player.Instance;
            if (player == null)
            {
                Debug.LogError("Player is null");
                yield break;
            }

            foreach (DialogueObject dialogueObject in _conversationToRun.DialogueObjects)
            {
                switch (dialogueObject.DialogueObjectType)
                {
                    case EDialogueObjectType.StandardDialogue:
                    {
                        yield return ProcessStandardDialogueObjects(dialogueObject.StandardDialogueObjects);
                        break;
                    }
                    case EDialogueObjectType.SpawnMaze:
                    {
                        MazeGenerator.Instance.BuildMaze(dialogueObject.MazeSpawnerDialogue.MazeSpawnData, dialogueObject.MazeSpawnerDialogue.MazeCompletionResults);
                        yield return YieldUntilMazeCompletion();
                        MazeGenerator.Instance.DestroyMaze();
                        MazeCompletionResult result = MazeSolverComponent.Instance.GetGameCompletionResultToApply();

                        yield return ProcessStandardDialogueObjects(result.DialogueResponses);
                        break;
                    }
                    default:
                        break;
                }
            }

            Debug.Log("Completed dialogue");
        }

        private IEnumerator ProcessStandardDialogueObjects(List<StandardDialogueObject> dialogueObjects)
        {
            foreach (StandardDialogueObject dialogueObject in dialogueObjects)
            {
                DialogueUI.Instance.ShowDialogue(dialogueObject);
                yield return YieldUntilInput();
            }
        }

        private IEnumerator YieldUntilInput()
        {
            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            yield return new WaitForEndOfFrame();
        }

        private IEnumerator YieldUntilMazeCompletion()
        {
            MazeSolverComponent mazeSolverComponent = MazeSolverComponent.Instance;
            while (mazeSolverComponent != null && mazeSolverComponent.IsStage(EGameStage.InGame))
            {
                yield return null;
            }
        }
    }
}
