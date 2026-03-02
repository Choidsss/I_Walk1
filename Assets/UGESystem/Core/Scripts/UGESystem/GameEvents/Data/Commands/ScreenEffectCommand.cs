using Newtonsoft.Json;
using UnityEngine;

namespace UGESystem
{
    /// <summary>
    /// Defines the types of screen effects that can be applied.
    /// </summary>
    public enum ScreenEffectType
    {
        /// <summary>Screen fades in from a color.</summary>
        FadeIn,
        /// <summary>Screen fades out to a color.</summary>
        FadeOut,
        /// <summary>Screen flashes with a color.</summary>
        Flash,
        /// <summary>Screen is tinted with a persistent color.</summary>
        Tint
    }

    /// <summary>
    /// A data transfer object (DTO) for <see cref="ScreenEffectCommand"/>, used for JSON serialization and deserialization.
    /// </summary>
    public class ScreenEffectCommandDto : IEventCommandDto
    {
        [JsonProperty] public ScreenEffectType EffectType { get; set; }
        [JsonProperty] public float Duration { get; set; }
        [JsonProperty] public Color TargetColor { get; set; }
        [JsonProperty] public float FlashHoldDuration { get; set; }

        // Transition Cleanup Flags
        [JsonProperty] public bool HideAll { get; set; }
        [JsonProperty] public bool HideUI { get; set; }
        [JsonProperty] public bool HideCharacters { get; set; }
        [JsonProperty] public bool HideBackground { get; set; }

        public EventCommand ToCommand()
        {
            return new ScreenEffectCommand(this);
        }
    }

    /// <summary>
    /// A command for controlling full-screen visual effects such as fades, flashes, and tints,
    /// with optional scene cleanup flags for transitions.
    /// </summary>
    [System.Serializable]
    [AvailableIn(GameEventType.Dialogue, GameEventType.CinematicText)]
    public class ScreenEffectCommand : EventCommand
    {
        [Header("Effect Settings")]
        [SerializeField] private ScreenEffectType _effectType;
        [JsonIgnore] public ScreenEffectType EffectType => _effectType;

        [Tooltip("Duration of the effect in seconds.")]
        [SerializeField] private float _duration = 1.0f;
        [JsonIgnore] public float Duration => _duration;
        
        [Tooltip("Target color for the effect. Alpha is used for Tint and FadeIn.")]
        [SerializeField] private Color _targetColor = Color.black;
        [JsonIgnore] public Color TargetColor => _targetColor;

        [Header("Flash & Tint Settings")]
        [SerializeField]
        [Tooltip("How long the effect stays at full intensity/color.")]
        private float _flashHoldDuration = 0.1f;
        [JsonIgnore] public float FlashHoldDuration => _flashHoldDuration;

        [Header("Transition Cleanup Options")]
        [Tooltip("Hides everything (UI, Characters, Background) at the peak of the effect.")]
        [SerializeField] private bool _hideAll;
        [JsonIgnore] public bool HideAll => _hideAll;

        [Tooltip("Hides the dialogue box and choice box.")]
        [SerializeField] private bool _hideUI;
        [JsonIgnore] public bool HideUI => _hideUI;

        [Tooltip("Hides all characters from the scene.")]
        [SerializeField] private bool _hideCharacters;
        [JsonIgnore] public bool HideCharacters => _hideCharacters;

        [Tooltip("Hides the currently active background.")]
        [SerializeField] private bool _hideBackground;
        [JsonIgnore] public bool HideBackground => _hideBackground;


        public ScreenEffectCommand()
        {
            CommandType = CommandType.ScreenEffect;
        }

        public ScreenEffectCommand(ScreenEffectCommandDto dto)
        {
            CommandType = CommandType.ScreenEffect;
            _effectType = dto.EffectType;
            _duration = dto.Duration;
            _targetColor = dto.TargetColor;
            _flashHoldDuration = dto.FlashHoldDuration;
            _hideAll = dto.HideAll;
            _hideUI = dto.HideUI;
            _hideCharacters = dto.HideCharacters;
            _hideBackground = dto.HideBackground;
        }

        public override IEventCommandDto ToDto()
        {
            return new ScreenEffectCommandDto
            {
                EffectType = _effectType,
                Duration = _duration,
                TargetColor = _targetColor,
                FlashHoldDuration = _flashHoldDuration,
                HideAll = _hideAll,
                HideUI = _hideUI,
                HideCharacters = _hideCharacters,
                HideBackground = _hideBackground
            };
        }
    }
}
