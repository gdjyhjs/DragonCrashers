using System;
using System.Collections.Generic;
using HappyHarvest;
using Template2DCommon;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace HappyHarvest
{
    /// <summary>
    /// 玩家物品管理系统，处理物品的存储、使用和装备等功能
    /// 目前固定大小为 9 格
    /// </summary>
    [Serializable]
    public class InventorySystem
    {
        // 库存最大格子数
        public const int InventorySize = 9;

        /// <summary>
        /// 库存条目结构，包含物品和堆叠数量
        /// </summary>
        [Serializable]
        public class InventoryEntry
        {
            public Item Item; // 物品引用
            public int StackSize; // 堆叠数量
        }

        // 当前装备的物品索引
        public int EquippedItemIdx { get; private set; }
        // 当前装备的物品属性
        public Item EquippedItem => Entries[EquippedItemIdx].Item;

        // 库存条目数组
        public InventoryEntry[] Entries = new InventoryEntry[InventorySize];

        /// <summary>
        /// 初始化库存系统
        /// </summary>
        public void Init()
        {
            EquippedItemIdx = 0;
        }

        /// <summary>
        /// 使用已装备的物品
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <returns>使用是否成功</returns>
        public bool UseEquippedObject(Vector3Int target)
        {
            // 检查是否有装备的物品且可在目标位置使用
            if (EquippedItem == null || !EquippedItem.CanUse(target))
                return false;

            bool used = EquippedItem.Use(target);

            if (used)
            {
                // 播放物品使用音效
                if (EquippedItem.UseSound != null && EquippedItem.UseSound.Length > 0)
                {
                    SoundManager.Instance.PlaySFXAt(GameManager.Instance.Player.transform.position,
                    EquippedItem.UseSound[Random.Range(0, EquippedItem.UseSound.Length)], false);
                }

                // 消耗品使用后减少堆叠数量
                if (EquippedItem.Consumable)
                {
                    Entries[EquippedItemIdx].StackSize -= 1;

                    if (Entries[EquippedItemIdx].StackSize == 0)
                    {
                        Entries[EquippedItemIdx].Item = null;
                    }

                    // 更新库存 UI
                    UIHandler.UpdateInventory(this);
                }
            }

            return used;
        }

        /// <summary>
        /// 检查库存是否能容纳指定数量的物品
        /// </summary>
        /// <param name="newItem">要添加的物品</param>
        /// <param name="amount">要添加的数量</param>
        /// <returns>是否可容纳</returns>
        public bool CanFitItem(Item newItem, int amount)
        {
            int toFit = amount;

            // 检查已有同类型物品的格子
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == newItem)
                {
                    int size = newItem.MaxStackSize - Entries[i].StackSize;
                    toFit -= size;

                    if (toFit <= 0)
                        return true;
                }
            }

            // 检查空格子
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == null)
                {
                    toFit -= newItem.MaxStackSize;
                    if (toFit <= 0)
                        return true;
                }
            }

            return toFit == 0;
        }

        /// <summary>
        /// 获取库存可容纳指定物品的最大数量
        /// </summary>
        /// <param name="item">物品类型</param>
        /// <returns>可容纳数量</returns>
        public int GetMaximumAmountFit(Item item)
        {
            int canFit = 0;
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == null)
                {
                    canFit += item.MaxStackSize;
                }
                else if (Entries[i].Item == item)
                {
                    canFit += item.MaxStackSize - Entries[i].StackSize;
                }
            }

            return canFit;
        }

        /// <summary>
        /// 获取指定物品在库存中的索引
        /// </summary>
        /// <param name="item">物品类型</param>
        /// <param name="returnOnlyNotFull">是否只返回未满的格子</param>
        /// <returns>物品索引，未找到返回 - 1</returns>
        public int GetIndexOfItem(Item item, bool returnOnlyNotFull)
        {
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == item &&
                (!returnOnlyNotFull || Entries[i].StackSize != Entries[i].Item.MaxStackSize))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 向库存添加物品
        /// </summary>
        /// <param name="newItem">要添加的物品</param>
        /// <param name="amount">要添加的数量，默认 1</param>
        /// <returns>添加是否成功</returns>
        public bool AddItem(Item newItem, int amount = 1)
        {
            int remainingToFit = amount;

            // 先检查已有同类型物品的格子
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == newItem && Entries[i].StackSize < newItem.MaxStackSize)
                {
                    int fit = Mathf.Min(newItem.MaxStackSize - Entries[i].StackSize, remainingToFit);
                    Entries[i].StackSize += fit;
                    remainingToFit -= fit;
                    UIHandler.UpdateInventory(this);

                    if (remainingToFit == 0)
                        return true;
                }
            }

            // 检查空格子
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == null)
                {
                    Entries[i].Item = newItem;
                    int fit = Mathf.Min(newItem.MaxStackSize - Entries[i].StackSize, remainingToFit);
                    remainingToFit -= fit;
                    Entries[i].StackSize = fit;

                    UIHandler.UpdateInventory(this);

                    if (remainingToFit == 0)
                        return true;
                }
            }

            // 库存已满，无法添加
            return remainingToFit == 0;
        }

        /// <summary>
        /// 从库存移除物品
        /// </summary>
        /// <param name="index">格子索引</param>
        /// <param name="count">要移除的数量</param>
        /// <returns>实际移除的数量</returns>
        public int Remove(int index, int count)
        {
            if (index < 0 || index >= Entries.Length)
                return 0;

            int amount = Mathf.Min(count, Entries[index].StackSize);

            Entries[index].StackSize -= amount;

            if (Entries[index].StackSize == 0)
            {
                Entries[index].Item = null;
            }

            UIHandler.UpdateInventory(this);
            return amount;
        }

        /// <summary>
        /// 装备下一个物品
        /// </summary>
        public void EquipNext()
        {
            EquippedItemIdx += 1;
            if (EquippedItemIdx >= InventorySize) EquippedItemIdx = 0;

            UIHandler.UpdateInventory(this);
        }

        /// <summary>
        /// 装备上一个物品
        /// </summary>
        public void EquipPrev()
        {
            EquippedItemIdx -= 1;
            if (EquippedItemIdx < 0) EquippedItemIdx = InventorySize - 1;

            UIHandler.UpdateInventory(this);
        }

        /// <summary>
        /// 装备指定索引的物品
        /// </summary>
        /// <param name="index">物品索引</param>
        public void EquipItem(int index)
        {
            if (index < 0 || index >= Entries.Length)
                return;

            EquippedItemIdx = index;
            UIHandler.UpdateInventory(this);
        }

        /// <summary>
        /// 保存库存数据
        /// </summary>
        /// <param name="data">保存数据列表</param>
        public void Save(ref List<InventorySaveData> data)
        {
            foreach (var entry in Entries)
            {
                if (entry.Item != null)
                {
                    data.Add(new InventorySaveData()
                    {
                        Amount = entry.StackSize,
                        ItemID = entry.Item.UniqueID
                    });
                }
                else
                {
                    data.Add(null);
                }
            }
        }

        /// <summary>
        /// 加载库存数据
        /// </summary>
        /// <param name="data">保存数据列表</param>
        public void Load(List<InventorySaveData> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                if (data[i] != null)
                {
                    Entries[i].Item = GameManager.Instance.ItemDatabase.GetFromID(data[i].ItemID);
                    Entries[i].StackSize = data[i].Amount;
                }
                else
                {
                    Entries[i].Item = null;
                    Entries[i].StackSize = 0;
                }
            }
        }
    }

    /// <summary>
    /// 库存保存数据结构
    /// </summary>
    [Serializable]
    public class InventorySaveData
    {
        public int Amount; // 物品数量
        public string ItemID; // 物品唯一标识
    }

