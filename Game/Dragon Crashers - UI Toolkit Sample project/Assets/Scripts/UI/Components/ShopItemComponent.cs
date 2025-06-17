using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理商店物品在UI中的视觉表示和交互逻辑。
    /// 将商店物品数据（如成本、折扣和内容）绑定到UI元素。
    /// </summary>
    public class ShopItemComponent
    {
        // 物品大小（正常或宽）的类选择器
        const string k_SizeNormalClass = "shop-item__size--normal";
        const string k_SizeWideClass = "shop-item__size--wide";

        // 将图标与货币/商店物品类型配对的ScriptableObject
        GameIconsSO m_GameIconsData;
        ShopItemSO m_ShopItemData;

        // 视觉元素
        Label m_Description;
        VisualElement m_ProductImage;
        VisualElement m_Banner;
        Label m_BannerLabel;
        VisualElement m_ContentCurrency;
        Label m_ContentValue;
        VisualElement m_CostIcon;
        Label m_Cost;
        VisualElement m_DiscountBadge;
        Label m_DiscountLabel;
        VisualElement m_DiscountSlash;
        VisualElement m_DiscountGroup;
        VisualElement m_SizeContainer;
        Label m_DiscountCost;
        Button m_BuyButton;
        VisualElement m_CostIconGroup;
        VisualElement m_DiscountIconGroup;

        Label m_CostFree;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="gameIconsData">包含通用图标数据的ScriptableObject。</param>
        /// <param name="shopItemData">包含产品信息的ScriptableObject。</param>
        public ShopItemComponent(GameIconsSO gameIconsData, ShopItemSO shopItemData)
        {
            m_GameIconsData = gameIconsData;
            m_ShopItemData = shopItemData;
        }

        /// <summary>
        /// 从提供的TemplateContainer中查询并分配对UI元素的引用。
        /// </summary>
        public void SetVisualElements(TemplateContainer shopItemElement)
        {
            // 查询ShopItemElement的各个部分
            m_SizeContainer = shopItemElement.Q("shop-item__container");
            m_Description = shopItemElement.Q<Label>("shop-item__description");
            m_ProductImage = shopItemElement.Q("shop-item__product-image");
            m_Banner = shopItemElement.Q("shop-item__banner");
            m_BannerLabel = shopItemElement.Q<Label>("shop-item__banner-label");
            m_DiscountBadge = shopItemElement.Q("shop-item__discount-badge");
            m_DiscountLabel = shopItemElement.Q<Label>("shop-item__badge-text");
            m_DiscountSlash = shopItemElement.Q("shop-item__discount-slash");
            m_ContentCurrency = shopItemElement.Q("shop-item__content-currency");
            m_ContentValue = shopItemElement.Q<Label>("shop-item__content-value");
            m_CostIcon = shopItemElement.Q("shop-item__cost-icon");
            m_Cost = shopItemElement.Q<Label>("shop-item__cost-price");
            m_CostFree = shopItemElement.Q<Label>("shop-item__cost-free");
            // m_DiscountIcon = shopItemElement.Q("shop-item__discount-icon");
            m_DiscountGroup = shopItemElement.Q("shop-item__discount-group");
            m_DiscountCost = shopItemElement.Q<Label>("shop-item__discount-price");
            m_BuyButton = shopItemElement.Q<Button>("shop-item__buy-button");

            m_CostIconGroup = shopItemElement.Q("shop-item__cost-icon-group");
            m_DiscountIconGroup = shopItemElement.Q("shop-item__discount-icon-group");
        }

        /// <summary>
        /// 设置数据源，更新大小类，并将数据绑定到商店物品的UI元素。
        /// </summary>
        /// <param name="shopItemElement"></param>
        public void SetGameData(TemplateContainer shopItemElement)
        {
            if (m_GameIconsData == null)
            {
                Debug.LogWarning("[ShopItemComponent] SetGameData: 缺少GameIcons ScriptableObject数据。");
                return;
            }

            if (shopItemElement == null)
            {
                Debug.LogWarning("[ShopItemComponent] SetGameData: 缺少Template对象。");
                return;
            }

            // 在顶级/根元素中设置数据源
            m_SizeContainer.dataSource = m_ShopItemData;

            UpdateSizeClass();

            BindProductInfo();

            BindBannerElements();

            BindCostElements();

            BindDiscountElements();
        }

        /// <summary>
        /// 此方法添加适当的USS类。打折物品使用更宽的样式。
        /// </summary>
        void UpdateSizeClass()
        {
            // 首先移除两个类以重置状态
            m_SizeContainer.RemoveFromClassList(k_SizeNormalClass);
            m_SizeContainer.RemoveFromClassList(k_SizeWideClass);

            // 根据物品是否打折添加适当的类
            if (m_ShopItemData.IsDiscounted)
            {
                m_SizeContainer.AddToClassList(k_SizeWideClass);
            }
            else
            {
                m_SizeContainer.AddToClassList(k_SizeNormalClass);
            }
        }

        /// <summary>
        /// 将ShopItemSO中的数据绑定到描述和产品图像。
        /// </summary>
        void BindProductInfo()
        {
            // 
            m_Description.SetBinding("text", m_ShopItemData.ItemNameLocalized);

            m_ProductImage.SetBinding("style.backgroundImage", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.Icon)),
                bindingMode = BindingMode.ToTarget // 单向绑定到UI
            });

            // 将m_ContentCurrency（收到的奖励类型）的背景图像绑定到ContentCurrencyIcon属性
            m_ContentCurrency.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.ContentCurrencyIcon)),
                bindingMode = BindingMode.ToTarget // 从数据到UI的单向绑定
            });

            // 将m_ContentValue（收到的奖励）的文本绑定到FormattedContentValue属性
            m_ContentValue.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FormattedContentValue)),
                bindingMode = BindingMode.ToTarget // 从数据到UI的单向绑定
            });

        }

        /// <summary>
        /// 显示或隐藏横幅，横幅显示有关商店物品状态的附加文本。
        /// </summary>
        private void BindBannerElements()
        {
            // 使用命名的DataBinding将横幅的可见性绑定到数据源
            m_Banner.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.BannerDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            m_BannerLabel.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.BannerDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 将促销横幅文本绑定到LocalizedString
            m_BannerLabel.SetBinding("text", m_ShopItemData.PromoBannerLocalized);
        }

        /// <summary>
        /// 绑定与成本相关的元素。
        /// </summary>
        void BindCostElements()
        {
            // 将成本文本绑定到FormattedCost
            m_Cost.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FormattedCost)),
                bindingMode = BindingMode.ToTarget
            });

            // 将显示样式绑定到显示/隐藏常规价格
            m_Cost.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.RegularPriceDisplay)),
                bindingMode = BindingMode.ToTarget
            });

            // 将“免费”文本标签绑定到使用本地化
            m_CostFree.SetBinding("text", m_ShopItemData.FreeTextLocalized);

            // 将显示样式绑定到显示/隐藏“免费”文本
            m_CostFree.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FreeTextDisplay)),
                bindingMode = BindingMode.ToTarget
            });

            // 将成本图标绑定到CostIconSprite属性
            m_CostIcon.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.CostIconSprite)),
                bindingMode = BindingMode.ToTarget
            });

            // 根据CostIconGroupDisplayStyle属性显示/隐藏CostIconGroup
            m_CostIconGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.CostIconGroupDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }

        /// <summary>
        /// 根据商店物品的DiscountDisplayStyle显示/隐藏与折扣相关的元素。
        /// </summary>
        void BindDiscountElements()
        {
            // 将折扣标签文本绑定到DiscountText
            m_DiscountLabel.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountText)),
                bindingMode = BindingMode.ToTarget
            });

            // 根据DiscountDisplayStyle属性显示/隐藏折扣标签
            m_DiscountLabel.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 将折扣后的成本文本绑定到DiscountedCost
            m_DiscountCost.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountedCost)),
                bindingMode = BindingMode.ToTarget
            });

            // 根据DiscountIconGroupDisplayStyle属性显示/隐藏DiscountIconGroup
            m_DiscountIconGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountIconGroupDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 显示/隐藏折扣徽章
            m_DiscountBadge.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 显示/隐藏折扣斜线
            m_DiscountSlash.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 显示/隐藏折扣组
            m_DiscountGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }

        /// <summary>
        /// 设置交互式按钮。
        /// </summary>
        public void RegisterCallbacks()
        {
            if (m_BuyButton == null)
                return;

            // 将成本/内容数据存储在每个按钮中以供以后使用
            m_BuyButton.userData = m_ShopItemData;
            m_BuyButton.RegisterCallback<ClickEvent>(BuyAction);

            // 防止按钮点击移动ScrollView
            m_BuyButton.RegisterCallback<PointerMoveEvent>(MovePointerEventHandler);
        }

        /// <summary>
        /// 防止鼠标意外的左右移动拖动父Scrollview
        /// </summary>
        /// <param name="evt">指针移动事件。</param>
        void MovePointerEventHandler(PointerMoveEvent evt)
        {
            evt.StopImmediatePropagation();
        }

        /// <summary>
        /// 点击商店物品上的购买按钮会触发一系列事件：
        ///      - ShopItemComponent（点击按钮） -->
        ///      - ShopController（购买物品） -->
        ///      - GameDataManager（验证资金） -->
        ///      - MagnetFXController（在UI上播放效果）
        /// </summary>
        /// <param name="evt"></param>
        void BuyAction(ClickEvent evt)
        {
            VisualElement clickedElement = evt.currentTarget as VisualElement;

            // 检索先前存储在自定义userData中的商店物品数据
            ShopItemSO shopItemData = clickedElement.userData as ShopItemSO;

            // 获取根VisualElement 
            VisualElement rootVisualElement = m_SizeContainer.panel.visualTree;

            // 转换为像素为单位的屏幕位置
            Vector2 clickPos = new Vector2(evt.position.x, evt.position.y);
            Vector2 screenPos = clickPos.GetScreenCoordinate(rootVisualElement);

            // 通知ShopController（传递商店物品数据 + 屏幕位置）
            ShopEvents.ShopItemClicked?.Invoke(shopItemData, screenPos);

            AudioManager.PlayDefaultButtonSound();
        }
    }
}