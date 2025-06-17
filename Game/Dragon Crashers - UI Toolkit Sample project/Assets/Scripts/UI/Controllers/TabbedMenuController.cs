using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{

    // 改编自: https://docs.unity3d.com/2021.2/Documentation/Manual/UIE-create-tabbed-menu-for-runtime.html

    public class TabbedMenuController
    {
        // 事件，用于通知其他对象
        public static event Action TabSelected;

        // UI的基础VisualElement（例如MailScreen、CharScreen、ShopScreen）
        readonly VisualElement m_Root;

        // 用于查询VisualElements的字符串
        readonly TabbedMenuIDs m_IDs;

        // 初始化根Visual Element以便重用（非MonoBehaviour的构造函数）
        public TabbedMenuController(VisualElement root, TabbedMenuIDs ids)
        {
            m_Root = root;
            m_IDs = ids;
        }

        // 为标签按钮设置点击事件
        public void RegisterTabCallbacks()
        {
            // 识别每个标签 
            UQueryBuilder<VisualElement> tabs = GetAllTabs();

            // 为每个Visual Element注册ClickTab事件处理程序
            tabs.ForEach(
                (t) =>
                {
                    t.RegisterCallback<ClickEvent>(OnTabClick);
                });
        }

        // 处理点击事件
        void OnTabClick(ClickEvent evt)
        {
            VisualElement clickedTab = evt.currentTarget as VisualElement;

            // 如果点击的标签未被选中，则选择正确的标签
            if (!IsTabSelected(clickedTab))
            {
                // 取消选择当前活动的其他标签
                GetAllTabs().Where(
                    (tab) => tab != clickedTab && IsTabSelected(tab)
                    ).ForEach(UnselectTab);

                // 选择点击的标签
                SelectTab(clickedTab);
                AudioManager.PlayDefaultButtonSound();
            }
        }

        // 返回给定标签对应的内容元素
        VisualElement FindContent(VisualElement tab)
        {
            return m_Root.Q(GetContentName(tab));
        }

        // 返回对应的内容名称，例如name1-tab对应的name1-content
        string GetContentName(VisualElement tab)
        {
            return tab.name.Replace(m_IDs.tabNameSuffix, m_IDs.contentNameSuffix);
        }

        // 定位所有具有标签类名的VisualElements
        UQueryBuilder<VisualElement> GetAllTabs()
        {
            return m_Root.Query<VisualElement>(className: m_IDs.tabClassName);
        }

        // 定位特定Visual Element（例如屏幕）上的第一个标签
        public VisualElement GetFirstTab(VisualElement visualElement)
        {
            return visualElement.Query<VisualElement>(className: m_IDs.tabClassName).First();
        }

        public bool IsTabSelected(string tabName)
        {
            VisualElement tabElement = m_Root.Query<VisualElement>(className: m_IDs.tabClassName, name: tabName);
            return IsTabSelected(tabElement);
        }

        public bool IsTabSelected(VisualElement tab)
        {
            return tab.ClassListContains(m_IDs.selectedTabClassName);
        }

        void UnselectOtherTabs(VisualElement tab)
        {
            GetAllTabs().Where(
                (t) => t != tab && IsTabSelected(t)).
                ForEach(UnselectTab);
        }

        // 按名称选择（在打开屏幕到特定标签时使用）
        public void SelectTab(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                return;
            }

            VisualElement namedTab = m_Root.Query<VisualElement>(tabName, className: m_IDs.tabClassName);

            if (namedTab == null)
            {
                Debug.Log("TabbedMenuController.SelectTab: 指定的标签无效");
                return;
            }

            UnselectOtherTabs(namedTab);
            SelectTab(namedTab);
        }

        // 选择给定的标签，找到对应的内容并显示内容
        void SelectTab(VisualElement tab)
        {
            // 突出显示标签
            tab.AddToClassList(m_IDs.selectedTabClassName);

            // 取消隐藏内容
            VisualElement content = FindContent(tab);
            content.RemoveFromClassList(m_IDs.unselectedContentClassName);

            // 通知其他对象 
            TabSelected?.Invoke();
        }

        // 选择给定Visual Element（例如屏幕）的第一个标签
        public void SelectFirstTab(VisualElement visualElement)
        {
            VisualElement firstTab = GetFirstTab(visualElement);

            if (firstTab != null)
            {
                // 取消选择当前活动的其他标签
                GetAllTabs().Where(
                    (tab) => tab != firstTab && IsTabSelected(tab)
                    ).ForEach(UnselectTab);

                // 仅选择第一个标签
                SelectTab(firstTab);
            }
        }

        // 取消选择特定的标签，找到对应的内容并隐藏内容
        void UnselectTab(VisualElement tab)
        {
            // 取消突出显示
            tab.RemoveFromClassList(m_IDs.selectedTabClassName);

            // 隐藏对应的内容
            VisualElement content = FindContent(tab);
            content.AddToClassList(m_IDs.unselectedContentClassName);
        }
    }
}