using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// 这是 UI 的一个功能单元的基类。它可以构成一个全屏界面，也可以只是
    /// 其中的一部分。
    /// </summary>

    public class UIView : IDisposable
    {
        protected bool m_HideOnAwake = true;

        // UI 显示其他底层 UI，部分透明
        protected bool m_IsOverlay;

        protected VisualElement m_TopElement;

        // 属性
        public VisualElement Root => m_TopElement;
        public bool IsTransparent => m_IsOverlay;
        public bool IsHidden => m_TopElement.style.display == DisplayStyle.None;

        // 构造函数
        /// <summary>
        /// 初始化 UIView 类的新实例。
        /// </summary>
        /// <param name="topElement">UXML 层次结构中最顶层的 VisualElement。</param>
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

        // 设置 UI 的 VisualElement。重写以自定义。
        protected virtual void SetVisualElements()
        {

        }

        // 注册 UI 中按钮的回调。重写以自定义。
        protected virtual void RegisterButtonCallbacks()
        {

        }

        // 显示 UI。
        public virtual void Show()
        {
            m_TopElement.style.display = DisplayStyle.Flex;
        }

        // 隐藏 UI。
        public virtual void Hide()
        {
            m_TopElement.style.display = DisplayStyle.None;
        }

        // 取消注册任何回调或事件处理程序。重写以自定义。
        public virtual void Dispose()
        {

        }
    }
}