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
        public string viewName; // ��ͼ����
        public bool state;      // ״̬
    }

    /// <summary>
    /// ���ڵ�ǰUI��ͼ�л�UIԪ����ʾ״̬�Ĺ�����
    /// </summary>
    public class ToggleOnView : MonoBehaviour
    {
        [Tooltip("����Ҫ�л���Ԫ�ص�UXML�ĵ�")]
        [SerializeField] UIDocument m_Document;

        [Tooltip("Ҫ�л����Ӿ�Ԫ�ص�����")]
        [SerializeField] string m_ElementID;

        [Header("������״̬������")]
        [Tooltip("���ݵ�ǰ���UI��ͼ����(����ͼ����ɫ��ͼ��)ָ����ʾ״̬")]
        [SerializeField] List<ViewState> m_ViewStates = new List<ViewState>();

        VisualElement m_ElementToToggle;

        void OnEnable()
        {
            Initialize(); // ��ʼ��
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
                Debug.LogWarning("[ToggleOnMenu] ��Ҫָ��UIDocument��");
                return;
            }

            m_ElementToToggle = m_Document.rootVisualElement.Q<VisualElement>(m_ElementID);

            if (m_ElementToToggle == null)
            {
                Debug.LogWarning("[ToggleOnMenu]: δ�ҵ�ָ��Ԫ�ء�");
                return;
            }

            m_ElementToToggle.style.visibility = Visibility.Visible;
        }

        // ��ǰ��ͼ�ı�ʱ����
        void OnCurrentViewChanged(string newViewName)
        {
            // ������ͼ����������ͼ����ƥ���ViewState
            var matchingViewState = m_ViewStates.FirstOrDefault(x => x.viewName == newViewName);

            // ����ҵ�ƥ�����ʹ����״ֵ̬������Ĭ��Ϊfalse
            bool isMatchingView = (matchingViewState.viewName != null) ? matchingViewState.state : false;

            // ����ƥ��������Ԫ�ص���ʾ״̬
            m_ElementToToggle.style.display = (isMatchingView) ? DisplayStyle.Flex : DisplayStyle.None;
            m_ElementToToggle.style.visibility = (isMatchingView) ? Visibility.Visible : Visibility.Hidden;
        }
    }
}