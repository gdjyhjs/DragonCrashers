using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 作物数据库，用于存储和管理所有可用的作物类型
    /// 通过CreateAssetMenu属性可在Unity编辑器中创建该数据库的实例
    /// </summary>
    [CreateAssetMenu(fileName = "CropDatabase", menuName = "2D Farming/Crop Database")]
    public class CropDatabase : BaseDatabase<Crop>
    {
        // 继承BaseDatabase的所有功能，无需额外实现
    }
}