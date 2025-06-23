using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace HappyHarvest
{
    /// <summary>
    /// 此特殊图块添加了第三条规则：相邻图块，因此您可以匹配与该图块不同的其他图块规则。
    /// 有关示例，请参见悬崖图块。
    /// </summary>
    [CreateAssetMenu]
    public class AdjacentRuleTile : RuleTile<AdjacentRuleTile.Neighbor>
    {
        // 相邻图块邻居规则类
        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            // 相邻图块规则标识
            public const int Adjacent = 3;
        }

        // 相邻图块数组（可匹配的相邻图块）
        public TileBase[] AdjacentTiles;

        /// <summary>
        /// 检查图块规则是否匹配
        /// </summary>
        /// <param name="neighbor">邻居规则类型</param>
        /// <param name="other">相邻图块</param>
        /// <returns>是否匹配规则</returns>
        public override bool RuleMatch(int neighbor, TileBase other)
        {
            // 处理相邻图块规则
            switch (neighbor)
            {
                case Neighbor.Adjacent:
                    // 检查相邻图块是否在允许的相邻图块数组中
                    return AdjacentTiles.Contains(other);
            }

            // 调用基类方法处理其他规则
            return base.RuleMatch(neighbor, other);
        }
    }
}