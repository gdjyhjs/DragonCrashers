using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // ��ʾ�˵���ť
    public class MenuBarView : UIView
    {
        // �����ڻ�ͷǻ״̬֮���л�����/ѡ����
        const string k_LabelInactiveClass = "menu__label";
        const string k_LabelActiveClass = "menu__label--active";

        const string k_IconInactiveClass = "menu__icon";
        const string k_IconActiveClass = "menu__icon--active";

        const string k_ButtonInactiveClass = "menu__button";
        const string k_ButtonActiveClass = "menu__button--active";

        // ����ƶ������ĳ���
        const int k_MoveTime = 150;
        const float k_Spacing = 100f;
        const float k_yOffset = -8f;

        // UI ��ť
        Button m_HomeScreenMenuButton;
        Button m_CharScreenMenuButton;
        Button m_InfoScreenMenuButton;
        Button m_ShopScreenMenuButton;
        Button m_MailScreenMenuButton;

        Button m_ActiveButton;
        bool m_InterruptAnimation;

        // ��ʾ��ǰ��Ĳ˵�
        VisualElement m_MenuMarker;

        public MenuBarView(VisualElement topElement) : base(topElement)
        {
            // �����ݺ��/�ӿڱ仯
            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            // ������ OptionsBarView �Ľ��/��ʯͼ�꣬����»�˵����
            MainMenuUIEvents.OptionsBarShopScreenShown += OnOptionsBarShopScreenShown;
        }

        public override void Dispose()
        {
            base.Dispose();

            // ȡ�������¼�
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;
            MainMenuUIEvents.OptionsBarShopScreenShown -= OnOptionsBarShopScreenShown;

            UnregisterButtonCallbacks();
        }

        // ���ò˵�Ԫ��
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

        // ע�ᰴť����Ļص�
        protected override void RegisterButtonCallbacks()
        {
            base.RegisterButtonCallbacks();

            // ע��ÿ����ť���ʱ�Ĳ���
            m_HomeScreenMenuButton.RegisterCallback<ClickEvent>(ClickHomeButton);
            m_CharScreenMenuButton.RegisterCallback<ClickEvent>(ClickCharButton);
            m_InfoScreenMenuButton.RegisterCallback<ClickEvent>(ClickInfoButton);
            m_ShopScreenMenuButton.RegisterCallback<ClickEvent>(ClickShopButton);
            m_MailScreenMenuButton.RegisterCallback<ClickEvent>(ClickMailButton);

            // �ȴ����湹����ɣ�GeometryChangedEvent���������ǿ��ܻ���Ŀ��
            m_MenuMarker.RegisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        // ��ѡ���ڴ��������£�ȡ��ע�ᰴť�ص��������ϸ��Ҫ�ģ���ȡ����Ӧ�ó�����������ڹ���
        // �����Ҫ�ض�����������ѡ��ȡ��ע�����ǡ�
        protected void UnregisterButtonCallbacks()
        {
            m_HomeScreenMenuButton.UnregisterCallback<ClickEvent>(ClickHomeButton);
            m_CharScreenMenuButton.UnregisterCallback<ClickEvent>(ClickCharButton);
            m_InfoScreenMenuButton.UnregisterCallback<ClickEvent>(ClickInfoButton);
            m_ShopScreenMenuButton.UnregisterCallback<ClickEvent>(ClickShopButton);
            m_MailScreenMenuButton.UnregisterCallback<ClickEvent>(ClickMailButton);

            m_MenuMarker.UnregisterCallback<GeometryChangedEvent>(OnGeometryChangedEvent);
        }

        // ֪ͨ MainMenuUIManager ��ʾһ���˵���Ļ��������ʾ����İ�ť��
        // �������ƶ����ð�ť

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

        // ����һ����ť��������ʾ���ǩ��ͼ��
        void ActivateButton(Button menuButton)
        {
            // �洢�˰�ť���Ա����л��ݺ��ʱˢ�±��
            m_ActiveButton = menuButton;

            HighlightElement(menuButton, k_ButtonInactiveClass, k_ButtonActiveClass, m_TopElement);

            // ���ñ�ǩ������������ǩ
            Label label = menuButton.Q<Label>(className: k_LabelInactiveClass);
            HighlightElement(label, k_LabelInactiveClass, k_LabelActiveClass, m_TopElement);

            // ����ͼ�겢��������ͼ��
            VisualElement icon = menuButton.Q<VisualElement>(className: k_IconInactiveClass);
            HighlightElement(icon, k_IconInactiveClass, k_IconActiveClass, m_TopElement);

        }

        // �����Ŀ�� VisualElement ʱ�ƶ����
        void MoveMarkerToClick(ClickEvent evt)
        {
            // �����ǵ��Ŀ�� VisualElement ���� ClickEvent �Ѿ��������ʱ�ƶ���ǣ��ȴ� "BubbleUp" �׶�
            // ��ȷ��Ŀ��Ԫ���Ѿ���ȫ�������¼�
            if (evt.propagationPhase == PropagationPhase.BubbleUp)
            {
                MoveMarkerToElement(evt.target as VisualElement);
            }
            AudioManager.PlayDefaultButtonSound();
        }

        // �������ƶ���Ŀ�� VisualElement
        void MoveMarkerToElement(VisualElement targetElement)
        {

            // ����ռ�λ��
            Vector2 targetInWorldSpace = targetElement.parent.LocalToWorld(targetElement.layout.position);

            // ת��Ϊ�˵���Ǹ�Ԫ�صľֲ��ռ�
            Vector3 targetInRootSpace = m_MenuMarker.parent.WorldToLocal(targetInWorldSpace);

            // ͼ���С֮��Ĳ���
            Vector3 offset = new Vector3(0f, targetElement.parent.layout.height - targetElement.layout.height + k_yOffset, 0f);

            Vector3 newPosition = targetInRootSpace - offset;

            // ����ݺ��/����ı䣬ʹ�ó���ʱ��Ϊ 0�����򣬸��ݾ������
            int duration = m_InterruptAnimation ? 0 : CalculateDuration(newPosition);

            // ʹ�ò��乤�߽��ж���
            m_MenuMarker.experimental.animation.Position(targetInRootSpace - offset, duration);

        }

        // ���ݾ�������Ƕ����ĳ���ʱ��
        int CalculateDuration(Vector3 newPosition)
        {
            // ����ƶ�����һ���ռ䣬�����Ӷ���Ķ���ʱ��
            Vector3 delta = m_MenuMarker.transform.position - newPosition;

            float distanceInPixels = Mathf.Abs(delta.y / k_Spacing);

            int duration = Mathf.Clamp((int)distanceInPixels * k_MoveTime, k_MoveTime, k_MoveTime * 4);
            return duration;
        }

        // Ϊ��ǰѡ���İ�ť/Ԫ�����ø�����ʽ
        void HighlightElement(VisualElement targetElement, string inactiveClass, string activeClass, VisualElement root)
        {
            if (targetElement == null)
                return;

            // �ҵ���ǰ���Ԫ�� 
            VisualElement currentSelection = root.Query<VisualElement>(className: activeClass);

            // �������ѡ�����Ѿ��ǵ�ǰ���Ԫ�أ���ִ���κβ���
            if (currentSelection == targetElement)
            {
                return;
            }

            // ȡ��������ʾ��ǰ���Ԫ��
            currentSelection.RemoveFromClassList(activeClass);
            currentSelection.AddToClassList(inactiveClass);

            // ������ʾĿ��Ԫ��
            targetElement.RemoveFromClassList(inactiveClass);
            targetElement.AddToClassList(activeClass);
        }


        // �¼�������

        // �������������л��ݺ��ʱ
        void OnGeometryChangedEvent(GeometryChangedEvent evt)
        {

            if (m_ActiveButton == null)
                m_ActiveButton = m_HomeScreenMenuButton;

            ActivateButton(m_ActiveButton);
            MoveMarkerToElement(m_ActiveButton);
        }

        // �л��ݺ��ʱ�ж϶�����������ƶ������ť
        void OnAspectRatioUpdated(MediaAspectRatio newAspectRatio)
        {
            m_InterruptAnimation = true;

            if (m_ActiveButton != null)
                MoveMarkerToElement(m_ActiveButton);

            m_InterruptAnimation = false;
        }

        // �� OptionsBar ���̵���Ļ�����ť���ƶ���ǣ�����
        // ����¼���
        private void OnOptionsBarShopScreenShown()
        {
            // �����¼����л����̵���Ļ
            MainMenuUIEvents.ShopScreenShown?.Invoke();

            // ������Ӧ�Ĳ˵���ť
            ActivateButton(m_ShopScreenMenuButton);

            // ������ƶ��� "�̵�" ��ť
            MoveMarkerToElement(m_ShopScreenMenuButton);
        }

    }
}