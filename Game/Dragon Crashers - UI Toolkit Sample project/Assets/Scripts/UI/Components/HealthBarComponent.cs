using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// һ���Զ����VisualElement��������ʾ���б����Ѫ�����Խ���������ʽ��ʾ��ǰ���������ֵ��
    /// </summary>
    [UxmlElement]
    public partial class HealthBarComponent : VisualElement
    {
        /// <summary>
        /// USS��ʽ������
        /// </summary>
        static class ClassNames
        {
            public static string HealthBarBackground = "health-bar__background";
            public static string HealthBarProgress = "health-bar__progress";
            public static string HealthBarTitle = "health-bar__title";
            public static string HealthBarLabel = "health-bar__label";
            public static string HealthBarContainer = "health-bar__container";
            public static string HealthBarTitleBackground = "health-bar__title_background";
        }

        // ����ֵ���ݵĺ��ֶ�
        string m_HealthBarTitle;

        readonly Label m_TitleLabel;
        readonly Label m_HealthStat;
        VisualElement m_Progress;
        VisualElement m_Background;
        VisualElement m_TitleBackground;

        HealthData m_HealthData;

        [UxmlAttribute]
        public string HealthBarTitle
        {
            get => m_HealthBarTitle;
            set => m_TitleLabel.text = value;
        }

        [CreateProperty]
        public HealthData HealthData
        {
            get => (HealthData)dataSource;
            set => dataSource = value;
        }

        // ���캯����ʼ��Ѫ��Ԫ��
        public HealthBarComponent()
        {
            // ���ⱳ��Ԫ��
            m_TitleBackground = new VisualElement { name = "HealthBarTitleBackground" };
            m_TitleBackground.AddToClassList(ClassNames.HealthBarTitleBackground);
            Add(m_TitleBackground);

            // �����ǩ
            m_TitleLabel = new Label() { name = "HealthBarTitle" };
            m_TitleLabel.AddToClassList(ClassNames.HealthBarTitle);
            m_TitleBackground.Add(m_TitleLabel);

            // ����������Խ���������ʽ����
            AddToClassList(ClassNames.HealthBarContainer);

            // Ѫ���ı���Ԫ��
            m_Background = new VisualElement { name = "HealthBarBackground" };
            m_Background.AddToClassList(ClassNames.HealthBarBackground);
            Add(m_Background);

            // ��ʾ��ǰ����ֵ�Ľ�����Ԫ��
            m_Progress = new VisualElement { name = "HealthBarProgress" };
            m_Progress.AddToClassList(ClassNames.HealthBarProgress);
            m_Background.Add(m_Progress);

            // ��ʾ��ǰ���������ֵ�ı�ǩ
            m_HealthStat = new Label() { name = "HealthBarStat" };
            m_HealthStat.AddToClassList(ClassNames.HealthBarLabel);
            m_Progress.Add(m_HealthStat);

            BindElements();
        }

        // ��Ԫ��
        void BindElements()
        {
            m_HealthStat.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(HealthData.HealthStatText)),
                bindingMode = BindingMode.ToTarget
            });

            m_Progress.SetBinding("style.width", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(HealthData.HealthProgressStyleLength)),
                bindingMode = BindingMode.ToTarget
            });
        }
    }
}