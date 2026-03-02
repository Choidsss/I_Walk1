using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGESystem
{
    /// <summary>
    /// The core executor for a single GameEvent. It processes a list of commands sequentially,
    /// using a strategy pattern to delegate execution to different handlers based on the event's context (e.g., Dialogue vs. Cinematic).
    /// </summary>
    public class UGEGameEventController : MonoBehaviour
    {
        /// <summary>
        /// Fired when a GameEvent has finished its execution.
        /// Passes the completed event.
        /// </summary>
        public static event Action<GameEvent> OnEventFinished;

        /// <summary>
        /// Reference to the UI Manager for displaying dialogue, choices, etc.
        /// </summary>
        public UGEUIManager UIManager { get; set; }
        /// <summary>
        /// Reference to the Character Manager for handling character display and animations.
        /// </summary>
        public UGECharacterManager CharacterManager { get; set; }
        /// <summary>
        /// Reference to the Camera Manager for handling camera movements and effects.
        /// </summary>
        public UGECameraManager CameraManager { get; set; }
        /// <summary>
        /// Reference to the Sound Manager for handling BGM and SFX.
        /// </summary>
        public UGESoundManager SoundManager { get; set; }
        /// <summary>
        /// Reference to the Input Manager for handling user input during events.
        /// </summary>
        public UGEInputManager InputManager { get; set; }
        
        /// <summary>
        /// Gets a value indicating whether a GameEvent is currently running.
        /// </summary>
        public bool IsEventRunning { get; private set; } = false;
        private bool _isSkipActive = false;
        /// <summary>
        /// Gets a value indicating whether the cinematic skip has been triggered.
        /// </summary>
        public bool IsSkipActive => _isSkipActive;
        
        private GameEvent _currentEvent;
        private Storyboard _currentStoryboard; // 현재 실행중인 스토리보드 컨텍스트
        /// <summary>
        /// Gets the storyboard currently being executed.
        /// </summary>
        public Storyboard CurrentStoryboard => _currentStoryboard;

        private int _commandIndex;
        private GameEventType _currentEventType;
        /// <summary>
        /// Gets the type of the event currently being executed.
        /// </summary>
        public GameEventType CurrentEventType => _currentEventType;
                
        /// <summary>
        /// Gets or sets a value indicating whether the controller is waiting for user input (e.g., for dialogue continuation or a choice).
        /// </summary>
        public bool IsWaitingForChoice { get; set; } = false;
        
        // Track the running coroutine to allow proper stopping if needed.
        private Coroutine _eventProcessCoroutine;

        private Dictionary<string, int> _labelMap;
        private Dictionary<GameEventType, Dictionary<Type, ICommandHandler>> _commandHandlers;
        private float _lastContinueTime = 0f;
        
        private void Awake()
        {
            InitializeCommandHandlers();
        }
        
        private void InitializeCommandHandlers()
        {
            // 모든 핸들러 인스턴스를 단 한 번만 생성하여 재사용합니다.
            var backgroundHandler = new DialogueNode_BackgroundCommandHandler();
            var characterHandler = new DialogueNode_CharacterCommandHandler();
            var choiceHandler = new DialogueNode_ChoiceCommandHandler();
            var dialogueHandler = new DialogueNode_DialogueCommandHandler();
            var endHandler = new DialogueNode_EndCommandHandler();
            var gotoHandler = new DialogueNode_GotoCommandHandler();
            var labelHandler = new DialogueNode_LabelCommandHandler();
            var cameraHandler = new UGECameraCommandHandler();
            var screenEffectHandler = new ScreenEffectCommandHandler();
            var playSoundHandler = new PlaySoundCommandHandler();
            var triggerEventHandler = new TriggerEventCommandHandler();
            var rewardHandler = new RewardCommandHandler();
            var characterUpdateHandler = new CharacterUpdateCommandHandler();
            var bubbleChatHandler = new BubbleChatCommandHandler();
            
            // 시네마틱 전용 다이얼로그 핸들러
            var cinematicDialogueHandler = new CinematicNode_DialogueCommandHandler();


            _commandHandlers = new Dictionary<GameEventType, Dictionary<Type, ICommandHandler>>
            {
                {
                    GameEventType.Dialogue, new Dictionary<Type, ICommandHandler>
                    {
                        { typeof(BackgroundCommand), backgroundHandler },
                        { typeof(CharacterCommand), characterHandler },
                        { typeof(ChoiceCommand), choiceHandler },
                        { typeof(DialogueCommand), dialogueHandler },
                        { typeof(EndCommand), endHandler },
                        { typeof(GotoCommand), gotoHandler },
                        { typeof(LabelCommand), labelHandler },
                        { typeof(UGECameraCommand), cameraHandler },
                        { typeof(ScreenEffectCommand), screenEffectHandler },
                        { typeof(PlaySoundCommand), playSoundHandler },
                        { typeof(TriggerEventCommand), triggerEventHandler },
                        { typeof(RewardCommand), rewardHandler },
                        { typeof(CharacterUpdateCommand), characterUpdateHandler },
                        { typeof(BubbleChatCommand), bubbleChatHandler },
                    }
                },
                {
                    GameEventType.CinematicText, new Dictionary<Type, ICommandHandler>
                    {
                        { typeof(BackgroundCommand), backgroundHandler },
                        { typeof(CharacterCommand), characterHandler },
                        { typeof(DialogueCommand), cinematicDialogueHandler },
                        { typeof(EndCommand), endHandler },
                        { typeof(UGECameraCommand), cameraHandler },
                        { typeof(ScreenEffectCommand), screenEffectHandler },
                        { typeof(PlaySoundCommand), playSoundHandler },
                        { typeof(TriggerEventCommand), triggerEventHandler },
                        { typeof(RewardCommand), rewardHandler },
                        { typeof(CharacterUpdateCommand), characterUpdateHandler },
                        { typeof(BubbleChatCommand), bubbleChatHandler },
                    }
                },
            };
        }
        
        /// <summary>
        /// Starts processing a given GameEvent.
        /// </summary>
        public void StartEvent(GameEvent gameEvent, GameEventType eventType, Storyboard storyboard)
        {
            // 1. Synchronous State Locking: Prevent re-entry immediately.
            if (IsEventRunning) return;
            
            // 2. Validation: Check if event is valid to run.
            if (gameEvent == null || gameEvent.Commands.Count == 0) return;

            // Lock the state immediately to block race conditions.
            IsEventRunning = true;
            _currentEvent = gameEvent;
            _currentStoryboard = storyboard;

            // --- Camera State Management ---
            // Capture initial camera state before any commands execute.
            if (CameraManager != null)
            {
                CameraManager.PrepareForEvent();
            }

            _eventProcessCoroutine = StartCoroutine(ProcessEventCoroutine(gameEvent, eventType, storyboard));
        }
        
        private IEnumerator ProcessEventCoroutine(GameEvent gameEvent, GameEventType eventType, Storyboard storyboard)
        {
            // Wait one frame to ensure initialization of managers or UI stability.
            // This yield is intentional to prevent input double-firing from previous nodes.
            yield return null;
        
            // Double-check validation (though StartEvent handles most).
            // If something went wrong during the yield, we must clean up.
            if (gameEvent == null) 
            {
                EndEvent(new EndCommand()); // Safety exit
                yield break;
            }

            // _currentEvent and _currentStoryboard are already set in StartEvent to maintain state locking.
            _currentEventType = eventType;
            _commandIndex = 0;
            IsWaitingForChoice = false;
            _isSkipActive = false;
        
            if (_currentEventType == GameEventType.CinematicText)
            {
                InputManager.OnSkipCinematic += SkipCinematicEvent;
                InputManager.EnableCinematicSkipListener(true);
            }
        
            BuildLabelMap();
        
            while (_commandIndex < _currentEvent.Commands.Count)
            {
                // External cancellation check: if IsEventRunning becomes false via external EndEvent, exit loop.
                if (!IsEventRunning) yield break;
        
                IGameEventCommand command = _currentEvent.Commands[_commandIndex];
                if (command == null)
                {
                    _commandIndex++;
                    continue;
                }
        
                Type commandType = command.GetType();
                if (!_commandHandlers.TryGetValue(_currentEventType, out var handlers))
                {
                    _commandIndex++;
                    continue;
                }
        
                if (handlers.TryGetValue(commandType, out var handler))
                {
                    yield return handler.Execute(command, this);
                    // Check again after handler execution to see if we were stopped.
                    if (!IsEventRunning) yield break;
                }
        
                if (IsWaitingForChoice)
                {
                    InputManager.EnableDialogueContinueListener(true);
                    yield return new WaitUntil(() => !IsWaitingForChoice);
                    InputManager.EnableDialogueContinueListener(false);
                    
                    // Add a small delay after waiting to prevent the same click from triggering the next command immediately.
                    // 대기 종료 후 짧은 지연을 추가하여 동일한 클릭이 다음 커맨드를 즉시 트리거하는 것을 방지합니다.
                    yield return null;
                }
        
                _commandIndex++;
            }
        
            EndEvent(new EndCommand());
        }
        
        public void ContinueEvent()
        {
            if (Time.time < _lastContinueTime + 0.2f) return;
            _lastContinueTime = Time.time;
            IsWaitingForChoice = false;
        }
        
        public void OnChoiceSelected(int choiceIndex)
        {
            var choiceCommand = _currentEvent.Commands[_commandIndex] as ChoiceCommand;
            if (choiceCommand == null) return;
        
            string targetLabel = choiceCommand.Choices[choiceIndex].TargetLabel;
            JumpToLabel(targetLabel);
            ContinueEvent();
        }
        
        public void JumpToLabel(string label)
        {
            if (_labelMap.TryGetValue(label, out int targetIndex)) _commandIndex = targetIndex;
            else _commandIndex++;
        }
        
        private void BuildLabelMap()
        {
            _labelMap = new Dictionary<string, int>();
            for (int i = 0; i < _currentEvent.Commands.Count; i++)
            {
                if (_currentEvent.Commands[i] is LabelCommand labelCommand)
                {
                    if (!string.IsNullOrEmpty(labelCommand.LabelName) && !_labelMap.ContainsKey(labelCommand.LabelName))
                        _labelMap.Add(labelCommand.LabelName, i);
                }
            }
        }
        
        private void SkipCinematicEvent()
        {
            if (_currentEventType == GameEventType.CinematicText) _isSkipActive = true;
        }
        
        /// <summary>
        /// Ends the current event, cleans up all system states, and kills the processing coroutine.
        /// Can be called naturally by the end of a command list or externally to force stop.
        /// </summary>
        public void EndEvent(EndCommand command)
        {
            // 0. Double-entry guard
            if (!IsEventRunning) return;

            // 1. Immediately stop input listening to prevent further interactions
            InputManager.EnableDialogueContinueListener(false);
            if (_currentEventType == GameEventType.CinematicText)
            {
                InputManager.OnSkipCinematic -= SkipCinematicEvent;
                InputManager.EnableCinematicSkipListener(false);
            }
            _isSkipActive = false;
                        
            // 2. BACKUP references before nullifying fields
            var finishedEvent = _currentEvent;
            var finishedStoryboard = _currentStoryboard;
            var coroutineToStop = _eventProcessCoroutine;
            
            // 3. CLEAN UP state immediately to allow next event to start without race
            IsEventRunning = false;
            _currentEvent = null;
            _currentStoryboard = null;
            _eventProcessCoroutine = null;

            // 4. NOTIFY listeners using backed-up references
            OnEventFinished?.Invoke(finishedEvent);

            bool isBranching = command != null && command.IsBranching;
            if (isBranching)
            {
                UGEDelayedEventBus.Publish(new JumpToNodeEvent(finishedStoryboard, command.TargetNodeID));
            }
                
            // 5. Reset Managers (Visual Cleanup) - Ensure we are in a clean state
            UIManager.HideAllUI();
            CharacterManager.HideAllCharacters();

            // --- CONDITIONAL CAMERA RESET ---
            // 분기(Branching) 중이 아닐 때만 카메라를 리셋하여 '골든 카메라'로 복구합니다.
            // 노드 간 전환 시에는 카메라 상태를 유지합니다.
            if (!isBranching)
            {
                CameraManager.ResetCamera();
            }
            
            if (UGESystemController.Instance != null)
            {
                if (UGESystemController.Instance.ScreenEffectManager != null)
                    UGESystemController.Instance.ScreenEffectManager.ClearEffect();
                
                // IMPORTANT: Reset global interaction state to prevent deadlock if interrupted during bubble chat or interaction.
                // 중요: 버블 챗이나 상호작용 도중 중단되었을 때의 조작 불능(Deadlock)을 방지하기 위해 전역 상호작용 상태를 초기화합니다.
                UGESystemController.Instance.IsInteracting = false;
            }

            // 6. NUCLEAR OPTION: Kill the coroutine stack immediately to prevent "ghost" logic execution.
            if (coroutineToStop != null)
            {
                StopCoroutine(coroutineToStop);
            }
        }
    }
}
