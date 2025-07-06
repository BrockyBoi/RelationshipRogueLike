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
using ShootYourShotGame;
using FireFighting;

namespace Dialogue
{
    public class ConversationManager : MonoBehaviour
    {
        public static ConversationManager Instance { get; private set; }

        [SerializeField]
        private bool _playCustomLevel = false;

        [SerializeField, HideIf("@_playCustomLevel")]
        private ELevel _levelToPlay = ELevel.None;

        [SerializeField, ShowIf("@_playCustomLevel")]
        private LevelConversationData _conversationData;
        public LevelConversationData ConversationData { get { return _conversationData; } }

        private Coroutine _conversationCoroutine;
        private Conversation _currentConversation;
        private Conversation _newConversationOnFinishDialogue;

        private bool _playerHasDied = false;

        [SerializeField]
        private bool _runDialogueOnStart = false;

        private bool _isInGame = false;

        private int _currentDialogueIndex = 0;
        private int _currentConversationObjectIndex = 0;

        private List<StandardDialogueObject> _dialogueObjectsThisConversation;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
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

            if (_playCustomLevel)
            {
                _levelToPlay = ELevel.None;
            }

            if (_runDialogueOnStart)
            {
                StartConversation();
            }
        }

        private void Update()
        {
            if (_isInGame)
            {
                return;
            }

            bool pressedNext = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space);
            bool pressedBack = Input.GetKeyDown(KeyCode.LeftArrow);

            if (pressedNext)
            {
                PressedNextDialogue();
            }
            else if (pressedBack)
            {
                PressedPreviousDialogue();
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
            SetConversationsForLevel();
            if (ensure(_conversationData != null, "No conversation data present"))
            {
                _dialogueObjectsThisConversation = new List<StandardDialogueObject>();

                _currentDialogueIndex = -1;
                StartNewConversation(_conversationData.ConversationToRun);
            }
        }

        private void StartNewConversation(Conversation conversation)
        {
            if (ensure(conversation != null && conversation.DialogueObjects.Count > 0, "Conversation is not valid"))
            {
                _currentConversation = conversation;
                _currentConversationObjectIndex = 0;
                _conversationCoroutine = StartCoroutine(ProcessDialogueObject(_currentConversation.DialogueObjects[_currentConversationObjectIndex]));//StartCoroutine(ProcessConversation(_conversationData.ConversationToRun));
            }
        }

        private void OnPlayerDeath()
        {
            _playerHasDied = true;
        }

        private void SetConversationsForLevel()
        {
            if (ensure(LevelDataManager.Instance != null, "Level Data Manager is null"))
            {
                LevelConversationData levelConversationData = LevelDataManager.Instance.GetLevelConversationData(_levelToPlay);
                if (ensure(levelConversationData != null && levelConversationData.ConversationToRun != null && levelConversationData.ConversationOnPlayerDeath != null, "Level Conversation Data has invalid elements"))
                {
                    _conversationData = levelConversationData;
                    AudioManager.Instance.PlayBackgroundMusic(levelConversationData.BackgroundMusicOnStart);
                }
            }
        }

