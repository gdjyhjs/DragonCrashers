using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 表示商店中的物品类型：软货币（金币）、硬货币（宝石）、血瓶或升级药水。
    /// </summary>
    [System.Serializable]
    public enum ShopItemType
    {
        Gold,  // 软货币（游戏内）
        Gems,  // 硬货币（用真钱购买）
        HealthPotion,  // 游戏中使用的物品示例（此演示中无功能）
        LevelUpPotion  // 提升角色等级（力量药水）
    }

    /// <summary>
    /// 用于购买物品的货币类型（金币、宝石、美元）。
    /// </summary>
    [System.Serializable]
    public enum CurrencyType
    {
        Gold,  // 金币
        Gems,  // 宝石
        USD  // 使用真钱购买宝石（此演示中免费）
    }

    /// <summary>
    /// 表示游戏商店中可供购买的物品，包括所有属性，如名称、图标、成本、折扣和内容类型。
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/ShopItems/ShopItemGameData",
        menuName = "UIToolkitDemo/ShopItem", order = 4)]
    public class ShopItemSO : ScriptableObject
    {

        [Tooltip("要购买的物品名称")]
        [SerializeField] string m_ItemName;  // 物品名称

        [SerializeField] LocalizedString m_ItemNameLocalized;  // 本地化的物品名称

        [Tooltip("用于表示产品的精灵图")]
        [SerializeField] Sprite m_Icon;  // 图标

        [Tooltip("如果等于 0 则为免费；CostInCurrencyType 中指定的货币成本金额")]
        [SerializeField] float m_Cost;  // 成本

        [Tooltip("如果值大于 0，UI 显示折扣标签（百分比折扣）")]
        [SerializeField] uint m_Discount;  // 折扣

        [Tooltip("如果不为空，UI 显示带有此文本的横幅")]
        [SerializeField] string m_PromoBannerText;  // 促销横幅文本
        [Tooltip("如果不为空，UI 显示带有此文本的横幅")]
        [SerializeField] LocalizedString m_PromoBannerLocalized;  // 本地化的促销横幅

        [SerializeField] LocalizedString m_FreeTextLocalized = new LocalizedString("SettingsTable", "Shop_Free");  // 本地化的免费文本
        [CreateProperty] public LocalizedString FreeTextLocalized => m_FreeTextLocalized;  // 本地化的免费文本

        [Tooltip("购买此物品后玩家获得的药水/硬币数量")]
        [SerializeField] uint m_ContentValue;  // 内容值

        [Tooltip("要购买的商店物品类型（硬币/宝石/药水）")]
        [SerializeField] ShopItemType m_ContentType;  // 内容类型

        // 属性
        [CreateProperty] public LocalizedString ItemNameLocalized => m_ItemNameLocalized;  // 本地化的物品名称
        [CreateProperty] public LocalizedString PromoBannerLocalized => m_PromoBannerLocalized;  // 本地化的促销横幅

        [CreateProperty] public string ItemName => m_ItemName;  // 物品名称
        [CreateProperty] public Sprite Icon => m_Icon;  // 图标
        [CreateProperty] public float Cost => m_Cost;  // 成本
        [CreateProperty] public uint Discount => m_Discount;  // 折扣
        [CreateProperty] public string PromoBannerText => m_PromoBannerText;  // 促销横幅文本
        [CreateProperty] public uint ContentAmount => m_ContentValue;  // 内容数量
        [CreateProperty] public ShopItemType ContentType => m_ContentType;  // 内容类型

        // 精灵图/图标的资源位置
        const string k_ResourcePath = "GameData/GameIcons";

        // 配对图标与货币/商店物品类型的可脚本化对象
        GameIconsSO m_GameIconsData;

        void OnEnable()
        {
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
        }

        /// <summary>
        /// 确定购买此物品所需的货币类型。软货币（金币）需要硬货币（宝石）。硬货币需要真钱美元。血瓶需要软货币（金币）。升级药水需要硬货币（宝石）。
        /// </summary>
        [CreateProperty]
        public CurrencyType CostInCurrencyType
        {
            get
            {
                switch (m_ContentType)
                {
                    case (ShopItemType.Gold):
                        return CurrencyType.Gems;
                    case (ShopItemType.Gems):
                        return CurrencyType.USD;
                    case (ShopItemType.HealthPotion):
                        return CurrencyType.Gold;
                    case (ShopItemType.LevelUpPotion):
                        return CurrencyType.Gems;
                    default:
                        return CurrencyType.Gems;
                }
            }
        }

        [CreateProperty]
        public DisplayStyle BannerDisplayStyle
        {
            get => string.IsNullOrEmpty(m_PromoBannerText) ? DisplayStyle.None : DisplayStyle.Flex;
        }

        [CreateProperty]
        public Sprite ContentCurrencyIcon
        {
            get => m_GameIconsData.GetShopTypeIcon(m_ContentType); // 根据 ContentType 检索相应的图标
        }

        [CreateProperty]
        public string FormattedContentValue
        {
            get => $" {m_ContentValue}"; // 返回格式化的内容值，前面有一个空格
        }

        [CreateProperty]
        public bool IsFree
        {
            get => Cost <= 0.00001f;
        }

        [CreateProperty]
        public bool IsCostInUSD
        {
            get => CostInCurrencyType == CurrencyType.USD;
        }

        /// <summary>
        /// 格式化的成本文本。
        /// </summary>
        [CreateProperty]
        public string FormattedCost
        {
            get
            {
                if (Cost <= 0.00001f)
                    return string.Empty;  // 对于免费物品，我们将使用本地化版本

                string currencyPrefix = (CostInCurrencyType == CurrencyType.USD) ? "$" : string.Empty;
                string decimalPlaces = (CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0";
                return currencyPrefix + Cost.ToString(decimalPlaces);
            }
        }

        [CreateProperty]
        public bool UseFreeText => Cost <= 0.00001f;

        [CreateProperty]
        public DisplayStyle RegularPriceDisplay => UseFreeText ? DisplayStyle.None : DisplayStyle.Flex;

        [CreateProperty]
        public DisplayStyle FreeTextDisplay => UseFreeText ? DisplayStyle.Flex : DisplayStyle.None;


        [CreateProperty]
        public Sprite CostIconSprite
        {
            get => m_GameIconsData.GetCurrencyIcon(CostInCurrencyType);
        }

        [CreateProperty]
        public DisplayStyle CostIconGroupDisplayStyle
        {
            get => IsFree || IsCostInUSD ? DisplayStyle.None : DisplayStyle.Flex;
        }

        [CreateProperty]
        public string DiscountText
        {
            get => Discount > 0 ? $"{Discount}%" : string.Empty;
        }

        [CreateProperty]
        public string DiscountedCost
        {
            get
            {
                if (Discount > 0)
                {
                    string currencyPrefix = (CostInCurrencyType == CurrencyType.USD) ? "$" : string.Empty;
                    string decimalPlaces = (CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0"; // 如果不是美元则无小数
                    return currencyPrefix + (((100 - Discount) / 100f) * Cost).ToString(decimalPlaces);
                }
                return string.Empty;
            }
        }

        [CreateProperty]
        public DisplayStyle DiscountIconGroupDisplayStyle
        {
            get => CostInCurrencyType == CurrencyType.USD ? DisplayStyle.None : DisplayStyle.Flex;
        }

        [CreateProperty]
        public bool IsDiscounted
        {
            get => Discount > 0;
        }

        [CreateProperty]
        public DisplayStyle DiscountDisplayStyle
        {
            get => IsDiscounted ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}