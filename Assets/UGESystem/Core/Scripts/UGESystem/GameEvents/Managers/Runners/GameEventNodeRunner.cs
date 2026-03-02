using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGESystem
{
    /// <summary>
    /// Implementation of <see cref="IEventNodeRunner"/> that starts a <see cref="GameEvent"/> asset
    /// within the <see cref="UGESystemController.Instance.GameEventController"/> and waits for its completion.
    /// </summary>
    public class GameEventNodeRunner : IEventNodeRunner
    {
        private Action<GameEvent> _onFinishHandler;

        /// <summary>
        /// Runs the game event associated with the given node.
        /// It starts the <see cref="GameEvent"/> using the <see cref="UGEGameEventController"/>
        /// and waits until the event is finished, reporting the result via the callback.
        /// </summary>
        /// <param name="node">The <see cref="EventNodeData"/> containing the <see cref="GameEvent"/> to run.</param>
        /// <param name="runner">The <see cref="UGEEventTaskRunner"/> that is managing this node's execution.</param>
        /// <param name="onComplete">Callback action to be invoked upon completion, providing the <see cref="NodeRunResult"/>.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        public IEnumerator Run(EventNodeData node, UGEEventTaskRunner runner, Action<NodeRunResult> onComplete)
        {
            if (node.GameEventAsset == null)
            {
#if UNITY_EDITOR
                Debug.LogError($"GameEventNodeRunner: Node '{node.Name}' has no GameEventAsset assigned.");
#endif
                onComplete(new NodeRunResult { Success = false });
                yield break;
            }

            UGESystemController.Instance.GameEventController.StartEvent(node.GameEventAsset, node.Type, runner.Storyboard);

            bool isEventDone = false;
            NodeRunResult result = new NodeRunResult();

            // Store the handler in a member field so it can be unsubscribed in Cleanup
            _onFinishHandler = (finishedEvent) =>
            {
                if (finishedEvent == node.GameEventAsset)
                {
                    isEventDone = true;
                }
            };
            
            UGEGameEventController.OnEventFinished += _onFinishHandler;
            yield return new WaitUntil(() => isEventDone);
            
            Cleanup();

            onComplete(result);
        }

        /// <summary>
        /// Unsubscribes from the static finished event to prevent memory leaks if the coroutine is interrupted.
        /// </summary>
        public void Cleanup()
        {
            if (_onFinishHandler != null)
            {
                UGEGameEventController.OnEventFinished -= _onFinishHandler;
                _onFinishHandler = null;
            }
        }
    }
}