        private IEnumerator ProcessDialogueObject(DialogueObject dialogueObject)
        {
            Player player = Player.Instance;
            if (!player)
            {
                Debug.LogError("Player is null");
                yield break;
            }

            //foreach (DialogueObject dialogueObject in dialogueObject)
            //{
            if (_playerHasDied)
            {
                _playerHasDied = false;
                StartNewConversation(_conversationData.ConversationOnPlayerDeath);
                //_newConversationOnFinishDialogue = _conversationData.ConversationOnPlayerDeath;
                //yield return StartCoroutine(ProcessConversation(_conversationData.ConversationOnPlayerDeath));
                yield break;
            }

            switch (dialogueObject.DialogueObjectType)
            {
                case EDialogueObjectType.StandardDialogue:
                    {
                        AddAndDisplayNewDialogue(dialogueObject.StandardDialogueObjects);
                        break;
                    }
                case EDialogueObjectType.SentimentDialogue:
                    {
                        ECharacterSentiment sentiment = Player.Instance.HealthComponent.GetCharacterSentiment();
                        if (dialogueObject.SentimentDialogueObject.SentimentDialogue.ContainsKey(sentiment))
                        {
                            ProcessBranchingDialogueObject(dialogueObject.SentimentDialogueObject.SentimentDialogue[sentiment]);
                        }
                        else
                        {
                            PressedNextDialogue();
                        }
                        break;
                    }
                case EDialogueObjectType.SpawnMaze:
                    {
                        RunGame<MazeGenerator, MazeSolverComponent, MazeGeneratorData, MazeCompletionResult>(EGameType.Maze, dialogueObject.MazeSpawnerData);
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

                        RunGame<MemoryGameGenerator, MemoryGameSolverComponent, MemoryGameGeneratorData, MemoryGameCompletionResult>(EGameType.Memory, dialogueObject.MemoryGameSpawnerData);

                        yield return YieldUntilGameIsInFinishedStage(MemoryGameSolverComponent.Instance);
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
                        break;
                    }
                case EDialogueObjectType.SpawnWhackAMole:
                    {
                        RunGame<WhackAMoleGenerator, WhackAMoleSolver, WhackAMoleGenerationData, WhackAMoleCompletionResult>(EGameType.WhackAMole, dialogueObject.WhackAMoleGameSpawnerData);
                        break;
                    }
                case EDialogueObjectType.SpawnButterflyCatching:
                    {
                        RunGame<CatchingButterfliesGenerator, CatchingButterfliesSolver, CatchingButterfliesGenerationData, CatchingButterfliesCompletionResult>(EGameType.CatchingButterflies, dialogueObject.CatchingButterflyGameSpawnerData);
                        break;
                    }
                case EDialogueObjectType.SpawnEndlessRunner:
                    {
                        RunGame<EndlessRunnerGenerator, EndlessRunnerSolver, EndlessRunnerGenerationData, EndlessRunnerCompletionResult>(EGameType.EndlessRunner, dialogueObject.EndlessRunnerSpawnerData);
                        break;
                    }
                case EDialogueObjectType.SpawnShootYourShot:
                    {
                        RunGame<ShootYourShotGameGenerator, ShootYourShotGameSolver, ShootYourShotGameGenerationData, ShootYourShotGameCompletionResult>(EGameType.ShootYourShot, dialogueObject.ShootYourShotSpawnerData);
                        break;
                    }
                case EDialogueObjectType.SpawnFireFighting:
                    {
                        RunGame<FireFightingGenerator, FireFightingSolver, FireFightingGenerationData, FireFightingCompletionResult>(EGameType.FireFighting, dialogueObject.FireFightingSpawnerData);
                        break;
                    }
                case EDialogueObjectType.SetBackgroundMusic:
                    {
                        AudioManager.Instance.PlayBackgroundMusic(dialogueObject.SetBackgroundMusicData.AudioClipToPlay);
                        break;
                    }
                case EDialogueObjectType.PlaySoundEffect:
                    {
                        AudioManager.Instance.PlaySoundEffect(dialogueObject.SetBackgroundMusicData.AudioClipToPlay);
                        break;
                    }
                case EDialogueObjectType.EndConversation:
                    {
                        if (ensure(LevelDataManager.Instance != null, "Level Data Manager is null"))
                        {
                            LevelDataManager.Instance.CompleteCurrentLevel();
                        }

                        if (ensure(GameSceneManager.Instance != null, "Game Scene manager is null"))
                        {
                            GameSceneManager.Instance.LoadMapLevel();
                        }
                        yield break;
                    }
                case EDialogueObjectType.LinkNewConversation:
                    {
                        StartNewConversation(dialogueObject.LinkedConverssationObject.NewConversation);
                        break;
                    }
                default:
                    break;
            }
            //}
        }

        private IEnumerator ProcessStandardGameLogic<GeneratorType, SolverType, GenerationDataType, CompletionResultType>(EGameType gameType, GenerationDataType generationData) 
            where GeneratorType : DialogueCreatedGameGenerator<SolverType, GenerationDataType, CompletionResultType> 
            where SolverType : GameSolverComponent<GenerationDataType, CompletionResultType> 
            where GenerationDataType : GameGenerationData<CompletionResultType>
            where CompletionResultType : GameCompletionResult, new()
        {
            _isInGame = true;
            SolverType solver;
            GeneratorType generator;
            MiniGameControllersManager.Instance.GetBothControllers(out solver, out generator, gameType);

            generator.GenerateGame(generationData);
            yield return YieldUntilGameIsInFinishedStage(solver);
            yield return new WaitForSeconds(2.5f);
            _isInGame = false;
            yield return ProcessGameResult(solver.GetCurrentCompletionResult());
        }

        //private void ProcessConversation(Conversation conversation)
        //{
        //    yield return ProcessDialogueObject(conversation.DialogueObjects);
        //}

        private IEnumerator ProcessStandardDialogueObjects(List<StandardDialogueObject> dialogueObjects)
        {
            AddAndDisplayNewDialogue(dialogueObjects);
            //foreach (StandardDialogueObject dialogueObject in dialogueObjects)
            //{
            //    yield return ProcessStandardDialogueObject(dialogueObject);
            //}
            yield break;
        }

        private IEnumerator ProcessStandardDialogueObject(StandardDialogueObject dialogueObject)
        {
            if (dialogueObject.GetDialogueString() != string.Empty)
            {
                DialogueUI.Instance.ShowDialogue(dialogueObject);
                yield return YieldUntilInput();
            }
        }

