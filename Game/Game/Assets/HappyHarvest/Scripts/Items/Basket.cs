using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 篮子物品类，用于收获成熟作物
    /// </summary>
    [CreateAssetMenu(menuName = "2D Farming/Items/Basket")]
    public class Basket : Item
    {
        /// <summary>
        /// 检查目标位置是否可使用篮子
        /// </summary>
        /// <param name="target">目标网格坐标</param>
        /// <returns>是否可使用</returns>
        public override bool CanUse(Vector3Int target)
        {
            // 获取目标位置的作物数据
            var data = GameManager.Instance.Terrain.GetCropDataAt(target);
            // 条件：存在作物数据、有生长中的作物、生长进度为 100%
            return data != null && data.GrowingCrop != null && Mathf.Approximately(data.GrowthRatio, 1.0f);
        }

        /// <summary>
        /// 使用篮子收获作物
        /// </summary>
        /// <param name="target">目标网格坐标</param>
        /// <returns>使用是否成功</returns>
        public override bool Use(Vector3Int target)
        {
            // 获取目标位置的作物数据
            var data = GameManager.Instance.Terrain.GetCropDataAt(target);
            // 检查背包是否能容纳收获的作物
            if (!GameManager.Instance.Player.CanFitInInventory(data.GrowingCrop.Produce,
            data.GrowingCrop.ProductPerHarvest))
                return false;

            // 收获作物
            var product = GameManager.Instance.Terrain.HarvestAt(target);

            if (product != null)
            {
                // 将收获的作物添加到玩家背包
                for (int i = 0; i < product.ProductPerHarvest; ++i)
                {
                    GameManager.Instance.Player.AddItem(product.Produce);
                }

                return true;
            }

            return false;
        }
    }
}