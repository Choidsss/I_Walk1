using System;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UGESystem
{
    /// <summary>
    /// Command to display a bubble chat (speech bubble) above a character's head in the game world.
    /// 게임 월드 내 캐릭터의 머리 위에 말풍선(버블 챗)을 표시하는 커맨드입니다.
    /// </summary>
    [Serializable]
    public class BubbleChatCommand : EventCommand
    {
        [Header("Target")]
        [CharacterId]
        [SerializeField] private string _targetCharacterId;
        public string TargetCharacterId => _targetCharacterId;

        [Header("Content")]
        [TextArea(3, 10)]
        [SerializeField] private string _text;
        public string Text => _text;

        [Header("Settings")]
        [Tooltip("If true, the event waits for user input (clicking the 'Next' button) to proceed.")]
        [SerializeField] private bool _waitUntilInput = true;
        public bool WaitUntilInput => _waitUntilInput;

        [Tooltip("Optional offset to adjust the bubble's position relative to the anchor.")]
        [SerializeField] private Vector3 _offset = Vector3.zero;
        public Vector3 Offset => _offset;

        // Future extension: Bubble design types (e.g., normal, shout, thought)
        // [SerializeField] private BubbleType _bubbleType = BubbleType.Normal;

        public BubbleChatCommand()
        {
            CommandType = CommandType.BubbleChat;
        }

        public override IEventCommandDto ToDto()
        {
            return new BubbleChatCommandDto
            {
                Type = GetType().FullName + ", Assembly-CSharp",
                TargetCharacterId = _targetCharacterId,
                Text = _text,
                WaitUntilInput = _waitUntilInput,
                Offset = _offset
            };
        }
    }

    [Serializable]
    public class BubbleChatCommandDto : IEventCommandDto
    {
        [JsonProperty("$type")]
        public string Type { get; set; }
        public string TargetCharacterId { get; set; }
        public string Text { get; set; }
        public bool WaitUntilInput { get; set; }
        public Vector3 Offset { get; set; }

        public EventCommand ToCommand()
        {
            var command = new BubbleChatCommand();
            
            var type = typeof(BubbleChatCommand);
            type.GetField("_targetCharacterId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(command, TargetCharacterId);
            type.GetField("_text", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(command, Text);
            type.GetField("_waitUntilInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(command, WaitUntilInput);
            type.GetField("_offset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(command, Offset);

            return command;
        }
    }
}
