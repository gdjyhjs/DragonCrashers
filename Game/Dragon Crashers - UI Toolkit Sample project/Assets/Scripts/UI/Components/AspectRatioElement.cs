using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 一个VisualElement，通过调整其内边距来强制指定的宽高比，从而影响子元素。
    /// </summary>
    [UxmlElement]
    public partial class AspectRatioElement : VisualElement
    {
        // 宽度比例。
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

        // 高度比例。
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

        // RatioWidth和RatioHeight的后备字段，默认宽高比为16:9。
        int m_RatioWidth = 16;
        int m_RatioHeight = 9;

        /// <summary>
        /// 清除元素上的所有内边距。
        /// </summary>
        private void ClearPadding()
        {
            style.paddingLeft = 0;
            style.paddingRight = 0;
            style.paddingBottom = 0;
            style.paddingTop = 0;
        }

        // 更新内边距。
        private void UpdateAspect()
        {
            // 所需的宽高比
            var designRatio = (float)RatioWidth / RatioHeight;

            // 当前的宽高比
            var currRatio = resolvedStyle.width / resolvedStyle.height;

            // 确定当前比例与所需比例之间的差异。
            var diff = currRatio - designRatio;

            // 定义一个小阈值以考虑浮点误差。
            const float epsilon = 0.01f;

            if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
            {
                ClearPadding();
                Debug.LogError($"[AspectRatio] 无效的宽度:{RatioWidth} 或高度:{RatioHeight}");
                return;
            }

            // 检查解析后的宽度和高度是否有效。
            if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
            {
                return;
            }

            // 如果元素比所需的宽高比宽。
            if (diff > epsilon)
            {
                var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
                style.paddingLeft = w;
                style.paddingRight = w;
                style.paddingTop = 0;
                style.paddingBottom = 0;
            }
            // 如果元素比所需的宽高比高。
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
                // 当前宽高比足够接近；清除任何内边距。
                ClearPadding();
            }
        }
    }

}