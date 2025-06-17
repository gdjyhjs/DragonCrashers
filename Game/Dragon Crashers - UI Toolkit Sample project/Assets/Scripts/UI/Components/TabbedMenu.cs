using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // 改编自：https://docs.unity3d.com/2021.2/Documentation/Manual/UIE-create-tabbed-menu-for-runtime.html

    // 这为UI文档建立了一个标签菜单系统。此版本对原始版本进行了修改，以支持同一文档中的多个标签菜单。 

    [System.Serializable]
    public struct TabbedMenuIDs
    {
        // 可点击标签的UXML选择器
        public string tabClassName;// = "tab";

        // 当前选中标签的UXML选择器 
        public string selectedTabClassName; //= "selected-tab";

        // 要隐藏的内容的UXML选择器
        public string unselectedContentClassName; // = "unselected-content";

        // 使用基名来将标签与其内容配对，例如 'name1-tab' 匹配 'name1-content'

        // 标签的后缀命名约定
        public string tabNameSuffix;// = "-tab";

        // 内容的后缀命名约定
        public string contentNameSuffix;// = "-content";

    }
    public class TabbedMenu : MonoBehaviour
    {
        [Tooltip("除非指定，否则默认为当前组件")]
        [SerializeField] UIDocument m_Document;

        [Tooltip("TabbedMenu的VisualElement，如果未指定，则默认为文档的rootVisualElement")]
        [SerializeField] string m_MenuElementName;

        TabbedMenuController m_Controller;
        VisualElement m_MenuElement;

        public TabbedMenuIDs m_TabbedMenuStrings;

        void OnEnable()
        {
            VisualElement root = m_Document.rootVisualElement;
            m_MenuElement = root.Q(m_MenuElementName);

            // 为特定元素创建一个新的TabbedMenuController（如果未指定，则回退到整个树）
            m_Controller = (string.IsNullOrEmpty(m_MenuElementName) || m_MenuElement == null) ?
                new TabbedMenuController(root, m_TabbedMenuStrings) : new TabbedMenuController(m_MenuElement, m_TabbedMenuStrings);

            // 设置标签上的点击事件
            m_Controller.RegisterTabCallbacks();

            MainMenuUIEvents.TabbedUIReset += OnTabbedUIReset;

        }

        void OnDisable()
        {
            MainMenuUIEvents.TabbedUIReset -= OnTabbedUIReset;
        }

        // 为方便起见填充默认名称 - 为每个标签菜单/UI使这些名称唯一
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

        // 选择第一个标签
        public void SelectFirstTab()
        {
            SelectFirstTab(m_MenuElement);
        }

        // 选择第一个标签（指定标签菜单下方的任何元素，通常是菜单屏幕）
        public void SelectFirstTab(VisualElement elementToQuery)
        {
            m_Controller.SelectFirstTab(elementToQuery);
        }

        // 通过字符串ID选择特定标签
        public void SelectTab(string tabName)
        {
            m_Controller.SelectTab(tabName);
        }

        // 检查标签是否被选中
        public bool IsTabSelected(VisualElement visualElement)
        {
            if (m_Controller == null || visualElement == null)
            {
                return false;
            }

            return m_Controller.IsTabSelected(visualElement);
        }

        // 事件处理方法
        void OnTabbedUIReset(string newView)
        {
            if (newView == m_MenuElementName)
            {
                SelectFirstTab();
            }
        }
    }
}