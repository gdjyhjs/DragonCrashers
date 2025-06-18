using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 
    /// </summary>
    public class ShopView : UIView
    {
        // 可点击标签按钮的类选择器
        const string k_TabClass = "shoptab";

        // 当前选定标签按钮的类选择器
        const string k_SelectedTabClass = "selected-shoptab";
        const string k_ShopTemplateContainerClass = "shop-item__template-container";

        // 精灵/图标的资源位置
        const string k_ResourcePath = "GameData/GameIcons";

        [Header("商店物品")]
        [Tooltip("要实例化的 ShopItem 元素资产")]
        [SerializeField] VisualTreeAsset m_ShopItemAsset;
        [SerializeField] GameIconsSO m_GameIconsData;

        // 视觉元素
        VisualElement m_ShopScrollView;

        VisualElement m_GoldTabButton;
        VisualElement m_GemTabButton;
        VisualElement m_PotionTabButton;

        public ShopView(VisualElement topElement) : base(topElement)
        {
            ShopEvents.ShopUpdated += OnShopUpdated;
            ShopEvents.TabSelected += OnTabSelected;

            // 这个 ScriptableObject 将数据类型（商店物品、技能、稀有度、类别等）与特定图标配对 
            // （默认路径 = Resources/GameData/GameIcons）
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
            m_ShopItemAsset = Resources.Load<VisualTreeAsset>("ShopItem") as VisualTreeAsset;
        }

        public override void Dispose()
        {
            base.Dispose();
            ShopEvents.ShopUpdated -= OnShopUpdated;
            ShopEvents.TabSelected -= OnTabSelected;

            UnregisterButtonCallbacks();
        }

        private void OnTabSelected(string tabName)
        {
            SelectTab(tabName);
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_ShopScrollView = m_TopElement.Q<VisualElement>("shop__content-scrollview");

            m_GoldTabButton = m_TopElement.Q<VisualElement>("shop-gold-shoptab");
            m_GemTabButton = m_TopElement.Q<VisualElement>("shop-gem-shoptab");
            m_PotionTabButton = m_TopElement.Q<VisualElement>("shop-potion-shoptab");
        }

        // 可选：在大多数情况下，取消注册按钮回调并不是严格必要的，这取决于应用程序的生命周期管理。
        // 如果需要特定场景，可以选择取消注册它们。
        protected void UnregisterButtonCallbacks()
        {
            m_GoldTabButton.UnregisterCallback<ClickEvent>(SelectGoldTab);
            m_GemTabButton.UnregisterCallback<ClickEvent>(SelectGemsTab);
            m_PotionTabButton.UnregisterCallback<ClickEvent>(SelectPotionTab);
        }

        protected override void RegisterButtonCallbacks()
        {
            m_GoldTabButton.RegisterCallback<ClickEvent>(SelectGoldTab);
            m_GemTabButton.RegisterCallback<ClickEvent>(SelectGemsTab);
            m_PotionTabButton.RegisterCallback<ClickEvent>(SelectPotionTab);
        }

        void SelectPotionTab(ClickEvent evt)
        {
            ClickTabButton(evt);
            ShopEvents.PotionSelected?.Invoke();
        }

        void SelectGemsTab(ClickEvent evt)
        {
            ClickTabButton(evt);
            ShopEvents.GemSelected?.Invoke();
        }

        void SelectGoldTab(ClickEvent evt)
        {
            ClickTabButton(evt);
            ShopEvents.GoldSelected?.Invoke();
        }

        // 使用 ClickEvent 处理标签按钮的点击操作
        void ClickTabButton(ClickEvent evt)
        {
            VisualElement clickedTab = evt.currentTarget as VisualElement;

            ClickTabButton(clickedTab);
        }

        // 使用 VisualElement 处理标签按钮的点击操作
        void ClickTabButton(VisualElement clickedTab)
        {
            // 如果点击的标签尚未选中，则选中它
            if (!IsTabSelected(clickedTab))
            {
                // 取消选择当前活动的其他标签
                UnselectOtherTabs(clickedTab);

                // 选择点击的标签
                SelectTab(clickedTab);

                // 播放默认声音
                AudioManager.PlayDefaultButtonSound();
            }
        }

        // 定位所有具有标签类名的 VisualElement
        UQueryBuilder<VisualElement> GetAllTabs()
        {
            return m_TopElement.Query<VisualElement>(className: k_TabClass);
        }

        // 取消选择特定标签
        void UnselectTab(VisualElement tab)
        {
            tab.RemoveFromClassList(k_SelectedTabClass);
        }

        void SelectTab(VisualElement tab)
        {
            tab.AddToClassList(k_SelectedTabClass);
        }

        // 通过名称选择特定标签，例如 "gold" 或 "gem" 
        public void SelectTab(string tabName)
        {
            switch (tabName)
            {
                case "gold":
                    ClickTabButton(m_GoldTabButton);
                    ShopEvents.GoldSelected?.Invoke();
                    break;
                case "gem":
                    ClickTabButton(m_GemTabButton);
                    ShopEvents.GemSelected?.Invoke();
                    break;
                case "potion":
                    ClickTabButton(m_PotionTabButton);
                    ShopEvents.PotionSelected?.Invoke();
                    break;
                default:
                    ShopEvents.GoldSelected?.Invoke();
                    break;
            }
        }

        public bool IsTabSelected(VisualElement tab)
        {
            return tab.ClassListContains(k_SelectedTabClass);
        }

        void UnselectOtherTabs(VisualElement tab)
        {

            GetAllTabs().Where(
                (t) => t != tab && IsTabSelected(t)).
                ForEach(UnselectTab);
        }

        // 用内容填充标签
        public void OnShopUpdated(List<ShopItemSO> shopItems)
        {
            if (shopItems == null || shopItems.Count == 0)
                return;

            // 为每个标签（金币、宝石、药水）生成物品
            VisualElement parentTab = m_ShopScrollView;

            ScrollView scrollView = parentTab.Q<ScrollView>(className: "unity-scroll-view");
            scrollView.scrollOffset = Vector2.zero;

            parentTab.Clear();

            foreach (ShopItemSO shopItem in shopItems)
            {
                CreateShopItemElement(shopItem, parentTab);
            }
        }

        void CreateShopItemElement(ShopItemSO shopItemData, VisualElement parentElement)
        {
            if (parentElement == null || shopItemData == null || m_ShopItemAsset == null)
                return;

            // 从模板 UXML 实例化一个新的 Visual Element
            TemplateContainer shopItemElem = m_ShopItemAsset.Instantiate();
            shopItemElem.AddToClassList(k_ShopTemplateContainerClass);

            // 为每个商店物品设置 VisualElement 和游戏数据
            ShopItemComponent shopItemController = new ShopItemComponent(m_GameIconsData, shopItemData);

            shopItemController.SetVisualElements(shopItemElem);
            shopItemController.SetGameData(shopItemElem);

            shopItemController.RegisterCallbacks();

            parentElement.Add(shopItemElem);

        }
    }
}