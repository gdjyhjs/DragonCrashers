using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理库存用户界面，包括过滤和选择状态。
    /// 过滤下拉菜单使用本地化字符串。
    /// </summary>
    public class InventoryView : UIView
    {
        /// <summary>
        /// 这些数组定义了用于过滤的内部值。使用数组可以让我们
        /// 与本地化表键（例如 "Inventory_Rarity_Common"）保持同步，并维护
        /// 下拉菜单的顺序。
        /// </summary>
        public static readonly string[] RarityKeys = { "All", "Common", "Rare", "Special" };
        public static readonly string[] SlotTypeKeys = { "All", "Weapon", "Shield", "Helmet", "Boots", "Gloves" };

        ScrollView m_ScrollViewParent;

        VisualElement m_InventoryBackButton;
        VisualElement m_InventoryPanel;

        DropdownField m_InventoryRarityDropdown;
        DropdownField m_InventorySlotTypeDropdown;

        // 每个装备物品的模板资源
        VisualTreeAsset m_GearItemAsset;

        // 当前选中的装备
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
            // 更新本地化文本
            UpdateLocalizedText();
        }

        public override void Dispose()
        {
            base.Dispose();
            InventoryEvents.GearItemClicked -= OnGearItemClicked;
            InventoryEvents.InventorySetup -= OnInventorySetup;
            InventoryEvents.InventoryUpdated -= OnInventoryUpdated;

            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;

            // 注销按钮回调
            UnregisterButtonCallbacks();
        }


        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_InventoryBackButton = m_TopElement.Q("inventory__back-button");
            m_InventoryPanel = m_TopElement.Q("inventory__screen");
            m_InventoryRarityDropdown = m_TopElement.Q<DropdownField>("inventory__rarity-dropdown");
            m_InventorySlotTypeDropdown = m_TopElement.Q<DropdownField>("inventory__slot-type-dropdown");

            // 定义滚动视图下的行元素
            m_ScrollViewParent = m_TopElement.Q<ScrollView>("inventory__scrollview");

            // 更新本地化文本
            UpdateLocalizedText();
        }

        protected override void RegisterButtonCallbacks()
        {
            // 注册返回按钮点击事件
            m_InventoryBackButton.RegisterCallback<ClickEvent>(CloseWindow);

            // 当下拉框值改变时注册回调
            m_InventoryRarityDropdown.RegisterValueChangedCallback(UpdateFilters);
            m_InventorySlotTypeDropdown.RegisterValueChangedCallback(UpdateFilters);
        }

        // 可选：注销按钮回调在大多数情况下不是严格必要的
        // 这取决于你的应用程序的生命周期管理。
        // 你可以根据具体情况选择注销它们。
        protected void UnregisterButtonCallbacks()
        {
            m_InventoryBackButton.UnregisterCallback<ClickEvent>(CloseWindow);

            // 注销下拉框值改变时的回调
            m_InventoryRarityDropdown.UnregisterValueChangedCallback(UpdateFilters);
            m_InventorySlotTypeDropdown.UnregisterValueChangedCallback(UpdateFilters);
        }

        // 将字符串转换为稀有度枚举
        Rarity GetRarity(string rarityString)
        {
            Rarity rarity = Rarity.Common;

            if (!Enum.TryParse<Rarity>(rarityString, out rarity))
            {
                Debug.Log("字符串 " + rarityString + " 转换失败");
            }
            return rarity;
        }

        // 将字符串转换为装备类型枚举
        EquipmentType GetGearType(string gearTypeString)
        {
            EquipmentType gearType = EquipmentType.Weapon;

            if (!Enum.TryParse<EquipmentType>(gearTypeString, out gearType))
            {
                Debug.LogWarning("转换 " + gearTypeString + " 失败");
            }
            return gearType;
        }

        /// <summary>
        /// 根据下拉菜单的选择更新过滤器。使用数组索引而不是字符串值
        /// 以保持与本地化显示文本的正确映射。
        /// </summary>
        void UpdateFilters(ChangeEvent<string> evt)
        {
            string gearTypeKey = SlotTypeKeys[m_InventorySlotTypeDropdown.index];
            string rarityKey = RarityKeys[m_InventoryRarityDropdown.index];

            EquipmentType gearType = GetGearType(gearTypeKey);
            Rarity rarity = GetRarity(rarityKey);

            InventoryEvents.GearFiltered?.Invoke(rarity, gearType);
        }

        // 遍历可用的槽位并为每个装备物品创建一个按钮
        void ShowGearItems(List<EquipmentSO> gearToShow)
        {
            // 找到滚动视图下用于存储装备物品按钮的元素并清除现有库存
            VisualElement contentContainer = m_ScrollViewParent.Q<VisualElement>("unity-content-container");
            contentContainer.Clear();

            for (int i = 0; i < gearToShow.Count; i++)
            {
                CreateGearItemButton(gearToShow[i], contentContainer);
            }
        }

        // 为库存生成一个物品并添加一个可点击的按钮来选择它
        void CreateGearItemButton(EquipmentSO gearData, VisualElement container)
        {
            if (container == null)
            {
                Debug.Log("InventoryScreen.CreateGearItemButton: 缺少父元素");
                return;
            }

            TemplateContainer gearUIElement = m_GearItemAsset.Instantiate();
            gearUIElement.AddToClassList("gear-item-spacing");

            GearItemComponent gearItem = new GearItemComponent(gearData);

            // 为GearItemComponent设置可视化元素
            gearItem.SetVisualElements(gearUIElement);
            gearItem.SetGameData(gearUIElement);
            gearItem.RegisterButtonCallbacks();

            // 添加到父元素
            container.Add(gearUIElement);
        }

        // 选择或取消选择一个物品
        void SelectGearItem(GearItemComponent gearItem, bool state)
        {
            if (gearItem == null)
                return;

            m_SelectedGear = (state) ? gearItem : null;
            gearItem.CheckItem(state);
        }

        // 显示和隐藏屏幕的方法
        public override void Show()
        {
            base.Show();

            InventoryEvents.ScreenEnabled?.Invoke();
            UpdateFilters(null);

            // 添加短过渡效果
            m_InventoryPanel.transform.scale = new Vector3(0.1f, 0.1f, 0.1f);
            m_InventoryPanel.experimental.animation.Scale(1f, 200);
        }

        // 关闭窗口
        void CloseWindow(ClickEvent evt)
        {
            Hide();
        }

        public override void Hide()
        {
            base.Hide();

            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();

            // 设置选中的装备，并通知InventoryScreenController
            if (m_SelectedGear != null)
                InventoryEvents.GearSelected?.Invoke(m_SelectedGear.GearData);

            m_SelectedGear = null;
        }

        // 事件处理方法
        void OnInventorySetup()
        {
            // 设置可视化元素
            SetVisualElements();
            // 注册按钮回调
            RegisterButtonCallbacks();
        }

        // 加载要在库存中显示的装备ScriptableObject列表
        void OnInventoryUpdated(List<EquipmentSO> gearToLoad)
        {
            // 显示装备物品
            ShowGearItems(gearToLoad);
        }

        // 在装备物品上添加选中标记以显示选择状态
        void OnGearItemClicked(GearItemComponent gearItem)
        {
            // 播放备用按钮音效
            AudioManager.PlayAltButtonSound();

            // 取消之前选中的物品
            SelectGearItem(m_SelectedGear, false);

            // 选择新的装备物品
            SelectGearItem(gearItem, true);
        }

        // 更新本地化文本
        void UpdateLocalizedText()
        {
            if (m_InventoryRarityDropdown == null || m_InventorySlotTypeDropdown == null)
                return;

            // 使用扩展方法更新稀有度下拉菜单
            string[] rarityChoices = new string[]
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_All"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Common"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Rare"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Inventory_Rarity_Special")
            };
            m_InventoryRarityDropdown.UpdateLocalizedChoices(rarityChoices, RarityKeys[m_InventoryRarityDropdown.index], RarityKeys);

            // 使用扩展方法更新槽位类型下拉菜单
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