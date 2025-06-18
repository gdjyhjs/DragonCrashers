using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class MailTabView : UIView
    {
        VisualElement m_InboxTab;
        VisualElement m_DeletedTab;

        // 选中标签的类名
        const string k_SelectedTabClassName = "selected-mailtab";

        public MailTabView(VisualElement topElement) : base(topElement)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            // 触发标签选择事件
            MailEvents.TabSelected?.Invoke(m_InboxTab.name);
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            // 获取收件箱标签元素
            m_InboxTab = m_TopElement.Q("tabs__inbox-tab");
            // 获取已删除标签元素
            m_DeletedTab = m_TopElement.Q("tabs__deleted-tab");
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            // 注册收件箱标签点击事件
            m_InboxTab.RegisterCallback<ClickEvent>(SelectInboxTab);
            // 注册已删除标签点击事件
            m_DeletedTab.RegisterCallback<ClickEvent>(SelectDeletedTab);
        }

        // 选择已删除消息的邮箱标签
        void SelectDeletedTab(ClickEvent evt)
        {
            // 移除收件箱标签的选中样式
            m_InboxTab.RemoveFromClassList(k_SelectedTabClassName);
            // 添加已删除标签的选中样式
            m_DeletedTab.AddToClassList(k_SelectedTabClassName);

            // 将点击的标签名称发送给控制器
            VisualElement clickedTab = evt.currentTarget as VisualElement;
            MailEvents.TabSelected(clickedTab.name);
        }

        // 选择收件箱邮箱标签
        void SelectInboxTab(ClickEvent evt)
        {
            // 移除已删除标签的选中样式
            m_DeletedTab.RemoveFromClassList(k_SelectedTabClassName);
            // 添加收件箱标签的选中样式
            m_InboxTab.AddToClassList(k_SelectedTabClassName);

            // 将点击的标签名称发送给控制器
            VisualElement clickedTab = evt.currentTarget as VisualElement;
            MailEvents.TabSelected(clickedTab.name);
        }

        public override void Dispose()
        {
            base.Dispose();

            // 可选：垃圾回收器会自动清理，除非
            // 引用了外部对象

            // 取消注册收件箱标签点击事件
            m_InboxTab.UnregisterCallback<ClickEvent>(SelectInboxTab);
            // 取消注册已删除标签点击事件
            m_DeletedTab.UnregisterCallback<ClickEvent>(SelectDeletedTab);
        }
    }
}