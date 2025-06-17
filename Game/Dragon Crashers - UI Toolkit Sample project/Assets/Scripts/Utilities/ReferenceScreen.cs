using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // 参考用的覆盖纹理；不使用时禁用
    [ExecuteInEditMode]
    [RequireComponent(typeof(UIDocument))]
    public class ReferenceScreen : MonoBehaviour
    {
        UIDocument document;
        VisualElement m_Root;

        // 透明度，范围从0到1
        [SerializeField] [Range(0f, 1f)] float m_Opacity = 0.5f;
        // 是否在播放时禁用
        public bool disableOnPlay = true;

        void OnEnable()
        {
            document = GetComponent<UIDocument>();
            m_Root = document.rootVisualElement;

            // 覆盖在最上层
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