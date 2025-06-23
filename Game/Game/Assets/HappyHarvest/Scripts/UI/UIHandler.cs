using System;
using System.Collections;
using System.Collections.Generic;
using Template2DCommon;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
namespace HappyHarvest
{
    /// <summary>
    /// 处理与主游戏界面 UI 相关的所有内容。将检索所有 UI 元素并包含各种静态函数，
    /// 这些函数用于更新 / 更改 UI，以便可以从与 UI 交互的任何其他类中调用它们。
    /// </summary>
    public class UIHandler : MonoBehaviour
    {
        protected static UIHandler s_Instance;

        // 光标类型枚举（不翻译，保持英文以避免游戏报错）
        public enum CursorType
        {
            Normal,
            Interact,
            System
        }

        [Header("光标设置")]
        // 普通光标纹理
        public Texture2D NormalCursor;
        // 交互光标纹理
        public Texture2D InteractCursor;

        [Header("UI 文档")]
        // 市场条目模板
        public VisualTreeAsset MarketEntryTemplate;

        [Header("声音")]
        // 市场出售音效
        public AudioClip MarketSellSound;

        protected UIDocument m_Document;

        // 库存槽位视觉元素列表
        protected List<VisualElement> m_InventorySlots;
        // 物品数量标签列表
        protected List<Label> m_ItemCountLabels;

        // 金币计数器标签
        protected Label m_CointCounter;

        // 市场弹出窗口
        protected VisualElement m_MarketPopup;
        // 市场内容滚动视图
        protected VisualElement m_MarketContentScrollview;

        // 计时器标签
        protected Label m_TimerLabel;

        // 购买按钮
        protected Button m_BuyButton;
        // 出售按钮
        protected Button m_SellButton;

        // 应用焦点状态
        protected bool m_HaveFocus = true;
        // 当前光标类型
        protected CursorType m_CurrentCursorType;

        // 设置菜单
        protected SettingMenu m_SettingMenu;
        // 仓库 UI
        protected WarehouseUI m_WarehouseUI;

        // 黑屏遮罩
        protected VisualElement m_Blocker;
        // 渐变完成回调
        protected System.Action m_FadeFinishClbk;

        // 天气标签
        private Label m_SunLabel;
        private Label m_RainLabel;
        private Label m_ThunderLabel;

        void Awake()
        {
            s_Instance = this;

            m_Document = GetComponent<UIDocument>();

            // 获取库存槽位和数量标签
            m_InventorySlots = m_Document.rootVisualElement.Query<VisualElement>("InventoryEntry").ToList();
            m_ItemCountLabels = m_Document.rootVisualElement.Query<Label>("ItemCount").ToList();

            // 为每个库存槽位添加点击事件
            for (int i = 0; i < m_InventorySlots.Count; ++i)
            {
                var i1 = i;
                m_InventorySlots[i].AddManipulator(new Clickable(() =>
                {
                    GameManager.Instance.Player.ChangeEquipItem(i1);
                }));
            }

            Debug.Assert(m_InventorySlots.Count == InventorySystem.InventorySize,
            "UI 中的物品槽位不足以容纳库存");

            // 获取金币计数器标签
            m_CointCounter = m_Document.rootVisualElement.Q<Label>("CoinAmount");

            // 初始化市场弹出窗口
            m_MarketPopup = m_Document.rootVisualElement.Q<VisualElement>("MarketPopup");
            m_MarketPopup.Q<Button>("CloseButton").clicked += CloseMarket;
            m_MarketPopup.visible = false;

            // 初始化买卖按钮
            m_BuyButton = m_MarketPopup.Q<Button>("BuyButton");
            m_BuyButton.clicked += ToggleToBuy;
            m_SellButton = m_MarketPopup.Q<Button>("SellButton");
            m_SellButton.clicked += ToggleToSell;

            // 获取市场内容滚动视图
            m_MarketContentScrollview = m_MarketPopup.Q<ScrollView>("ContentScrollView");

            // 获取计时器标签
            m_TimerLabel = m_Document.rootVisualElement.Q<Label>("Timer");

            // 初始化设置菜单和仓库 UI
            m_SettingMenu = new SettingMenu(m_Document.rootVisualElement);
            m_SettingMenu.OnOpen += () => { GameManager.Instance.Pause(); };
            m_SettingMenu.OnClose += () => { GameManager.Instance.Resume(); };

            m_WarehouseUI = new WarehouseUI(m_Document.rootVisualElement.Q<VisualElement>("WarehousePopup"), MarketEntryTemplate);

            // 初始化黑屏遮罩
            m_Blocker = m_Document.rootVisualElement.Q<VisualElement>("Blocker");

            m_Blocker.style.opacity = 1.0f;
            m_Blocker.schedule.Execute(() => { FadeFromBlack(() => { }); }).ExecuteLater(500);

            m_Blocker.RegisterCallback<TransitionEndEvent>(evt =>
            {
                m_FadeFinishClbk?.Invoke();
            });

            // 初始化天气标签并添加点击事件
            m_SunLabel = m_Document.rootVisualElement.Q<Label>("SunLabel");
            m_RainLabel = m_Document.rootVisualElement.Q<Label>("RainLabel");
            m_ThunderLabel = m_Document.rootVisualElement.Q<Label>("ThunderLabel");

            m_SunLabel.AddManipulator(new Clickable(() => { GameManager.Instance.WeatherSystem?.ChangeWeather(WeatherSystem.WeatherType.Sun); }));
            m_RainLabel.AddManipulator(new Clickable(() => { GameManager.Instance.WeatherSystem?.ChangeWeather(WeatherSystem.WeatherType.Rain); }));
            m_ThunderLabel.AddManipulator(new Clickable(() => { GameManager.Instance.WeatherSystem?.ChangeWeather(WeatherSystem.WeatherType.Thunder); }));
        }

