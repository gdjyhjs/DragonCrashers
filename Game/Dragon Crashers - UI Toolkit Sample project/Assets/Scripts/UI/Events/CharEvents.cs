using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与CharScreen/CharScreenController相关的公共静态委托。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class CharEvents
    {
        // 在角色界面选择下一个角色
        public static Action NextCharacterSelected;
        // 在角色界面选择上一个角色
        public static Action LastCharacterSelected;

        // 角色界面启动时触发
        public static Action ScreenStarted;

        // 角色界面隐藏时触发
        public static Action ScreenEnded;

        // 使用特定的装备物品槽索引打开库存
        public static Action<int> InventoryOpened;

        // 自动装备装备
        public static Action GearAutoEquipped;
        // 全部卸下装备
        public static Action GearAllUnequipped;

        // 点击升级按钮
        public static Action LevelUpClicked;

        // 在角色界面显示升级按钮
        public static Action<bool> LevelUpButtonEnabled;

        // 升级过程成功/失败
        public static Action<bool> CharacterLeveledUp;

        // 提升角色属性等级
        public static Action<CharacterData> LevelIncremented;

        // 更新等级计量窗口
        public static Action<float> LevelUpdated;

        // 角色预览初始化后触发
        public static Action PreviewInitialized;

        // 显示角色
        public static Action<CharacterData> CharacterShown;

        // 装备物品被卸下时触发
        public static Action<EquipmentSO> GearItemUnequipped;

        // 更新装备槽；提供装备数据和槽索引
        public static Action<EquipmentSO, int> GearSlotUpdated;

        // 在当前角色上自动装备装备
        // 提供要自动装备的角色数据
        public static Action<CharacterData> CharacterAutoEquipped;

        // 初始化所有角色的起始装备时触发
        // 提供带有起始装备数据的角色列表
        public static Action<List<CharacterData>> GearDataInitialized;

        // 使用升级药剂并提供消耗药剂的角色数据
        public static Action<CharacterData> LevelPotionUsed;

        // 静态函数，用于提供LevelMeterData
        public static Func<LevelMeterData> GetLevelMeterData;
    }
}