using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 此用户界面演示了如何显示存储在多个ScriptableObject
    /// 资源中的文本，模拟一个电子邮件客户端。它管理三个其他
    /// UIView的生命周期：MailboxView、MailContentView和MailTabView
    /// </summary>
    public class MailView : UIView
    {
        // 用户界面由三部分组成：收件箱/已删除的标签按钮、一个滚动视图来显示消息，以及
        // 一个内容窗口。每个部分

        VisualElement m_MailboxContainer;
        VisualElement m_ContentContainer;
        VisualElement m_TabContainer;

        MailboxView m_MailboxView;
        MailContentView m_MailContentView;
        MailTabView m_MailTabView;

        public MailView(VisualElement topElement) : base(topElement)
        {
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            // 获取邮箱容器元素
            m_MailboxContainer = m_TopElement.Q<VisualElement>("mailbox__container");
            // 获取内容容器元素
            m_ContentContainer = m_TopElement.Q<VisualElement>("content__container");
            // 获取标签容器元素
            m_TabContainer = m_TopElement.Q<VisualElement>("tabs__container");
        }

        // 设置子显示（MailboxView、MailContentView、MailTabView）
        public override void Initialize()
        {
            base.Initialize();

            // 显示用于选择的标签按钮
            m_MailTabView = new MailTabView(m_TabContainer);
            m_MailTabView.Show();

            // 显示消息列表
            m_MailboxView = new MailboxView(m_MailboxContainer);
            m_MailboxView.Show();

            // 显示选中邮件消息的内容
            m_MailContentView = new MailContentView(m_ContentContainer);
            m_MailContentView.Show();
        }

        // 释放所有子显示
        public override void Dispose()
        {
            base.Dispose();
            m_MailboxView.Dispose();
            m_MailContentView.Dispose();
            m_MailTabView.Dispose();
        }
    }
}