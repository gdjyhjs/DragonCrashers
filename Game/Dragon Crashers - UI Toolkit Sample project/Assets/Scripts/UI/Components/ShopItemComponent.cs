using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// �����̵���Ʒ��UI�е��Ӿ���ʾ�ͽ����߼���
    /// ���̵���Ʒ���ݣ���ɱ����ۿۺ����ݣ��󶨵�UIԪ�ء�
    /// </summary>
    public class ShopItemComponent
    {
        // ��Ʒ��С�������������ѡ����
        const string k_SizeNormalClass = "shop-item__size--normal";
        const string k_SizeWideClass = "shop-item__size--wide";

        // ��ͼ�������/�̵���Ʒ������Ե�ScriptableObject
        GameIconsSO m_GameIconsData;
        ShopItemSO m_ShopItemData;

        // �Ӿ�Ԫ��
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
        /// ���캯����
        /// </summary>
        /// <param name="gameIconsData">����ͨ��ͼ�����ݵ�ScriptableObject��</param>
        /// <param name="shopItemData">������Ʒ��Ϣ��ScriptableObject��</param>
        public ShopItemComponent(GameIconsSO gameIconsData, ShopItemSO shopItemData)
        {
            m_GameIconsData = gameIconsData;
            m_ShopItemData = shopItemData;
        }

        /// <summary>
        /// ���ṩ��TemplateContainer�в�ѯ�������UIԪ�ص����á�
        /// </summary>
        public void SetVisualElements(TemplateContainer shopItemElement)
        {
            // ��ѯShopItemElement�ĸ�������
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
        /// ��������Դ�����´�С�࣬�������ݰ󶨵��̵���Ʒ��UIԪ�ء�
        /// </summary>
        /// <param name="shopItemElement"></param>
        public void SetGameData(TemplateContainer shopItemElement)
        {
            if (m_GameIconsData == null)
            {
                Debug.LogWarning("[ShopItemComponent] SetGameData: ȱ��GameIcons ScriptableObject���ݡ�");
                return;
            }

            if (shopItemElement == null)
            {
                Debug.LogWarning("[ShopItemComponent] SetGameData: ȱ��Template����");
                return;
            }

            // �ڶ���/��Ԫ������������Դ
            m_SizeContainer.dataSource = m_ShopItemData;

            UpdateSizeClass();

            BindProductInfo();

            BindBannerElements();

            BindCostElements();

            BindDiscountElements();
        }

        /// <summary>
        /// �˷�������ʵ���USS�ࡣ������Ʒʹ�ø������ʽ��
        /// </summary>
        void UpdateSizeClass()
        {
            // �����Ƴ�������������״̬
            m_SizeContainer.RemoveFromClassList(k_SizeNormalClass);
            m_SizeContainer.RemoveFromClassList(k_SizeWideClass);

            // ������Ʒ�Ƿ��������ʵ�����
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
        /// ��ShopItemSO�е����ݰ󶨵������Ͳ�Ʒͼ��
        /// </summary>
        void BindProductInfo()
        {
            // 
            m_Description.SetBinding("text", m_ShopItemData.ItemNameLocalized);

            m_ProductImage.SetBinding("style.backgroundImage", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.Icon)),
                bindingMode = BindingMode.ToTarget // ����󶨵�UI
            });

            // ��m_ContentCurrency���յ��Ľ������ͣ��ı���ͼ��󶨵�ContentCurrencyIcon����
            m_ContentCurrency.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.ContentCurrencyIcon)),
                bindingMode = BindingMode.ToTarget // �����ݵ�UI�ĵ����
            });

            // ��m_ContentValue���յ��Ľ��������ı��󶨵�FormattedContentValue����
            m_ContentValue.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FormattedContentValue)),
                bindingMode = BindingMode.ToTarget // �����ݵ�UI�ĵ����
            });

        }

        /// <summary>
        /// ��ʾ�����غ���������ʾ�й��̵���Ʒ״̬�ĸ����ı���
        /// </summary>
        private void BindBannerElements()
        {
            // ʹ��������DataBinding������Ŀɼ��԰󶨵�����Դ
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

            // ����������ı��󶨵�LocalizedString
            m_BannerLabel.SetBinding("text", m_ShopItemData.PromoBannerLocalized);
        }

        /// <summary>
        /// ����ɱ���ص�Ԫ�ء�
        /// </summary>
        void BindCostElements()
        {
            // ���ɱ��ı��󶨵�FormattedCost
            m_Cost.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FormattedCost)),
                bindingMode = BindingMode.ToTarget
            });

            // ����ʾ��ʽ�󶨵���ʾ/���س���۸�
            m_Cost.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.RegularPriceDisplay)),
                bindingMode = BindingMode.ToTarget
            });

            // ������ѡ��ı���ǩ�󶨵�ʹ�ñ��ػ�
            m_CostFree.SetBinding("text", m_ShopItemData.FreeTextLocalized);

            // ����ʾ��ʽ�󶨵���ʾ/���ء���ѡ��ı�
            m_CostFree.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.FreeTextDisplay)),
                bindingMode = BindingMode.ToTarget
            });

            // ���ɱ�ͼ��󶨵�CostIconSprite����
            m_CostIcon.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.CostIconSprite)),
                bindingMode = BindingMode.ToTarget
            });

            // ����CostIconGroupDisplayStyle������ʾ/����CostIconGroup
            m_CostIconGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.CostIconGroupDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }

        /// <summary>
        /// �����̵���Ʒ��DiscountDisplayStyle��ʾ/�������ۿ���ص�Ԫ�ء�
        /// </summary>
        void BindDiscountElements()
        {
            // ���ۿ۱�ǩ�ı��󶨵�DiscountText
            m_DiscountLabel.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountText)),
                bindingMode = BindingMode.ToTarget
            });

            // ����DiscountDisplayStyle������ʾ/�����ۿ۱�ǩ
            m_DiscountLabel.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // ���ۿۺ�ĳɱ��ı��󶨵�DiscountedCost
            m_DiscountCost.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountedCost)),
                bindingMode = BindingMode.ToTarget
            });

            // ����DiscountIconGroupDisplayStyle������ʾ/����DiscountIconGroup
            m_DiscountIconGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountIconGroupDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // ��ʾ/�����ۿۻ���
            m_DiscountBadge.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // ��ʾ/�����ۿ�б��
            m_DiscountSlash.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // ��ʾ/�����ۿ���
            m_DiscountGroup.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(ShopItemSO.DiscountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }

        /// <summary>
        /// ���ý���ʽ��ť��
        /// </summary>
        public void RegisterCallbacks()
        {
            if (m_BuyButton == null)
                return;

            // ���ɱ�/�������ݴ洢��ÿ����ť���Թ��Ժ�ʹ��
            m_BuyButton.userData = m_ShopItemData;
            m_BuyButton.RegisterCallback<ClickEvent>(BuyAction);

            // ��ֹ��ť����ƶ�ScrollView
            m_BuyButton.RegisterCallback<PointerMoveEvent>(MovePointerEventHandler);
        }

        /// <summary>
        /// ��ֹ�������������ƶ��϶���Scrollview
        /// </summary>
        /// <param name="evt">ָ���ƶ��¼���</param>
        void MovePointerEventHandler(PointerMoveEvent evt)
        {
            evt.StopImmediatePropagation();
        }

        /// <summary>
        /// ����̵���Ʒ�ϵĹ���ť�ᴥ��һϵ���¼���
        ///      - ShopItemComponent�������ť�� -->
        ///      - ShopController��������Ʒ�� -->
        ///      - GameDataManager����֤�ʽ� -->
        ///      - MagnetFXController����UI�ϲ���Ч����
        /// </summary>
        /// <param name="evt"></param>
        void BuyAction(ClickEvent evt)
        {
            VisualElement clickedElement = evt.currentTarget as VisualElement;

            // ������ǰ�洢���Զ���userData�е��̵���Ʒ����
            ShopItemSO shopItemData = clickedElement.userData as ShopItemSO;

            // ��ȡ��VisualElement 
            VisualElement rootVisualElement = m_SizeContainer.panel.visualTree;

            // ת��Ϊ����Ϊ��λ����Ļλ��
            Vector2 clickPos = new Vector2(evt.position.x, evt.position.y);
            Vector2 screenPos = clickPos.GetScreenCoordinate(rootVisualElement);

            // ֪ͨShopController�������̵���Ʒ���� + ��Ļλ�ã�
            ShopEvents.ShopItemClicked?.Invoke(shopItemData, screenPos);

            AudioManager.PlayDefaultButtonSound();
        }
    }
}