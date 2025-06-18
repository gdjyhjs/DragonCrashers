using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理角色视图UI，包括升级按钮、装备槽和角色统计。
    /// 使用事件驱动更新（库存）和运行时数据绑定（药水标签、角色名称、力量）的混合方式。
    /// </summary>
    public class CharView : UIView
    {
        // 升级按钮禁用类名
        const string k_LevelUpButtonInactiveClass = "footer__level-up-button--inactive";
        // 升级按钮类名
        const string k_LevelUpButtonClass = "footer__level-up-button";

        // 装备槽按钮数组
        readonly Button[] m_GearSlots = new Button[4];
        // 空装备槽精灵
        readonly Sprite m_EmptyGearSlotSprite;

        // 上一个角色按钮
        Button m_LastCharButton;
        // 下一个角色按钮
        Button m_NextCharButton;
        // 自动装备按钮
        Button m_AutoEquipButton;
        // 卸下装备按钮
        Button m_UnequipButton;
        // 升级按钮
        Button m_LevelUpButton;

        // 角色标签
        Label m_CharacterLabel;
        // 下一级所需药水标签
        Label m_PotionsForNextLevel;
        // 药水数量标签
        Label m_PotionCount;
        // 力量标签
        Label m_PowerLabel;

        // 升级按钮特效
        VisualElement m_LevelUpButtonVFX;

        // 角色统计视图
        CharStatsView m_CharStatsView; // 显示角色统计的窗口

        /// <summary>
        /// 使用指定的顶级UI元素初始化角色视图。
        /// </summary>
        /// <param name="topElement">UI的根视觉元素。</param>
        public CharView(VisualElement topElement) : base(topElement)
        {
            // 订阅升级按钮启用事件
            CharEvents.LevelUpButtonEnabled += OnLevelUpButtonEnabled;
            // 订阅角色显示事件
            CharEvents.CharacterShown += OnCharacterUpdated;
            // 订阅预览初始化事件
            CharEvents.PreviewInitialized += OnInitialized;
            // 订阅装备槽更新事件
            CharEvents.GearSlotUpdated += OnGearSlotUpdated;

            // 订阅游戏数据接收事件
            GameDataManager.GameDataReceived += OnGameDataReceived;

            // 请求游戏数据
            GameDataManager.GameDataRequested?.Invoke();

            // 从ScriptableObject图标中定位空装备槽精灵
            var gameIconsData = Resources.Load("GameData/GameIcons") as GameIconsSO;
            m_EmptyGearSlotSprite = gameIconsData.emptyGearSlotIcon;

            // 初始化角色统计视图
            m_CharStatsView = new CharStatsView(topElement.Q<VisualElement>("CharStatsWindow"));
            // 显示角色统计视图
            m_CharStatsView.Show();
        }

        /// <summary>
        /// 从GameDataManager接收GameData时调用。
        /// 应用运行时数据绑定。
        /// </summary>
        /// <param name="gameData">要绑定的GameData对象。</param>
        void OnGameDataReceived(GameData gameData)
        {
            // 将游戏数据绑定到UI
            BindGameDataToUI(gameData);
        }

        /// <summary>
        /// 为药水数量、下一级所需药水、角色力量和角色名称的标签添加绑定
        /// </summary>
        /// <param name="gameData"></param>
        void BindGameDataToUI(GameData gameData)
        {
            // 角色力量标签的绑定
            m_PowerLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.CurrentPower)),
                bindingMode = BindingMode.ToTarget
            });

            // 角色名称标签的绑定
            m_CharacterLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.CharacterName)),
                bindingMode = BindingMode.ToTarget
            });

            // 药水数量标签的绑定
            var potionBinding = new DataBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(string.Empty), // 无直接路径 -- 使用转换器
                bindingMode = BindingMode.ToTarget
            };

            // 格式化字符串标签（可用药水数量 / 升级所需药水数量）
            potionBinding.sourceToUiConverters.AddConverter((ref GameData data) =>
                FormatPotionCountLabel(data.LevelUpPotions));
            m_PotionCount.SetBinding("text", potionBinding);

            // 下一级所需药水标签的绑定
            m_PotionsForNextLevel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.PotionsForNextLevel)),
                bindingMode = BindingMode.ToTarget
            });
        }

        /// <summary>
        /// 根据药水可用性用适当的颜色格式化药水数量标签。
        /// </summary>
        /// <param name="potionCount">当前药水数量。</param>
        /// <returns>格式化后的药水数量字符串。</returns>
        string FormatPotionCountLabel(uint potionCount)
        {
            if (m_PotionsForNextLevel == null)
            {
                Debug.LogWarning("[CharView] FormatPotionCountLabel: PotionsForNextLevel标签未设置。");
                return potionCount.ToString();
            }

            string potionsForNextLevelString = m_PotionsForNextLevel.text.TrimStart('/');

            if (!string.IsNullOrEmpty(potionsForNextLevelString) &&
                int.TryParse(potionsForNextLevelString, out int potionsForNextLevel))
            {
                int potionsCount = (int)potionCount;

                // 根据比较结果更新药水数量标签的颜色
                m_PotionCount.style.color = (potionsForNextLevel > potionsCount)
                    ? new Color(0.88f, 0.36f, 0f) // 药水不足时为橙色
                    : new Color(0.81f, 0.94f, 0.48f); // 药水充足时为绿色
            }

            return potionCount.ToString();
        }

        /// <summary>
        /// 处理事件处理程序并清理资源。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            // 取消订阅升级按钮启用事件
            CharEvents.LevelUpButtonEnabled -= OnLevelUpButtonEnabled;
            // 取消订阅角色显示事件
            CharEvents.CharacterShown -= OnCharacterUpdated;
            // 取消订阅预览初始化事件
            CharEvents.PreviewInitialized -= OnInitialized;
            // 取消订阅装备槽更新事件
            CharEvents.GearSlotUpdated -= OnGearSlotUpdated;

            // 取消订阅游戏数据接收事件
            GameDataManager.GameDataReceived -= OnGameDataReceived;

            // 注销按钮回调
            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// 设置对UI中Visual Elements的引用。
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // 获取装备槽按钮
            m_GearSlots[0] = m_TopElement.Q<Button>("char-inventory__slot1");
            m_GearSlots[1] = m_TopElement.Q<Button>("char-inventory__slot2");
            m_GearSlots[2] = m_TopElement.Q<Button>("char-inventory__slot3");
            m_GearSlots[3] = m_TopElement.Q<Button>("char-inventory__slot4");

            // 获取下一个角色按钮
            m_NextCharButton = m_TopElement.Q<Button>("char__next-button");
            // 获取上一个角色按钮
            m_LastCharButton = m_TopElement.Q<Button>("char__last-button");

            // 获取自动装备按钮
            m_AutoEquipButton = m_TopElement.Q<Button>("char__auto-equip-button");
            // 获取卸下装备按钮
            m_UnequipButton = m_TopElement.Q<Button>("char__unequip-button");
            // 获取升级按钮
            m_LevelUpButton = m_TopElement.Q<Button>("char__level-up-button");
            // 获取升级按钮特效
            m_LevelUpButtonVFX = m_TopElement.Q<VisualElement>("char__level-up-button-vfx");

            // 获取角色标签
            m_CharacterLabel = m_TopElement.Q<Label>("char__label");
            // 获取药水数量标签
            m_PotionCount = m_TopElement.Q<Label>("char__potion-count");
            // 获取下一级所需药水标签
            m_PotionsForNextLevel = m_TopElement.Q<Label>("char__potion-to-advance");
            // 获取力量标签
            m_PowerLabel = m_TopElement.Q<Label>("char__power-label");
        }

        /// <summary>
        /// 注册按钮回调以处理按钮点击事件。
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // 注册装备槽按钮点击事件
            m_GearSlots[0].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[1].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[2].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[3].RegisterCallback<ClickEvent>(ShowInventory);

            // 注册下一个角色按钮点击事件
            m_NextCharButton.RegisterCallback<ClickEvent>(GoToNextCharacter);
            // 注册上一个角色按钮点击事件
            m_LastCharButton.RegisterCallback<ClickEvent>(GoToLastCharacter);

            // 注册自动装备按钮点击事件
            m_AutoEquipButton.RegisterCallback<ClickEvent>(AutoEquipSlots);
            // 注册卸下装备按钮点击事件
            m_UnequipButton.RegisterCallback<ClickEvent>(UnequipSlots);
            // 注册升级按钮点击事件
            m_LevelUpButton.RegisterCallback<ClickEvent>(LevelUpCharacter);
        }

        /// <summary>
        /// 注销按钮回调以防止内存泄漏。在大多数情况下可选，
        /// 取决于应用程序的生命周期管理。
        /// </summary>
        protected void UnregisterButtonCallbacks()
        {
            // 注销装备槽按钮点击事件
            m_GearSlots[0].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[1].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[2].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[3].UnregisterCallback<ClickEvent>(ShowInventory);

            // 注销下一个角色按钮点击事件
            m_NextCharButton.UnregisterCallback<ClickEvent>(GoToNextCharacter);
            // 注销上一个角色按钮点击事件
            m_LastCharButton.UnregisterCallback<ClickEvent>(GoToLastCharacter);

            // 注销自动装备按钮点击事件
            m_AutoEquipButton.UnregisterCallback<ClickEvent>(AutoEquipSlots);
            // 注销卸下装备按钮点击事件
            m_UnequipButton.UnregisterCallback<ClickEvent>(UnequipSlots);
            // 注销升级按钮点击事件
            m_LevelUpButton.UnregisterCallback<ClickEvent>(LevelUpCharacter);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Show()
        {
            base.Show();

            // 重置标签式UI
            MainMenuUIEvents.TabbedUIReset?.Invoke("CharScreen");
            // 触发屏幕开始事件
            CharEvents.ScreenStarted?.Invoke();
        }

        /// <summary>
        /// 隐藏角色视图UI并通知屏幕结束。
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            // 触发屏幕结束事件
            CharEvents.ScreenEnded?.Invoke();
        }

        /// <summary>
        /// 点击升级按钮时触发角色升级功能。
        /// </summary>
        /// <param name="evt"></param>
        void LevelUpCharacter(ClickEvent evt)
        {
            // 触发升级点击事件
            CharEvents.LevelUpClicked?.Invoke();
        }

        /// <summary>
        /// 通知CharScreenController卸下所有装备
        /// </summary>
        /// <param name="evt"></param>
        void UnequipSlots(ClickEvent evt)
        {
            // 播放备用按钮音效
            AudioManager.PlayAltButtonSound();
            // 触发所有装备卸下事件
            CharEvents.GearAllUnequipped?.Invoke();
        }

        /// <summary>
        /// 装备空槽中可用的最佳装备。
        /// </summary>
        void AutoEquipSlots(ClickEvent evt)
        {
            // 播放备用按钮音效
            AudioManager.PlayAltButtonSound();
            // 触发自动装备事件
            CharEvents.GearAutoEquipped?.Invoke();
        }

        /// <summary>
        /// 选择角色视图中的上一个角色。
        /// </summary>
        void GoToLastCharacter(ClickEvent evt)
        {
            // 播放备用按钮音效
            AudioManager.PlayAltButtonSound();
            // 触发上一个角色选择事件
            CharEvents.LastCharacterSelected?.Invoke();
        }

        /// <summary>
        /// 选择角色视图中的下一个角色。
        /// </summary>
        void GoToNextCharacter(ClickEvent evt)
        {
            // 播放备用按钮音效
            AudioManager.PlayAltButtonSound();
            // 触发下一个角色选择事件
            CharEvents.NextCharacterSelected?.Invoke();
        }

        /// <summary>
        /// 点击装备槽时打开库存屏幕。
        /// </summary>
        void ShowInventory(ClickEvent evt)
        {
            VisualElement clickedElement = evt.target as VisualElement;

            if (clickedElement == null)
                return;

            char slotNumber = clickedElement.name[clickedElement.name.Length - 1];
            int slot = (int)char.GetNumericValue(slotNumber) - 1;

            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();

            // 触发库存屏幕显示事件
            MainMenuUIEvents.InventoryScreenShown?.Invoke();

            // 触发库存打开事件
            CharEvents.InventoryOpened?.Invoke(slot);
        }

        // 事件处理方法

        void OnInitialized()
        {
            // 设置视觉元素
            SetVisualElements();
            // 注册按钮回调
            RegisterButtonCallbacks();
        }

        /// <summary>
        /// 选择新角色时更新角色视图。
        /// </summary>
        /// <param name="characterToShow">要显示的角色数据。</param>
        void OnCharacterUpdated(CharacterData characterToShow)
        {
            if (characterToShow == null)
                return;

            // 更新角色标签的数据源
            m_CharacterLabel.dataSource = characterToShow;
            m_PowerLabel.dataSource = characterToShow;
            m_PotionsForNextLevel.dataSource = characterToShow;

            // 更新角色统计视图
            m_CharStatsView.UpdateCharacterStats(characterToShow);

            // 激活角色预览实例
            characterToShow.PreviewInstance.gameObject.SetActive(true);
        }

        /// <summary>
        /// 更新装备槽的视觉表示。
        /// </summary>
        /// <param name="gearData">要显示的装备数据。</param>
        /// <param name="slotToUpdate">要更新的装备槽索引。</param>
        void OnGearSlotUpdated(EquipmentSO gearData, int slotToUpdate)
        {
            Button activeSlot = m_GearSlots[slotToUpdate];

            // 加号图标是char-inventory__slot-n的第一个子元素
            VisualElement addSymbol = activeSlot.ElementAt(0);

            // 背景精灵是char-inventory__slot-n的第二个子元素
            VisualElement gearElement = activeSlot.ElementAt(1);

            if (gearData == null)
            {
                if (gearElement != null)
                    // 设置装备槽背景为空白精灵
                    gearElement.style.backgroundImage = new StyleBackground(m_EmptyGearSlotSprite);

                if (addSymbol != null)
                    // 显示加号图标
                    addSymbol.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (gearElement != null)
                    // 设置装备槽背景为装备精灵
                    gearElement.style.backgroundImage = new StyleBackground(gearData.sprite);

                if (addSymbol != null)
                    // 隐藏加号图标
                    addSymbol.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// 根据药水可用性切换升级按钮特效和状态。
        /// </summary>
        /// <param name="state">如果可以升级则为true，否则为false。</param>
        void OnLevelUpButtonEnabled(bool state)
        {
            if (m_LevelUpButtonVFX == null || m_LevelUpButton == null)
                return;

            // 显示或隐藏升级按钮特效
            m_LevelUpButtonVFX.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;

            if (state)
            {
                // 启用按钮并允许鼠标指针激活:hover伪状态
                m_LevelUpButton.SetEnabled(true);
                m_LevelUpButton.pickingMode = PickingMode.Position;

                // 添加和移除样式类以激活按钮
                m_LevelUpButton.AddToClassList(k_LevelUpButtonClass);
                m_LevelUpButton.RemoveFromClassList(k_LevelUpButtonInactiveClass);
            }
            else
            {
                // 禁用按钮并禁止鼠标指针激活:hover伪状态
                m_LevelUpButton.SetEnabled(false);
                m_LevelUpButton.pickingMode = PickingMode.Ignore;
                m_LevelUpButton.AddToClassList(k_LevelUpButtonInactiveClass);
                m_LevelUpButton.RemoveFromClassList(k_LevelUpButtonClass);
            }
        }
    }
}