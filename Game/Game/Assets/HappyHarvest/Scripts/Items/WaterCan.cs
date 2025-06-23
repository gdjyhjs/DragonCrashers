using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 水壶物品类，用于灌溉已翻耕的地块
    /// </summary>
    [CreateAssetMenu(fileName = "WaterCan", menuName = "2D Farming/Items/Water Can")]
    public class WaterCan : Item
    {
        /// <summary>
        /// 检查目标位置是否可灌溉
        /// </summary>
        public override bool CanUse(Vector3Int target)
        {
            // 条件：地形管理器存在且目标地块已翻耕
            return GameManager.Instance.Terrain.IsTilled(target);
        }

        /// <summary>
        /// 使用水壶灌溉地块
        /// </summary>
        public override bool Use(Vector3Int target)
        {
            // 调用地形管理器的灌溉方法
            GameManager.Instance.Terrain.WaterAt(target);
            return true;
        }
    }
}