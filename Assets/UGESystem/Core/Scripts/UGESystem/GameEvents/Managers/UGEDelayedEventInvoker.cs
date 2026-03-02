using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGESystem
{
    /// <summary>
    /// A simple manager component that queues actions from the <see cref="UGEDelayedEventBus"/> and invokes them in <c>LateUpdate</c> to prevent race conditions.
    /// It uses a snapshot-based loop to prevent infinite recursion if events trigger each other.
    /// <br/>
    /// <see cref="UGEDelayedEventBus"/>의 액션을 큐에 넣고 <c>LateUpdate</c>에서 호출하여 경합 상태를 방지하는 관리자 컴포넌트입니다.
    /// 이벤트 간의 상호 트리거로 인한 무한 루프를 방지하기 위해 스냅샷 기반 루프를 사용합니다.
    /// </summary>
    public class UGEDelayedEventInvoker : MonoBehaviour
    {
        private readonly Queue<Action> _actionQueue = new Queue<Action>();

        private void LateUpdate()
        {
            // Capture the current count to process only events queued BEFORE this update starts.
            // This prevents infinite loops if an event handler publishes another event to this bus.
            // 이번 업데이트 시작 전에 쌓인 이벤트만 처리하기 위해 현재 개수를 캡처합니다.
            // 이는 이벤트 핸들러가 다시 이벤트를 발행할 때 발생할 수 있는 무한 루프를 방지합니다.
            int initialCount = _actionQueue.Count;

            for (int i = 0; i < initialCount; i++)
            {
                if (_actionQueue.Count > 0)
                {
                    _actionQueue.Dequeue()?.Invoke();
                }
            }
        }

        /// <summary>
        /// Adds an action to the queue to be invoked at the end of the current frame.
        /// 현재 프레임 끝에 호출될 액션을 큐에 추가합니다.
        /// </summary>
        /// <param name="action">The action to enqueue.</param>
        public void Enqueue(Action action)
        {
            _actionQueue.Enqueue(action);
        }
    }
}
