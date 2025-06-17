using System;
using System.Collections.Generic;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与InventoryScreen/InventoryController相关的公共静态委托。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class InventoryEvents
    {
        // 装备物品被点击时触发的事件
        public static Action<GearItemComponent> GearItemClicked;

        // 库存界面出现时触发的事件
        public static Action ScreenEnabled;

        // 选择装备物品时触发的事件
        public static Action<EquipmentSO> GearSelected;

        // 过滤装备物品时触发的事件
        public static Action<Rarity, EquipmentType> GearFiltered;

        // 初始设置时触发的事件
        public static Action InventorySetup;

        // 刷新库存时触发的事件
        public static Action<List<EquipmentSO>> InventoryUpdated;

        // 从角色界面自动装备时触发的事件
        public static Action<List<EquipmentSO>> GearAutoEquipped;
    }
}