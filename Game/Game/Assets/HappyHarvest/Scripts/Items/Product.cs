using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 产品类，继承自 Item 基类，代表可出售的农产品
    /// </summary>
    [CreateAssetMenu(menuName = "2D Farming/Items/Product")]
    public class Product : Item
    {
        // 产品出售价格
        public int SellPrice = 1;

        /// <summary>
        /// 检查产品是否可使用（始终返回 true）
        /// </summary>
        public override bool CanUse(Vector3Int target)
        {
            return true;
        }

        /// <summary>
        /// 使用产品（始终返回 true，无实际使用逻辑）
        /// </summary>
        public override bool Use(Vector3Int target)
        {
            return true;
        }

        /// <summary>
        /// 判断产品是否需要目标位置（返回 false，产品无需目标）
        /// </summary>
        public override bool NeedTarget()
        {
            return false;
        }
    }
}