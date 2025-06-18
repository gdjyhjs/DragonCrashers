using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// ���û�������ʾ�������ʾ�洢�ڶ��ScriptableObject
    /// ��Դ�е��ı���ģ��һ�������ʼ��ͻ��ˡ���������������
    /// UIView���������ڣ�MailboxView��MailContentView��MailTabView
    /// </summary>
    public class MailView : UIView
    {
        // �û���������������ɣ��ռ���/��ɾ���ı�ǩ��ť��һ��������ͼ����ʾ��Ϣ���Լ�
        // һ�����ݴ��ڡ�ÿ������

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
            // ��ȡ��������Ԫ��
            m_MailboxContainer = m_TopElement.Q<VisualElement>("mailbox__container");
            // ��ȡ��������Ԫ��
            m_ContentContainer = m_TopElement.Q<VisualElement>("content__container");
            // ��ȡ��ǩ����Ԫ��
            m_TabContainer = m_TopElement.Q<VisualElement>("tabs__container");
        }

        // ��������ʾ��MailboxView��MailContentView��MailTabView��
        public override void Initialize()
        {
            base.Initialize();

            // ��ʾ����ѡ��ı�ǩ��ť
            m_MailTabView = new MailTabView(m_TabContainer);
            m_MailTabView.Show();

            // ��ʾ��Ϣ�б�
            m_MailboxView = new MailboxView(m_MailboxContainer);
            m_MailboxView.Show();

            // ��ʾѡ���ʼ���Ϣ������
            m_MailContentView = new MailContentView(m_ContentContainer);
            m_MailContentView.Show();
        }

        // �ͷ���������ʾ
        public override void Dispose()
        {
            base.Dispose();
            m_MailboxView.Dispose();
            m_MailContentView.Dispose();
            m_MailTabView.Dispose();
        }
    }
}