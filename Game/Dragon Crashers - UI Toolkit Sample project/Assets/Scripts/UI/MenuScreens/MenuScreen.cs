using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{

    // 已弃用。

    public abstract class MenuScreen : MonoBehaviour
    {
        [Tooltip("此菜单面板/屏幕在UXML中的字符串ID。")]
        [SerializeField] protected string m_ScreenName;

        [Header("UI管理")]
        //[Tooltip("在此显式设置主菜单（或从当前游戏对象自动获取）。")]
        //[SerializeField] protected MainMenuUIManager m_MainMenuUIManager;
        [Tooltip("在此显式设置UI文档（或从当前游戏对象自动获取）。")]
        [SerializeField] protected UIDocument m_Document;

        // 视觉元素
        protected VisualElement m_Screen; // 屏幕
        protected VisualElement m_Root; // 根元素

        // UXML元素名称（默认为类名）
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(m_ScreenName))
                m_ScreenName = this.GetType().Name;
        }

        protected virtual void Awake()
        {
            // 如果在检查器中未设置，则默认为当前UIDocument
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();

            if (m_Document == null)
            {
                Debug.LogWarning("MenuScreen " + m_ScreenName + ": 缺少UIDocument。检查脚本执行顺序。");
                return;
            }
            else
            {
                SetVisualElements();
                RegisterButtonCallbacks();
            }
        }

        // 一般工作流程使用字符串ID查询VisualTreeAsset并在UXML中查找匹配的视觉元素。
        // 为每个MenuScreen子类自定义此方法以识别任何功能性视觉元素（按钮、控件等）。
        protected virtual void SetVisualElements()
        {
            // 获取根视觉元素的引用
            if (m_Document != null)
                m_Root = m_Document.rootVisualElement;

            m_Screen = GetVisualElement(m_ScreenName);
        }

        // 一旦有了视觉元素，就可以在这里添加按钮事件，使用RegisterCallback功能。
        // 这允许你使用许多不同的事件（ClickEvent、ChangeEvent等）
        protected virtual void RegisterButtonCallbacks()
        {

        }

        public bool IsVisible()
        {
            if (m_Screen == null)
                return false;

            return (m_Screen.style.display == DisplayStyle.Flex);
        }

        // 使用DisplayStyle切换UI的显示和隐藏。
        public static void ShowVisualElement(VisualElement visualElement, bool state)
        {
            if (visualElement == null)
                return;

            visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // 按名称返回一个元素
        public VisualElement GetVisualElement(string elementName)
        {
            if (string.IsNullOrEmpty(elementName) || m_Root == null)
                return null;

            // 查询并返回元素
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