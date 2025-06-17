using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// һ��VisualElement��ͨ���������ڱ߾���ǿ��ָ���Ŀ�߱ȣ��Ӷ�Ӱ����Ԫ�ء�
    /// </summary>
    [UxmlElement]
    public partial class AspectRatioElement : VisualElement
    {
        // ��ȱ�����
        [UxmlAttribute("width")]
        public int RatioWidth
        {
            get => m_RatioWidth;
            set
            {
                m_RatioWidth = value;
                UpdateAspect();
            }
        }

        // �߶ȱ�����
        [UxmlAttribute("height")]
        public int RatioHeight
        {
            get => m_RatioHeight;
            set
            {
                m_RatioHeight = value;
                UpdateAspect();
            }
        }

        // RatioWidth��RatioHeight�ĺ��ֶΣ�Ĭ�Ͽ�߱�Ϊ16:9��
        int m_RatioWidth = 16;
        int m_RatioHeight = 9;

        /// <summary>
        /// ���Ԫ���ϵ������ڱ߾ࡣ
        /// </summary>
        private void ClearPadding()
        {
            style.paddingLeft = 0;
            style.paddingRight = 0;
            style.paddingBottom = 0;
            style.paddingTop = 0;
        }

        // �����ڱ߾ࡣ
        private void UpdateAspect()
        {
            // ����Ŀ�߱�
            var designRatio = (float)RatioWidth / RatioHeight;

            // ��ǰ�Ŀ�߱�
            var currRatio = resolvedStyle.width / resolvedStyle.height;

            // ȷ����ǰ�������������֮��Ĳ��졣
            var diff = currRatio - designRatio;

            // ����һ��С��ֵ�Կ��Ǹ�����
            const float epsilon = 0.01f;

            if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
            {
                ClearPadding();
                Debug.LogError($"[AspectRatio] ��Ч�Ŀ��:{RatioWidth} ��߶�:{RatioHeight}");
                return;
            }

            // ��������Ŀ�Ⱥ͸߶��Ƿ���Ч��
            if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
            {
                return;
            }

            // ���Ԫ�ر�����Ŀ�߱ȿ�
            if (diff > epsilon)
            {
                var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
                style.paddingLeft = w;
                style.paddingRight = w;
                style.paddingTop = 0;
                style.paddingBottom = 0;
            }
            // ���Ԫ�ر�����Ŀ�߱ȸߡ�
            else if (diff < -epsilon)
            {
                var h = (resolvedStyle.height - (resolvedStyle.width * (1 / designRatio))) * 0.5f;
                style.paddingLeft = 0;
                style.paddingRight = 0;
                style.paddingTop = h;
                style.paddingBottom = h;
            }
            else
            {
                // ��ǰ��߱��㹻�ӽ�������κ��ڱ߾ࡣ
                ClearPadding();
            }
        }
    }

}