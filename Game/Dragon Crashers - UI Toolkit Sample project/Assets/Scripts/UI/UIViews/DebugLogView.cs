using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class DebugLogView : MonoBehaviour
    {
        // UI日志文档
        [SerializeField] private UIDocument m_UILogDocument;  // 包含UI的UI文档
        // 日志标签
        private Label m_LogLabel;
        // 最大日志行数
        private const int MaxLogs = 10;  // 要显示的最大日志行数
        // 日志消息数组
        private string[] m_LogMessages = new string[MaxLogs] { "", "", "", "", "", "", "", "", "", "" };  // 存储日志消息
        // 日志索引
        private int m_LogIndex = 0;

        void OnEnable()
        {
            // 订阅日志消息接收事件
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            if (m_LogLabel != null)
                // 清空日志标签文本
                m_LogLabel.text = string.Empty;
            // 脚本禁用时取消订阅
            Application.logMessageReceived -= HandleLog;
        }

        void Start()
        {
            // 查找UI中的Label元素以显示日志
            var rootElement = m_UILogDocument.rootVisualElement;
            // 获取日志标签
            m_LogLabel = rootElement.Q<Label>("log__label");

            // 更新日志UI
            UpdateLogUI();
        }

        // 每次创建日志时调用的方法
        void HandleLog(string logString, string stackTrace, LogType type)
        {
            // 将新的日志消息添加到数组中
            m_LogMessages[m_LogIndex] = logString;
            // 循环缓冲区
            m_LogIndex = (m_LogIndex + 1) % MaxLogs;

            // 使用最新的日志更新UI
            UpdateLogUI();
        }

        // 更新UI以显示最新的日志消息
        void UpdateLogUI()
        {
            // 将所有日志连接成一个字符串并更新Label文本
            string combinedLogs = string.Join("\n", m_LogMessages);
            if (m_LogLabel != null)
            {
                // 设置日志标签文本
                m_LogLabel.text = combinedLogs;
            }
        }
    }
}