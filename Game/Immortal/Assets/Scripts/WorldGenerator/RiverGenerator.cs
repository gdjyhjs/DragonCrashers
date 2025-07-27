using UnityEngine;

/// <summary>
/// 河流生成工具类
/// </summary>
public static class RiverGenerator
{
    /// <summary>
    /// 生成河流
    /// </summary>
    public static void Generate(int[,] mapData, Vector2Int mapSize)
    {
        int riverCount = Random.Range(2, 5);

        for (int i = 0; i < riverCount; i++)
        {
            Vector2Int start = GetEdgePosition(mapSize);
            Vector2Int end;
            do
            {
                end = GetEdgePosition(mapSize);
            } while (start == end);

            GenerateRiverPath(mapData, mapSize, start, end);
        }
    }

    /// <summary>
    /// 获取地图边缘位置作为河流起点/终点
    /// </summary>
    private static Vector2Int GetEdgePosition(Vector2Int mapSize)
    {
        if (Random.value < 0.5f)
        {
            // 左右边缘
            return new Vector2Int(
                Random.value < 0.5f ? 0 : mapSize.x - 1,
                Random.Range(0, mapSize.y)
            );
        }
        else
        {
            // 上下边缘
            return new Vector2Int(
                Random.Range(0, mapSize.x),
                Random.value < 0.5f ? 0 : mapSize.y - 1
            );
        }
    }

    /// <summary>
    /// 生成河流路径
    /// </summary>
    private static void GenerateRiverPath(int[,] mapData, Vector2Int mapSize, Vector2Int start, Vector2Int end)
    {
        Vector2Int current = start;
        Vector2Int direction = end - start;
        direction = new Vector2Int(
            direction.x != 0 ? Mathf.RoundToInt((float)direction.x / Mathf.Abs(direction.x)) : 0,
            direction.y != 0 ? Mathf.RoundToInt((float)direction.y / Mathf.Abs(direction.y)) : 0
        );

        int riverWidth = Random.Range(1, 3);
        int maxSteps = 10000;
        int step = 0;

        while (current != end && step < maxSteps)
        {
            step++;
            // 绘制河流
            for (int w = -riverWidth / 2; w <= riverWidth / 2; w++)
            {
                for (int h = -riverWidth / 2; h <= riverWidth / 2; h++)
                {
                    int x = current.x + w;
                    int y = current.y + h;

                    if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y)
                    {
                        if ((mapData[x, y] & (int)MapData.Ocean) == 0)
                        {
                            mapData[x, y] |= (int)MapData.River;
                        }
                    }
                }
            }

            // 移动到下一个位置
            if (Random.value < 0.7f)
            {
                current += new Vector2Int(
                    direction.x != 0 ? direction.x : 0,
                    direction.y != 0 ? direction.y : 0
                );
            }
            else
            {
                current += new Vector2Int(
                    direction.x != 0 ? 0 : (Random.value < 0.5f ? -1 : 1),
                    direction.y != 0 ? 0 : (Random.value < 0.5f ? -1 : 1)
                );
            }

            // 限制范围
            current.x = Mathf.Clamp(current.x, 0, mapSize.x - 1);
            current.y = Mathf.Clamp(current.y, 0, mapSize.y - 1);
        }

        // 绘制终点
        for (int w = -riverWidth / 2; w <= riverWidth / 2; w++)
        {
            for (int h = -riverWidth / 2; h <= riverWidth / 2; h++)
            {
                int x = end.x + w;
                int y = end.y + h;

                if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y && (mapData[x, y] & (int)MapData.Ocean) == 0)
                {
                    mapData[x, y] |= (int)MapData.River;
                }
            }
        }
    }
}