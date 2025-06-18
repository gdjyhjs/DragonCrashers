using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class MailboxView : UIView
    {
        // ����
        Sprite m_NewMailIcon;
        Sprite m_OldMailIcon;
        GameIconsSO m_GameIcons;

        VisualTreeAsset m_MailMessageAsset;
        VisualElement m_MailboxContainer;

        // ��ǰѡ�е��ʼ���Ŀ�����Ե�ǰѡ�е������ǩ����Ĭ��Ϊ������Ŀ
        int m_CurrentMessageIndex = 0;

        const string k_MailMessageClass = "mail-message";
        const string k_MailMessageSelectedClass = "mail-message-selected";
        const string k_MailMessageDeletedClass = "mail-message-deleted";

        // ��ѡ��������
        const string k_IconResourcePath = "GameData/GameIcons";
        const string k_MailMessageAssetPath = "MailMessage";

        // ���캯��
        public MailboxView(VisualElement topElement) : base(topElement)
        {
            m_MailMessageAsset = Resources.Load<VisualTreeAsset>(k_MailMessageAssetPath);
            m_GameIcons = Resources.Load<GameIconsSO>(k_IconResourcePath);

            m_NewMailIcon = m_GameIcons.newMailIcon;
            m_OldMailIcon = m_GameIcons.oldMailIcon;

            // ע����������¼�
            MailEvents.MailboxUpdated += OnMailboxUpdated;
            // ע��ɾ������¼�
            MailEvents.DeleteClicked += OnDeleteClicked;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // ���ʼ���Ϣ�洢�ڹ�����ͼ��
            m_MailboxContainer = m_TopElement.Q<VisualElement>("unity-content-container");
        }

        public override void Dispose()
        {
            base.Dispose();

            // ȡ��ע����������¼�
            MailEvents.MailboxUpdated -= OnMailboxUpdated;
            // ȡ��ע��ɾ������¼�
            MailEvents.DeleteClicked -= OnDeleteClicked;
        }

        // ���õ�ǰ����
        void ResetCurrentIndex()
        {
            m_CurrentMessageIndex = 0;
            // ������ʾ��һ����Ϣ
            HighlightFirstMessage();
            // ����һ���ʼ�Ԫ�ر��Ϊ�Ѷ�
            MarkMailElementAsRead(GetFirstMailElement());
        }

        // ע�⣺Ϊ���Ż����ܣ���ʹ��ListView

        // ��������
        void UpdateMailbox(List<MailMessageSO> messageList, VisualElement container)
        {
            // ������еĻ�ռλ���ʼ���Ϣ
            container.Clear();

            if (messageList.Count == 0)
                return;

            // ʵ�����ʼ���Ϣ���������
            foreach (MailMessageSO msg in messageList)
            {
                if (msg != null)
                    CreateMailMessage(msg, container);
            }

            // ����������Ϊ��ͷ��������ʾ��һ����Ϣ
            ResetCurrentIndex();

            // ʹ��ѡ�е�������������
            MailEvents.MessageSelected?.Invoke(m_CurrentMessageIndex);
        }

        // ���������е������Ŀ
        void ClickMessage(ClickEvent evt)
        {
            // ��������ʼ���Ŀ
            VisualElement clickedElement = evt.currentTarget as VisualElement;

            // ������ʾ�����ʼ���Ϣ���Ϊ�Ѷ�
            MarkMailElementAsRead(clickedElement);

            VisualElement backgroundElement = clickedElement.Q(className: k_MailMessageClass);
            // ������ʾ��Ϣ
            HighlightMessage(backgroundElement);

            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();

            // ʹ��ѡ�е�������������
            m_CurrentMessageIndex = clickedElement.parent.IndexOf(clickedElement);
            MailEvents.MessageSelected?.Invoke(m_CurrentMessageIndex);
        }

        // ������ʾ����

        // ������ʾ����Ԫ��
        void HighlightMessage(VisualElement elementToHighlight)
        {
            if (elementToHighlight == null)
                return;

            // ȡ��ѡ�����������Ӿ�Ԫ��
            GetAllMailElements().
                Where((element) => element.ClassListContains(k_MailMessageSelectedClass)).
                ForEach(UnhighlightMessage);

            elementToHighlight.AddToClassList(k_MailMessageSelectedClass);
        }

        // ȡ��������ʾ����Ԫ��
        void UnhighlightMessage(VisualElement elementToUnhighlight)
        {
            if (elementToUnhighlight == null)
                return;

            elementToUnhighlight.RemoveFromClassList(k_MailMessageSelectedClass);
        }

        // ������ʾ��һ����Ϣ
        void HighlightFirstMessage()
        {
            VisualElement firstElement = GetFirstMailElement();
            if (firstElement != null)
            {
                HighlightMessage(firstElement);
            }
        }

        // ����һ���ʼ���Ϣ��������ӵ�����������
        void CreateMailMessage(MailMessageSO mailData, VisualElement mailboxContainer)
        {
            if (mailboxContainer == null || mailData == null || m_MailMessageAsset == null)
            {
                return;
            }

            // ʵ�����ʼ���Ϣ��VisualTreeAsset
            // ע�⣺�����ʵ���Ϸ�����һ�������TemplateContainerԪ��
            TemplateContainer instance = m_MailMessageAsset.Instantiate();

            // ΪTemplateContainer�ĵ�һ����Ԫ�أ��ʼ���Ϣ�������ʼ���Ϣ��
            instance.hierarchy[0].AddToClassList(k_MailMessageClass);

            mailboxContainer.Add(instance);
            instance.RegisterCallback<ClickEvent>(ClickMessage);

            // ������ڡ����⡢���µ���Ϣ
            ReadMailData(mailData, instance);
        }

        // ��ScriptableObject��ȡ����
        void ReadMailData(MailMessageSO mailData, TemplateContainer instance)
        {
            // ��ȡScriptableObject����
            Label subjectLine = instance.Q<Label>("mail-item__subject");
            subjectLine.text = mailData.SubjectLine;

            Label date = instance.Q<Label>("mail-item__date");
            date.text = mailData.FormattedDate;

            VisualElement badge = instance.Q<VisualElement>("mail-item__badge");
            badge.visible = mailData.IsImportant;

            VisualElement newIcon = instance.Q<VisualElement>("mail-item__new");
            newIcon.style.backgroundImage = (mailData.IsNew) ? new StyleBackground(m_NewMailIcon) : new StyleBackground(m_OldMailIcon);
        }

        // ��δ��ͼ�꣨NewIcon������Ϊ�Ѷ�
        void MarkMailElementAsRead(VisualElement messageElement)
        {
            if (messageElement == null)
                return;

            MailEvents.MarkedAsRead.Invoke(m_CurrentMessageIndex);

            VisualElement newIcon = messageElement.Q<VisualElement>("mail-item__new");
            newIcon.style.backgroundImage = new StyleBackground(m_OldMailIcon);
        }

        // ��ȡ���о����ʼ���Ϣ���VisualElement
        UQueryBuilder<VisualElement> GetAllMailElements()
        {
            return m_TopElement.Query<VisualElement>(className: k_MailMessageClass);
        }

        // ���ص�ǰѡ�е��ʼ���Ϣ
        VisualElement GetSelectedMailMessage()
        {
            return m_TopElement.Query<VisualElement>(className: k_MailMessageSelectedClass);
        }

        // ���ص�һ���ʼ���Ϣ
        VisualElement GetFirstMailElement()
        {
            return m_MailboxContainer.Query<VisualElement>(className: k_MailMessageClass);
        }

        // �¼�������

        // �������ʱ�Ĵ�����
        void OnMailboxUpdated(List<MailMessageSO> messagesToShow)
        {
            UpdateMailbox(messagesToShow, m_MailboxContainer);
        }

        // ɾ�����ʱ�Ĵ�����
        void OnDeleteClicked()
        {
            VisualElement elemToDelete = GetSelectedMailMessage().parent;
            elemToDelete.AddToClassList(k_MailMessageDeletedClass);
        }
    }
}