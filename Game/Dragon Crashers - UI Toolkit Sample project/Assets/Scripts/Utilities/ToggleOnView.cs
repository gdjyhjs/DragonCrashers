using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace UIToolkitDemo
{
    [System.Serializable]
    public struct ViewState
    {
        public string viewName; // 视图名称
        public bool state;      // 状态
    }

    /// <summary>
    /// 基于当前UI视图切换UI元素显示状态的工具类
    /// </summary>
    public class ToggleOnView : MonoBehaviour
    {
        [Tooltip("包含要切换的元素的UXML文档")]
        [SerializeField] UIDocument m_Document;

        [Tooltip("要切换的视觉元素的名称")]
        [SerializeField] string m_ElementID;

        [Header("在以下状态下启用")]
        [Tooltip("根据当前活动的UI视图名称(主视图、角色视图等)指定显示状态")]
        [SerializeField] List<ViewState> m_ViewStates = new List<ViewState>();

        VisualElement m_ElementToToggle;

        void OnEnable()
        {
            Initialize(); // 初始化
            MainMenuUIEvents.CurrentViewChanged += OnCurrentViewChanged;
        }

        void OnDisable()
        {
            MainMenuUIEvents.CurrentViewChanged -= OnCurrentViewChanged;
        }

        private void Initialize()
        {
            if (m_Document == null)
            {
                Debug.LogWarning("[ToggleOnMenu] 需要指定UIDocument。");
                return;
            }

            m_ElementToToggle = m_Document.rootVisualElement.Q<VisualElement>(m_ElementID);

            if (m_ElementToToggle == null)
            {
                Debug.LogWarning("[ToggleOnMenu]: 未找到指定元素。");
                return;
            }

            m_ElementToToggle.style.visibility = Visibility.Visible;
        }

        // 当前视图改变时调用
        void OnCurrentViewChanged(string newViewName)
        {
            // 查找视图名称与新视图名称匹配的ViewState
            var matchingViewState = m_ViewStates.FirstOrDefault(x => x.viewName == newViewName);

            // 如果找到匹配项，则使用其状态值；否则默认为false
            bool isMatchingView = (matchingViewState.viewName != null) ? matchingViewState.state : false;

            // 根据匹配结果设置元素的显示状态
            m_ElementToToggle.style.display = (isMatchingView) ? DisplayStyle.Flex : DisplayStyle.None;
            m_ElementToToggle.style.visibility = (isMatchingView) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}