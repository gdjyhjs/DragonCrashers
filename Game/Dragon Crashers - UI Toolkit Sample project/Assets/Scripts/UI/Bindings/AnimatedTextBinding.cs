using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 一种自定义绑定，当数据源更改时为标签的文本值设置动画。
    /// UxmlObject属性允许它在UI构建器中显示。
    /// </summary>
    [UxmlObject]
    public partial class AnimatedTextBinding : CustomBinding, IDataSourceProvider
    {
        // 数据源属性
        public object dataSource { get; set; }
        public PropertyPath dataSourcePath { get; set; }

        // 动画状态
        uint m_CurrentValue = 0;
        uint m_TargetValue = 0;
        bool m_IsAnimating = false;

        readonly float m_AnimationDuration = 0.5f; // 动画持续时间（秒）
        float m_AnimationStartTime = 0f;

        /// <summary>
        /// 构造函数。初始化AnimatedTextBinding类的新实例。
        /// </summary>
        public AnimatedTextBinding()
        {
            // updateTrigger是一个属性，用于确定绑定何时应更新其目标值。
            // 在这个例子中，OnSourceChanged在源数据更改时触发更新
            updateTrigger = BindingUpdateTrigger.OnSourceChanged;
        }

        /// <summary>
        /// 通过在数据源值更改时为标签的文本设置动画来更新绑定。
        /// </summary>
        /// <param name="context">包含目标元素的绑定上下文。</param>
        /// <returns>一个BindingResult，指示更新的成功或失败。</returns>
        protected override BindingResult Update(in BindingContext context)
        {
            // 标签元素
            var element = context.targetElement;

            // 尝试从数据源获取值
            if (!TryGetValue(out uint newValue))
            {
                // 数据源无效
                return new BindingResult(BindingStatus.Failure,
                    "[AnimatedTextBinding] Update: 从数据源检索值失败。");
            }

            // 检查元素是否为标签
            if (element is not Label label)
            {
                return new BindingResult(BindingStatus.Failure,
                    "[AnimatedTextBinding] Update: 目标元素不是标签。");
            }

            // 检查新值是否与目标值不同
            if (newValue == m_TargetValue)
            {
                return new BindingResult(BindingStatus.Success);
            }

            // 初始化动画参数
            m_CurrentValue = m_IsAnimating ? GetCurrentAnimatedValue() : m_TargetValue;

            m_TargetValue = newValue;
            m_AnimationStartTime = Time.realtimeSinceStartup;
            m_IsAnimating = true;

            // 开始动画
            AnimateValue(label);

            return new BindingResult(BindingStatus.Success);
        }

        /// <summary>
        /// 从当前值到目标值为标签的文本值设置动画。
        /// </summary>
        /// <param name="label">要更新的标签。</param>
        void AnimateValue(Label label)
        {
            if (!m_IsAnimating)
                return;

            // 计算已用时间
            float elapsedTime = Time.realtimeSinceStartup - m_AnimationStartTime;

            // 归一化进度t（0 = 开始；1 = 完成）基于已用时间相对于动画持续时间
            float t = Mathf.Clamp01(elapsedTime / m_AnimationDuration);

            // 在当前值和目标值之间进行插值
            uint interpolatedValue = (uint)Mathf.Lerp(m_CurrentValue, m_TargetValue, t);
            label.text = interpolatedValue.ToString();

            // 如果动画完成，设置目标值和标志
            if (t >= 1f)
            {
                m_CurrentValue = m_TargetValue;
                m_IsAnimating = false;
            }
            else
            {
                // 否则每帧调用AnimateValue；IVisualElementScheduler允许你根据特定时间或间隔安排任务
                label.schedule.Execute(() => AnimateValue(label)).StartingIn(0);
            }
        }

        /// <summary>
        /// 获取动画期间的当前插值值。
        /// </summary>
        /// <returns>作为uint的插值值。</returns>
        uint GetCurrentAnimatedValue()
        {
            float elapsedTime = Time.realtimeSinceStartup - m_AnimationStartTime;

            float t = Mathf.Clamp01(elapsedTime / m_AnimationDuration);

            return (uint)Mathf.Lerp(m_CurrentValue, m_TargetValue, t);
        }

        /// <summary>
        /// 尝试从数据源检索值。
        /// </summary>
        /// <param name="value">检索到的值。</param>
        /// <returns>如果成功检索到值，则为true；否则为false。</returns>
        bool TryGetValue(out uint value)
        {
            value = default;

            if (dataSource == null)
                return false;

            // 使用PropertyContainer获取值
            if (PropertyContainer.TryGetValue(dataSource, dataSourcePath, out object objValue))
            {
                if (objValue is uint uintValue)
                {
                    value = uintValue;
                    return true;
                }
            }
            return false;
        }
    }
}