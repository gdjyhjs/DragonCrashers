using UnityEngine;

/// <summary>
/// 自然特征生成工具类（平原）
/// </summary>
public static class NaturalFeatureGenerator
{
    /// <summary>
    /// 生成自然特征
    /// </summary>
    public static void Generate(int[,] mapData, Vector2Int mapSize)
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                int cell = mapData[x, y];

                // 海洋、湖泊、山峰、森林 跳过
                if ((cell & (int)(MapData.Ocean | MapData.Lake | MapData.Mountain | MapData.Forest)) != 0)
                    continue;

                // 湖泊跳过
                if ((cell & (int)MapData.Lake) != 0)
                    continue;

                // 其余标记为平原
                else
                {
                    mapData[x, y] |= (int)MapData.Plain;
                }
            }
        }
    }
}