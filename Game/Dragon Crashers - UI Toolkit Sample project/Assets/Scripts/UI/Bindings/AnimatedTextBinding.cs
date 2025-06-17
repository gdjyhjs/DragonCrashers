using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// һ���Զ���󶨣�������Դ����ʱΪ��ǩ���ı�ֵ���ö�����
    /// UxmlObject������������UI����������ʾ��
    /// </summary>
    [UxmlObject]
    public partial class AnimatedTextBinding : CustomBinding, IDataSourceProvider
    {
        // ����Դ����
        public object dataSource { get; set; }
        public PropertyPath dataSourcePath { get; set; }

        // ����״̬
        uint m_CurrentValue = 0;
        uint m_TargetValue = 0;
        bool m_IsAnimating = false;

        readonly float m_AnimationDuration = 0.5f; // ��������ʱ�䣨�룩
        float m_AnimationStartTime = 0f;

        /// <summary>
        /// ���캯������ʼ��AnimatedTextBinding�����ʵ����
        /// </summary>
        public AnimatedTextBinding()
        {
            // updateTrigger��һ�����ԣ�����ȷ���󶨺�ʱӦ������Ŀ��ֵ��
            // ����������У�OnSourceChanged��Դ���ݸ���ʱ��������
            updateTrigger = BindingUpdateTrigger.OnSourceChanged;
        }

        /// <summary>
        /// ͨ��������Դֵ����ʱΪ��ǩ���ı����ö��������°󶨡�
        /// </summary>
        /// <param name="context">����Ŀ��Ԫ�صİ������ġ�</param>
        /// <returns>һ��BindingResult��ָʾ���µĳɹ���ʧ�ܡ�</returns>
        protected override BindingResult Update(in BindingContext context)
        {
            // ��ǩԪ��
            var element = context.targetElement;

            // ���Դ�����Դ��ȡֵ
            if (!TryGetValue(out uint newValue))
            {
                // ����Դ��Ч
                return new BindingResult(BindingStatus.Failure,
                    "[AnimatedTextBinding] Update: ������Դ����ֵʧ�ܡ�");
            }

            // ���Ԫ���Ƿ�Ϊ��ǩ
            if (element is not Label label)
            {
                return new BindingResult(BindingStatus.Failure,
                    "[AnimatedTextBinding] Update: Ŀ��Ԫ�ز��Ǳ�ǩ��");
            }

            // �����ֵ�Ƿ���Ŀ��ֵ��ͬ
            if (newValue == m_TargetValue)
            {
                return new BindingResult(BindingStatus.Success);
            }

            // ��ʼ����������
            m_CurrentValue = m_IsAnimating ? GetCurrentAnimatedValue() : m_TargetValue;

            m_TargetValue = newValue;
            m_AnimationStartTime = Time.realtimeSinceStartup;
            m_IsAnimating = true;

            // ��ʼ����
            AnimateValue(label);

            return new BindingResult(BindingStatus.Success);
        }

        /// <summary>
        /// �ӵ�ǰֵ��Ŀ��ֵΪ��ǩ���ı�ֵ���ö�����
        /// </summary>
        /// <param name="label">Ҫ���µı�ǩ��</param>
        void AnimateValue(Label label)
        {
            if (!m_IsAnimating)
                return;

            // ��������ʱ��
            float elapsedTime = Time.realtimeSinceStartup - m_AnimationStartTime;

            // ��һ������t��0 = ��ʼ��1 = ��ɣ���������ʱ������ڶ�������ʱ��
            float t = Mathf.Clamp01(elapsedTime / m_AnimationDuration);

            // �ڵ�ǰֵ��Ŀ��ֵ֮����в�ֵ
            uint interpolatedValue = (uint)Mathf.Lerp(m_CurrentValue, m_TargetValue, t);
            label.text = interpolatedValue.ToString();

            // ���������ɣ�����Ŀ��ֵ�ͱ�־
            if (t >= 1f)
            {
                m_CurrentValue = m_TargetValue;
                m_IsAnimating = false;
            }
            else
            {
                // ����ÿ֡����AnimateValue��IVisualElementScheduler����������ض�ʱ�������������
                label.schedule.Execute(() => AnimateValue(label)).StartingIn(0);
            }
        }

        /// <summary>
        /// ��ȡ�����ڼ�ĵ�ǰ��ֵֵ��
        /// </summary>
        /// <returns>��Ϊuint�Ĳ�ֵֵ��</returns>
        uint GetCurrentAnimatedValue()
        {
            float elapsedTime = Time.realtimeSinceStartup - m_AnimationStartTime;

            float t = Mathf.Clamp01(elapsedTime / m_AnimationDuration);

            return (uint)Mathf.Lerp(m_CurrentValue, m_TargetValue, t);
        }

        /// <summary>
        /// ���Դ�����Դ����ֵ��
        /// </summary>
        /// <param name="value">��������ֵ��</param>
        /// <returns>����ɹ�������ֵ����Ϊtrue������Ϊfalse��</returns>
        bool TryGetValue(out uint value)
        {
            value = default;

            if (dataSource == null)
                return false;

            // ʹ��PropertyContainer��ȡֵ
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