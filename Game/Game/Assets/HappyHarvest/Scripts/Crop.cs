using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;
namespace HappyHarvest
{
    /// <summary>
    /// 用于定义地图上作物的数据类。存储作物的所有生长阶段、生长时间等信息
    /// </summary>
    [CreateAssetMenu(fileName = "Crop", menuName = "2D Farming/Crop")]
    public class Crop : ScriptableObject, IDatabaseEntry
    {
        /// <summary>
        /// 数据库键（实现 IDatabaseEntry 接口）
        /// </summary>
        public string Key => UniqueID;

        // 作物唯一标识符
        public string UniqueID = "";

        // 生长阶段对应的图块数组
        public TileBase[] GrowthStagesTiles;

        // 收获时产生的产品
        public Product Produce;

        // 完整生长所需时间
        public float GrowthTime = 1.0f;
        // 可收获次数
        public int NumberOfHarvest = 1;
        // 收获后进入的生长阶段
        public int StageAfterHarvest = 1;
        // 每次收获的产品数量
        public int ProductPerHarvest = 1;
        // 干旱死亡计时器（秒）
        public float DryDeathTimer = 30.0f;
        // 收获时的视觉效果
        public VisualEffect HarvestEffect;

        /// <summary>
        /// 根据生长比例获取当前生长阶段
        /// </summary>
        /// <param name="growRatio">生长比例（0-1）</param>
        /// <returns>当前生长阶段索引</returns>
        public int GetGrowthStage(float growRatio)
        {
            return Mathf.FloorToInt(growRatio * (GrowthStagesTiles.Length - 1));
        }
    }
}