        void Update()
        {
            // 更新计时器标签显示当前时间
            m_TimerLabel.text = GameManager.Instance.CurrentTimeAsString();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            m_HaveFocus = hasFocus;
            if (!hasFocus)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            else
                ChangeCursor(m_CurrentCursorType);
        }

        // 玩家库存变化时调用，更新库存 UI
        public static void UpdateInventory(InventorySystem system)
        {
            s_Instance.UpdateInventory_Internal(system);
        }

        // 更新金币数量 UI
        public static void UpdateCoins(int amount)
        {
            s_Instance.UpdateCoins_Internal(amount);
        }

        // 打开市场 UI 并暂停游戏
        public static void OpenMarket()
        {
            s_Instance.OpenMarket_Internal();
            GameManager.Instance.Pause();
        }

        // 关闭市场 UI 并恢复游戏
        public static void CloseMarket()
        {
            SoundManager.Instance.PlayUISound();
            s_Instance.m_MarketPopup.visible = false;
            GameManager.Instance.Resume();
        }

        // 打开仓库 UI
        public static void OpenWarehouse()
        {
            s_Instance.m_WarehouseUI.Open();
        }

        // 更改光标类型
        public static void ChangeCursor(CursorType cursorType)
        {
            if (s_Instance.m_HaveFocus)
            {
                switch (cursorType)
                {
                    case CursorType.Interact:
                        Cursor.SetCursor(s_Instance.InteractCursor, Vector2.zero, CursorMode.Auto);
                        break;
                    case CursorType.Normal:
                        Cursor.SetCursor(s_Instance.NormalCursor, Vector2.zero, CursorMode.Auto);
                        break;
                    case CursorType.System:
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        break;
                }
            }

            s_Instance.m_CurrentCursorType = cursorType;
        }

        // 更新天气图标状态
        public static void UpdateWeatherIcons(WeatherSystem.WeatherType currentWeather)
        {
            s_Instance.m_SunLabel.EnableInClassList("on-button", currentWeather == WeatherSystem.WeatherType.Sun);
            s_Instance.m_RainLabel.EnableInClassList("on-button", currentWeather == WeatherSystem.WeatherType.Rain);
            s_Instance.m_ThunderLabel.EnableInClassList("on-button", currentWeather == WeatherSystem.WeatherType.Thunder);

            s_Instance.m_SunLabel.EnableInClassList("off-button", currentWeather != WeatherSystem.WeatherType.Sun);
            s_Instance.m_RainLabel.EnableInClassList("off-button", currentWeather != WeatherSystem.WeatherType.Rain);
            s_Instance.m_ThunderLabel.EnableInClassList("off-button", currentWeather != WeatherSystem.WeatherType.Thunder);
        }

        // 场景加载完成后调用，控制天气标签显示
        public static void SceneLoaded()
        {
            s_Instance.m_SunLabel.parent.style.display =
            GameManager.Instance.WeatherSystem == null ? DisplayStyle.None : DisplayStyle.Flex;
        }

        // 内部方法：打开市场 UI
        private void OpenMarket_Internal()
        {
            m_MarketPopup.visible = true;

            // 默认为出售标签页
            ToggleToSell();

            GameManager.Instance.Player.ToggleControl(false);
        }

