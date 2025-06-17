using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��ʾ�̵��е���Ʒ���ͣ�����ң���ң���Ӳ���ң���ʯ����Ѫƿ������ҩˮ��
    /// </summary>
    [System.Serializable]
    public enum ShopItemType
    {
        Gold,  // ����ң���Ϸ�ڣ�
        Gems,  // Ӳ���ң�����Ǯ����
        HealthPotion,  // ��Ϸ��ʹ�õ���Ʒʾ��������ʾ���޹��ܣ�
        LevelUpPotion  // ������ɫ�ȼ�������ҩˮ��
    }

    /// <summary>
    /// ���ڹ�����Ʒ�Ļ������ͣ���ҡ���ʯ����Ԫ����
    /// </summary>
    [System.Serializable]
    public enum CurrencyType
    {
        Gold,  // ���
        Gems,  // ��ʯ
        USD  // ʹ����Ǯ����ʯ������ʾ����ѣ�
    }

    /// <summary>
    /// ��ʾ��Ϸ�̵��пɹ��������Ʒ�������������ԣ������ơ�ͼ�ꡢ�ɱ����ۿۺ��������͡�
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/ShopItems/ShopItemGameData",
        menuName = "UIToolkitDemo/ShopItem", order = 4)]
    public class ShopItemSO : ScriptableObject
    {

        [Tooltip("Ҫ�������Ʒ����")]
        [SerializeField] string m_ItemName;  // ��Ʒ����

        [SerializeField] LocalizedString m_ItemNameLocalized;  // ���ػ�����Ʒ����

        [Tooltip("���ڱ�ʾ��Ʒ�ľ���ͼ")]
        [SerializeField] Sprite m_Icon;  // ͼ��

        [Tooltip("������� 0 ��Ϊ��ѣ�CostInCurrencyType ��ָ���Ļ��ҳɱ����")]
        [SerializeField] float m_Cost;  // �ɱ�

        [Tooltip("���ֵ���� 0��UI ��ʾ�ۿ۱�ǩ���ٷֱ��ۿۣ�")]
        [SerializeField] uint m_Discount;  // �ۿ�

        [Tooltip("�����Ϊ�գ�UI ��ʾ���д��ı��ĺ��")]
        [SerializeField] string m_PromoBannerText;  // ��������ı�
        [Tooltip("�����Ϊ�գ�UI ��ʾ���д��ı��ĺ��")]
        [SerializeField] LocalizedString m_PromoBannerLocalized;  // ���ػ��Ĵ������

        [SerializeField] LocalizedString m_FreeTextLocalized = new LocalizedString("SettingsTable", "Shop_Free");  // ���ػ�������ı�
        [CreateProperty] public LocalizedString FreeTextLocalized => m_FreeTextLocalized;  // ���ػ�������ı�

        [Tooltip("�������Ʒ����һ�õ�ҩˮ/Ӳ������")]
        [SerializeField] uint m_ContentValue;  // ����ֵ

        [Tooltip("Ҫ������̵���Ʒ���ͣ�Ӳ��/��ʯ/ҩˮ��")]
        [SerializeField] ShopItemType m_ContentType;  // ��������

        // ����
        [CreateProperty] public LocalizedString ItemNameLocalized => m_ItemNameLocalized;  // ���ػ�����Ʒ����
        [CreateProperty] public LocalizedString PromoBannerLocalized => m_PromoBannerLocalized;  // ���ػ��Ĵ������

        [CreateProperty] public string ItemName => m_ItemName;  // ��Ʒ����
        [CreateProperty] public Sprite Icon => m_Icon;  // ͼ��
        [CreateProperty] public float Cost => m_Cost;  // �ɱ�
        [CreateProperty] public uint Discount => m_Discount;  // �ۿ�
        [CreateProperty] public string PromoBannerText => m_PromoBannerText;  // ��������ı�
        [CreateProperty] public uint ContentAmount => m_ContentValue;  // ��������
        [CreateProperty] public ShopItemType ContentType => m_ContentType;  // ��������

        // ����ͼ/ͼ�����Դλ��
        const string k_ResourcePath = "GameData/GameIcons";

        // ���ͼ�������/�̵���Ʒ���͵Ŀɽű�������
        GameIconsSO m_GameIconsData;

        void OnEnable()
        {
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
        }

        /// <summary>
        /// ȷ���������Ʒ����Ļ������͡�����ң���ң���ҪӲ���ң���ʯ����Ӳ������Ҫ��Ǯ��Ԫ��Ѫƿ��Ҫ����ң���ң�������ҩˮ��ҪӲ���ң���ʯ����
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
            get => m_GameIconsData.GetShopTypeIcon(m_ContentType); // ���� ContentType ������Ӧ��ͼ��
        }

        [CreateProperty]
        public string FormattedContentValue
        {
            get => $" {m_ContentValue}"; // ���ظ�ʽ��������ֵ��ǰ����һ���ո�
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
        /// ��ʽ���ĳɱ��ı���
        /// </summary>
        [CreateProperty]
        public string FormattedCost
        {
            get
            {
                if (Cost <= 0.00001f)
                    return string.Empty;  // ���������Ʒ�����ǽ�ʹ�ñ��ػ��汾

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
                    string decimalPlaces = (CostInCurrencyType == CurrencyType.USD) ? "0.00" : "0"; // ���������Ԫ����С��
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