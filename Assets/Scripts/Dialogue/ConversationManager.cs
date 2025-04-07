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

        private Coroutine _conversationCoroutine;

        private void Start ()
        {
            _conversationCoroutine = StartCoroutine(ProcessConversation(_conversationToRun));
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
                            dialogueObject.MemoryGameSpawnerData.GenerateMemoryGameData();
                            MemoryGameSolverComponent instance = MemoryGameSolverComponent.Instance;
                            StandardDialogueObject openingDialogue;

                            openingDialogue = dialogueObject.MemoryGameSpawnerData.MemoryGameRelatedDialogue.GetGameCreationDialogueObject();

                            yield return ProcessStandardDialogueObject(openingDialogue);

                            MemoryGameGenerator.Instance.GenerateGame(dialogueObject.MemoryGameSpawnerData);
                            yield return YieldUntilMemoryGameCompletion();
                            MemoryGameGenerator.Instance.DestroyGrid();

                            MemoryGameCompletionResult result = instance.IsLookingForSingleMemoryType ? instance.GetGameCompletionResultToApplyBySucceeding() :
                                                                                                        instance.GetGameCompletionResultToApplyByGuessesLeft();
                            if (instance.IsLookingForSingleMemoryType)
                            {
                                StandardDialogueObject closingDialogue = result.GameRelatedDialogue.GetGameClosingDialogueObject();

                                yield return ProcessStandardDialogueObject(closingDialogue);
                            }
                            else
                            {
                                yield return ProcessDialogues(result.MemoryGameDialogueResponses);
                            }
                            break;
                        }
                    case EDialogueObjectType.EndConversation:
                        {
                            yield return ProcessStandardDialogueObjects(dialogueObject.EndConversationObject.FinalDialogue);
                            StopCoroutine(_conversationCoroutine);
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
                List<DialogueObject> dialogueObjects = new List<DialogueObject>();
                dialogueObjects.Add(dialogueObject.CustomDialogue.GetSentimentDialogue());
                yield return ProcessDialogues(dialogueObjects);
            }
            else if (dialogueObject.GetDialogueString() != string.Empty)
            {
                DialogueUI.Instance.ShowDialogue(dialogueObject);
                yield return YieldUntilInput();
            }
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
