using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理任务栏 UI，用于打开设置视图和商店视图。使用简单的
    /// 文本动画更新宝石和金币总数。
    /// </summary>
    public class OptionsBarView : UIView
    {
        VisualElement m_OptionsButton;
        VisualElement m_ShopGemButton;
        VisualElement m_ShopGoldButton;
        Label m_GoldLabel;
        Label m_GemLabel;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="topElement"></param>
        public OptionsBarView(VisualElement topElement) : base(topElement)
        {
            // 订阅 GameDataReceived 事件，该事件在收到新游戏数据时触发。
            GameDataManager.GameDataReceived += OnGameDataReceived;

            // 向 GameDataManager 请求游戏数据。
            GameDataManager.GameDataRequested?.Invoke();
        }

        /// <summary>
        /// 处理游戏数据接收，并将金币和宝石值绑定到各自的标签。
        /// </summary>
        /// <param name="gameData">收到的游戏数据。</param>
        void OnGameDataReceived(GameData gameData)
        {
            // 这里进行数据绑定
            m_GoldLabel.SetBinding("text", new AnimatedTextBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(nameof(GameData.Gold)),
            });

            m_GemLabel.SetBinding("text", new AnimatedTextBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(nameof(GameData.Gems)),
            });
        }

        /// <summary>
        /// 取消订阅事件并取消注册按钮回调。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            GameDataManager.GameDataReceived -= OnGameDataReceived;
            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// 设置选项栏 UI 中的视觉元素引用。
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_OptionsButton = m_TopElement.Q("options-bar__button");
            m_ShopGoldButton = m_TopElement.Q("options-bar__gold-button");
            m_ShopGemButton = m_TopElement.Q("options-bar__gem-button");
            m_GoldLabel = m_TopElement.Q<Label>("options-bar__gold-count");
            m_GemLabel = m_TopElement.Q<Label>("options-bar__gem-count");
        }

        /// <summary>
        /// 设置按钮点击事件
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            m_OptionsButton.RegisterCallback<ClickEvent>(ShowOptionsScreen);
            m_ShopGemButton.RegisterCallback<ClickEvent>(OpenGemShop);
            m_ShopGoldButton.RegisterCallback<ClickEvent>(OpenGoldShop);
        }

        /// <summary>
        /// 取消注册选项和商店按钮的点击事件处理程序。
        /// </summary>
        void UnregisterButtonCallbacks()
        {
            m_OptionsButton.UnregisterCallback<ClickEvent>(ShowOptionsScreen);
            m_ShopGemButton.UnregisterCallback<ClickEvent>(OpenGemShop);
            m_ShopGoldButton.UnregisterCallback<ClickEvent>(OpenGoldShop);
        }

        /// <summary>
        /// 点击选项按钮时打开设置视图。
        /// </summary>
        /// <param name="evt">点击事件。</param>
        void ShowOptionsScreen(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            MainMenuUIEvents.SettingsScreenShown?.Invoke();
        }

        /// <summary>
        /// 点击金币按钮时打开金币商店标签。
        /// </summary>
        /// <param name="evt">点击事件。</param>
        void OpenGoldShop(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // 显示商店屏幕
            MainMenuUIEvents.OptionsBarShopScreenShown?.Invoke();

            // 打开到金币产品的标签
            ShopEvents.TabSelected?.Invoke("gold");
        }

        /// <summary>
        /// 点击宝石按钮时打开宝石商店标签。
        /// </summary>
        /// <param name="evt">点击事件。</param>
        void OpenGemShop(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // 显示商店屏幕
            MainMenuUIEvents.OptionsBarShopScreenShown?.Invoke();

            // 打开到宝石产品的标签
            ShopEvents.TabSelected?.Invoke("gem");
        }
    }
}