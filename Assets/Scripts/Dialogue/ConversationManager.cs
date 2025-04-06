using Maze;
using Maze.Generation;
using MainPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue.UI;
using GeneralGame;
using MemoryGame;
using MemoryGame.Generation;

namespace Dialogue
{
    public class ConversationManager : MonoBehaviour
    {
        [SerializeField]
        private Conversation _conversationToRun;

        private void Start ()
        {
            StartCoroutine(ProcessConversation(_conversationToRun));
        }

        private IEnumerator ProcessDialogues(List<DialogueObject> dialogueObjects)
        {
            Player player = Player.Instance;
            if (!player)
            {
                Debug.LogError("Player is null");
                yield break;
            }

            foreach (DialogueObject dialogueObject in dialogueObjects)
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
                            MazeGenerator.Instance.GenerateGame(dialogueObject.MazeSpawnerData);
                            yield return YieldUntilMazeCompletion();
                            MazeGenerator.Instance.DestroyGrid();
                            MazeCompletionResult result = MazeSolverComponent.Instance.GetGameCompletionResultToApplyByTimeRemaining();

                            yield return ProcessDialogues(result.MazeDialogueResponses);
                            break;
                        }
                    case EDialogueObjectType.SpawnMemoryGame:
                        {
                            StandardDialogueObject openingDialogue = dialogueObject.MemoryGameSpawnerData.OpeningDialogue.GetGameCreationDialogueObject();
                            yield return ProcessStandardDialogueObject(openingDialogue);

                            MemoryGameGenerator.Instance.GenerateGame(dialogueObject.MemoryGameSpawnerData);
                            yield return YieldUntilMemoryGameCompletion();
                            MemoryGameGenerator.Instance.DestroyGrid();

                            MemoryGameCompletionResult result = MemoryGameSolverComponent.Instance.GetGameCompletionResultToApplyBySucceeding();
                            StandardDialogueObject closingDialogue = result.GameRelatedDialogue.GetGameClosingDialogueObject();

                            yield return ProcessStandardDialogueObject(closingDialogue);
                            break;
                        }
                    case EDialogueObjectType.EndConversation:
                        {
                            yield return ProcessStandardDialogueObjects(dialogueObject.EndConversationObject.FinalDialogue);
                            yield break;
                        }
                    case EDialogueObjectType.LinkNewConversation:
                        {
                            yield return ProcessConversation(dialogueObject.LinkedConverssationObject.NewConversation);
                            yield break;
                        }
                    default:
                        break;
                }
            }
        }

        private IEnumerator ProcessConversation(Conversation conversation)
        {
            yield return ProcessDialogues(conversation.DialogueObjects);
        }

        private IEnumerator ProcessStandardDialogueObjects(List<StandardDialogueObject> dialogueObjects)
        {
            foreach (StandardDialogueObject dialogueObject in dialogueObjects)
            {
                yield return ProcessStandardDialogueObject(dialogueObject);
            }
        }

        private IEnumerator ProcessStandardDialogueObject(StandardDialogueObject dialogueObject)
        {
            if (dialogueObject.CustomDialogue.PlayerHasSentiment())
            {
                yield return ProcessDialogues(dialogueObject.CustomDialogue.GetSentimentDialogue());
            }
            else
            {
                DialogueUI.Instance.ShowDialogue(dialogueObject);
            }
            yield return YieldUntilInput();
        }

        private IEnumerator YieldUntilInput()
        {
            yield return new WaitForEndOfFrame();

            while (!Input.anyKeyDown)
            {
                yield return null;
            }

            yield return new WaitForEndOfFrame();
        }

        private IEnumerator YieldUntilMazeCompletion()
        {
            MazeSolverComponent mazeSolverComponent = MazeSolverComponent.Instance;
            while (mazeSolverComponent && !mazeSolverComponent.IsStage(EGameStage.GameFinished))
            {
                yield return null;
            }
        }

        private IEnumerator YieldUntilMemoryGameCompletion()
        {
            MemoryGameSolverComponent memoryGameSolverComponent = MemoryGameSolverComponent.Instance;
            while (memoryGameSolverComponent && !memoryGameSolverComponent.IsStage(EGameStage.GameFinished))
            {
                yield return null;
            }
        }
    }
}
