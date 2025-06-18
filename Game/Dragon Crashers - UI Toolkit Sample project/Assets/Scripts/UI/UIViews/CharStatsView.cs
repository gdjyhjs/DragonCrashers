using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理CharView屏幕的角色统计部分
    /// </summary>
    public class CharStatsView : UIView
    {
        // 点击冷却时间
        const float k_ClickCooldown = 0.2f;

        // 下次可点击按钮的时间
        float m_TimeToNextClick = 0f;

        // 游戏图标数据
        GameIconsSO m_GameIconsData;

        // 角色数据
        CharacterData m_CharacterData;

        // 静态基础数据
        CharacterBaseSO m_BaseStats;

        // 等级标签
        Label m_LevelLabel;

        // 活动框架
        VisualElement m_ActiveFrame;
        // 活动索引
        int m_ActiveIndex;

        // 技能图标数组
        VisualElement[] m_SkillIcons = new VisualElement[3];
        // 基础技能数组
        SkillSO[] m_BaseSkills = new SkillSO[3];

        // 角色类标签
        Label m_CharacterClass;
        // 稀有度标签
        Label m_Rarity;
        // 攻击类型标签
        Label m_AttackType;

        // 下一个技能按钮
        Button m_NextSkillButton;
        // 上一个技能按钮
        Button m_LastSkillButton;

        // 传记标题标签
        Label m_BioTitle;

        // 设置和生命周期方法

        /// <summary>
        /// 使用指定的顶级UI元素初始化CharStatsView的新实例。
        /// </summary>
        /// <param name="topElement">UI的根视觉元素。</param>
        public CharStatsView(VisualElement topElement) : base(topElement)
        {
        }

        /// <summary>
        /// 注销任何回调。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            // 注销回调
            UnregisterCallbacks();
        }

        /// <summary>
        /// 查询并初始化所有必需的视觉元素。
        /// </summary>
        protected override void SetVisualElements()
        {
            // 获取等级标签
            m_LevelLabel = m_TopElement.Q<Label>("char-stats__level-number");

            // 获取角色类标签
            m_CharacterClass = m_TopElement.Q<Label>("char-stats__class-label");
            // 获取稀有度标签
            m_Rarity = m_TopElement.Q<Label>("char-stats__rarity-label");
            // 获取攻击类型标签
            m_AttackType = m_TopElement.Q<Label>("char-stats__attack-type-label");

            // 获取技能图标
            m_SkillIcons[0] = m_TopElement.Q("char-skills__icon1");
            m_SkillIcons[1] = m_TopElement.Q("char-skills__icon2");
            m_SkillIcons[2] = m_TopElement.Q("char-skills__icon3");

            // 获取下一个技能按钮
            m_NextSkillButton = m_TopElement.Q<Button>("char-skills__next-button");
            // 获取上一个技能按钮
            m_LastSkillButton = m_TopElement.Q<Button>("char-skills__last-button");

            // 获取传记标题标签
            m_BioTitle = m_TopElement.Q<Label>("char-bio__title");

            // 获取活动框架
            m_ActiveFrame = m_TopElement.Q("char-skills__active");

            // 确保在绑定之前加载m_GameIconsData
            m_GameIconsData = Resources.Load("GameData/GameIcons") as GameIconsSO;
            if (m_GameIconsData == null)
            {
                Debug.LogError("[CharStatsView] SetVisualElements: 无法从Resources加载GameIconsSO。");
            }
        }

        /// <summary>
        /// 为交互式UI元素注册事件回调。
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // 设置点击事件
            m_NextSkillButton.RegisterCallback<ClickEvent>(SelectNextSkill);
            m_LastSkillButton.RegisterCallback<ClickEvent>(SelectLastSkill);
            m_SkillIcons[0].RegisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[1].RegisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[2].RegisterCallback<ClickEvent>(SelectSkill);

            // 布局构建后初始化活动框架位置
            m_SkillIcons[0].RegisterCallback<GeometryChangedEvent>(InitializeSkillMarker);
        }

        // 更新方法/数据绑定

        /// <summary>
        /// 切换角色时更新窗口。由CharView类外部调用。
        /// </summary>
        /// <param name="charData">要显示的角色数据。</param>
        public void UpdateCharacterStats(CharacterData charData)
        {
            m_CharacterData = charData;

            // 缓存技能
            m_BaseStats = charData.CharacterBaseData;
            m_BaseSkills[0] = m_BaseStats.Skill1;
            m_BaseSkills[1] = m_BaseStats.Skill2;
            m_BaseSkills[2] = m_BaseStats.Skill3;

            // 使用当前等级更新所有技能
            foreach (var skill in m_BaseSkills)
            {
                if (skill != null)
                {
                    // 更新技能等级
                    skill.UpdateLevel(charData.CurrentLevel);
                }
            }

            // 设置角色统计和传记的数据源
            SetCharDataSource();

            // 重新绑定角色和传记数据（第一个和第三个标签页）
            BindCharacterData();

            // 重置所选技能
            UpdateSkillView(0);
        }

        /// <summary>
        /// 设置角色统计和传记元素的数据源。
        /// </summary>
        void SetCharDataSource()
        {
            // 确保角色数据用作主要数据源
            m_LevelLabel.dataSource = m_CharacterData;

            // 设置角色统计的数据源
            VisualElement statsContainer = m_TopElement.Q<VisualElement>("char-data-stats-content");
            statsContainer.dataSource = m_BaseStats;

            // 设置传记和标题的数据源
            VisualElement bioContainer = m_TopElement.Q<VisualElement>("char-data-bio-content");
            bioContainer.dataSource = m_BaseStats;

            // 更新角色图标
            m_GameIconsData.UpdateIcons(m_BaseStats.CharacterClass, m_BaseStats.Rarity, m_BaseStats.AttackType);

            // 设置技能图标的数据源
            m_SkillIcons[0].dataSource = m_BaseStats;
            m_SkillIcons[1].dataSource = m_BaseStats;
            m_SkillIcons[2].dataSource = m_BaseStats;
        }

        /// <summary>
        /// 设置技能元素的数据源。
        /// </summary>
        /// <param name="newSkillDataSource"></param>
        void SetSkillDataSources(SkillSO newSkillDataSource)
        {
            VisualElement skillContainer = m_TopElement.Q<VisualElement>("char-data-skills-content");
            skillContainer.dataSource = newSkillDataSource;
        }

        /// <summary>
        /// 更新/重新绑定需要预处理的特殊LocalizedStrings。 
        /// 这些LocalizedStrings来自枚举并在本地化之前转换为字符串。
        /// </summary>
        void BindCharacterData()
        {
            if (m_BaseStats == null)
            {
                Debug.LogWarning("[CharStatsView] BindCharacterData: m_BaseStats为空");
                return;
            }

            // 将LocalizedStrings绑定到需要从枚举值进行预处理的UI元素
            m_CharacterClass.SetBinding("text", m_BaseStats.CharacterClassLocalized);
            m_Rarity.SetBinding("text", m_BaseStats.RarityLocalized);
            m_AttackType.SetBinding("text", m_BaseStats.AttackTypeLocalized);
            m_BioTitle.SetBinding("text", m_BaseStats.BioTitleLocalized);
        }

        /// <summary>
        /// 从交互式UI元素注销事件回调以防止内存泄漏。
        /// </summary>
        void UnregisterCallbacks()
        {
            m_NextSkillButton.UnregisterCallback<ClickEvent>(SelectNextSkill);
            m_LastSkillButton.UnregisterCallback<ClickEvent>(SelectLastSkill);
            m_SkillIcons[0].UnregisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[1].UnregisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[2].UnregisterCallback<ClickEvent>(SelectSkill);

            // 布局构建后初始化活动框架位置
            m_SkillIcons[0].UnregisterCallback<GeometryChangedEvent>(InitializeSkillMarker);
        }

        /// <summary>
        /// 处理从按钮选择上一个技能的点击事件。
        /// </summary>
        /// <param name="evt"></param>
        void SelectLastSkill(ClickEvent evt)
        {
            if (Time.time < m_TimeToNextClick)
                return;

            m_TimeToNextClick = Time.time + k_ClickCooldown;

            // 仅在直接点击视觉元素时选择
            m_ActiveIndex--;
            if (m_ActiveIndex < 0)
            {
                m_ActiveIndex = 2;
            }

            // 更新技能视图
            UpdateSkillView(m_ActiveIndex);
            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();
        }

        /// <summary>
        /// 处理从按钮选择下一个技能的点击事件。
        /// </summary>
        /// <param name="evt"></param>
        void SelectNextSkill(ClickEvent evt)
        {
            if (Time.time < m_TimeToNextClick)
                return;

            m_TimeToNextClick = Time.time + k_ClickCooldown;
            m_ActiveIndex++;

            if (m_ActiveIndex > 2)
            {
                m_ActiveIndex = 0;
            }

            // 更新技能视图
            UpdateSkillView(m_ActiveIndex);
            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();
        }

        /// <summary>
        /// 根据点击元素的名称确定点击了哪个技能图标。
        /// </summary>
        /// <param name="evt">技能按钮的点击事件。</param>
        void SelectSkill(ClickEvent evt)
        {
            VisualElement clickedElement = evt.target as VisualElement;

            if (clickedElement == null)
                return;

            // 从点击元素的名称中获取最后一个字符（例如char-skills__icon1, char-skills__icon2等）
            // 并将其转换为整数
            int index = (int)Char.GetNumericValue(clickedElement.name[clickedElement.name.Length - 1]) - 1;
            index = Mathf.Clamp(index, 0, m_BaseSkills.Length - 1);

            // 显示相应的ScriptableObject数据
            UpdateSkillView(index);

            // 播放点击音效
            AudioManager.PlayAltButtonSound();
        }

        /// <summary>
        /// 更新UI以显示指定索引处技能的信息。
        /// </summary>
        /// <param name="index">要显示的技能的索引。</param>
        void UpdateSkillView(int index)
        {
            // 没有有效角色数据时中止
            if (m_CharacterData == null || m_BaseSkills == null || m_BaseSkills[index] == null)
                return;

            SkillSO newSkillDataSource = m_BaseSkills[index];

            // 在设置为数据源之前更新技能的等级
            newSkillDataSource.UpdateLevel(m_CharacterData.CurrentLevel);

            // 更新数据源以使用当前选择的技能
            SetSkillDataSources(newSkillDataSource);

            // 突出显示所选技能图标
            MarkTargetElement(m_SkillIcons[index], 300);

            // 跟踪新的活动索引
            m_ActiveIndex = index;
        }

        /// <summary>
        /// UI布局构建后初始化活动技能标记的位置。
        /// </summary>
        /// <param name="evt">几何变化事件数据。</param>
        // 布局构建后设置活动框架
        void InitializeSkillMarker(GeometryChangedEvent evt)
        {
            // 将其位置设置在第一个图标上
            MarkTargetElement(m_SkillIcons[0], 0);
        }

        /// <summary>
        /// 动画化活动框架以突出显示指定的目标元素。
        /// </summary>
        /// <param name="targetElement">要突出显示的UI元素。</param>
        /// <param name="duration">动画持续时间（毫秒）。</param>
        void MarkTargetElement(VisualElement targetElement, int duration = 200)
        {
            // 目标元素，转换为活动框架的根空间
            Vector3 targetInRootSpace = GetRelativePosition(targetElement, m_ActiveFrame);

            // 填充偏移
            Vector3 offset = new Vector3(10, 10, 0f);

            // 动画化活动框架位置
            m_ActiveFrame.experimental.animation.Position(targetInRootSpace - offset, duration);
        }

        /// <summary>
        /// 将元素的位置转换为另一个元素的父元素的坐标空间。
        /// </summary>
        /// <param name="elementToConvert">要转换的元素。</param>
        /// <param name="targetElement">目标元素。</param>
        /// <returns>目标元素在新根空间中的位置。</returns>
        static Vector3 GetRelativePosition(VisualElement elementToConvert, VisualElement targetElement)
        {
            Vector2 worldSpacePosition = elementToConvert.parent.LocalToWorld(elementToConvert.layout.position);
            VisualElement newRoot = targetElement.parent;
            return newRoot.WorldToLocal(worldSpacePosition);
        }
    }
}