        // 切换到出售模式
        private void ToggleToSell()
        {
            m_SellButton.AddToClassList("activeButton");
            m_BuyButton.RemoveFromClassList("activeButton");

            m_SellButton.SetEnabled(false);
            m_BuyButton.SetEnabled(true);

            // 清空现有条目（可优化为对象池）
            m_MarketContentScrollview.contentContainer.Clear();

            // 生成出售条目
            for (int i = 0; i < GameManager.Instance.Player.Inventory.Entries.Length; ++i)
            {
                var item = GameManager.Instance.Player.Inventory.Entries[i].Item;
                if (item == null)
                    continue;

                var clone = MarketEntryTemplate.CloneTree();

                clone.Q<Label>("ItemName").text = item.DisplayName;
                clone.Q<VisualElement>("ItemIcone").style.backgroundImage = new StyleBackground(item.ItemSprite);

                var button = clone.Q<Button>("ActionButton");

                if (item is Product product)
                {
                    int count = GameManager.Instance.Player.Inventory.Entries[i].StackSize;
                    button.text = $"出售 {count} 获得 {product.SellPrice * count}";

                    int i1 = i;
                    button.clicked += () =>
                    {
                        GameManager.Instance.Player.SellItem(i1, count);
                        m_MarketContentScrollview.contentContainer.Remove(clone.contentContainer);
                    };
                }
                else
                {
                    button.SetEnabled(false);
                    button.text = "无法出售";
                }

                m_MarketContentScrollview.Add(clone.contentContainer);
            }
        }

        // 切换到购买模式
        private void ToggleToBuy()
        {
            m_SellButton.RemoveFromClassList("activeButton");
            m_BuyButton.AddToClassList("activeButton");

            m_BuyButton.SetEnabled(false);
            m_SellButton.SetEnabled(true);

            // 清空现有条目（可优化为对象池）
            m_MarketContentScrollview.contentContainer.Clear();

            // 生成购买条目
            for (int i = 0; i < GameManager.Instance.MarketEntries.Length; ++i)
            {
                var item = GameManager.Instance.MarketEntries[i];

                var clone = MarketEntryTemplate.CloneTree();

                clone.Q<Label>("ItemName").text = item.DisplayName;
                clone.Q<VisualElement>("ItemIcone").style.backgroundImage = new StyleBackground(item.ItemSprite);

                var button = clone.Q<Button>("ActionButton");

                if (GameManager.Instance.Player.Coins >= item.BuyPrice)
                {
                    button.text = $"购买 1 花费 {item.BuyPrice}";
                    int i1 = i;
                    button.clicked += () =>
                    {
                        if (GameManager.Instance.Player.BuyItem(item))
                        {
                            if (GameManager.Instance.Player.Coins < item.BuyPrice)
                            {
                                button.text = $"余额不足，需要 {item.BuyPrice}";
                                button.SetEnabled(false);
                            }
                        }
                    };
                    button.SetEnabled(true);
                }
                else
                {
                    button.text = $"余额不足，需要 {item.BuyPrice}";
                    button.SetEnabled(false);
                }

                m_MarketContentScrollview.Add(clone.contentContainer);
            }
        }

        // 播放买卖音效
        public static void PlayBuySellSound(Vector3 location)
        {
            SoundManager.Instance.PlaySFXAt(location, s_Instance.MarketSellSound, false);
        }

        // 淡入黑屏效果
        public static void FadeToBlack(System.Action onFinished)
        {
            s_Instance.m_FadeFinishClbk = onFinished;

            s_Instance.m_Blocker.schedule.Execute(() =>
            {
                s_Instance.m_Blocker.style.opacity = 1.0f;
            }).ExecuteLater(10);
        }

        // 从黑屏淡出
        public static void FadeFromBlack(System.Action onFinished)
        {
            s_Instance.m_FadeFinishClbk = onFinished;

            s_Instance.m_Blocker.schedule.Execute(() =>
            {
                s_Instance.m_Blocker.style.opacity = 0.0f;
            }).ExecuteLater(10);
        }

        // 内部方法：更新金币计数器
        private void UpdateCoins_Internal(int amount)
        {
            m_CointCounter.text = amount.ToString();
        }

        // 内部方法：更新库存 UI
        private void UpdateInventory_Internal(InventorySystem system)
        {
            for (int i = 0; i < system.Entries.Length; ++i)
            {
                var item = system.Entries[i].Item;
                m_InventorySlots[i][0].style.backgroundImage =
                item == null ? new StyleBackground((Sprite)null) : new StyleBackground(item.ItemSprite);

                // 显示 / 隐藏物品数量标签
                if (item == null || system.Entries[i].StackSize < 2)
                {
                    m_ItemCountLabels[i].style.visibility = Visibility.Hidden;
                }
                else
                {
                    m_ItemCountLabels[i].style.visibility = Visibility.Visible;
                    m_ItemCountLabels[i].text = system.Entries[i].StackSize.ToString();
                }

                // 标记当前装备的物品槽位
                if (system.EquippedItemIdx == i)
                {
                    m_InventorySlots[i].AddToClassList("equipped");
                }
                else
                {
                    m_InventorySlots[i].RemoveFromClassList("equipped");
                }
            }
        }
    }
}