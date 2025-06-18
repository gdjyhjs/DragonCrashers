using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class MailboxView : UIView
    {
        // 精灵
        Sprite m_NewMailIcon;
        Sprite m_OldMailIcon;
        GameIconsSO m_GameIcons;

        VisualTreeAsset m_MailMessageAsset;
        VisualElement m_MailboxContainer;

        // 当前选中的邮件项目（来自当前选中的邮箱标签），默认为顶部项目
        int m_CurrentMessageIndex = 0;

        const string k_MailMessageClass = "mail-message";
        const string k_MailMessageSelectedClass = "mail-message-selected";
        const string k_MailMessageDeletedClass = "mail-message-deleted";

        // 类选择器名称
        const string k_IconResourcePath = "GameData/GameIcons";
        const string k_MailMessageAssetPath = "MailMessage";

        // 构造函数
        public MailboxView(VisualElement topElement) : base(topElement)
        {
            m_MailMessageAsset = Resources.Load<VisualTreeAsset>(k_MailMessageAssetPath);
            m_GameIcons = Resources.Load<GameIconsSO>(k_IconResourcePath);

            m_NewMailIcon = m_GameIcons.newMailIcon;
            m_OldMailIcon = m_GameIcons.oldMailIcon;

            // 注册邮箱更新事件
            MailEvents.MailboxUpdated += OnMailboxUpdated;
            // 注册删除点击事件
            MailEvents.DeleteClicked += OnDeleteClicked;
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // 将邮件消息存储在滚动视图下
            m_MailboxContainer = m_TopElement.Q<VisualElement>("unity-content-container");
        }

        public override void Dispose()
        {
            base.Dispose();

            // 取消注册邮箱更新事件
            MailEvents.MailboxUpdated -= OnMailboxUpdated;
            // 取消注册删除点击事件
            MailEvents.DeleteClicked -= OnDeleteClicked;
        }

        // 重置当前索引
        void ResetCurrentIndex()
        {
            m_CurrentMessageIndex = 0;
            // 高亮显示第一条消息
            HighlightFirstMessage();
            // 将第一条邮件元素标记为已读
            MarkMailElementAsRead(GetFirstMailElement());
        }

        // 注意：为了优化性能，请使用ListView

        // 更新邮箱
        void UpdateMailbox(List<MailMessageSO> messageList, VisualElement container)
        {
            // 清除现有的或占位的邮件消息
            container.Clear();

            if (messageList.Count == 0)
                return;

            // 实例化邮件消息以填充邮箱
            foreach (MailMessageSO msg in messageList)
            {
                if (msg != null)
                    CreateMailMessage(msg, container);
            }

            // 将索引重置为开头并高亮显示第一条消息
            ResetCurrentIndex();

            // 使用选中的索引更新内容
            MailEvents.MessageSelected?.Invoke(m_CurrentMessageIndex);
        }

        // 处理邮箱中点击的项目
        void ClickMessage(ClickEvent evt)
        {
            // 被点击的邮件项目
            VisualElement clickedElement = evt.currentTarget as VisualElement;

            // 高亮显示并将邮件消息标记为已读
            MarkMailElementAsRead(clickedElement);

            VisualElement backgroundElement = clickedElement.Q(className: k_MailMessageClass);
            // 高亮显示消息
            HighlightMessage(backgroundElement);

            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();

            // 使用选中的索引更新内容
            m_CurrentMessageIndex = clickedElement.parent.IndexOf(clickedElement);
            MailEvents.MessageSelected?.Invoke(m_CurrentMessageIndex);
        }

        // 高亮显示方法

        // 高亮显示给定元素
        void HighlightMessage(VisualElement elementToHighlight)
        {
            if (elementToHighlight == null)
                return;

            // 取消选择所有其他视觉元素
            GetAllMailElements().
                Where((element) => element.ClassListContains(k_MailMessageSelectedClass)).
                ForEach(UnhighlightMessage);

            elementToHighlight.AddToClassList(k_MailMessageSelectedClass);
        }

        // 取消高亮显示给定元素
        void UnhighlightMessage(VisualElement elementToUnhighlight)
        {
            if (elementToUnhighlight == null)
                return;

            elementToUnhighlight.RemoveFromClassList(k_MailMessageSelectedClass);
        }

        // 高亮显示第一条消息
        void HighlightFirstMessage()
        {
            VisualElement firstElement = GetFirstMailElement();
            if (firstElement != null)
            {
                HighlightMessage(firstElement);
            }
        }

        // 生成一条邮件消息并将其添加到邮箱容器中
        void CreateMailMessage(MailMessageSO mailData, VisualElement mailboxContainer)
        {
            if (mailboxContainer == null || mailData == null || m_MailMessageAsset == null)
            {
                return;
            }

            // 实例化邮件消息的VisualTreeAsset
            // 注意：这会在实例上方创建一个额外的TemplateContainer元素
            TemplateContainer instance = m_MailMessageAsset.Instantiate();

            // 为TemplateContainer的第一个子元素（邮件消息）分配邮件消息类
            instance.hierarchy[0].AddToClassList(k_MailMessageClass);

            mailboxContainer.Add(instance);
            instance.RegisterCallback<ClickEvent>(ClickMessage);

            // 填充日期、主题、徽章等信息
            ReadMailData(mailData, instance);
        }

        // 从ScriptableObject获取数据
        void ReadMailData(MailMessageSO mailData, TemplateContainer instance)
        {
            // 读取ScriptableObject数据
            Label subjectLine = instance.Q<Label>("mail-item__subject");
            subjectLine.text = mailData.SubjectLine;

            Label date = instance.Q<Label>("mail-item__date");
            date.text = mailData.FormattedDate;

            VisualElement badge = instance.Q<VisualElement>("mail-item__badge");
            badge.visible = mailData.IsImportant;

            VisualElement newIcon = instance.Q<VisualElement>("mail-item__new");
            newIcon.style.backgroundImage = (mailData.IsNew) ? new StyleBackground(m_NewMailIcon) : new StyleBackground(m_OldMailIcon);
        }

        // 将未读图标（NewIcon）更改为已读
        void MarkMailElementAsRead(VisualElement messageElement)
        {
            if (messageElement == null)
                return;

            MailEvents.MarkedAsRead.Invoke(m_CurrentMessageIndex);

            VisualElement newIcon = messageElement.Q<VisualElement>("mail-item__new");
            newIcon.style.backgroundImage = new StyleBackground(m_OldMailIcon);
        }

        // 获取所有具有邮件消息类的VisualElement
        UQueryBuilder<VisualElement> GetAllMailElements()
        {
            return m_TopElement.Query<VisualElement>(className: k_MailMessageClass);
        }

        // 返回当前选中的邮件消息
        VisualElement GetSelectedMailMessage()
        {
            return m_TopElement.Query<VisualElement>(className: k_MailMessageSelectedClass);
        }

        // 返回第一条邮件消息
        VisualElement GetFirstMailElement()
        {
            return m_MailboxContainer.Query<VisualElement>(className: k_MailMessageClass);
        }

        // 事件处理方法

        // 邮箱更新时的处理方法
        void OnMailboxUpdated(List<MailMessageSO> messagesToShow)
        {
            UpdateMailbox(messagesToShow, m_MailboxContainer);
        }

        // 删除点击时的处理方法
        void OnDeleteClicked()
        {
            VisualElement elemToDelete = GetSelectedMailMessage().parent;
            elemToDelete.AddToClassList(k_MailMessageDeletedClass);
        }
    }
}