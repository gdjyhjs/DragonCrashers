using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // �ο��õĸ���������ʹ��ʱ����
    [ExecuteInEditMode]
    [RequireComponent(typeof(UIDocument))]
    public class ReferenceScreen : MonoBehaviour
    {
        UIDocument document;
        VisualElement m_Root;

        // ͸���ȣ���Χ��0��1
        [SerializeField] [Range(0f, 1f)] float m_Opacity = 0.5f;
        // �Ƿ��ڲ���ʱ����
        public bool disableOnPlay = true;

        void OnEnable()
        {
            document = GetComponent<UIDocument>();
            m_Root = document.rootVisualElement;

            // ���������ϲ�
            document.sortingOrder = 9999;
        }

        private void Start()
        {
            if (disableOnPlay)
            {
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (m_Root != null)
                m_Root.style.opacity = m_Opacity;
        }
    }
}