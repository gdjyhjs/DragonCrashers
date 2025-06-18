using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// ���� UI ��һ�����ܵ�Ԫ�Ļ��ࡣ�����Թ���һ��ȫ�����棬Ҳ����ֻ��
    /// ���е�һ���֡�
    /// </summary>

    public class UIView : IDisposable
    {
        protected bool m_HideOnAwake = true;

        // UI ��ʾ�����ײ� UI������͸��
        protected bool m_IsOverlay;

        protected VisualElement m_TopElement;

        // ����
        public VisualElement Root => m_TopElement;
        public bool IsTransparent => m_IsOverlay;
        public bool IsHidden => m_TopElement.style.display == DisplayStyle.None;

        // ���캯��
        /// <summary>
        /// ��ʼ�� UIView �����ʵ����
        /// </summary>
        /// <param name="topElement">UXML ��νṹ������ VisualElement��</param>
        public UIView(VisualElement topElement)
        {
            m_TopElement = topElement ?? throw new ArgumentNullException(nameof(topElement));
            Initialize();
        }

        public virtual void Initialize()
        {
            if (m_HideOnAwake)
            {
                Hide();
            }
            SetVisualElements();
            RegisterButtonCallbacks();
        }

        // ���� UI �� VisualElement����д���Զ��塣
        protected virtual void SetVisualElements()
        {

        }

        // ע�� UI �а�ť�Ļص�����д���Զ��塣
        protected virtual void RegisterButtonCallbacks()
        {

        }

        // ��ʾ UI��
        public virtual void Show()
        {
            m_TopElement.style.display = DisplayStyle.Flex;
        }

        // ���� UI��
        public virtual void Hide()
        {
            m_TopElement.style.display = DisplayStyle.None;
        }

        // ȡ��ע���κλص����¼����������д���Զ��塣
        public virtual void Dispose()
        {

        }
    }
}