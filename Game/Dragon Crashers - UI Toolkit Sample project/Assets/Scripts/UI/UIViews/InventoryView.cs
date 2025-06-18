using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// �������û����棬�������˺�ѡ��״̬��
    /// ���������˵�ʹ�ñ��ػ��ַ�����
    /// </summary>
    public class InventoryView : UIView
    {
        /// <summary>
        /// ��Щ���鶨�������ڹ��˵��ڲ�ֵ��ʹ���������������
        /// �뱾�ػ���������� "Inventory_Rarity_Common"������ͬ������ά��
        /// �����˵���˳��
        /// </summary>
        public static readonly string[] RarityKeys = { "All", "Common", "Rare", "Special" };
        public static readonly string[] SlotTypeKeys = { "All", "Weapon", "Shield", "Helmet", "Boots", "Gloves" };

        ScrollView m_ScrollViewParent;

        VisualElement m_InventoryBackButton;
        VisualElement m_InventoryPanel;

        DropdownField m_InventoryRarityDropdown;
        DropdownField m_InventorySlotTypeDropdown;

        // ÿ��װ����Ʒ��ģ����Դ
        VisualTreeAsset m_GearItemAsset;

        // ��ǰѡ�е�װ��
        GearItemComponent m_SelectedGear;

        public InventoryView(VisualElement topElement) : base(topElement)
        {
            InventoryEvents.GearItemClicked += OnGearItemClicked;
            InventoryEvents.InventorySetup += OnInventorySetup;
            InventoryEvents.InventoryUpdated += OnInventoryUpdated;

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;

            m_GearItemAsset = Resources.Load("GearItem") as VisualTreeAsset;
        }

        void OnSelectedLocaleChanged(Locale obj)
        {
            // ���±��ػ��ı�
            UpdateLocalizedText();
        }

        public override void Dispose()
        {
            base.Dispose();
            InventoryEvents.GearItemClicked -= OnGearItemClicked;
            InventoryEvents.InventorySetup -= OnInventorySetup;
            InventoryEvents.InventoryUpdated -= OnInventoryUpdated;

            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;

            // ע����ť�ص�
            UnregisterButtonCallbacks();
        }


        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_InventoryBackButton = m_TopElement.Q("inventory__back-button");
            m_InventoryPanel = m_TopElement.Q("inventory__screen");
            m_InventoryRarityDropdown = m_TopElement.Q<DropdownField>("inventory__rarity-dropdown");
            m_InventorySlotTypeDropdown = m_TopElement.Q<DropdownField>("inventory__slot-type-dropdown");

            // ���������ͼ�µ���Ԫ��
            m_ScrollViewParent = m_TopElement.Q<ScrollView>("inventory__scrollview");

            // ���±��ػ��ı�
            UpdateLocalizedText();
        }

        protected override void RegisterButtonCallbacks()
        {
            // ע�᷵�ذ�ť����¼�
            m_InventoryBackButton.RegisterCallback<ClickEvent>(CloseWindow);

            // ��������ֵ�ı�ʱע��ص�
            m_InventoryRarityDropdown.RegisterValueChangedCallback(UpdateFilters);
            m_InventorySlotTypeDropdown.RegisterValueChangedCallback(UpdateFilters);
        }

        // ��ѡ��ע����ť�ص��ڴ��������²����ϸ��Ҫ��
        // ��ȡ�������Ӧ�ó�����������ڹ���
        // ����Ը��ݾ������ѡ��ע�����ǡ�
        protected void UnregisterButtonCallbacks()
        {
            m_InventoryBackButton.UnregisterCallback<ClickEvent>(CloseWindow);

            // ע��������ֵ�ı�ʱ�Ļص�
            m_InventoryRarityDropdown.UnregisterValueChangedCallback(UpdateFilters);
            m_InventorySlotTypeDropdown.UnregisterValueChangedCallback(UpdateFilters);
        }

        // ���ַ���ת��Ϊϡ�ж�ö��
        Rarity GetRarity(string rarityString)
        {
            Rarity rarity = Rarity.Common;

            if (!Enum.TryParse<Rarity>(rarityString, out rarity))
            {
                Debug.Log("�ַ��� " + rarityString + " ת��ʧ��");
            }
            return rarity;
        }

        // ���ַ���ת��Ϊװ������ö��
        EquipmentType GetGearType(string gearTypeString)
        {
            EquipmentType gearType = EquipmentType.Weapon;

            if (!Enum.TryParse<EquipmentType>(gearTypeString, out gearType))
            {
                Debug.LogWarning("ת�� " + gearTypeString + " ʧ��");
            }
            return gearType;
        }

        /// <summary>
        /// ���������˵���ѡ����¹�������ʹ�����������������ַ���ֵ
        /// �Ա����뱾�ػ���ʾ�ı�����ȷӳ�䡣
        /// </summary>
        void UpdateFilters(ChangeEvent<string> evt)
        {
            string gearTypeKey = SlotTypeKeys[m_InventorySlotTypeDropdown.index];
            string rarityKey = RarityKeys[m_InventoryRarityDropdown.index];

            EquipmentType gearType = GetGearType(gearTypeKey);
            Rarity rarity = GetRarity(rarityKey);

            InventoryEvents.GearFiltered?.Invoke(rarity, gearType);
        }

        // �������õĲ�λ��Ϊÿ��װ����Ʒ����һ����ť
        void ShowGearItems(List<EquipmentSO> gearToShow)
        {
            // �ҵ�������ͼ�����ڴ洢װ����Ʒ��ť��Ԫ�ز�������п��
            VisualElement contentContainer = m_ScrollViewParent.Q<VisualElement>("unity-content-container");
            contentContainer.Clear();

            for (int i = 0; i < gearToShow.Count; i++)
            {
                CreateGearItemButton(gearToShow[i], contentContainer);
            }
        }

        // Ϊ�������һ����Ʒ�����һ���ɵ���İ�ť��ѡ����
        void CreateGearItemButton(EquipmentSO gearData, VisualElement container)
        {
            if (container == null)
            {
                Debug.Log("InventoryScreen.CreateGearItemButton: ȱ�ٸ�Ԫ��");
                return;
            }

            TemplateContainer gearUIElement = m_GearItemAsset.Instantiate();
            gearUIElement.AddToClassList("gear-item-spacing");

            GearItemComponent gearItem = new GearItemComponent(gearData);

            // ΪGearItemComponent���ÿ��ӻ�Ԫ��
            gearItem.SetVisualElements(gearUIElement);
            gearItem.SetGameData(gearUIElement);
            gearItem.RegisterButtonCallbacks();

            // ��ӵ���Ԫ��
            container.Add(gearUIElement);
        }

        // ѡ���ȡ��ѡ��һ����Ʒ
        void SelectGearItem(GearItemComponent gearItem, bool state)
        {
            if (gearItem == null)
                return;

            m_SelectedGear = (state) ? gearItem : null;
            gearItem.CheckItem(state);
        }

        // ��ʾ��������Ļ�ķ���
        public override void Show()
        {
            base.Show();

            InventoryEvents.ScreenEnabled?.Invoke();
            UpdateFilters(null);

            // ��Ӷ̹���Ч��
            m_InventoryPanel.transform.scale = new Vector3(0.1f, 0.1f, 0.1f);
            m_InventoryPanel.experimental.animation.Scale(1f, 200);
        }

        // �رմ���
        void CloseWindow(ClickEvent evt)
        {
            Hide();
        }

        public override void Hide()
        {
            base.Hide();

            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();

            // ����ѡ�е�װ������֪ͨInventoryScreenController
            if (m_SelectedGear != null)
                InventoryEvents.GearSelected?.Invoke(m_SelectedGear.GearData);

            m_SelectedGear = null;
        }

        // �¼�������
        void OnInventorySetup()
        {
            // ���ÿ��ӻ�Ԫ��
            SetVisualElements();
            // ע�ᰴť�ص�
            RegisterButtonCallbacks();
        }

        // ����Ҫ�ڿ������ʾ��װ��ScriptableObject�б�
        void OnInventoryUpdated(List<EquipmentSO> gearToLoad)
        {
            // ��ʾװ����Ʒ
            ShowGearItems(gearToLoad);
        }

        // ��װ����Ʒ�����ѡ�б������ʾѡ��״̬
        void OnGearItemClicked(GearItemComponent gearItem)
        {
            // ���ű��ð�ť��Ч
            AudioManager.PlayAltButtonSound();

            // ȡ��֮ǰѡ�е���Ʒ
            SelectGearItem(m_SelectedGear, false);

            // ѡ���µ�װ����Ʒ
            SelectGearItem(gearItem, true);
        }

        // ���±��ػ��ı�
        void UpdateLocalizedText()
        {
            if (m_InventoryRarityDropdown == null || m_InventorySlotTypeDropdown == null)
                return;

            // ʹ����չ��������ϡ�ж������˵�
            string[] rarityChoices = new string[]
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_All"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Common"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Rare"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Special")
            };
            m_InventoryRarityDropdown.UpdateLocalizedChoices(rarityChoices, RarityKeys[m_InventoryRarityDropdown.index], RarityKeys);

            // ʹ����չ�������²�λ���������˵�
            string[] slotTypeChoices = new string[]
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_All"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Weapon"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Shield"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Helmet"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Boots"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_SlotType_Gloves")
            };
            m_InventorySlotTypeDropdown.UpdateLocalizedChoices(slotTypeChoices, SlotTypeKeys[m_InventorySlotTypeDropdown.index], SlotTypeKeys);
        }
    }
}