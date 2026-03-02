using System.Collections;
using UnityEngine;

namespace UGESystem
{
    /// <summary>
    /// Handler for executing the <see cref="BubbleChatCommand"/>.
    /// <br/>
    /// <see cref="BubbleChatCommand"/> 실행을 담당하는 핸들러입니다.
    /// </summary>
    public class BubbleChatCommandHandler : ICommandHandler
    {
        public IEnumerator Execute(IGameEventCommand command, UGEGameEventController controller)
        {
            var bubbleCmd = command as BubbleChatCommand;
            if (bubbleCmd == null) yield break;

            // 1. Find the target interactable object in the world
            var interactable = controller.CharacterManager.GetInteractableByCharacterId(bubbleCmd.TargetCharacterId);
            if (interactable == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[BubbleChat] Could not find InteractableObject with CharacterID: {bubbleCmd.TargetCharacterId}");
#endif
                yield break;
            }

            // 2. Prepare UI & Characters: Hide VN elements to switch to world interaction
            controller.UIManager.HideDialogue();
            controller.CharacterManager.HideAllCharacters();

            // 3. Set global interaction state
            UGESystemController.Instance.IsInteracting = true;

            // 4. Handle NPC LookAt (Smooth rotation towards player)
            Transform npcTransform = interactable.transform;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Coroutine lookAtCoroutine = null;
            if (player != null)
            {
                lookAtCoroutine = controller.StartCoroutine(SmoothLookAt(npcTransform, player.transform.position));
            }

            // 5. Spawn and setup Bubble UI
            var bubblePrefab = controller.UIManager.BubbleChatPrefab;
            if (bubblePrefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[BubbleChat] BubbleChatPrefab is not assigned in UGEUIManager!");
#endif
                UGESystemController.Instance.IsInteracting = false;
                yield break;
            }

            Transform anchor = interactable.BubbleAnchor != null ? interactable.BubbleAnchor : npcTransform;
            UGEBubbleChatView bubbleInstance = UnityEngine.Object.Instantiate(bubblePrefab, anchor);
            
            bubbleInstance.transform.localPosition = Vector3.zero;
            bubbleInstance.transform.localRotation = Quaternion.identity;

            // 6. Setup tracking for completion
            bool isDone = false;
            System.Action cleanupAction = () => {
                if (lookAtCoroutine != null) controller.StopCoroutine(lookAtCoroutine);
                UGESystemController.Instance.IsInteracting = false;
                isDone = true;
            };

            bubbleInstance.Setup(anchor, bubbleCmd.Offset, bubbleCmd.Text, bubbleCmd.WaitUntilInput, cleanupAction);

            // 7. Handle waiting logic directly in the handler
            if (bubbleCmd.WaitUntilInput)
            {
                // Wait until the bubble chat is clicked through
                yield return new WaitUntil(() => isDone);
            }
            else
            {
                // If not waiting, just clean up state and proceed immediately
                UGESystemController.Instance.IsInteracting = false;
            }
        }

        private IEnumerator SmoothLookAt(Transform actor, Vector3 targetPos)
        {
            float rotationSpeed = 5f;
            while (true)
            {
                if (actor == null) yield break;
                Vector3 direction = (targetPos - actor.position).normalized;
                direction.y = 0; // Keep Y axis fixed

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    actor.rotation = Quaternion.Slerp(actor.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
                yield return null;
            }
        }
    }
}
