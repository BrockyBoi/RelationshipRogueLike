using Maze;
using Maze.Generation;
using MainPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                        StandardDialogueObject diag = dialogueObject.StandardDialogueObject;
                        Debug.Log(diag.CharacterName + " says: " + diag.GetDialogueString(player.HealthComponent));
                        yield return YieldUntilInput();
                        break;
                    }
                    case EDialogueObjectType.SpawnMaze:
                    {
                        MazeGenerator.Instance.BuildMaze(dialogueObject.MazeSpawnerDialogue.MazeSpawnData, dialogueObject.MazeSpawnerDialogue.MazeCompletionResults);
                        yield return YieldUntilMazeCompletion();
                        MazeGenerator.Instance.DestroyMaze();
                        MazeCompletionResult result = MazeSolverComponent.Instance.GetMazeCompletionResultToApply();
                        Debug.Log(result.DialogueResponse.CharacterName + " says: " + result.DialogueResponse.DialogueResponse);
                        break;
                    }
                    default:
                        break;
                }
            }

            Debug.Log("Completed dialogue");
        }

        private IEnumerator YieldUntilInput()
        {
            while (!Input.anyKeyDown)
            {
                yield return null;
            }
        }

        private IEnumerator YieldUntilMazeCompletion()
        {
            MazeSolverComponent mazeSolverComponent = MazeSolverComponent.Instance;
            while (mazeSolverComponent != null && !mazeSolverComponent.HasGameFinished)
            {
                yield return null;
            }
        }
    }
}
