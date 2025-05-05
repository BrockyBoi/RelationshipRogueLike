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
using GeneralGame.Results;
using Characters;
using WhackAMole;

namespace Dialogue
{
    public class ConversationManager : MonoBehaviour
    {
        public static ConversationManager Instance { get; private set; }

        [SerializeField]
        private Conversation _conversationToRun;

        [SerializeField]
        private Conversation _conversationOnPlayerDeath;

        private Coroutine _conversationCoroutine;

        private bool _playerHasDied = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }

        private void Start ()
        {
            _playerHasDied = false;
            _conversationCoroutine = StartCoroutine(ProcessConversation(_conversationToRun));
            Player player = Player.Instance;
            if (player)
            {
                HealthComponent healthComponent = player.HealthComponent;
                if (healthComponent)
                {
                    healthComponent.OnDeath += OnPlayerDeath;
                }
            }
        }

        private void OnDestroy()
        {
            Player player = Player.Instance;
            if (player)
            {
                HealthComponent healthComponent = player.HealthComponent;
                if (healthComponent)
                {
                    healthComponent.OnDeath -= OnPlayerDeath;
                }
            }
        }

        private void OnPlayerDeath()
        {
            _playerHasDied = true;
        }

        public void SetConversationsForLevel(Conversation conversationToRun, Conversation deathConversation)
        {
            _conversationToRun = conversationToRun;
            _conversationOnPlayerDeath = deathConversation;
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
                if (_playerHasDied)
                {
                    _playerHasDied = false;
                    yield return StartCoroutine(ProcessConversation(_conversationOnPlayerDeath));
                    yield break;
                }

                switch (dialogueObject.DialogueObjectType)
                {
                    case EDialogueObjectType.StandardDialogue:
                        {
                            yield return ProcessStandardDialogueObjects(dialogueObject.StandardDialogueObjects);
                            break;
                        }
                    case EDialogueObjectType.SentimentDialogue:
                        {
                            ECharacterSentiment sentiment = Player.Instance.HealthComponent.GetCharacterSentiment();
                            if (dialogueObject.SentimentDialogueObject.SentimentDialogue.ContainsKey(sentiment))
                            {
                                yield return ProcessBranchingDialogueObject(dialogueObject.SentimentDialogueObject.SentimentDialogue[sentiment]);
                            }
                            else
                            {
                                Debug.LogWarning(sentiment + " was not in sentiment dialogue dictionary");
                            }
                            break;
                        }
                    case EDialogueObjectType.SpawnMaze:
                        {
                            MazeGenerator.Instance.GenerateGame(dialogueObject.MazeSpawnerData);
                            yield return YieldUntilGameIsInFinishedStage(MazeSolverComponent.Instance);
                            MazeGenerator.Instance.DestroyGrid();
                            MazeCompletionResult result = MazeSolverComponent.Instance.GetGameCompletionResultToApplyByTimeRemaining();

                            yield return ProcessGameResult(result);
                            break;
                        }
                    case EDialogueObjectType.SpawnMemoryGame:
                        {
                            dialogueObject.MemoryGameSpawnerData.GenerateMemoryGameData();
                            MemoryGameSolverComponent instance = MemoryGameSolverComponent.Instance;
                            StandardDialogueObject openingDialogue;

                            if (dialogueObject.MemoryGameSpawnerData.MemoryGameRelatedDialogue.HasOpeningDialogue)
                            {
                                openingDialogue = dialogueObject.MemoryGameSpawnerData.MemoryGameRelatedDialogue.GetGameCreationDialogueObject();

                                yield return ProcessStandardDialogueObject(openingDialogue);
                            }

                            MemoryGameGenerator.Instance.GenerateGame(dialogueObject.MemoryGameSpawnerData);
                            yield return YieldUntilGameIsInFinishedStage(instance);
                            MemoryGameGenerator.Instance.DestroyGrid();

                            MemoryGameCompletionResult result = instance.IsLookingForSingleMemoryType ? instance.GetGameCompletionResultToApplyBySucceeding() :
                                                                                                        instance.GetGameCompletionResultToApplyByGuessesLeft();
                            if (dialogueObject.MemoryGameSpawnerData.MemoryGameRelatedDialogue.HasClosingDialogue)
                            {
                                if (instance.IsLookingForSingleMemoryType)
                                {
                                    StandardDialogueObject closingDialogue = result.GameRelatedDialogue.GetGameClosingDialogueObject();

                                    yield return ProcessStandardDialogueObject(closingDialogue);
                                }
                            }
                            else
                            {
                                yield return ProcessGameResult(result);
                            }
                            break;
                        }
                    case EDialogueObjectType.SpawnWhackAMole:
                        {
                            WhackAMoleGenerator.Instance.GenerateGame(dialogueObject.WhackAMoleGameSpawnerData);
                            yield return YieldUntilGameIsInFinishedStage(WhackAMoleSolver.Instance);
                            WhackAMoleGenerator.Instance.DeleteGameObjects();

                            WhackAMoleCompletionResult result = WhackAMoleSolver.Instance.GetResultByHealthRemaining();

                            yield return ProcessGameResult(result);
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
            if (dialogueObject.GetDialogueString() != string.Empty)
            {
                DialogueUI.Instance.ShowDialogue(dialogueObject);
                yield return YieldUntilInput();
            }
        }

        private IEnumerator ProcessBranchingDialogueObject(BranchingDialogueObject branchingDialogue)
        {
            if (branchingDialogue != null)
            {
                if (branchingDialogue.OnlyUsesDialogue)
                {
                    yield return ProcessStandardDialogueObjects(branchingDialogue.DialogueObjects);
                }
                else
                {
                    yield return ProcessConversation(branchingDialogue.NewConversation);
                }
            }
        }

        private IEnumerator ProcessGameResult(GameCompletionResult gameResult)
        {
            yield return ProcessResultDialogueIntoCharacterDialogue();

            if (gameResult.BranchingDialogue.OnlyUsesDialogue)
            {
                yield return ProcessStandardDialogueObjects(gameResult.BranchingDialogue.DialogueObjects);
            }
            else
            {
                yield return ProcessConversation(gameResult.BranchingDialogue.NewConversation);
            }
        }

        private IEnumerator ProcessResultDialogueIntoCharacterDialogue()
        {
            PotentialPlayerDialogueUI.Instance.DestroyAllDialogueOptions();
            yield break;

            // Still working on it
            PotentialPlayerDialogueUIObject dialogueUI = PotentialPlayerDialogueUI.Instance.GetCurrentlyHighlightedPotentialPlayerDialogueUI();
            if (dialogueUI)
            {
                float timeToMove = PotentialPlayerDialogueUI.Instance.TimeToMoveFromResultToDialogue;
                Vector3 finalLocation;
                RectTransform rect;

                DialogueUI.Instance.GetMoveFromGameResultToConversationData(out finalLocation, out rect);

                //GlobalFunctions.LerpObjectToLocation(dialogueUI, dialogueUI.gameObject, finalLocation, timeToMove);
                GlobalFunctions.LerpRectTransform(dialogueUI, dialogueUI.GetComponent<RectTransform>(), rect, timeToMove);

                yield return new WaitForSeconds(timeToMove);

                PotentialPlayerDialogueUI.Instance.DestroyAllDialogueOptions();
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

        private IEnumerator YieldUntilGameIsInFinishedStage(BaseGameSolverComponent gameSolver)
        {
            while (gameSolver && !gameSolver.IsStage(EGameStage.GameFinished))
            {
                yield return null;
            }
        }
    }
}
