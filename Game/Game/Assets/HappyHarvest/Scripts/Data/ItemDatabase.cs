using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 物品数据库，用于管理游戏中所有物品的配置数据
    /// </summary>
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "2D Farming/Item Database")]
    public class ItemDatabase : BaseDatabase<Item>
    {
        // 继承自 BaseDatabase，无需额外实现
        // 通过 Unity 的 CreateAssetMenu 特性支持在编辑器中创建数据库实例
    }
}