#if UNITY_EDITOR

    /// <summary>
    /// 库存系统属性抽屉（编辑器扩展）
    /// </summary>
    [CustomPropertyDrawer(typeof(InventorySystem))]
    public class InventoryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            container.Add(new Label("起始库存"));

            ListView list = new ListView();
            list.showBoundCollectionSize = false;
            list.bindingPath = nameof(InventorySystem.Entries);
            list.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            list.style.flexGrow = 1;
            list.reorderable = true;
            list.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            list.showBorder = true;

            container.Add(list);

            return container;
        }
    }

    /// <summary>
    /// 库存条目属性抽屉（编辑器扩展）
    /// </summary>
    [CustomPropertyDrawer(typeof(InventorySystem.InventoryEntry))]
    public class InventoryEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var itemProperty = property.FindPropertyRelative(nameof(InventorySystem.InventoryEntry.Item));
            var stackProperty = property.FindPropertyRelative(nameof(InventorySystem.InventoryEntry.StackSize));

            container.style.flexDirection = FlexDirection.Row;

            var itemLabel = new Label($"物品 :");
            itemLabel.style.width = Length.Percent(10);
            var itemField = new PropertyField(itemProperty, "");
            itemField.style.width = Length.Percent(40);
            var stackSizeLabel = new Label("数量 :");
            stackSizeLabel.style.width = Length.Percent(10);
            var stackField = new PropertyField(stackProperty, "");
            stackField.style.width = Length.Percent(40);

            container.Add(itemLabel);
            container.Add(itemField);
            container.Add(stackSizeLabel);
            container.Add(stackField);

            return container;
        }
    }

#endif
}