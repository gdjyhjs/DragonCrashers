using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public class MailTabView : UIView
    {
        VisualElement m_InboxTab;
        VisualElement m_DeletedTab;

        // ѡ�б�ǩ������
        const string k_SelectedTabClassName = "selected-mailtab";

        public MailTabView(VisualElement topElement) : base(topElement)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            // ������ǩѡ���¼�
            MailEvents.TabSelected?.Invoke(m_InboxTab.name);
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            // ��ȡ�ռ����ǩԪ��
            m_InboxTab = m_TopElement.Q("tabs__inbox-tab");
            // ��ȡ��ɾ����ǩԪ��
            m_DeletedTab = m_TopElement.Q("tabs__deleted-tab");
        }

        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            // ע���ռ����ǩ����¼�
            m_InboxTab.RegisterCallback<ClickEvent>(SelectInboxTab);
            // ע����ɾ����ǩ����¼�
            m_DeletedTab.RegisterCallback<ClickEvent>(SelectDeletedTab);
        }

        // ѡ����ɾ����Ϣ�������ǩ
        void SelectDeletedTab(ClickEvent evt)
        {
            // �Ƴ��ռ����ǩ��ѡ����ʽ
            m_InboxTab.RemoveFromClassList(k_SelectedTabClassName);
            // �����ɾ����ǩ��ѡ����ʽ
            m_DeletedTab.AddToClassList(k_SelectedTabClassName);

            // ������ı�ǩ���Ʒ��͸�������
            VisualElement clickedTab = evt.currentTarget as VisualElement;
            MailEvents.TabSelected(clickedTab.name);
        }

        // ѡ���ռ��������ǩ
        void SelectInboxTab(ClickEvent evt)
        {
            // �Ƴ���ɾ����ǩ��ѡ����ʽ
            m_DeletedTab.RemoveFromClassList(k_SelectedTabClassName);
            // ����ռ����ǩ��ѡ����ʽ
            m_InboxTab.AddToClassList(k_SelectedTabClassName);

            // ������ı�ǩ���Ʒ��͸�������
            VisualElement clickedTab = evt.currentTarget as VisualElement;
            MailEvents.TabSelected(clickedTab.name);
        }

        public override void Dispose()
        {
            base.Dispose();

            // ��ѡ���������������Զ���������
            // �������ⲿ����

            // ȡ��ע���ռ����ǩ����¼�
            m_InboxTab.UnregisterCallback<ClickEvent>(SelectInboxTab);
            // ȡ��ע����ɾ����ǩ����¼�
            m_DeletedTab.UnregisterCallback<ClickEvent>(SelectDeletedTab);
        }
    }
}