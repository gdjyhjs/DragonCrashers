using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// InfoView管理一组链接到外部资源的按钮。使用"GameData/Links"中的ScriptableObjects在Inspector中定义按钮详细信息（元素名称、标签和URL）。
    /// 这允许非程序员（如设计师）在不更改代码的情况下修改这些值。
    /// </summary>
    public class InfoView : UIView
    {
        // 按钮列表
        List<Button> m_Buttons;
        // 资源链接列表
        List<ResourceLinkSO> m_ResourceLinks;

        // 资源路径
        const string k_ResourcePath = "GameData/Links";

        /// <summary>
        /// InfoView的构造函数。加载资源链接数据并动态设置按钮。
        /// </summary>
        /// <param name="topElement">根VisualElement。</param>
        public InfoView(VisualElement topElement) : base(topElement)
        {
            // 加载资源链接数据
            m_ResourceLinks = Resources.LoadAll<ResourceLinkSO>(k_ResourcePath).ToList();
            m_Buttons = new List<Button>();

            // 使用单独的方法设置按钮，因为基类的SetVisualElements和RegisterButtonCallbacks已经运行
            SetupButtons();
        }

        // 注意：注销按钮回调是可选的，在这种情况下省略。必要时使用UnregisterCallback和UnregisterValueChangedCallback方法注销回调。

        /// <summary>
        /// 通过分配文本标签和注册点击事件来设置按钮
        /// </summary>
        void SetupButtons()
        {
            // 为每个按钮注册URL
            for (int i = 0; i < m_ResourceLinks.Count; i++)
            {
                // 复制索引以避免lambda表达式中的闭包
                int index = i;

                // 使用ResourceLink中的元素ID在视觉树中定位按钮
                m_Buttons.Add(m_TopElement.Q<Button>(m_ResourceLinks[index].ButtonElementId));

                // 如果找到按钮元素，则设置其标签和点击事件
                if (m_Buttons[index] != null)
                {
                    // 绑定按钮标签
                    BindButtonLabel(m_Buttons[index], m_ResourceLinks[index]);
                    // 绑定按钮URL
                    BindURL(m_Buttons[index], m_ResourceLinks[index]);
                }
            }
        }

        /// <summary>
        /// 将按钮的标签（LocalizedText属性）绑定到ResourceLink的ButtonText属性。
        /// </summary>
        /// <param name="button">要绑定标签的按钮。</param>
        /// <param name="resourceLink">要绑定标签的ResourceLinkSO。</param>
        void BindButtonLabel(Button button, ResourceLinkSO resourceLink)
        {
            if (resourceLink.ButtonLabel != null)
            {
                // 分配LocalizedString作为绑定；在这种情况下，数据源是隐式的，因为LocalizedString本身就是数据源
                button.SetBinding("text", resourceLink.ButtonLabel);
            }
        }

        /// <summary>
        /// 将按钮的点击事件绑定到ResourceLinkSO的TargetURL属性。这在Inspector中进行更改时更新按钮的clickEvent目标URL。
        /// </summary>
        /// <param name="button">其点击事件将被绑定的按钮。</param>
        /// <param name="resourceLink">提供URL数据的ResourceLinkSO。</param>
        void BindURL(Button button, ResourceLinkSO resourceLink)
        {
            var dataBinding = new DataBinding
            {
                // 要绑定的对象
                dataSource = resourceLink,
                // 对象内的特定属性
                dataSourcePath = new PropertyPath(nameof(resourceLink.TargetURL)),
                // 从数据源到UI的单向绑定
                bindingMode = BindingMode.ToTarget
            };

            // 确保只注册最近的点击事件，并使用更新后的TargetURL
            button.UnregisterCallback<ClickEvent>(evt => OpenURL(resourceLink.TargetURL));
            button.RegisterCallback<ClickEvent>(evt => OpenURL(resourceLink.TargetURL));

            // 分配目标按钮和数据源之间的绑定
            button.SetBinding("clickEvent", dataBinding);
        }

        /// <summary>
        /// 在默认浏览器中打开指定的URL并播放点击音效。
        /// </summary>
        /// <param name="URL">要打开的URL。</param>
        static void OpenURL(string URL)
        {
            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();
            // 打开URL
            Application.OpenURL("https://www.yellowshange.com");
        }
    }
}