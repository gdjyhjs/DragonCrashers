using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 一个自定义的VisualElement，用于显示带有标题的血条，以进度条的形式显示当前和最大生命值。
    /// </summary>
    [UxmlElement]
    public partial class HealthBarComponent : VisualElement
    {
        /// <summary>
        /// USS样式的类名
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

        // 生命值数据的后备字段
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

        // 构造函数初始化血条元素
        public HealthBarComponent()
        {
            // 标题背景元素
            m_TitleBackground = new VisualElement { name = "HealthBarTitleBackground" };
            m_TitleBackground.AddToClassList(ClassNames.HealthBarTitleBackground);
            Add(m_TitleBackground);

            // 标题标签
            m_TitleLabel = new Label() { name = "HealthBarTitle" };
            m_TitleLabel.AddToClassList(ClassNames.HealthBarTitle);
            m_TitleBackground.Add(m_TitleLabel);

            // 添加容器类以进行整体样式设置
            AddToClassList(ClassNames.HealthBarContainer);

            // 血条的背景元素
            m_Background = new VisualElement { name = "HealthBarBackground" };
            m_Background.AddToClassList(ClassNames.HealthBarBackground);
            Add(m_Background);

            // 显示当前生命值的进度条元素
            m_Progress = new VisualElement { name = "HealthBarProgress" };
            m_Progress.AddToClassList(ClassNames.HealthBarProgress);
            m_Background.Add(m_Progress);

            // 显示当前和最大生命值的标签
            m_HealthStat = new Label() { name = "HealthBarStat" };
            m_HealthStat.AddToClassList(ClassNames.HealthBarLabel);
            m_Progress.Add(m_HealthStat);

            BindElements();
        }

        // 绑定元素
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