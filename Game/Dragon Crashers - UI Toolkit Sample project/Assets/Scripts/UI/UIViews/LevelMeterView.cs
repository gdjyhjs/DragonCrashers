using UnityEngine.UIElements;
using System.Threading.Tasks;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// 使用自定义径向计数器显示玩家的总经验等级（所有四个角色等级的总和）
    /// </summary>
    public class LevelMeterView : UIView
    {
        // 表示玩家等级的径向进度条的向量元素
        RadialProgress m_LevelMeterRadialProgressBar;

        // 显示玩家当前数字等级的标签
        Label m_LevelMeterNumberLabel;

        // 显示玩家基于总等级的排名的标签（悬停时可见）
        Label m_LevelMeterRankLabel;

        // 保存玩家的等级数据，该数据绑定到用户界面
        readonly LevelMeterData m_LevelMeterData;

        bool m_IsRankLabelVisible;
        bool m_IsCooldownActive;

        const int k_DelayInSeconds = 1;

        /// <summary>
        /// LevelMeterView的构造函数，使用提供的数据初始化等级计量器并注册悬停事件。
        /// </summary>
        /// <param name="topElement">包含等级计量器用户界面的根VisualElement。</param>
        /// <param name="levelMeterData">玩家总等级的数据源。</param>
        public LevelMeterView(VisualElement topElement, LevelMeterData levelMeterData) : base(topElement)
        {
            // 监听角色等级变化
            m_LevelMeterData = levelMeterData;

            // 注册指针离开事件
            topElement.RegisterCallback<PointerLeaveEvent>(evt => OnPointerLeave());

            // 使用指针按下事件切换排名标签的可见性
            topElement.RegisterCallback<PointerDownEvent>(evt => OnPointerDown());

            // 如果拖动到等级计量器上，则启用排名标签可见性
            topElement.RegisterCallback<PointerEnterEvent>(evt => OnPointerEnter());
        }

        /// <summary>
        /// 设置可视化元素和数据绑定。
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // 设置可视化元素
            SetVisualElements();
            // 设置数据绑定
            SetupDataBindings();

            // 默认隐藏
            m_LevelMeterRankLabel.style.display = DisplayStyle.Flex;
            m_LevelMeterRankLabel.style.opacity = 0;
        }

        /// <summary>
        /// 查询等级计量器的顶部元素以分配不同的用户界面元素
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // 引用圆形进度条
            m_LevelMeterRadialProgressBar = m_TopElement.Q<RadialProgress>("level-meter__radial-progress");

            // 引用等级数字标签
            m_LevelMeterNumberLabel = m_TopElement.Q<Label>("level-meter__number");

            m_LevelMeterRankLabel = m_TopElement.Q<Label>("level-meter__rank");
        }

        /// <summary>
        /// 使用运行时数据绑定系统设置数据绑定
        /// </summary>
        void SetupDataBindings()
        {
            // 为TotalLevels创建数据绑定
            var totalLevelBinding = new DataBinding()
            {
                dataSource = m_LevelMeterData, // 数据来源（LevelMeterData）
                dataSourcePath =
                    new PropertyPath(nameof(m_LevelMeterData.TotalLevels)), // 指向TotalLevels属性的路径
                bindingMode = BindingMode.ToTarget // 单向绑定（数据 -> 用户界面）
            };

            // 为径向计数器应用每个绑定的转换器（int到float）
            totalLevelBinding.sourceToUiConverters.AddConverter((ref int total) => (float)total);

            // 将径向进度条的Progress属性绑定到TotalLevels
            m_LevelMeterRadialProgressBar.SetBinding("Progress", totalLevelBinding);

            // 绑定数字文本
            m_LevelMeterNumberLabel.SetBinding("text", totalLevelBinding);

            // 使用不同的转换器绑定相同的数据源；当鼠标悬停在等级计量器上时显示此文本
            var rankLevelBinding = new DataBinding()
            {
                dataSource = m_LevelMeterData, // 数据来源（LevelMeterData）
                dataSourcePath =
                    new PropertyPath(nameof(m_LevelMeterData.TotalLevels)), // 指向TotalLevels属性的路径
                bindingMode = BindingMode.ToTarget // 单向绑定（数据 -> 用户界面）
            };

            rankLevelBinding.sourceToUiConverters.AddConverter(
                (ref int total) => LevelMeterData.GetRankFromLevel(total));

            m_LevelMeterRankLabel.SetBinding("text", rankLevelBinding);
        }

        /// <summary>
        /// 当指针（鼠标或触摸）按下或进入元素时调用。
        /// </summary>
        private void OnPointerEnter()
        {
            if (!m_IsRankLabelVisible)
            {
                // 当指针进入或点击时立即显示标签
                ShowRankLabel(true);
            }
        }

        /// <summary>
        /// 当指针（鼠标或触摸）离开元素时调用
        /// </summary>
        private async void OnPointerLeave()
        {
            if (m_IsRankLabelVisible && !m_IsCooldownActive)
            {
                await StartCooldown();
            }
        }

        /// <summary>
        /// 当指针（鼠标或触摸）按下时调用
        /// </summary>
        private void OnPointerDown()
        {
            if (!m_IsRankLabelVisible)
            {
                // 立即显示标签
                ShowRankLabel(true);
            }
        }

        /// <summary>
        /// 切换排名标签的可见性。
        /// </summary>
        private void ShowRankLabel(bool state)
        {
            if (state)
            {
                m_LevelMeterRankLabel.style.display = DisplayStyle.Flex;
                m_LevelMeterRankLabel.style.opacity = 1f; // 显示排名标签
                m_IsRankLabelVisible = true;
            }
            else
            {
                m_LevelMeterRankLabel.style.opacity = 0f; // 隐藏排名标签
                m_IsRankLabelVisible = false;
            }
        }

        /// <summary>
        /// 启动冷却时间，在n秒后隐藏排名标签
        /// </summary>
        private async Task StartCooldown()
        {
            m_IsCooldownActive = true;

            await Task.Delay(k_DelayInSeconds * 1000); // 等待n秒
            // 冷却时间结束后隐藏标签
            ShowRankLabel(false);

            m_IsCooldownActive = false;
        }
    }
}