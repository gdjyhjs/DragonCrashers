using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class DebugLogView : MonoBehaviour
    {
        // UI��־�ĵ�
        [SerializeField] private UIDocument m_UILogDocument;  // ����UI��UI�ĵ�
        // ��־��ǩ
        private Label m_LogLabel;
        // �����־����
        private const int MaxLogs = 10;  // Ҫ��ʾ�������־����
        // ��־��Ϣ����
        private string[] m_LogMessages = new string[MaxLogs] { "", "", "", "", "", "", "", "", "", "" };  // �洢��־��Ϣ
        // ��־����
        private int m_LogIndex = 0;

        void OnEnable()
        {
            // ������־��Ϣ�����¼�
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            if (m_LogLabel != null)
                // �����־��ǩ�ı�
                m_LogLabel.text = string.Empty;
            // �ű�����ʱȡ������
            Application.logMessageReceived -= HandleLog;
        }

        void Start()
        {
            // ����UI�е�LabelԪ������ʾ��־
            var rootElement = m_UILogDocument.rootVisualElement;
            // ��ȡ��־��ǩ
            m_LogLabel = rootElement.Q<Label>("log__label");

            // ������־UI
            UpdateLogUI();
        }

        // ÿ�δ�����־ʱ���õķ���
        void HandleLog(string logString, string stackTrace, LogType type)
        {
            // ���µ���־��Ϣ��ӵ�������
            m_LogMessages[m_LogIndex] = logString;
            // ѭ��������
            m_LogIndex = (m_LogIndex + 1) % MaxLogs;

            // ʹ�����µ���־����UI
            UpdateLogUI();
        }

        // ����UI����ʾ���µ���־��Ϣ
        void UpdateLogUI()
        {
            // ��������־���ӳ�һ���ַ���������Label�ı�
            string combinedLogs = string.Join("\n", m_LogMessages);
            if (m_LogLabel != null)
            {
                // ������־��ǩ�ı�
                m_LogLabel.text = combinedLogs;
            }
        }
    }
}