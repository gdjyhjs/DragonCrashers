using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{

    // �����á�

    public abstract class MenuScreen : MonoBehaviour
    {
        [Tooltip("�˲˵����/��Ļ��UXML�е��ַ���ID��")]
        [SerializeField] protected string m_ScreenName;

        [Header("UI����")]
        //[Tooltip("�ڴ���ʽ�������˵�����ӵ�ǰ��Ϸ�����Զ���ȡ����")]
        //[SerializeField] protected MainMenuUIManager m_MainMenuUIManager;
        [Tooltip("�ڴ���ʽ����UI�ĵ�����ӵ�ǰ��Ϸ�����Զ���ȡ����")]
        [SerializeField] protected UIDocument m_Document;

        // �Ӿ�Ԫ��
        protected VisualElement m_Screen; // ��Ļ
        protected VisualElement m_Root; // ��Ԫ��

        // UXMLԪ�����ƣ�Ĭ��Ϊ������
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(m_ScreenName))
                m_ScreenName = this.GetType().Name;
        }

        protected virtual void Awake()
        {
            // ����ڼ������δ���ã���Ĭ��Ϊ��ǰUIDocument
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();

            if (m_Document == null)
            {
                Debug.LogWarning("MenuScreen " + m_ScreenName + ": ȱ��UIDocument�����ű�ִ��˳��");
                return;
            }
            else
            {
                SetVisualElements();
                RegisterButtonCallbacks();
            }
        }

        // һ�㹤������ʹ���ַ���ID��ѯVisualTreeAsset����UXML�в���ƥ����Ӿ�Ԫ�ء�
        // Ϊÿ��MenuScreen�����Զ���˷�����ʶ���κι������Ӿ�Ԫ�أ���ť���ؼ��ȣ���
        protected virtual void SetVisualElements()
        {
            // ��ȡ���Ӿ�Ԫ�ص�����
            if (m_Document != null)
                m_Root = m_Document.rootVisualElement;

            m_Screen = GetVisualElement(m_ScreenName);
        }

        // һ�������Ӿ�Ԫ�أ��Ϳ�����������Ӱ�ť�¼���ʹ��RegisterCallback���ܡ�
        // ��������ʹ����಻ͬ���¼���ClickEvent��ChangeEvent�ȣ�
        protected virtual void RegisterButtonCallbacks()
        {

        }

        public bool IsVisible()
        {
            if (m_Screen == null)
                return false;

            return (m_Screen.style.display == DisplayStyle.Flex);
        }

        // ʹ��DisplayStyle�л�UI����ʾ�����ء�
        public static void ShowVisualElement(VisualElement visualElement, bool state)
        {
            if (visualElement == null)
                return;

            visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // �����Ʒ���һ��Ԫ��
        public VisualElement GetVisualElement(string elementName)
        {
            if (string.IsNullOrEmpty(elementName) || m_Root == null)
                return null;

            // ��ѯ������Ԫ��
            return m_Root.Q(elementName);
        }

        public virtual void ShowScreen()
        {
            ShowVisualElement(m_Screen, true);
        }

        public virtual void HideScreen()
        {
            if (IsVisible())
            {
                ShowVisualElement(m_Screen, false);
            }
        }
    }
}