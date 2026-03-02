using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UGESystem
{
    /// <summary>
    /// Manager responsible for creating a persistent and high-priority UI canvas to render full-screen visual effects
    /// such as fades, flashes, and tints.
    /// <br/>
    /// 페이드, 플래시, 틴트와 같은 전체 화면 효과를 렌더링하기 위해 영구적이고 우선순위가 높은 UI 캔버스를 생성하는 관리자입니다.
    /// </summary>
    public class UGEScreenEffectManager : MonoBehaviour
    {
        private Image _overlayImage;
        private Coroutine _currentEffectCoroutine;

        /// <summary>
        /// Gets the current color of the overlay image.
        /// </summary>
        public Color CurrentImageColor => _overlayImage != null ? _overlayImage.color : Color.clear;

        private void Awake()
        {
            SetupOverlayImage();
        }

        private void SetupOverlayImage()
        {
            GameObject canvasGO = new GameObject("UGEScreenEffectCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; 

            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject imageGO = new GameObject("OverlayImage");
            imageGO.transform.SetParent(canvasGO.transform);
            
            _overlayImage = imageGO.AddComponent<Image>();
            _overlayImage.color = new Color(0, 0, 0, 0); 
            _overlayImage.raycastTarget = false;

            RectTransform rectTransform = imageGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            DontDestroyOnLoad(canvasGO);
        }

        // --- Easing Utilities ---
        private float EaseOutQuad(float t) => t * (2 - t);
        private float EaseInQuad(float t) => t * t;
        private float EaseInOutQuad(float t) => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;

        /// <summary>
        /// Fades the screen from the current color to transparent.
        /// </summary>
        /// <param name="onPeakReached">Optional callback executed at the START of FadeIn (when screen is obscured).</param>
        public IEnumerator FadeIn(Color fromColor, float duration, Action onPeakReached = null)
        {
            if (_overlayImage == null) yield break;

            // FadeIn peak is the very beginning.
            if (fromColor.a > 0) _overlayImage.color = fromColor;
            onPeakReached?.Invoke();

            Color startColor = _overlayImage.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0);
            
            yield return LerpColor(startColor, targetColor, duration, EaseOutQuad);
        }

        /// <summary>
        /// Fades the screen from the current color to a target color.
        /// </summary>
        /// <param name="onPeakReached">Optional callback executed at the END of FadeOut (when screen is obscured).</param>
        public IEnumerator FadeOut(Color targetColor, float duration, Action onPeakReached = null)
        {
            if (_overlayImage == null) yield break;
            yield return LerpColor(_overlayImage.color, targetColor, duration, EaseInQuad);
            onPeakReached?.Invoke();
        }

        /// <summary>
        /// Flashes the screen: Rapid attack, brief hold, then smooth decay.
        /// </summary>
        /// <param name="onPeakReached">Optional callback executed at the PEAK of the flash.</param>
        public IEnumerator Flash(Color flashColor, float attackDuration, float holdDuration, float decayDuration, Action onPeakReached = null)
        {
            if (_overlayImage == null) yield break;

            // 1. Attack
            yield return LerpColor(_overlayImage.color, flashColor, attackDuration, EaseInQuad);

            // Peak Reached
            onPeakReached?.Invoke();

            // 2. Hold
            if (holdDuration > 0) yield return new WaitForSeconds(holdDuration);

            // 3. Decay
            Color transparent = new Color(flashColor.r, flashColor.g, flashColor.b, 0);
            yield return LerpColor(flashColor, transparent, decayDuration, EaseOutQuad);
        }

        /// <summary>
        /// Tints the screen: Transitions to color, holds it, then restores to original.
        /// </summary>
        /// <param name="onPeakReached">Optional callback executed when the tint transition is complete.</param>
        public IEnumerator Tint(Color tintColor, float transitionDuration, float holdDuration, Action onPeakReached = null)
        {
            if (_overlayImage == null) yield break;

            Color originalColor = _overlayImage.color;

            // 1. Transition to Tint
            yield return LerpColor(originalColor, tintColor, transitionDuration, EaseInOutQuad);

            // Peak Reached (fully tinted)
            onPeakReached?.Invoke();

            // 2. Hold
            if (holdDuration > 0) yield return new WaitForSeconds(holdDuration);

            // 3. Restore
            yield return LerpColor(tintColor, originalColor, transitionDuration, EaseInOutQuad);
        }

        private IEnumerator LerpColor(Color start, Color end, float duration, System.Func<float, float> easingFunc)
        {
            if (duration <= 0)
            {
                _overlayImage.color = end;
                yield break;
            }

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                if (easingFunc != null) t = easingFunc(t);
                
                _overlayImage.color = Color.Lerp(start, end, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _overlayImage.color = end;
        }

        public void ClearEffect()
        {
            if (_currentEffectCoroutine != null)
            {
                StopCoroutine(_currentEffectCoroutine);
                _currentEffectCoroutine = null;
            }
            if (_overlayImage == null) return;
            _overlayImage.color = new Color(0, 0, 0, 0);
        }
    }
}
