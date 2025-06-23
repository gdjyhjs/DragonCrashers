using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 种子袋物品类，用于在耕地上种植作物
    /// </summary>
    [CreateAssetMenu(fileName = "SeedBag", menuName = "2D Farming/Items/SeedBag")]
    public class SeedBag : Item
    {
        // 种植的作物类型
        public Crop PlantedCrop;

        /// <summary>
        /// 检查目标位置是否可种植
        /// </summary>
        public override bool CanUse(Vector3Int target)
        {
            // 条件：地形管理器存在且目标位置可种植
            return GameManager.Instance.Terrain.IsPlantable(target);
        }

        /// <summary>
        /// 使用种子袋种植作物
        /// </summary>
        public override bool Use(Vector3Int target)
        {
            // 调用地形管理器的种植方法
            GameManager.Instance.Terrain.PlantAt(target, PlantedCrop);
            return true;
        }
    }
}