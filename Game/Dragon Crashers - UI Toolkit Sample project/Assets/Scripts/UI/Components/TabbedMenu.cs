using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // �ı��ԣ�https://docs.unity3d.com/2021.2/Documentation/Manual/UIE-create-tabbed-menu-for-runtime.html

    // ��ΪUI�ĵ�������һ����ǩ�˵�ϵͳ���˰汾��ԭʼ�汾�������޸ģ���֧��ͬһ�ĵ��еĶ����ǩ�˵��� 

    [System.Serializable]
    public struct TabbedMenuIDs
    {
        // �ɵ����ǩ��UXMLѡ����
        public string tabClassName;// = "tab";

        // ��ǰѡ�б�ǩ��UXMLѡ���� 
        public string selectedTabClassName; //= "selected-tab";

        // Ҫ���ص����ݵ�UXMLѡ����
        public string unselectedContentClassName; // = "unselected-content";

        // ʹ�û���������ǩ����������ԣ����� 'name1-tab' ƥ�� 'name1-content'

        // ��ǩ�ĺ�׺����Լ��
        public string tabNameSuffix;// = "-tab";

        // ���ݵĺ�׺����Լ��
        public string contentNameSuffix;// = "-content";

    }
    public class TabbedMenu : MonoBehaviour
    {
        [Tooltip("����ָ��������Ĭ��Ϊ��ǰ���")]
        [SerializeField] UIDocument m_Document;

        [Tooltip("TabbedMenu��VisualElement�����δָ������Ĭ��Ϊ�ĵ���rootVisualElement")]
        [SerializeField] string m_MenuElementName;

        TabbedMenuController m_Controller;
        VisualElement m_MenuElement;

        public TabbedMenuIDs m_TabbedMenuStrings;

        void OnEnable()
        {
            VisualElement root = m_Document.rootVisualElement;
            m_MenuElement = root.Q(m_MenuElementName);

            // Ϊ�ض�Ԫ�ش���һ���µ�TabbedMenuController�����δָ��������˵���������
            m_Controller = (string.IsNullOrEmpty(m_MenuElementName) || m_MenuElement == null) ?
                new TabbedMenuController(root, m_TabbedMenuStrings) : new TabbedMenuController(m_MenuElement, m_TabbedMenuStrings);

            // ���ñ�ǩ�ϵĵ���¼�
            m_Controller.RegisterTabCallbacks();

            MainMenuUIEvents.TabbedUIReset += OnTabbedUIReset;

        }

        void OnDisable()
        {
            MainMenuUIEvents.TabbedUIReset -= OnTabbedUIReset;
        }

        // Ϊ����������Ĭ������ - Ϊÿ����ǩ�˵�/UIʹ��Щ����Ψһ
        void OnValidate()
        {
            if (string.IsNullOrEmpty(m_TabbedMenuStrings.tabClassName))
            {
                m_TabbedMenuStrings.tabClassName = "tab";
            }

            if (string.IsNullOrEmpty(m_TabbedMenuStrings.selectedTabClassName))
            {
                m_TabbedMenuStrings.selectedTabClassName = "selected-tab";
            }

            if (string.IsNullOrEmpty(m_TabbedMenuStrings.unselectedContentClassName))
            {
                m_TabbedMenuStrings.unselectedContentClassName = "unselected-content";
            }

            if (string.IsNullOrEmpty(m_TabbedMenuStrings.tabNameSuffix))
            {
                m_TabbedMenuStrings.tabNameSuffix = "-tab";
            }

            if (string.IsNullOrEmpty(m_TabbedMenuStrings.contentNameSuffix))
            {
                m_TabbedMenuStrings.contentNameSuffix = "-content";
            }
        }

        // ѡ���һ����ǩ
        public void SelectFirstTab()
        {
            SelectFirstTab(m_MenuElement);
        }

        // ѡ���һ����ǩ��ָ����ǩ�˵��·����κ�Ԫ�أ�ͨ���ǲ˵���Ļ��
        public void SelectFirstTab(VisualElement elementToQuery)
        {
            m_Controller.SelectFirstTab(elementToQuery);
        }

        // ͨ���ַ���IDѡ���ض���ǩ
        public void SelectTab(string tabName)
        {
            m_Controller.SelectTab(tabName);
        }

        // ����ǩ�Ƿ�ѡ��
        public bool IsTabSelected(VisualElement visualElement)
        {
            if (m_Controller == null || visualElement == null)
            {
                return false;
            }

            return m_Controller.IsTabSelected(visualElement);
        }

        // �¼�������
        void OnTabbedUIReset(string newView)
        {
            if (newView == m_MenuElementName)
            {
                SelectFirstTab();
            }
        }
    }
}