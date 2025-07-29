using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public static class AreaExpander
{
    /// <summary>
    /// 将一个点沿周围扩大形成一片区域
    /// </summary>
    /// <param name="point">起始点</param>
    /// <param name="dirList">扩大的方向，每次扩大从里面随机一个方向，里面可能存在相同的元素用于提高某个方向的权重</param>
    /// <param name="pointCanUse">判断目标格子是否可用的方法，需要传入要检查的点</param>
    /// <param name="targetCount">需要扩大的目标数量（包含起始点）</param>
    public static Vector2Int[] ExpandPointToArea(Vector2Int point, Vector2Int[] dirList, Func<Vector2Int, bool> pointCanUse, int targetCount)
    {
        // 验证参数有效性
        if (dirList == null || dirList.Length == 0)
            dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.left, Vector2Int.down };
        if (targetCount < 1)
            targetCount = 6;
        if (pointCanUse == null)
            pointCanUse = (v) => true;

        // 检查起始点是否可用
        if (!pointCanUse(point))
        {
            Debug.Log("奇点不可用：" + point);
            return Array.Empty<Vector2Int>();
        }

        // 存储已扩展的点
        HashSet<Vector2Int> expandedPoints = new HashSet<Vector2Int>();
        expandedPoints.Add(point);

        // 存储待扩展的边缘点
        List<Vector2Int> edgePoints = new List<Vector2Int>();
        edgePoints.Add(point);

        Random random = new Random();

        // 继续扩展直到达到目标数量或无法再扩展
        while (expandedPoints.Count < targetCount && edgePoints.Count > 0)
        {
            // 随机选择一个边缘点作为扩展起点
            int edgeIndex = random.Next(edgePoints.Count);
            Vector2Int currentPoint = edgePoints[edgeIndex];

            // 尝试从随机方向扩展
            bool expanded = false;
            // 为了增加随机性，打乱方向尝试顺序
            var shuffledDirs = dirList.OrderBy(d => random.Next()).ToArray();
            foreach (var dir in shuffledDirs)
            {
                Vector2Int newPoint = currentPoint + dir;

                // 检查新点是否可用且未被添加过
                if (!expandedPoints.Contains(newPoint) && pointCanUse(newPoint))
                {
                    expandedPoints.Add(newPoint);
                    edgePoints.Add(newPoint);
                    expanded = true;

                    // 如果达到目标数量，停止扩展
                    if (expandedPoints.Count == targetCount)
                        break;
                }
            }

            // 如果当前边缘点无法再扩展，从边缘列表中移除
            if (!expanded)
            {
                edgePoints.RemoveAt(edgeIndex);
            }
        }
        return expandedPoints.ToArray();
    }


    /// <summary>
    /// 判断位置是否靠近其他地方
    /// </summary>
    public static bool IsNearLandOrIsland(Vector2Int pos, List<Vector2Int> checkList, int checkDis)
    {
        foreach (var item in checkList)
        {
            if (Vector2Int.Distance(pos, item) < checkDis)
            {
                return true;
            }
        }
        return false;
    }
}
