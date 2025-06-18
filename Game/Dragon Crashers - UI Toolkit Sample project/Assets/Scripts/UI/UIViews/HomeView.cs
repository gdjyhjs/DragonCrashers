using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理主屏幕显示，显示关卡信息、关卡选择和游玩选项。监听语言环境变化并相应更新显示的关卡信息。
    /// </summary>
    public class HomeView : UIView
    {
        // 游玩关卡按钮
        VisualElement m_PlayLevelButton;
        // 关卡缩略图
        VisualElement m_LevelThumbnail;

        // 关卡编号
        Label m_LevelNumber;
        // 关卡标签
        Label m_LevelLabel;

        // 当前关卡数据
        LevelSO m_CurrentLevelData;

        // 聊天视图
        ChatView m_ChatView;
        public ChatView ChatView => m_ChatView;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="topElement">VisualTree的最顶层/根元素。</param>
        public HomeView(VisualElement topElement) : base(topElement)
        {
            m_ChatView = new ChatView(topElement);

            // 订阅关卡信息显示事件
            HomeEvents.LevelInfoShown += OnShowLevelInfo;

            // 监听语言环境变化
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        /// <summary>
        /// 设置对UI元素的引用。
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            // 获取游玩关卡按钮
            m_PlayLevelButton = m_TopElement.Q("home-play__level-button");
            // 获取关卡名称标签
            m_LevelLabel = m_TopElement.Q<Label>("home-play__level-name");
            // 获取关卡编号标签
            m_LevelNumber = m_TopElement.Q<Label>("home-play__level-number");

            // 获取关卡背景缩略图
            m_LevelThumbnail = m_TopElement.Q("home-play__background");
        }

        /// <summary>
        /// 注册游玩按钮点击事件以加载游戏场景。
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // 注册点击游玩按钮事件
            m_PlayLevelButton.RegisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// 取消订阅和注销以防止内存泄漏。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            // 取消订阅关卡信息显示事件
            HomeEvents.LevelInfoShown -= OnShowLevelInfo;
            // 取消监听语言环境变化
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

            // 取消注册点击游玩按钮事件
            m_PlayLevelButton.UnregisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// 播放音效并通知任何关联的游玩逻辑。
        /// </summary>
        /// <param name="evt"></param>
        void ClickPlayButton(ClickEvent evt)
        {
            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();
            // 触发播放按钮点击事件
            HomeEvents.PlayButtonClicked?.Invoke();
        }

        // 事件处理方法

        /// <summary>
        /// 在主视图上显示指定的关卡信息。
        /// </summary>
        /// <param name="levelData">ScriptableObject关卡数据。</param>
        void OnShowLevelInfo(LevelSO levelData)
        {
            if (levelData == null)
                return;

            // 缓存一份用于语言环境更新
            m_CurrentLevelData = levelData;

            // 显示关卡信息
            ShowLevelInfo(levelData.LevelNumberFormatted, levelData.LevelSubtitle, levelData.Thumbnail);
        }

        /// <summary>
        /// 随着语言环境变化重新获取并更新本地化字符串。
        /// </summary>
        /// <param name="locale">新的语言环境。</param>
        void OnLocaleChanged(Locale locale)
        {
            // 显示关卡信息
            ShowLevelInfo(m_CurrentLevelData.LevelNumberFormatted, m_CurrentLevelData.LevelSubtitle,
                m_CurrentLevelData.Thumbnail);
        }

        /// <summary>
        /// 显示关卡信息
        /// </summary>
        /// <param name="levelNumberFormatted"></param>
        /// <param name="levelName"></param>
        /// <param name="thumbnail"></param>
        public void ShowLevelInfo(string levelNumberFormatted, string levelName, Sprite thumbnail)
        {
            if (m_LevelNumber == null || m_LevelLabel == null || m_LevelThumbnail == null)
                return;

            // 设置关卡编号文本
            m_LevelNumber.text = levelNumberFormatted;
            // 设置关卡名称文本
            m_LevelLabel.text = levelName;
            // 设置关卡缩略图背景
            m_LevelThumbnail.style.backgroundImage = new StyleBackground(thumbnail);
        }
    }
}