using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;

namespace UIToolkitDemo
{
    /// <summary>
    /// ������Ļ����ʾ�ı���Ϣ��ģ�����촰�ڵ�UI��
    /// </summary>
    public class ChatView : UIView
    {
        // ʹ���첽�ȴ�������Э�̣���MonoBehaviour��
        const int k_DelayBetweenKeys = 20; // ����
        const int k_DelayBetweenLines = 1000; // ����

        Label m_ChatText;

        // ����������ɫ
        const string k_TagOpen = "<color=green>";
        const string k_TagClose = "</color>";

        public ChatView(VisualElement topElement) : base(topElement)
        {
            // �����촰����ʾʱ�����¼�
            HomeEvents.ChatWindowShown += OnShowChats;
        }

        public override void Dispose()
        {
            base.Dispose();
            // ȡ���������촰����ʾ�¼�
            HomeEvents.ChatWindowShown -= OnShowChats;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            // ��ȡ�����ı���Label���
            m_ChatText = m_TopElement.Q<Label>("home-chat__text");
        }

        void OnShowChats(List<ChatSO> chatData)
        {
            if (m_ChatText == null)
                return;

            // �����첽��������
            _ = ChatRoutine(chatData);
        }

        // ����������Ϣ�������ʾ
        async Task ChatRoutine(List<ChatSO> chatData)
        {
            while (true) // ����ѭ��
            {
                foreach (ChatSO chatObject in chatData)
                {
                    // �첽������ʾ��Ϣ
                    await AnimateMessageAsync(chatObject.chatname, chatObject.message);
                    // ÿ����Ϣ֮����ӳ�
                    await Task.Delay(k_DelayBetweenLines);
                }
            }
        }

        // ����ַ�����UIԪ���ı�������ÿ���ַ�֮������С�ӳ�
        async Task AnimateMessageAsync(string chatName, string message)
        {
            // ��������ı�
            m_ChatText.text = string.Empty;
            // ������������
            m_ChatText.text = k_TagOpen + " (" + chatName + ")" + k_TagClose + " ";

            foreach (char c in message.ToCharArray())
            {
                // ÿ���ַ�֮����ӳ�
                await Task.Delay(k_DelayBetweenKeys);
                // ����ַ��������ı�
                m_ChatText.text += c;
            }
        }
    }
}