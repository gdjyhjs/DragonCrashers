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
        // �ɵ����ǩ��ť����ѡ����
        const string k_TabClass = "shoptab";

        // ��ǰѡ����ǩ��ť����ѡ����
        const string k_SelectedTabClass = "selected-shoptab";
        const string k_ShopTemplateContainerClass = "shop-item__template-container";

        // ����/ͼ�����Դλ��
        const string k_ResourcePath = "GameData/GameIcons";

        [Header("�̵���Ʒ")]
        [Tooltip("Ҫʵ������ ShopItem Ԫ���ʲ�")]
        [SerializeField] VisualTreeAsset m_ShopItemAsset;
        [SerializeField] GameIconsSO m_GameIconsData;

        // �Ӿ�Ԫ��
        VisualElement m_ShopScrollView;

        VisualElement m_GoldTabButton;
        VisualElement m_GemTabButton;
        VisualElement m_PotionTabButton;

        public ShopView(VisualElement topElement) : base(topElement)
        {
            ShopEvents.ShopUpdated += OnShopUpdated;
            ShopEvents.TabSelected += OnTabSelected;

            // ��� ScriptableObject ���������ͣ��̵���Ʒ�����ܡ�ϡ�жȡ����ȣ����ض�ͼ����� 
            // ��Ĭ��·�� = Resources/GameData/GameIcons��
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

        // ��ѡ���ڴ��������£�ȡ��ע�ᰴť�ص��������ϸ��Ҫ�ģ���ȡ����Ӧ�ó�����������ڹ���
        // �����Ҫ�ض�����������ѡ��ȡ��ע�����ǡ�
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

        // ʹ�� ClickEvent �����ǩ��ť�ĵ������
        void ClickTabButton(ClickEvent evt)
        {
            VisualElement clickedTab = evt.currentTarget as VisualElement;

            ClickTabButton(clickedTab);
        }

        // ʹ�� VisualElement �����ǩ��ť�ĵ������
        void ClickTabButton(VisualElement clickedTab)
        {
            // �������ı�ǩ��δѡ�У���ѡ����
            if (!IsTabSelected(clickedTab))
            {
                // ȡ��ѡ��ǰ���������ǩ
                UnselectOtherTabs(clickedTab);

                // ѡ�����ı�ǩ
                SelectTab(clickedTab);

                // ����Ĭ������
                AudioManager.PlayDefaultButtonSound();
            }
        }

        // ��λ���о��б�ǩ������ VisualElement
        UQueryBuilder<VisualElement> GetAllTabs()
        {
            return m_TopElement.Query<VisualElement>(className: k_TabClass);
        }

        // ȡ��ѡ���ض���ǩ
        void UnselectTab(VisualElement tab)
        {
            tab.RemoveFromClassList(k_SelectedTabClass);
        }

        void SelectTab(VisualElement tab)
        {
            tab.AddToClassList(k_SelectedTabClass);
        }

        // ͨ������ѡ���ض���ǩ������ "gold" �� "gem" 
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

        // ����������ǩ
        public void OnShopUpdated(List<ShopItemSO> shopItems)
        {
            if (shopItems == null || shopItems.Count == 0)
                return;

            // Ϊÿ����ǩ����ҡ���ʯ��ҩˮ��������Ʒ
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

            // ��ģ�� UXML ʵ����һ���µ� Visual Element
            TemplateContainer shopItemElem = m_ShopItemAsset.Instantiate();
            shopItemElem.AddToClassList(k_ShopTemplateContainerClass);

            // Ϊÿ���̵���Ʒ���� VisualElement ����Ϸ����
            ShopItemComponent shopItemController = new ShopItemComponent(m_GameIconsData, shopItemData);

            shopItemController.SetVisualElements(shopItemElem);
            shopItemController.SetGameData(shopItemElem);

            shopItemController.RegisterCallbacks();

            parentElement.Add(shopItemElem);

        }
    }
}