        private void ProcessBranchingDialogueObject(BranchingDialogueObject branchingDialogue)
        {
            if (branchingDialogue != null)
            {
                AddAndDisplayNewDialogue(branchingDialogue.DialogueObjects);

                if (!branchingDialogue.OnlyUsesDialogue)
                {
                    _newConversationOnFinishDialogue = branchingDialogue.NewConversation;
                    //yield return ProcessConversation(branchingDialogue.NewConversation);
                }
            }
        }

        private IEnumerator ProcessGameResult(GameCompletionResult gameResult)
        {
            yield return ProcessResultDialogueIntoCharacterDialogue();

            AddAndDisplayNewDialogue(gameResult.BranchingDialogue.DialogueObjects);
            if (!gameResult.BranchingDialogue.OnlyUsesDialogue)
            {
                _newConversationOnFinishDialogue = gameResult.BranchingDialogue.NewConversation;
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

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            yield return new WaitForEndOfFrame();
        }

        private IEnumerator YieldUntilGameIsInFinishedStage(BaseGameSolverComponent gameSolver)
        {
            yield return new WaitUntil(() => gameSolver && gameSolver.IsStage(EGameStage.GameFinished));
        }

        public void PressedNextDialogue()
        {
            if (_currentDialogueIndex < _dialogueObjectsThisConversation.Count - 1)
            {
                _currentDialogueIndex++;
                DisplayCurrentDialogue();
            }
            else if (_newConversationOnFinishDialogue == null && ensure(_currentConversation.DialogueObjects.IsValidIndex(_currentConversationObjectIndex + 1), (_currentConversationObjectIndex + 1) + " is out of index for the dialogue object count"))
            {
                _currentConversationObjectIndex++;
                StartCoroutine(ProcessDialogueObject(_currentConversation.DialogueObjects[_currentConversationObjectIndex]));
            }
            else if (_newConversationOnFinishDialogue != null)
            {
                Conversation conversation = _newConversationOnFinishDialogue;
                _newConversationOnFinishDialogue = null;
                StartNewConversation(conversation);
            }
        }

        public void PressedPreviousDialogue()
        {
            _currentDialogueIndex = Mathf.Max(0, _currentDialogueIndex - 1);
            DisplayCurrentDialogue();
        }

        private void DisplayCurrentDialogue()
        {
            if (ensure(_dialogueObjectsThisConversation.IsValidIndex(_currentDialogueIndex), _currentDialogueIndex + " is not valid index out of " + _dialogueObjectsThisConversation.Count + " dialogue objects"))
            {
                DialogueUI.Instance.ShowDialogue(_dialogueObjectsThisConversation[_currentDialogueIndex]);
            }
        }

        private void AddDialogueObjects(List<StandardDialogueObject> dialogueObjects)
        {
            _dialogueObjectsThisConversation.AddRange(dialogueObjects);
        }

        private void AddAndDisplayNewDialogue(List<StandardDialogueObject> dialogueObjects)
        {
            if (dialogueObjects.Count > 0)
            {
                _currentDialogueIndex++;
                AddDialogueObjects(dialogueObjects);
                DisplayCurrentDialogue();

            }
        }

        public void RunGame<GeneratorType, SolverType, GenerationDataType, CompletionResultType>(EGameType gameType, GenerationDataType generationData)
            where GeneratorType : DialogueCreatedGameGenerator<SolverType, GenerationDataType, CompletionResultType>
            where SolverType : GameSolverComponent<GenerationDataType, CompletionResultType>
            where GenerationDataType : GameGenerationData<CompletionResultType>
            where CompletionResultType : GameCompletionResult, new()
        {
            _isInGame = true;
            SolverType solver;
            GeneratorType generator;
            MiniGameControllersManager.Instance.GetBothControllers(out solver, out generator, gameType);
            System.Action callback = null;
            callback = () => 
            {
                solver.OnGameStop -= callback;
                PotentialPlayerDialogueUI.Instance.DestroyAllDialogueOptions();
                CompletionResultType completionResult = solver.GetCurrentCompletionResult();
                if (completionResult.BranchingDialogue.DialogueObjects.Count > 0)
                {
                    AddAndDisplayNewDialogue(completionResult.BranchingDialogue.DialogueObjects);
                }
                else
                {
                    PressedNextDialogue();
                }

                _isInGame = false;

                if (!completionResult.BranchingDialogue.OnlyUsesDialogue)
                {
                    _newConversationOnFinishDialogue = completionResult.BranchingDialogue.NewConversation;
                }
            };
            solver.OnGameStop += callback;

            generator.GenerateGame(generationData);
        }
    }
}
