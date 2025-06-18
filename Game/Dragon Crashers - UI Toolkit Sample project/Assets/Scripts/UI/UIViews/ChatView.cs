using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

namespace UIToolkitDemo
{
    /// <summary>
    /// 在主屏幕上显示文本消息以模拟聊天窗口的UI。
    /// </summary>
    public class ChatView : UIView
    {
        // 使用异步等待而不是协程（非MonoBehaviour）
        const int k_DelayBetweenKeys = 20; // 毫秒
        const int k_DelayBetweenLines = 1000; // 毫秒

        Label m_ChatText;

        // 聊天名称颜色
        const string k_TagOpen = "<color=green>";
        const string k_TagClose = "</color>";

        public ChatView(VisualElement topElement) : base(topElement)
        {
            // 当聊天窗口显示时触发事件
            HomeEvents.ChatWindowShown += OnShowChats;
        }

        public override void Dispose()
        {
            base.Dispose();
            // 取消订阅聊天窗口显示事件
            HomeEvents.ChatWindowShown -= OnShowChats;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            // 获取聊天文本的Label组件
            m_ChatText = m_TopElement.Q<Label>("home-chat__text");
        }

        void OnShowChats(List<ChatSO> chatData)
        {
            if (m_ChatText == null)
                return;

            // 启动异步聊天任务
            _ = ChatRoutine(chatData);
        }

        // 遍历聊天消息并逐个显示
        async Task ChatRoutine(List<ChatSO> chatData)
        {
            while (true) // 无限循环
            {
                foreach (ChatSO chatObject in chatData)
                {
                    // 异步动画显示消息
                    await AnimateMessageAsync(chatObject.chatname, chatObject.message);
                    // 每行消息之间的延迟
                    await Task.Delay(k_DelayBetweenLines);
                }
            }
        }

        // 逐个字符递增UI元素文本，并在每个字符之间设置小延迟
        async Task AnimateMessageAsync(string chatName, string message)
        {
            // 清空聊天文本
            m_ChatText.text = string.Empty;
            // 设置聊天名称
            m_ChatText.text = k_TagOpen + " (" + chatName + ")" + k_TagClose + " ";

            foreach (char c in message.ToCharArray())
            {
                // 每个字符之间的延迟
                await Task.Delay(k_DelayBetweenKeys);
                // 添加字符到聊天文本
                m_ChatText.text += c;
            }
        }
    }
}