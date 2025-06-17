using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Chat/ChatMessages", menuName = "UIToolkitDemo/ChatMessage", order = 6)]
    public class ChatSO : ScriptableObject
    {
        public string chatname;  // 聊天名称
        public string message;  // 消息
    }
}