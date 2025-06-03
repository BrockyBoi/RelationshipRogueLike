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
using CatchingButterflies;

using static GlobalFunctions;
using Sirenix.OdinInspector;
using EndlessRunner;
using GeneralGame.Generation;
using static UnityEditor.Experimental.GraphView.GraphView;
using ShootYourShotGame;

namespace Dialogue
{
    public class ConversationManager : MonoBehaviour
    {
        public static ConversationManager Instance { get; private set; }

        [SerializeField]
        private LevelConversationData _conversationData;

        public LevelConversationData ConversationData { get { return _conversationData; } }

        private Coroutine _conversationCoroutine;

        private bool _playerHasDied = false;

        [SerializeField]
        private bool _runDialogueOnStart = false;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(this);
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start ()
        {
            _playerHasDied = false;
            Player player = Player.Instance;
            if (ensure(player, "Player is not valid"))
            {
                HealthComponent healthComponent = player.HealthComponent;
                if (healthComponent)
                {
                    healthComponent.OnDeath += OnPlayerDeath;
                }
            }

            if (_runDialogueOnStart)
            {
                StartConversation();
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

        [Button]
        public void StartConversation()
        {
            if (ensure(_conversationData != null, "No conversation data present"))
            {
                _conversationCoroutine = StartCoroutine(ProcessConversation(_conversationData.ConversationToRun));
            }
        }

        private void OnPlayerDeath()
        {
            _playerHasDied = true;
        }

        public void SetConversationsForLevel(LevelConversationData levelConversationData)
        {
            if (levelConversationData == null ||
                levelConversationData.ConversationToRun == null ||
                levelConversationData.ConversationOnPlayerDeath == null)
            {
                Debug.LogError("Level Conversation Data has invalid elements");
            }

            _conversationData = levelConversationData;
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
                    yield return StartCoroutine(ProcessConversation(_conversationData.ConversationOnPlayerDeath));
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
                            yield return ProcessStandardGameLogic<MazeGenerator, MazeSolverComponent, MazeGeneratorData, MazeCompletionResult>(EGameTypes.Maze, dialogueObject.MazeSpawnerData);
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
                            yield return ProcessStandardGameLogic<WhackAMoleGenerator, WhackAMoleSolver, WhackAMoleGenerationData, WhackAMoleCompletionResult>(EGameTypes.WhackAMole, dialogueObject.WhackAMoleGameSpawnerData);
                            WhackAMoleGenerator.Instance.DeleteGameObjects();
                            break;
                        }
                    case EDialogueObjectType.SpawnButterflyCatching:
                        {
                            yield return ProcessStandardGameLogic<CatchingButterfliesGenerator, CatchingButterfliesSolver, CatchingButterfliesGenerationData, CatchingButterfliesCompletionResult>(EGameTypes.CatchingButterflies, dialogueObject.CatchingButterflyGameSpawnerData);
                            break;
                        }
                    case EDialogueObjectType.SpawnEndlessRunner:
                        {
                            yield return ProcessStandardGameLogic<EndlessRunnerGenerator, EndlessRunnerSolver, EndlessRunnerGenerationData, EndlessRunnerCompletionResult>(EGameTypes.EndlessRunner, dialogueObject.EndlessRunnerSpawnerData);
                            break;
                        }
                    case EDialogueObjectType.SpawnShootYourShot:
                        {
                            yield return ProcessStandardGameLogic<ShootYourShotGameGenerator, ShootYourShotGameSolver, ShootYourShotGameGenerationData, ShootYourShotGameCompletionResult>(EGameTypes.ShootYourShot, dialogueObject.ShootYourShotSpawnerData);
                            break;
                        }
                    case EDialogueObjectType.EndConversation:
                        {
                            yield return ProcessStandardDialogueObjects(dialogueObject.EndConversationObject.FinalDialogue);

                            yield return YieldUntilInput();
                            StopCoroutine(_conversationCoroutine);
                            GameSceneManager.Instance.LoadMapLevel();
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

        private IEnumerator ProcessStandardGameLogic<GeneratorType, SolverType, GenerationDataType, CompletionResultType>(EGameTypes gameType, GenerationDataType generationData) 
            where GeneratorType : DialogueCreatedGameGenerator<SolverType, GenerationDataType, CompletionResultType> 
            where SolverType : GameSolverComponent<GenerationDataType, CompletionResultType> 
            where GenerationDataType : GameGenerationData<CompletionResultType>
            where CompletionResultType : GameCompletionResult, new()
        {
            SolverType solver;
            GeneratorType generator;
            MiniGameControllersManager.Instance.GetBothControllers(out solver, out generator, gameType);

            generator.GenerateGame(generationData);
            yield return YieldUntilGameIsInFinishedStage(solver);
            yield return ProcessGameResult(solver.GetCurrentCompletionResult());
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

            while (!Input.GetMouseButtonDown(0))
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
