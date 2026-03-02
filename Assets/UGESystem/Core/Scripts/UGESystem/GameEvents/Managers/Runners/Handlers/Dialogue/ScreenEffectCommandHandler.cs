using System;
using System.Collections;
using UnityEngine;

namespace UGESystem
{
    /// <summary>
    /// Command handler for <see cref="ScreenEffectCommand"/> that interacts with <see cref="UGEScreenEffectManager"/>
    /// to execute full-screen visual effects with optional scene cleanup at the peak of the effect.
    /// </summary>
    public class ScreenEffectCommandHandler : ICommandHandler
    {
        public IEnumerator Execute(IGameEventCommand genericCommand, UGEGameEventController controller)
        {
            var command = (ScreenEffectCommand)genericCommand;
            var screenEffectManager = UGESystemController.Instance.ScreenEffectManager;

            if (screenEffectManager == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ScreenEffectCommandHandler] UGEScreenEffectManager is not available.");
#endif
                yield break;
            }

            // Define the cleanup logic to be executed at the peak of the effect
            Action cleanupAction = () => {
                if (command.HideAll)
                {
                    controller.UIManager.HideAllUI();
                    controller.CharacterManager.HideAllCharacters();
                }
                else
                {
                    if (command.HideUI) controller.UIManager.HideDialogueAndChoices();
                    if (command.HideCharacters) controller.CharacterManager.HideAllCharacters();
                    if (command.HideBackground) controller.UIManager.HideBackground();
                }
            };

            switch (command.EffectType)
            {
                case ScreenEffectType.FadeOut:
                    // Standard fade to color
                    yield return screenEffectManager.FadeOut(command.TargetColor, command.Duration, cleanupAction);
                    break;

                case ScreenEffectType.FadeIn:
                    // Start from target color (fully covering) and fade to transparent
                    yield return screenEffectManager.FadeIn(command.TargetColor, command.Duration, cleanupAction);
                    break;
                
                case ScreenEffectType.Flash:
                    // Instant high-impact attack (fixed 0.02s) for snappy feel.
                    // Duration from the editor is used purely for the decay/afterimage time.
                    float attackTime = 0.02f; 
                    float decayTime = Mathf.Max(0.05f, command.Duration);
                    yield return screenEffectManager.Flash(command.TargetColor, attackTime, command.FlashHoldDuration, decayTime, cleanupAction);
                    break;

                case ScreenEffectType.Tint:
                    // Gradual transition into a color, hold, then return.
                    yield return screenEffectManager.Tint(command.TargetColor, command.Duration, command.FlashHoldDuration, cleanupAction);
                    break;

                default:
#if UNITY_EDITOR
                    Debug.LogWarning($"[ScreenEffectCommandHandler] Unknown ScreenEffectType: {command.EffectType}");
#endif
                    break;
            }
        }
    }
}
