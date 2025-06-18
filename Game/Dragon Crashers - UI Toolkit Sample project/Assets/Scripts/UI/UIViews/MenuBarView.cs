using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // 显示菜单按钮
    public class MenuBarView : UIView
    {
        // 用于在活动和非活动状态之间切换的类/选择器
        const string k_LabelInactiveClass = "menu__label";
        const string k_LabelActiveClass = "menu__label--active";

        const string k_IconInactiveClass = "menu__icon";
        const string k_IconActiveClass = "menu__icon--active";

        const string k_ButtonInactiveClass = "menu__button";
        const string k_ButtonActiveClass = "menu__button--active";

        // 标记移动动画的常量
        const int k_MoveTime = 150;
        const float k_Spacing = 100f;
        const float k_yOffset = -8f;

        // UI 按钮
        Button m_HomeScreenMenuButton;
        Button m_CharScreenMenuButton;
        Button m_InfoScreenMenuButton;
        Button m_ShopScreenMenuButton;
        Button m_MailScreenMenuButton;

        Button m_ActiveButton;
        bool m_InterruptAnimation;

        // 表示当前活动的菜单
        VisualElement m_MenuMarker;

        public MenuBarView(VisualElement topElement) : base(topElement)
        {
            // 监听纵横比/视口变化
            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            // 如果点击 OptionsBarView 的金币/宝石图标，则更新活动菜单标记
            MainMenuUIEvents.OptionsBarShopScreenShown += OnOptionsBarShopScreenShown;
        }

        public override void Dispose()
        {
            base.Dispose();

            // 取消订阅事件
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;
            MainMenuUIEvents.OptionsBarShopScreenShown -= OnOptionsBarShopScreenShown;

            UnregisterButtonCallbacks();
        }

        // 设置菜单元素
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_HomeScreenMenuButton = m_TopElement.Q<Button>("menu__home-button");
            m_CharScreenMenuButton = m_TopElement.Q<Button>("menu__char-button");
            m_InfoScreenMenuButton = m_TopElement.Q<Button>("menu__info-button");
            m_ShopScreenMenuButton = m_TopElement.Q<Button>("menu__shop-button");
            m_MailScreenMenuButton = m_TopElement.Q<Button>("menu__mail-button");

            m_MenuMarker = m_TopElement.Q("menu__current-marker");
        }

        // 注册按钮点击的回调
        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            // 注册每个按钮点击时的操作
            m_HomeScreenMenuButton.RegisterCallback<ClickEvent>(ClickHomeButton);
            m_CharScreenMenuButton.RegisterCallback<ClickEvent>(ClickCharButton);
            m_InfoScreenMenuButton.RegisterCallback<ClickEvent>(ClickInfoButton);
            m_ShopScreenMenuButton.RegisterCallback<ClickEvent>(ClickShopButton);
            m_MailScreenMenuButton.RegisterCallback<ClickEvent>(ClickMailButton);

            // 等待界面构建完成（GeometryChangedEvent），否则标记可能会错过目标
            m_MenuMarker.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        // 可选：在大多数情况下，取消注册按钮回调并不是严格必要的，这取决于应用程序的生命周期管理。
        // 如果需要特定场景，可以选择取消注册它们。
        protected void UnregisterButtonCallbacks()
        {
            m_HomeScreenMenuButton.UnregisterCallback<ClickEvent>(ClickHomeButton);
            m_CharScreenMenuButton.UnregisterCallback<ClickEvent>(ClickCharButton);
            m_InfoScreenMenuButton.UnregisterCallback<ClickEvent>(ClickInfoButton);
            m_ShopScreenMenuButton.UnregisterCallback<ClickEvent>(ClickShopButton);
            m_MailScreenMenuButton.UnregisterCallback<ClickEvent>(ClickMailButton);

            m_MenuMarker.UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        // 通知 MainMenuUIManager 显示一个菜单屏幕。高亮显示点击的按钮并
        // 将活动标记移动到该按钮

        void ClickHomeButton(ClickEvent evt)
        {
            ActivateButton(m_HomeScreenMenuButton);
            MainMenuUIEvents.HomeScreenShown?.Invoke();

            MoveMarkerToClick(evt);
        }

        void ClickCharButton(ClickEvent evt)
        {
            ActivateButton(m_CharScreenMenuButton);
            MainMenuUIEvents.CharScreenShown?.Invoke();
            MoveMarkerToClick(evt);

        }
        void ClickInfoButton(ClickEvent evt)
        {
            ActivateButton(m_InfoScreenMenuButton);
            MainMenuUIEvents.InfoScreenShown?.Invoke();
            MoveMarkerToClick(evt);
        }

        void ClickShopButton(ClickEvent evt)
        {
            MainMenuUIEvents.ShopScreenShown?.Invoke();
            ActivateButton(m_ShopScreenMenuButton);
            MoveMarkerToClick(evt);
        }

        void ClickMailButton(ClickEvent evt)
        {
            MainMenuUIEvents.MailScreenShown?.Invoke();
            ActivateButton(m_MailScreenMenuButton);
            MoveMarkerToClick(evt);
        }

        // 激活一个按钮，高亮显示其标签和图标
        void ActivateButton(Button menuButton)
        {
            // 存储此按钮，以便在切换纵横比时刷新标记
            m_ActiveButton = menuButton;

            HighlightElement(menuButton, k_ButtonInactiveClass, k_ButtonActiveClass, m_TopElement);

            // 启用标签并禁用其他标签
            Label label = menuButton.Q<Label>(className: k_LabelInactiveClass);
            HighlightElement(label, k_LabelInactiveClass, k_LabelActiveClass, m_TopElement);

            // 启用图标并禁用其他图标
            VisualElement icon = menuButton.Q<VisualElement>(className: k_IconInactiveClass);
            HighlightElement(icon, k_IconInactiveClass, k_IconActiveClass, m_TopElement);

        }

        // 当点击目标 VisualElement 时移动标记
        void MoveMarkerToClick(ClickEvent evt)
        {
            // 当我们点击目标 VisualElement 并且 ClickEvent 已经处理完毕时移动标记；等待 "BubbleUp" 阶段
            // 以确保目标元素已经完全处理了事件
            if (evt.propagationPhase == PropagationPhase.BubbleUp)
            {
                MoveMarkerToElement(evt.target as VisualElement);
            }
            AudioManager.PlayDefaultButtonSound();
        }

        // 将活动标记移动到目标 VisualElement
        void MoveMarkerToElement(VisualElement targetElement)
        {

            // 世界空间位置
            Vector2 targetInWorldSpace = targetElement.parent.LocalToWorld(targetElement.layout.position);

            // 转换为菜单标记父元素的局部空间
            Vector3 targetInRootSpace = m_MenuMarker.parent.WorldToLocal(targetInWorldSpace);

            // 图像大小之间的差异
            Vector3 offset = new Vector3(0f, targetElement.parent.layout.height - targetElement.layout.height + k_yOffset, 0f);

            Vector3 newPosition = targetInRootSpace - offset;

            // 如果纵横比/主题改变，使用持续时间为 0；否则，根据距离计算
            int duration = m_InterruptAnimation ? 0 : CalculateDuration(newPosition);

            // 使用补间工具进行动画
            m_MenuMarker.experimental.animation.Position(targetInRootSpace - offset, duration);

        }

        // 根据距离计算标记动画的持续时间
        int CalculateDuration(Vector3 newPosition)
        {
            // 如果移动超过一个空间，则增加额外的动画时间
            Vector3 delta = m_MenuMarker.transform.position - newPosition;

            float distanceInPixels = Mathf.Abs(delta.y / k_Spacing);

            int duration = Mathf.Clamp((int)distanceInPixels * k_MoveTime, k_MoveTime, k_MoveTime * 4);
            return duration;
        }

        // 为当前选定的按钮/元素启用高亮样式
        void HighlightElement(VisualElement targetElement, string inactiveClass, string activeClass, VisualElement root)
        {
            if (targetElement == null)
                return;

            // 找到当前活动的元素 
            VisualElement currentSelection = root.Query<VisualElement>(className: activeClass);

            // 如果我们选择了已经是当前活动的元素，则不执行任何操作
            if (currentSelection == targetElement)
            {
                return;
            }

            // 取消高亮显示当前活动的元素
            currentSelection.RemoveFromClassList(activeClass);
            currentSelection.AddToClassList(inactiveClass);

            // 高亮显示目标元素
            targetElement.RemoveFromClassList(inactiveClass);
            targetElement.AddToClassList(activeClass);
        }


        // 事件处理方法

        // 当启动场景或切换纵横比时
        void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {

            if (m_ActiveButton == null)
                m_ActiveButton = m_HomeScreenMenuButton;

            ActivateButton(m_ActiveButton);
            MoveMarkerToElement(m_ActiveButton);
        }

        // 切换纵横比时中断动画并将标记移动到活动按钮
        void OnAspectRatioUpdated(MediaAspectRatio newAspectRatio)
        {
            m_InterruptAnimation = true;

            if (m_ActiveButton != null)
                MoveMarkerToElement(m_ActiveButton);

            m_InterruptAnimation = false;
        }

        // 从 OptionsBar 打开商店屏幕。激活按钮并移动标记，无需
        // 点击事件。
        private void OnOptionsBarShopScreenShown()
        {
            // 调用事件以切换到商店屏幕
            MainMenuUIEvents.ShopScreenShown?.Invoke();

            // 激活相应的菜单按钮
            ActivateButton(m_ShopScreenMenuButton);

            // 将标记移动到 "商店" 按钮
            MoveMarkerToElement(m_ShopScreenMenuButton);
        }

    }
}