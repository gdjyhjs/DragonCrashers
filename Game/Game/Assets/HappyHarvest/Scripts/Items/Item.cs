using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 物品基类，所有物品都应继承自该类
    /// 实现了 IDatabaseEntry 接口以支持数据库管理
    /// </summary>
    public abstract class Item : ScriptableObject, IDatabaseEntry
    {
        /// <summary>
        /// 数据库键（实现 IDatabaseEntry 接口）
        /// </summary>
        public string Key => UniqueID;

        [Tooltip("数据库中用于标识该物品的名称，保存系统使用")]
        // 物品唯一标识符
        public string UniqueID = "DefaultID";

        // 物品显示名称
        public string DisplayName;
        // 物品图标
        public Sprite ItemSprite;
        // 最大堆叠数量
        public int MaxStackSize = 10;
        // 是否为消耗品
        public bool Consumable = true;
        // 购买价格（-1 表示不可购买）
        public int BuyPrice = -1;

        [Tooltip("装备时在玩家手中实例化的预制体")]
        // 装备时的视觉预制体
        public GameObject VisualPrefab;
        // 使用物品时触发的玩家动画触发器名称
        public string PlayerAnimatorTriggerUse = "GenericToolSwing";

        [Tooltip("使用物品时触发的音效")]
        // 使用物品时的音效数组
        public AudioClip[] UseSound;

        /// <summary>
        /// 检查物品是否可在目标位置使用
        /// </summary>
        /// <param name="target">目标网格坐标</param>
        /// <returns>是否可使用</returns>
        public abstract bool CanUse(Vector3Int target);

        /// <summary>
        /// 使用物品（需子类实现具体逻辑）
        /// </summary>
        /// <param name="target">目标网格坐标</param>
        /// <returns>使用是否成功</returns>
        public abstract bool Use(Vector3Int target);

        /// <summary>
        /// 判断物品是否需要目标位置（默认需要）
        /// 无需目标的物品（如可直接使用的消耗品）可重写此方法
        /// </summary>
        public virtual bool NeedTarget()
        {
            return true;
        }
    }
}