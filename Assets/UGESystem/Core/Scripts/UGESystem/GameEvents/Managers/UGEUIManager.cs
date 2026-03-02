using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace UGESystem
{
    /// <summary>
    /// Manages all UI elements related to the event system.
    /// This includes dialogue boxes, choice panels, cinematic text, and background displays.
    /// </summary>
    public class UGEUIManager : MonoBehaviour
    {
        [Header("Dialogue Elements")]
        [SerializeField] private GameObject _dialogueBox;
        [SerializeField] private TextMeshProUGUI _characterNameText;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private Button _continueButton;
        [Tooltip("Optional: Assign the adjuster component if you want to support text pagination.")]
        [SerializeField] private TMP_ContentSizeAdjuster _dialogueSizeAdjuster;
        
        [Tooltip("A standalone button for skipping/continuing events when dialogue UI is hidden.")]
        [SerializeField] private Button _globalSkipButton;

        [Header("Choice Elements")]
        [SerializeField] private GameObject _choiceBox;
        [SerializeField] private List<Button> _choiceButtons;

        [Header("Cinematic Text Elements")]
        [SerializeField] private GameObject _cinematicTextBox;
        [SerializeField] private TextMeshProUGUI _cinematicTextMesh;

        [Header("Background Elements")]
        [SerializeField] private RawImage _backgroundRawImage;
        [SerializeField] private VideoPlayer _backgroundVideoPlayer;
        [SerializeField] private RenderTexture _videoRenderTexture;

        [Header("Bubble Chat Elements")]
        [SerializeField] private UGEBubbleChatView _bubbleChatPrefab;
        /// <summary>
        /// Gets the prefab for creating bubble chat instances.
        /// </summary>
        public UGEBubbleChatView BubbleChatPrefab => _bubbleChatPrefab;

        private void Start()
        {
            if (_continueButton != null)
            {
                _continueButton.onClick.AddListener(() =>
                {
                    var inputManager = UGESystemController.Instance.InputManager;
                    if (inputManager != null)
                    {
                        inputManager.TriggerContinueDialogue();
                    }
                });
            }

            if (_globalSkipButton != null)
            {
                _globalSkipButton.onClick.AddListener(() =>
                {
                    var inputManager = UGESystemController.Instance.InputManager;
                    var eventController = UGESystemController.Instance.GameEventController;

                    if (inputManager != null && eventController != null)
                    {
                        if (eventController.CurrentEventType == GameEventType.CinematicText)
                        {
                            inputManager.TriggerSkipCinematic();
                        }
                        else
                        {
                            inputManager.TriggerContinueDialogue();
                        }
                    }
                });
                _globalSkipButton.gameObject.SetActive(false);
            }

            if (_backgroundRawImage != null) _backgroundRawImage.gameObject.SetActive(false);
            if (_backgroundVideoPlayer != null) _backgroundVideoPlayer.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            var inputManager = UGESystemController.Instance.InputManager;
            if (inputManager != null)
            {
                inputManager.OnContinueDialogue += OnContinueClicked;
            }

            if (_backgroundVideoPlayer != null)
            {
                _backgroundVideoPlayer.prepareCompleted += OnVideoPrepared;
            }
        }

        private void OnDisable()
        {
            if (UGESystemController.HasInstance)
            {
                var inputManager = UGESystemController.Instance.InputManager;
                if (inputManager != null)
                {
                    inputManager.OnContinueDialogue -= OnContinueClicked;
                }
            }

            if (_backgroundVideoPlayer != null)
            {
                _backgroundVideoPlayer.prepareCompleted -= OnVideoPrepared;
            }
        }
        
        private void OnContinueClicked()
        {
            if (UGESystemController.Instance.IsInteracting)
            {
                return;
            }

            bool isDialogueActive = _dialogueBox != null && _dialogueBox.activeInHierarchy;
            bool isContinueActive = _continueButton != null && _continueButton.gameObject.activeInHierarchy;
            bool isSkipActive = _globalSkipButton != null && _globalSkipButton.gameObject.activeInHierarchy;

            if (!isDialogueActive && !isContinueActive && !isSkipActive)
            {
                return;
            }

            if (_dialogueSizeAdjuster != null)
            {
                if (_dialogueSizeAdjuster.TryShowNextPage())
                {
                    return;
                }
            }

            UGESystemController.Instance.GameEventController.ContinueEvent();
        }

        public void ShowDialogue(string characterName, string dialogue)
        {
            if(_dialogueBox == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("UIManager's 'Dialogue Box' field is empty! Please check the object in the inspector.");
#endif
                return;
            }

            if(_choiceBox != null) _choiceBox.SetActive(false);
            if(_dialogueBox != null) _dialogueBox.SetActive(true);
            if (_continueButton != null) _continueButton.gameObject.SetActive(true);
            if (_globalSkipButton != null) _globalSkipButton.gameObject.SetActive(false);

            if(_characterNameText != null) _characterNameText.text = characterName;
            if(_dialogueText != null) _dialogueText.text = dialogue;
        }

        /// <summary>
        /// Explicitly hides the dialogue box UI.
        /// </summary>
        public void HideDialogue()
        {
            if (_dialogueBox != null) _dialogueBox.SetActive(false);
            if (_continueButton != null) _continueButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Explicitly hides the choice box UI.
        /// </summary>
        public void HideChoices()
        {
            if (_choiceBox != null) _choiceBox.SetActive(false);
        }

        /// <summary>
        /// Integrated cleanup for both dialogue and choice UI elements.
        /// </summary>
        public void HideDialogueAndChoices()
        {
            HideDialogue();
            HideChoices();
        }

        public void ShowChoices(List<ChoiceOption> choices, Action<int> onChoiceSelected)
        {
            if (_continueButton != null) _continueButton.gameObject.SetActive(false);
            if (_dialogueBox != null) _dialogueBox.SetActive(false);
            if (_choiceBox != null) _choiceBox.SetActive(true);

            for (int i = 0; i < _choiceButtons.Count; i++)
            {
                if (i < choices.Count)
                {
                    _choiceButtons[i].gameObject.SetActive(true);
                    
                    var buttonTexts = _choiceButtons[i].GetComponentsInChildren<TextMeshProUGUI>();

                    foreach(var btntext in buttonTexts)
                    {
                        if (btntext != null)
                        {
                            btntext.text = choices[i].Text;
                        }
                    }

                    int choiceIndex = i;
                    _choiceButtons[i].onClick.RemoveAllListeners();
                    _choiceButtons[i].onClick.AddListener(() =>
                    {
                        onChoiceSelected?.Invoke(choiceIndex);
                    });
                }
                else
                {
                    _choiceButtons[i].gameObject.SetActive(false);
                }
            }
        }

        public System.Collections.IEnumerator ShowCinematicText(string text, float animationDuration = 0.5f)
        {
            if (_cinematicTextBox == null || _cinematicTextMesh == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("UIManager's 'Cinematic Text' field is empty! Please check the object in the inspector.");
#endif
                yield break;
            }
            
            if(_dialogueBox != null) _dialogueBox.SetActive(false);
            if(_choiceBox != null) _choiceBox.SetActive(false);
            if (_continueButton != null) _continueButton.gameObject.SetActive(false);
            
            if (_globalSkipButton != null) _globalSkipButton.gameObject.SetActive(true);

            _cinematicTextBox.SetActive(true);
            _cinematicTextMesh.text = text;

            Canvas.ForceUpdateCanvases();
            yield return null;

            RectTransform textRect = _cinematicTextMesh.rectTransform;
            Vector2 finalPosition = textRect.anchoredPosition;
            Vector2 startPosition = finalPosition - new Vector2(0, _cinematicTextMesh.preferredHeight + 10); 

            textRect.anchoredPosition = startPosition;
            
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                if (UGESystemController.Instance.GameEventController.IsSkipActive)
                {
                    break;
                }

                float t = elapsedTime / animationDuration;
                float easedT = 1 - Mathf.Pow(1 - t, 3); // Cubic ease-out
                
                textRect.anchoredPosition = Vector2.Lerp(startPosition, finalPosition, easedT);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            textRect.anchoredPosition = finalPosition;
        }

        public void HideCinematicText()
        {
            if (_cinematicTextBox != null)
            {
                _cinematicTextBox.SetActive(false);
            }
        }

        public void HideAllUI()
        {
            if (_dialogueBox != null) _dialogueBox.SetActive(false);
            if (_choiceBox != null) _choiceBox.SetActive(false);
            if (_continueButton != null) _continueButton.gameObject.SetActive(false);
            if (_globalSkipButton != null) _globalSkipButton.gameObject.SetActive(false);
            if (_cinematicTextBox != null) _cinematicTextBox.SetActive(false); 
            HideBackground();
            ClearAllBubbleChats();
        }

        public void ClearAllBubbleChats()
        {
            var activeBubbles = FindObjectsByType<UGEBubbleChatView>(FindObjectsSortMode.None);
            foreach (var bubble in activeBubbles)
            {
                if (bubble != null && bubble.gameObject != null)
                {
                    Destroy(bubble.gameObject);
                }
            }
        }

        public void SetModeBackgroundWait()
        {
            if (_dialogueBox != null) _dialogueBox.SetActive(false);
            if (_choiceBox != null) _choiceBox.SetActive(false);
            if (_continueButton != null) _continueButton.gameObject.SetActive(false);
            if (_cinematicTextBox != null) _cinematicTextBox.SetActive(false);
            
            if (_globalSkipButton != null) _globalSkipButton.gameObject.SetActive(true);
        }

        public void ShowImageBackground(Texture2D image)
        {
            if (_backgroundRawImage == null) return;

            if (_backgroundVideoPlayer != null)
            {
                _backgroundVideoPlayer.Stop();
                _backgroundVideoPlayer.gameObject.SetActive(false);
            }

            _backgroundRawImage.texture = image;
            _backgroundRawImage.gameObject.SetActive(true);
        }

        public void PlayVideoBackground(VideoClip video)
        {
            if (_backgroundRawImage == null || _backgroundVideoPlayer == null || _videoRenderTexture == null) return;

            ClearVideoRenderTexture();

            _backgroundRawImage.gameObject.SetActive(false);
            _backgroundVideoPlayer.gameObject.SetActive(true);
            
            _backgroundRawImage.texture = _videoRenderTexture;
            _backgroundVideoPlayer.clip = video;
            _backgroundVideoPlayer.targetTexture = _videoRenderTexture;
            
            _backgroundVideoPlayer.Prepare();
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            if (source == _backgroundVideoPlayer)
            {
                if (_backgroundRawImage != null) _backgroundRawImage.gameObject.SetActive(true);
                source.Play();
            }
        }

        private void ClearVideoRenderTexture()
        {
            if (_videoRenderTexture == null) return;

            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = _videoRenderTexture;
            GL.Clear(true, true, Color.black);
            RenderTexture.active = rt;
        }

        public void HideBackground()
        {
            if (_backgroundRawImage != null)
            {
                _backgroundRawImage.gameObject.SetActive(false);
            }
            if (_backgroundVideoPlayer != null)
            {
                _backgroundVideoPlayer.Stop();
                _backgroundVideoPlayer.gameObject.SetActive(false);
            }
            
            ClearVideoRenderTexture();
        }
    }
}
