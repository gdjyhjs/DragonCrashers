using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 锄头物品类，用于翻耕地块
    /// </summary>
    [CreateAssetMenu(fileName = "Hoe", menuName = "2D Farming/Items/Hoe")]
    public class Hoe : Item
    {
        /// <summary>
        /// 检查目标位置是否可使用锄头
        /// </summary>
        /// <param name="target">目标网格坐标</param>
        /// <returns>是否可使用</returns>
        public override bool CanUse(Vector3Int target)
        {
            // 条件：存在地形管理器且目标位置可翻耕
            return GameManager.Instance?.Terrain != null && GameManager.Instance.Terrain.IsTillable(target);
        }

        /// <summary>
        /// 使用锄头翻耕地块
        /// </summary>
        /// <param name="target">目标网格坐标</param>
        /// <returns>使用是否成功</returns>
        public override bool Use(Vector3Int target)
        {
            // 调用地形管理器的翻耕方法
            GameManager.Instance.Terrain.TillAt(target);
            return true;
        }
    }
}