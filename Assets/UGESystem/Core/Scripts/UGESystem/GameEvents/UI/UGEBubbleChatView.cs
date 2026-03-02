using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UGESystem
{
    /// <summary>
    /// Manages a single bubble chat instance, handling its billboard effect, 
    /// text pagination, and user interaction.
    /// <br/>
    /// 단일 버블 챗 인스턴스를 관리하며, 빌보드 효과, 텍스트 페이지네이션 및 사용자 상호작용을 처리합니다.
    /// </summary>
    public class UGEBubbleChatView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _textComponent;
        [SerializeField] private Button _nextButton;
        [SerializeField] private GameObject _nextIcon;
        [SerializeField] private GameObject _doneIcon;

        [Header("Billboard Settings")]
        [SerializeField] private bool _billboardEnabled = true;

        private Canvas _canvas;
        private Camera _mainCam;
        private int _totalPages = 1;
        private int _currentPage = 1;
        private Action _onComplete;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            
            // Try to find any valid camera immediately
            FindAndAssignCamera();

            if (_nextButton != null)
            {
                _nextButton.onClick.AddListener(OnNextButtonClicked);
            }
        }

        private void FindAndAssignCamera()
        {
            if (_mainCam == null)
            {
                // 1. Try finding by MainCamera tag
                _mainCam = Camera.main;
                
                // 2. Fallback: Find any active camera in the scene if tag is missing or incorrect
                if (_mainCam == null)
                {
                    _mainCam = FindFirstObjectByType<Camera>();
                }
            }

            if (_canvas != null && _mainCam != null)
            {
                if (_canvas.renderMode == RenderMode.WorldSpace)
                {
                    _canvas.worldCamera = _mainCam;
                }
            }
        }

        /// <summary>
        /// Initializes the bubble chat with content and apply offset.
        /// </summary>
        public void Setup(Transform target, Vector3 offset, string text, bool waitUntilInput, Action onComplete)
        {
            // Apply offset as local position relative to the anchor (parent)
            transform.localPosition = offset;
            
            _textComponent.text = text;
            _onComplete = onComplete;

            // Reset state
            _currentPage = 1;
            _textComponent.pageToDisplay = _currentPage;
            
            // Force update to calculate page count
            _textComponent.ForceMeshUpdate();
            _totalPages = _textComponent.textInfo.pageCount;

            UpdateIcons();

            if (_nextButton != null)
            {
                _nextButton.gameObject.SetActive(waitUntilInput);
            }
        }

        private void LateUpdate()
        {
            // Ensure the world camera is assigned to the canvas if it's missing.
            // 캔버스의 worldCamera가 없거나 메인 카메라가 바뀐 경우를 대비해 지속적으로 체크합니다.
            if (_canvas != null && (_canvas.worldCamera == null || _mainCam == null))
            {
                FindAndAssignCamera();
            }

            if (_billboardEnabled && _mainCam != null)
            {
                // Simple billboard to face the camera in World Space
                transform.rotation = _mainCam.transform.rotation;
            }
        }

        private void OnNextButtonClicked()
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                _textComponent.pageToDisplay = _currentPage;
                UpdateIcons();
            }
            else
            {
                // All pages read, finalize.
                _onComplete?.Invoke();
                Destroy(gameObject);
            }
        }

        private void UpdateIcons()
        {
            bool hasMorePages = _currentPage < _totalPages;
            if (_nextIcon != null) _nextIcon.SetActive(hasMorePages);
            if (_doneIcon != null) _doneIcon.SetActive(!hasMorePages);
        }
    }
}
