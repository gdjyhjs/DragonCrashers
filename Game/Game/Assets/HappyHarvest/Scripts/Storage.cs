using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 仓库存储系统，用于管理物品的存储和取回操作
    /// </summary>
    public class Storage
    {
        // 存储的物品条目列表
        public List<InventorySystem.InventoryEntry> Content { get; private set; }

        /// <summary>
        /// 初始化仓库存储系统
        /// </summary>
        public Storage()
        {
            Content = new List<InventorySystem.InventoryEntry>();
        }

        /// <summary>
        /// 向仓库存储物品
        /// </summary>
        /// <param name="entry">要存储的物品条目</param>
        public void Store(InventorySystem.InventoryEntry entry)
        {
            // 查找已存储的相同物品（通过唯一键匹配）
            var idx = Content.FindIndex(inventoryEntry => inventoryEntry.Item.Key == entry.Item.Key);
            if (idx != -1)
            {
                // 找到相同物品则叠加数量
                Content[idx].StackSize += entry.StackSize;
            }
            else
            {
                // 未找到则添加新条目
                Content.Add(new InventorySystem.InventoryEntry()
                {
                    Item = entry.Item,
                    StackSize = entry.StackSize
                });
            }
        }

        /// <summary>
        /// 从仓库取回物品
        /// </summary>
        /// <param name="contentIndex">物品索引</param>
        /// <param name="amount">要取回的数量</param>
        /// <returns>实际取回的数量</returns>
        public int Retrieve(int contentIndex, int amount)
        {
            Debug.Assert(contentIndex < Content.Count, "尝试从仓库取回不存在的物品条目");

            // 计算实际可取回的数量（不超过库存数量）
            int actualAmount = Mathf.Min(amount, Content[contentIndex].StackSize);

            // 更新库存数量
            Content[contentIndex].StackSize -= actualAmount;

            return actualAmount;
        }
    }
}
