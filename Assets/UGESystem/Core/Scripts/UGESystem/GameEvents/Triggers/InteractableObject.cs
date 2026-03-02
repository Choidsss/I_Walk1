using UnityEngine;

namespace UGESystem
{
    /// <summary>
    /// Component placed on objects that can be interacted with by the player.
    /// It supports both raycast-based interaction and proximity-based interaction with smooth LookAt directing.
    /// <br/>
    /// 플레이어가 상호작용할 수 있는 오브젝트에 배치되는 컴포넌트로, 레이캐스트 방식과 근접 감지 방식(부드러운 시선 처리 포함)을 모두 지원합니다.
    /// </summary>
    public class InteractableObject : MonoBehaviour
    {
        [Header("Identity")]
        [Tooltip("Unique ID to be published to EventBus upon player interaction.")]
        [SerializeField] private string _interactionID;
        /// <summary>
        /// Gets the unique ID of this interactable object.
        /// 이 상호작용 가능한 오브젝트의 고유 ID를 가져옵니다.
        /// </summary>
        public string InteractionID => _interactionID;

        [Tooltip("Link this object to a character from the CharacterDatabase.")]
        [CharacterId]
        [SerializeField] private string _characterID;
        /// <summary>
        /// Gets the character ID associated with this object.
        /// 이 오브젝트와 연결된 캐릭터 ID를 가져옵니다.
        /// </summary>
        public string CharacterID => _characterID;

        [Header("Interaction Settings")]
        [Tooltip("Used to visually indicate the interactable distance.")]
        [SerializeField] private float _interactionRange = 2f;
        /// <summary>
        /// Gets the range within which interaction is possible.
        /// 상호작용이 가능한 범위를 가져옵니다.
        /// </summary>
        public float InteractionRange => _interactionRange;

        [Tooltip("If true, the object will detect player proximity and allow interaction via the 'Interact' key (F).")]
        [SerializeField] private bool _useProximityInteraction = false;

        [Tooltip("If true, the object will smoothly rotate to look at the player when within range.")]
        [SerializeField] private bool _lookAtPlayerInRange = false;

        [Tooltip("Speed of the smooth rotation.")]
        [SerializeField] private float _rotationSpeed = 5f;

        [Header("Bubble Chat Settings")]
        [Tooltip("The position where the bubble chat will appear. Typically above the character's head.")]
        [SerializeField] private Transform _bubbleAnchor;
        /// <summary>
        /// Gets the transform where the bubble chat should be anchored.
        /// 버블 챗이 표시될 위치(앵커)를 가져옵니다.
        /// </summary>
        public Transform BubbleAnchor => _bubbleAnchor;

        private Transform _playerTransform;
        private Quaternion _initialRotation;
        private bool _isPlayerInRange = false;

        private void Awake()
        {
            _initialRotation = transform.rotation;
        }

        private void OnEnable()
        {
            // Find player and cache reference.
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }

            // Subscribe to the global interaction input.
            if (UGESystemController.Instance.InputManager != null)
            {
                UGESystemController.Instance.InputManager.OnInteract += HandleProximityInteraction;
            }
        }

        private void OnDisable()
        {
            if (UGESystemController.HasInstance && UGESystemController.Instance.InputManager != null)
            {
                UGESystemController.Instance.InputManager.OnInteract -= HandleProximityInteraction;
            }
        }

        private void Update()
        {
            // Guard: Self-interaction protection for player object.
            if (gameObject.CompareTag("Player") || _playerTransform == null) return;

            // CRITICAL: If the system is currently playing an event (Dialogue, Bubble Chat, etc.), 
            // this script should NOT interfere with rotation at all.
            // 시스템이 현재 이벤트를 재생 중이라면, 이 스크립트는 회전에 일절 간섭하지 않아야 합니다.
            if (UGESystemController.Instance.IsInteracting || UGESystemController.Instance.GameEventController.IsEventRunning)
            {
                return; 
            }

            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            _isPlayerInRange = distance <= _interactionRange;

            // Handle smooth LookAt logic when NOT in a global event.
            if (_isPlayerInRange && _lookAtPlayerInRange)
            {
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                direction.y = 0; // Lock Y axis

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
                }
            }
            else
            {
                // Return to initial rotation smoothly when player leaves range.
                if (transform.rotation != _initialRotation)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, _initialRotation, Time.deltaTime * _rotationSpeed);
                }
            }
        }

        private void HandleProximityInteraction()
        {
            // Guard: Must use proximity, must have valid ID, must be in range, and not already interacting.
            if (!_useProximityInteraction || string.IsNullOrEmpty(_interactionID) || !_isPlayerInRange) return;
            if (UGESystemController.Instance.IsInteracting || UGESystemController.Instance.GameEventController.IsEventRunning) return;
            if (gameObject.CompareTag("Player")) return;

            TriggerInteraction();
        }

        private void TriggerInteraction()
        {
            UGEDelayedEventBus.Publish(new InteractionTriggeredEvent(_interactionID));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _interactionRange);

            if (_bubbleAnchor != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(_bubbleAnchor.position, 0.1f);
                Gizmos.DrawLine(transform.position, _bubbleAnchor.position);
            }
        }
    }
}
