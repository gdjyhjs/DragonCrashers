using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Drawing;
using UnityEngine.UIElements;

/// <summary>
/// 海洋、大陆和岛屿生成工具类 - 优化版
/// 特点：板块靠近边界时扩张概率逐渐降低，避免直线边缘
/// </summary>
public static class OceanContinentGenerator
{
    /// <summary>
    /// 边界影响大小
    /// </summary>
    private static int borderInfluenceX;
    private static int borderInfluenceY;

    /// <summary>
    /// 当前创建的陆地格子数
    /// </summary>
    private static int continentGridCount;

    /// <summary>
    /// 目标创建陆地格子数
    /// </summary>
    private static int targetGridCount;

    /// <summary>
    /// 生成海洋、大陆和岛屿
    /// </summary>
    public static void Generate(int[,] mapData, MapGeneratorConfig config)
    {
        Vector2Int mapSize = config.mapSize;

        System.DateTime startTime = System.DateTime.Now;

        // 增加一些随机性
        targetGridCount = Mathf.RoundToInt(config.continentRatio * Random.Range(0.8f, 1.2f) * mapSize.x * mapSize.y);
        float borderInfluenceRatio = config.borderInfluenceRatio * Random.Range(0.8f, 1.2f);
        borderInfluenceX = Mathf.RoundToInt(mapSize.x * borderInfluenceRatio);
        borderInfluenceY = Mathf.RoundToInt(mapSize.y * borderInfluenceRatio);
        continentGridCount = 0;

        // 初始化为海洋
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                mapData[x, y] = (int)MapData.Ocean;
            }
        }

        // 生成多个大陆板块并连接
        GenerateContinentalPlates(mapData, mapSize);

        // 计算总耗时
        System.DateTime stepTime = System.DateTime.Now;
        System.TimeSpan totalTime = stepTime - startTime;
        Debug.Log($"陆地生成完成！总耗时: {totalTime.TotalMilliseconds:F2}ms ({totalTime.TotalSeconds:F2}秒)");

        // 收集海洋区域坐标
        List<Vector2Int> oceanPositions = new List<Vector2Int>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (mapData[x, y] == (int)MapData.Ocean)
                {
                    oceanPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // 生成不同规模的岛屿
        GenerateIslands(mapData, config, oceanPositions, config.smallIslandCount, config.smallIslandSize, "Small");
        GenerateIslands(mapData, config, oceanPositions, config.mediumIslandCount, config.mediumIslandSize, "Medium");
        GenerateIslands(mapData, config, oceanPositions, config.largeIslandCount, config.largeIslandSize, "Large");
    }

    /// <summary>
    /// 生成多个大陆板块并连接形成完整大陆
    /// </summary>
    private static void GenerateContinentalPlates(int[,] mapData, Vector2Int mapSize)
    {
        // 记录开始时间
        System.DateTime startTime = System.DateTime.Now;

        // 决定生成5个大陆板块
        int plateCount = 5;
        List<HashSet<Vector2Int>> plates = new List<HashSet<Vector2Int>>();

        // 为每个板块生成独立的位置和形状
        for (int i = 0; i < plateCount; i++)
        {
            // 确保板块位置分散
            Vector2Int plateCenter = GetValidPlateCenter(mapData, mapSize, plates, i);

            // 生成单个大陆板块（超巨型岛屿）
            HashSet<Vector2Int> plate = GeneratePlate(mapData, mapSize, plateCenter, i);
            plates.Add(plate);
        }

        // 计算总耗时
        System.DateTime stepTime = System.DateTime.Now;
        System.TimeSpan totalTime = stepTime - startTime;
        Debug.Log($"大陆板块生成完成！总耗时: {totalTime.TotalMilliseconds:F2}ms ({totalTime.TotalSeconds:F2}秒)");

        // 连接所有板块形成完整大陆
        ConnectAllPlates(mapData, mapSize, plates);

        System.DateTime step2Time = System.DateTime.Now;
        System.TimeSpan total2Time = step2Time - stepTime;
        Debug.Log($"连接板块完成！总耗时: {total2Time.TotalMilliseconds:F2}ms ({total2Time.TotalSeconds:F2}秒)");

        // 处理被包围的海洋
        ResolveEnclosedOceans(mapData, mapSize);

        System.DateTime endTime = System.DateTime.Now;
        System.TimeSpan total3Time = endTime - step2Time;
        Debug.Log($"处理被包围的海洋完成！总耗时: {total3Time.TotalMilliseconds:F2}ms ({total3Time.TotalSeconds:F2}秒)");
    }

    /// <summary>
    /// 获取有效的板块中心位置，确保板块间有足够距离
    /// </summary>
    private static Vector2Int GetValidPlateCenter(int[,] mapData, Vector2Int mapSize, List<HashSet<Vector2Int>> existingPlates, int plateIndex)
    {
        // 计算中心安全区域（远离边界影响区）
        int safeMinX = borderInfluenceX;
        int safeMaxX = mapSize.x - borderInfluenceX - 1;
        int safeMinY = borderInfluenceY;
        int safeMaxY = mapSize.y - borderInfluenceY - 1;
        Vector2 safeSize = new Vector2Int(safeMaxX - safeMinX, safeMaxY - safeMinY); // 安全区域大小
        Vector2 startPoint, endPoint; // 随机起点和终点
        Vector2 areaSize = safeSize / 3; // 将安全区域分割为九宫格，每一个格子大小
        Vector2 edgeStart = new Vector2Int(safeMinX, safeMinY); // 边界起点

        //Debug.Log($"安全区域大小:{safeSize} >> 每一个格子大小:{areaSize} >> 边界起点:{edgeStart} >> borderInfluenceX:{borderInfluenceX}  borderInfluenceY:{borderInfluenceY} >> 边界终点:{new Vector2Int(safeMaxX, safeMaxY)}   ");
        // 在格子中取 Random.Range(15, 35) 随机起点，Random.Range(65, 85)随机终点点
        switch (plateIndex)
        {
            case 0: // 右下
                startPoint = edgeStart + areaSize * 2;
                endPoint = edgeStart + areaSize * 2 + areaSize * Random.Range(0.4f, 0.65f);
                break;
            case 1: // 左上
                startPoint = edgeStart + areaSize * Random.Range(0.35f, 0.6f);
                endPoint = edgeStart + areaSize;
                break;
            case 2: // 右上
                startPoint = edgeStart + new Vector2(areaSize.x * 2 + areaSize.x * Random.Range(0.15f, 0.35f), areaSize.y * Random.Range(0.15f, 0.35f));
                endPoint = edgeStart + new Vector2(areaSize.x * 2 + areaSize.x * Random.Range(0.65f, 0.85f), areaSize.y * Random.Range(0.65f, 0.85f));
                break;
            case 3: // 左下
                startPoint = edgeStart + new Vector2(areaSize.x * Random.Range(0.15f, 0.35f), areaSize.y * 2 + areaSize.y * Random.Range(0.15f, 0.35f));
                endPoint = edgeStart + new Vector2(areaSize.x * Random.Range(0.65f, 0.85f), areaSize.y * 2 + areaSize.y * Random.Range(0.65f, 0.85f));
                break;
            default: // 中心附近
                startPoint = edgeStart + areaSize + areaSize * Random.Range(0.15f, 0.35f);
                endPoint = edgeStart + areaSize + areaSize * Random.Range(0.65f, 0.85f);
                break;
        }
        Vector2Int center = new Vector2Int(Mathf.RoundToInt(Random.Range(startPoint.x, endPoint.x)), Mathf.RoundToInt(Random.Range(startPoint.y, endPoint.y)));
        //Debug.Log($"随机中心位置 {plateIndex} >> {center} 起点：{startPoint}  终点：{endPoint}");

        return center;
    }

    /// <summary>
    /// 生成单个大陆板块，实现边界概率衰减
    /// </summary>
    private static HashSet<Vector2Int> GeneratePlate(int[,] mapData, Vector2Int mapSize, Vector2Int center, int plateIndex)
    {

        HashSet<Vector2Int> plate = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 初始化中心
        mapData[center.x, center.y] = (int)MapData.Continent;
        queue.Enqueue(center);
        plate.Add(center);

        // 8个方向
        Vector2Int[] directions;
        switch (plateIndex)
        {
            case 0: // 右下
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                    new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
                    , new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1)
                };

                break;
            case 1: // 左上
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                    new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
                    , new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1)
                };

                break;
            case 2: // 右上
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                    new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
                    , new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(-1, -1)
                };

                break;
            case 3: // 左下
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                    new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
                    , new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1)
                };

                break;
            default: // 中心附近
                directions = new Vector2Int[]{
                    Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                    new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
                };

                break;
        }

        // 板块大小
        int targetSize = Mathf.RoundToInt(targetGridCount / 5 * Random.Range(0.8f, 1.2f));

        // 板块形状参数
        float baseExpansionRate = Random.Range(0.7f, 0.9f);     // 基础扩张速率

        while (plate.Count < targetSize && queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // 计算当前位置的边界影响因子（0-1）
            float borderFactor = CalculateBorderFactor(
                current,
                mapSize,
                borderInfluenceX,
                borderInfluenceY
            );

            // 打乱方向
            ShuffleDirections(directions);

            // 每个位置尝试的方向数量
            int directionsToTry = Random.Range(2, 6);

            for (int i = 0; i < directionsToTry; i++)
            {
                Vector2Int dir = directions[i];
                Vector2Int next = current + dir;

                // 检查边界
                if (next.x < 0 || next.x >= mapSize.x || next.y < 0 || next.y >= mapSize.y)
                    continue;

                // 检查是否已在板块中
                if (plate.Contains(next))
                    continue;

                // 计算下一个位置的边界影响因子
                float nextBorderFactor = CalculateBorderFactor(
                    next,
                    mapSize,
                    borderInfluenceX,
                    borderInfluenceY
                );

                // 根据边界因子调整扩张概率（越靠近边界，扩张概率越低）
                float adjustedExpansionRate = baseExpansionRate * (1 - nextBorderFactor);

                // 随机扩展，创造不规则形状
                if (Random.value < adjustedExpansionRate)
                {
                    mapData[next.x, next.y] = (int)MapData.Continent;
                    plate.Add(next);
                    queue.Enqueue(next);
                }
            }
        }

        return plate;
    }

    /// <summary>
    /// 计算边界影响因子（0-1）
    /// 0 = 完全不受边界影响，1 = 位于边界最边缘
    /// </summary>
    private static float CalculateBorderFactor(
        Vector2Int position,
        Vector2Int mapSize,
        int borderInfluenceX,
        int borderInfluenceY
    )
    {
        // 计算到各边界的距离比例
        float distanceToLeft = (float)position.x / borderInfluenceX;
        float distanceToRight = (float)(mapSize.x - 1 - position.x) / borderInfluenceX;
        float distanceToTop = (float)position.y / borderInfluenceY;
        float distanceToBottom = (float)(mapSize.y - 1 - position.y) / borderInfluenceY;

        // 找到最小的距离比例（最靠近哪个边界）
        float minDistanceRatio = Mathf.Min(
            distanceToLeft,
            distanceToRight,
            distanceToTop,
            distanceToBottom
        );

        // 计算影响因子（超出边界影响区则为0，否则为1 - 距离比例）
        return Mathf.Clamp01(1 - minDistanceRatio);
    }

    /// <summary>
    /// 连接所有大陆板块
    /// </summary>
    private static void ConnectAllPlates(int[,] mapData, Vector2Int mapSize, List<HashSet<Vector2Int>> plates)
    {
        // 使用最小生成树算法连接所有板块
        List<int> connectedPlates = new List<int> { 0 }; // 从第一个板块开始

        // 8个方向
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        while (connectedPlates.Count < plates.Count)
        {
            // 找到最近的两个板块（一个已连接，一个未连接）
            float minDistance = float.MaxValue;
            Vector2Int closestFrom = Vector2Int.zero;
            Vector2Int closestTo = Vector2Int.zero;
            int plateToConnect = -1;

            foreach (int connectedIdx in connectedPlates)
            {
                for (int i = 0; i < plates.Count; i++)
                {
                    if (connectedPlates.Contains(i))
                        continue;

                    // 找到两个板块间的最近点
                    foreach (var pos1 in plates[connectedIdx])
                    {
                        foreach (var pos2 in plates[i])
                        {
                            float distance = Vector2.Distance(pos1, pos2);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestFrom = pos1;
                                closestTo = pos2;
                                plateToConnect = i;
                            }
                        }
                    }
                }
            }

            if (plateToConnect != -1)
            {
                // 生成陆地连接两个板块
                CreateLandBridge(
                    mapData,
                    mapSize,
                    closestFrom,
                    closestTo,
                    directions,
                    borderInfluenceX,
                    borderInfluenceY
                );
                connectedPlates.Add(plateToConnect);
            }
            else
            {
                // 无法找到连接，退出循环
                break;
            }
        }
    }

    /// <summary>
    /// 创建陆桥连接两个点，同样应用边界概率衰减
    /// </summary>
    private static void CreateLandBridge(
        int[,] mapData,
        Vector2Int mapSize,
        Vector2Int start,
        Vector2Int end,
        Vector2Int[] directions,
        int borderInfluenceX,
        int borderInfluenceY
    )
    {
        // 使用Bresenham算法生成连接线
        List<Vector2Int> bridgePoints = GetLinePoints(start, end);

        // 扩展连接线形成更自然的陆桥
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> bridge = new HashSet<Vector2Int>();

        // 初始化桥的所有点
        foreach (var point in bridgePoints)
        {
            if (point.x >= 0 && point.x < mapSize.x && point.y >= 0 && point.y < mapSize.y)
            {
                mapData[point.x, point.y] = (int)MapData.Continent;
                queue.Enqueue(point);
                bridge.Add(point);
            }
        }

        // 陆桥基础扩展概率
        float baseBridgeExpansion = 0.7f;

        // 扩展桥的宽度，使连接更自然
        int bridgeWidth = Random.Range(3, 6); // 陆桥宽度
        while (queue.Count > 0 && bridge.Count < bridgeWidth * bridgePoints.Count)
        {
            Vector2Int current = queue.Dequeue();

            // 计算当前位置的边界影响因子
            float borderFactor = CalculateBorderFactor(
                current,
                mapSize,
                borderInfluenceX,
                borderInfluenceY
            );

            // 调整陆桥扩展概率（靠近边界时降低）
            float adjustedExpansion = baseBridgeExpansion * (1 - borderFactor);

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (next.x >= 0 && next.x < mapSize.x && next.y >= 0 && next.y < mapSize.y &&
                    !bridge.Contains(next))
                {
                    // 应用调整后的扩展概率
                    if (Random.value < adjustedExpansion)
                    {
                        mapData[next.x, next.y] = (int)MapData.Continent;
                        bridge.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 使用Bresenham算法获取两点之间的线段点
    /// </summary>
    private static List<Vector2Int> GetLinePoints(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> points = new List<Vector2Int>();

        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            points.Add(new Vector2Int(x0, y0));

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }

        return points;
    }

    /// <summary>
    /// 打乱方向数组顺序，增加随机性
    /// </summary>
    private static void ShuffleDirections(Vector2Int[] directions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            int randomIndex = Random.Range(i, directions.Length);
            Vector2Int temp = directions[i];
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 处理被陆地完全包围的海洋区域
    /// </summary>
    private static void ResolveEnclosedOceans(int[,] mapData, Vector2Int mapSize)
    {
        // 先标记所有连通到地图边缘的海洋
        bool[,] isConnectedToEdge = new bool[mapSize.x, mapSize.y];
        Queue<Vector2Int> edgeQueue = new Queue<Vector2Int>();

        // 检查边缘单元格
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // 边缘单元格且是海洋
                if ((x == 0 || x == mapSize.x - 1 || y == 0 || y == mapSize.y - 1) &&
                    mapData[x, y] == (int)MapData.Ocean)
                {
                    isConnectedToEdge[x, y] = true;
                    edgeQueue.Enqueue(new Vector2Int(x, y));
                }
            }
        }

        // 8个方向
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1)
        };

        // 标记所有与边缘连通的海洋
        while (edgeQueue.Count > 0)
        {
            Vector2Int current = edgeQueue.Dequeue();
            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (next.x >= 0 && next.x < mapSize.x && next.y >= 0 && next.y < mapSize.y)
                {
                    if (mapData[next.x, next.y] == (int)MapData.Ocean && !isConnectedToEdge[next.x, next.y])
                    {
                        isConnectedToEdge[next.x, next.y] = true;
                        edgeQueue.Enqueue(next);
                    }
                }
            }
        }

        // 将不与边缘连通的海洋（被包围的）转换为陆地或湖泊
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (mapData[x, y] == (int)MapData.Ocean && !isConnectedToEdge[x, y])
                {
                    mapData[x, y] = (int)MapData.Continent;
                }
            }
        }
    }

    /// <summary>
    /// 生成指定类型岛屿
    /// </summary>
    private static void GenerateIslands(int[,] mapData, MapGeneratorConfig config, List<Vector2Int> oceanPositions, int islandCount, int baseIslandSize, string islandType)
    {
        int createCount = Mathf.RoundToInt(islandCount * Random.Range(0.8f, 1.2f));
        List<Vector2Int> usedPositions = new List<Vector2Int>();

        for (int i = 0; i < createCount; i++)
        {
            // 使用预设格子数的开方来估算岛屿长度
            int isLandLength = Mathf.RoundToInt(Mathf.Sqrt(baseIslandSize) * Random.Range(0.8f, 1.2f));

            Vector2Int pos = SelectIslandPosition(mapData, oceanPositions, usedPositions, config.mapSize, isLandLength);
            if (pos == Vector2Int.zero) break;

            usedPositions.Add(pos);
            GenerateIsland(mapData, config.mapSize, pos.x, pos.y, isLandLength);
        }
    }

    /// <summary>
    /// 选择岛屿生成位置
    /// </summary>
    private static Vector2Int SelectIslandPosition(int[,] mapData, List<Vector2Int> oceanPositions, List<Vector2Int> usedPositions, Vector2Int mapSize, int isLandLength)
    {
        // 打乱海洋位置列表增加随机性
        List<Vector2Int> shuffledPositions = new List<Vector2Int>(oceanPositions);
        for (int i = 0; i < shuffledPositions.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPositions.Count);
            Vector2Int temp = shuffledPositions[i];
            shuffledPositions[i] = shuffledPositions[randomIndex];
            shuffledPositions[randomIndex] = temp;
        }

        foreach (Vector2Int pos in shuffledPositions)
        {
            if (!usedPositions.Contains(pos) && !IsNearLandOrIsland(mapData, pos, mapSize, usedPositions, isLandLength))
                return pos;
        }
        return Vector2Int.zero;
    }

    /// <summary>
    /// 判断位置是否靠近陆地或其他岛屿
    /// </summary>
    private static bool IsNearLandOrIsland(int[,] mapData, Vector2Int pos, Vector2Int mapSize, List<Vector2Int> usedPositions, int checkRange)
    {
        for (int x = pos.x - checkRange; x <= pos.x + checkRange; x++)
        {
            for (int y = pos.y - checkRange; y <= pos.y + checkRange; y++)
            {
                if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y) continue;
                if (mapData[x, y] == (int)MapData.Continent ||
                    mapData[x, y] == (int)MapData.Island ||
                    usedPositions.Contains(new Vector2Int(x, y)))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 生成单个岛屿
    /// </summary>
    private static void GenerateIsland(int[,] mapData, Vector2Int mapSize, int centerX, int centerY, int length)
    {
        //Debug.Log("生成岛屿，长度：" + length);
        // 使用更自然的岛屿形状生成算法
        float falloff = Random.Range(0.7f, 1.0f); // 衰减系数，控制岛屿边缘陡峭程度

        // 生成基础圆形并添加随机扰动
        for (int x = -length; x <= length; x++)
        {
            for (int y = -length; y <= length; y++)
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                float normalizedDistance = distance / length;

                // 添加随机扰动使岛屿形状不规则
                float randomFactor = 1 + Random.Range(-0.3f, 0.3f);
                float adjustedDistance = normalizedDistance * randomFactor;

                if (adjustedDistance < falloff)
                {
                    int worldX = centerX + x;
                    int worldY = centerY + y;

                    if (worldX >= 0 && worldX < mapSize.x && worldY >= 0 && worldY < mapSize.y)
                    {
                        mapData[worldX, worldY] = (int)MapData.Island;
                    }
                }
            }
        }

        // 添加小岛分支，使岛屿更自然
        int branches = Random.Range(1, 4);
        for (int i = 0; i < branches; i++)
        {
            float angle = Random.Range(0, Mathf.PI * 2);
            int branchLength = Random.Range(length / 3, length * 2 / 3);
            int branchWidth = Random.Range(1, 3);

            for (int l = 0; l < branchLength; l++)
            {
                int xOffset = Mathf.RoundToInt(Mathf.Cos(angle) * l);
                int yOffset = Mathf.RoundToInt(Mathf.Sin(angle) * l);

                for (int w = -branchWidth; w <= branchWidth; w++)
                {
                    for (int h = -branchWidth; h <= branchWidth; h++)
                    {
                        int worldX = centerX + xOffset + w;
                        int worldY = centerY + yOffset + h;

                        if (worldX >= 0 && worldX < mapSize.x && worldY >= 0 && worldY < mapSize.y)
                        {
                            // 分支末端逐渐变细
                            if (Random.value > (float)l / branchLength)
                            {
                                mapData[worldX, worldY] = (int)MapData.Island;
                            }
                        }
                    }
                }
            }
        }
    }
}
