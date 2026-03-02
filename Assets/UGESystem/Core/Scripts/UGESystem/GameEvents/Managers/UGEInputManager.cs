using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UGESystem
{
    /// <summary>
    /// Centralized manager for event-related input.
    /// It uses InputActionReferences to allow project-specific binding without code modification.
    /// <br/>
    /// 이벤트 관련 입력을 위한 중앙 관리자입니다.
    /// 코드 수정 없이 프로젝트별 바인딩이 가능하도록 InputActionReference를 사용합니다.
    /// </summary>
    public class UGEInputManager : MonoBehaviour
    {
        [Header("Dialogue Actions")]
        [Tooltip("Assign the action used to continue dialogue (e.g., Space, Mouse Left Click).")]
        [SerializeField] private InputActionReference _continueDialogueAction;
        
        [Tooltip("Assign the action used to skip cinematic sequences (e.g., ESC, Tab).")]
        [SerializeField] private InputActionReference _skipCinematicAction;

        [Header("Player Actions (Example)")]
        [Tooltip("Assign the action used for player interaction (e.g., F key, E key).")]
        [SerializeField] private InputActionReference _interactAction;

        /// <summary>
        /// Triggered when the continue dialogue action is performed.
        /// </summary>
        public event Action OnContinueDialogue;
        /// <summary>
        /// Triggered when the skip cinematic action is performed.
        /// </summary>
        public event Action OnSkipCinematic;
        /// <summary>
        /// Triggered when the interact action is performed.
        /// </summary>
        public event Action OnInteract;

        private bool _isContinueListenerActive = false;
        private bool _isSkipListenerActive = false;

        private void OnEnable()
        {
            if (_continueDialogueAction != null)
            {
                _continueDialogueAction.action.Enable();
                _continueDialogueAction.action.performed += HandleContinueDialogue;
            }

            if (_skipCinematicAction != null)
            {
                _skipCinematicAction.action.Enable();
                _skipCinematicAction.action.performed += HandleSkipCinematic;
            }

            if (_interactAction != null)
            {
                _interactAction.action.Enable();
                _interactAction.action.started += HandleInteract;
            }
        }

        private void OnDisable()
        {
            if (_continueDialogueAction != null)
            {
                _continueDialogueAction.action.performed -= HandleContinueDialogue;
            }

            if (_skipCinematicAction != null)
            {
                _skipCinematicAction.action.performed -= HandleSkipCinematic;
            }

            if (_interactAction != null)
            {
                _interactAction.action.started -= HandleInteract;
            }
        }

        public void EnableDialogueContinueListener(bool enable) => _isContinueListenerActive = enable;
        public void EnableCinematicSkipListener(bool enable) => _isSkipListenerActive = enable;

        public void TriggerContinueDialogue()
        {
            if (_isContinueListenerActive) OnContinueDialogue?.Invoke();
        }

        public void TriggerSkipCinematic()
        {
            if (_isSkipListenerActive) OnSkipCinematic?.Invoke();
        }

        private void HandleContinueDialogue(InputAction.CallbackContext context)
        {
            if (_isContinueListenerActive) OnContinueDialogue?.Invoke();
        }

        private void HandleSkipCinematic(InputAction.CallbackContext context)
        {
            if (_isSkipListenerActive) OnSkipCinematic?.Invoke();
        }

        private void HandleInteract(InputAction.CallbackContext context)
        {
            if (!UGESystemController.Instance.IsInteracting)
            {
                OnInteract?.Invoke();
            }
        }
    }
}
