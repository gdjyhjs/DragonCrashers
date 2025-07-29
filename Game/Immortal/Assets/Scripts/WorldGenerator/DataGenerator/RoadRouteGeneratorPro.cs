using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// 道路和航线生成工具类
/// </summary>
public static class RoadRouteGeneratorPro
{
    // 存储已生成的道路点，用于快速查询
    private static HashSet<Vector2Int> _roadPoints = new HashSet<Vector2Int>();
    // 存储所有居住区域点（用于连接道路）
    private static HashSet<Vector2Int> _settlementArea = new HashSet<Vector2Int>();


    /// <summary>
    /// 生成道路和航线
    /// </summary>
    public static void Generate(int[,] mapData, MapGeneratorConfig config,
                              Vector2Int[] originPoints,  // 起源点（不直接连接）
                              Vector2Int[] settlementArea, // 居住区域（实际连接的点）
                              List<Vector2Int> landList,
                              List<List<Vector2Int>> islandList)
    {
        // 初始化数据结构
        _roadPoints.Clear();
        _settlementArea.Clear();
        foreach (var pos in settlementArea)
            _settlementArea.Add(pos);

        // 过滤掉起源点，只保留需要连接的居住区域点
        HashSet<Vector2Int> originSet = new HashSet<Vector2Int>(originPoints);
        List<Vector2Int> validSettlements = settlementArea.Where(pos => !originSet.Contains(pos)).ToList();

        // 分类居住点（按类型）
        var (majorSettlements, minorSettlements) = ClassifySettlements(mapData, validSettlements);

        // 生成主道路网络（城市、宗门，2格宽）
        GenerateRoadNetwork(mapData, config.mapSize, majorSettlements, 2);

        // 生成次要道路（村庄、部落，1格宽）
        GenerateRoadNetwork(mapData, config.mapSize, minorSettlements, 1);

        // 生成岛屿码头和航线
        GenerateIslandConnections(mapData, config.mapSize, validSettlements, landList, islandList);
    }


    /// <summary>
    /// 分类居住点（主/次要）
    /// </summary>
    private static (List<Vector2Int> major, List<Vector2Int> minor)
        ClassifySettlements(int[,] mapData, List<Vector2Int> settlements)
    {
        List<Vector2Int> major = new List<Vector2Int>();
        List<Vector2Int> minor = new List<Vector2Int>();

        foreach (var pos in settlements)
        {
            int cellData = mapData[pos.x, pos.y];
            if ((cellData & (int)(MapData.City | MapData.Sect)) != 0)
                major.Add(pos);
            else if ((cellData & (int)(MapData.Village | MapData.Tribe)) != 0)
                minor.Add(pos);
        }

        return (major, minor);
    }


    /// <summary>
    /// 生成道路网络（支持不同宽度，自动连接到现有网络）
    /// </summary>
    private static void GenerateRoadNetwork(int[,] mapData, Vector2Int mapSize,
                                          List<Vector2Int> settlements, int width)
    {
        if (settlements.Count == 0) return;

        // 已连接到网络的点
        HashSet<Vector2Int> connected = new HashSet<Vector2Int>();
        // 优先连接第一个点作为网络起点
        connected.Add(settlements[0]);

        // 逐步扩展网络
        while (connected.Count < settlements.Count)
        {
            // 找到最近的未连接点和已连接点
            (Vector2Int closestUnconnected, Vector2Int closestConnected) =
                FindClosestPair(settlements, connected);

            if (closestUnconnected == Vector2Int.zero) break;

            // 生成两点间的道路
            GenerateRoadSegment(mapData, mapSize, closestConnected, closestUnconnected, width);
            connected.Add(closestUnconnected);
        }

        // 确保孤立点连接到网络（未被包含在上述步骤中的点）
        foreach (var settlement in settlements)
        {
            if (!connected.Contains(settlement))
            {
                var nearestRoad = FindNearestRoadOrSettlement(settlement);
                if (nearestRoad != Vector2Int.zero)
                    GenerateRoadSegment(mapData, mapSize, settlement, nearestRoad, width);
            }
        }
    }


    /// <summary>
    /// 生成两点间的道路段
    /// </summary>
    private static void GenerateRoadSegment(int[,] mapData, Vector2Int mapSize,
                                          Vector2Int start, Vector2Int end, int width)
    {
        // 使用bresenham算法生成直线路径
        var path = BresenhamLine(start, end);

        foreach (var pos in path)
        {
            // 标记道路并记录到道路网络
            for (int w = -width / 2; w <= width / 2; w++)
            {
                for (int h = -width / 2; h <= width / 2; h++)
                {
                    int x = pos.x + w;
                    int y = pos.y + h;

                    if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y)
                    {
                        // 河流上的道路标记为桥梁
                        if ((mapData[x, y] & (int)MapData.River) != 0)
                            mapData[x, y] |= (int)MapData.Bridge;
                        // 标记道路
                        mapData[x, y] |= (int)MapData.Road;
                        // 添加到道路网络
                        _roadPoints.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
    }


    /// <summary>
    /// 生成岛屿连接（码头和航线）
    /// </summary>
    private static void GenerateIslandConnections(int[,] mapData, Vector2Int mapSize,
                                                List<Vector2Int> settlements,
                                                List<Vector2Int> landList,
                                                List<List<Vector2Int>> islandList)
    {
        // 1. 收集岛屿上的居住点和大陆居住点
        var (islandSettlements, continentSettlements) = SeparateIslandAndContinent(settlements, islandList);
        if (islandSettlements.Count == 0) return;

        // 2. 为岛屿生成码头并连接到内部道路
        Dictionary<Vector2Int, Vector2Int> islandDocks = new Dictionary<Vector2Int, Vector2Int>(); // 岛屿居住点 -> 码头
        foreach (var island in islandList)
        {
            var islandArea = new HashSet<Vector2Int>(island);
            // 岛屿上的居住点
            var settlementsOnIsland = islandSettlements.Where(pos => islandArea.Contains(pos)).ToList();
            if (settlementsOnIsland.Count == 0) continue;

            // 生成码头（岛屿边缘靠海位置）
            var dock = FindIslandDockPosition(mapData, mapSize, island, islandArea);
            if (dock == Vector2Int.zero) continue;

            // 标记码头
            mapData[dock.x, dock.y] |= (int)MapData.Dock;
            // 码头连接到岛屿内部道路/居住点
            var nearestSettlement = FindNearestInList(settlementsOnIsland, dock);
            GenerateRoadSegment(mapData, mapSize, dock, nearestSettlement, 1);

            // 记录岛屿码头（去重）
            if (!islandDocks.ContainsValue(dock))
            {
                foreach (var s in settlementsOnIsland)
                    islandDocks[s] = dock;
            }
        }

        // 3. 为大陆生成码头并连接到大陆道路
        var continentArea = new HashSet<Vector2Int>(landList);
        foreach (var islandDock in islandDocks.Values.Distinct())
        {
            // 找到最近的大陆位置
            var nearestContinentPos = FindNearestContinentPosition(islandDock, continentArea, mapData);
            if (nearestContinentPos == Vector2Int.zero) continue;

            // 生成大陆码头
            var continentDock = FindContinentDockPosition(mapData, mapSize, nearestContinentPos, continentArea);
            if (continentDock == Vector2Int.zero) continue;

            // 标记码头并连接到大陆道路
            mapData[continentDock.x, continentDock.y] |= (int)MapData.Dock;
            var nearestContinentSettlement = FindNearestInList(continentSettlements, continentDock);
            if (nearestContinentSettlement != Vector2Int.zero)
                GenerateRoadSegment(mapData, mapSize, continentDock, nearestContinentSettlement, 1);

            // 生成岛屿-大陆航线
            GenerateRoute(mapData, mapSize, islandDock, continentDock);
        }
    }


    /// <summary>
    /// 生成航线（连接两个码头）
    /// </summary>
    private static void GenerateRoute(int[,] mapData, Vector2Int mapSize, Vector2Int startDock, Vector2Int endDock)
    {
        var path = BresenhamLine(startDock, endDock);
        foreach (var pos in path)
        {
            if (pos.x < 0 || pos.x >= mapSize.x || pos.y < 0 || pos.y >= mapSize.y) continue;
            // 只在海洋上标记航线
            if ((mapData[pos.x, pos.y] & (int)MapData.Ocean) != 0)
                mapData[pos.x, pos.y] |= (int)MapData.Route;
        }
    }


    // ------------------------------ 辅助方法 ------------------------------
    /// <summary>
    /// 分离岛屿和大陆上的居住点
    /// </summary>
    /// <param name="settlements">所有居住点集合</param>
    /// <param name="islandList">岛屿的点集合列表，每个元素代表一个岛屿的所有点</param>
    /// <returns>返回元组，第一个元素是岛屿上的居住点列表，第二个元素是大陆上的居住点列表</returns>
    private static (List<Vector2Int> islandSettlements, List<Vector2Int> continentSettlements)
        SeparateIslandAndContinent(List<Vector2Int> settlements, List<List<Vector2Int>> islandList)
    {
        List<Vector2Int> islandSettlements = new List<Vector2Int>();
        List<Vector2Int> continentSettlements = new List<Vector2Int>();
        var allIslandPoints = new HashSet<Vector2Int>();

        // 先收集所有岛屿的点，存入哈希集合，方便后续快速判断
        foreach (var oneIsland in islandList)
        {
            foreach (var point in oneIsland)
            {
                allIslandPoints.Add(point);
            }
        }

        // 遍历居住点，判断属于岛屿还是大陆
        foreach (var pos in settlements)
        {
            if (allIslandPoints.Contains(pos))
            {
                islandSettlements.Add(pos);
            }
            else
            {
                continentSettlements.Add(pos);
            }
        }

        return (islandSettlements, continentSettlements);
    }

    /// <summary>
    /// 寻找岛屿上的码头位置（边缘靠海）
    /// </summary>
    private static Vector2Int FindIslandDockPosition(int[,] mapData, Vector2Int mapSize,
                                                  List<Vector2Int> island, HashSet<Vector2Int> islandArea)
    {
        // 优先检查岛屿边缘点
        foreach (var pos in island)
        {
            // 检查是否靠海（相邻格为海洋）
            if (IsAdjacentToOcean(pos, mapData, mapSize))
                return pos;
        }
        // 如果没找到，从岛屿中心向外搜索
        var center = GetCenterPoint(island);
        for (int r = 1; r <= 20; r++) // 最大搜索半径20
        {
            foreach (var pos in GetCirclePoints(center, r))
            {
                if (islandArea.Contains(pos) && IsAdjacentToOcean(pos, mapData, mapSize))
                    return pos;
            }
        }
        return Vector2Int.zero;
    }

    /// <summary>
    /// 寻找大陆码头位置（靠近岛屿且靠海）
    /// </summary>
    private static Vector2Int FindContinentDockPosition(int[,] mapData, Vector2Int mapSize,
                                                     Vector2Int nearPos, HashSet<Vector2Int> continentArea)
    {
        // 从最近大陆位置向外搜索靠海点
        for (int r = 0; r <= 10; r++)
        {
            foreach (var pos in GetCirclePoints(nearPos, r))
            {
                if (continentArea.Contains(pos) && IsAdjacentToOcean(pos, mapData, mapSize))
                    return pos;
            }
        }
        return Vector2Int.zero;
    }

    /// <summary>
    /// 找到最近的大陆位置
    /// </summary>
    private static Vector2Int FindNearestContinentPosition(Vector2Int from, HashSet<Vector2Int> continentArea, int[,] mapData)
    {
        return FindNearestInArea(from, continentArea);
    }

    /// <summary>
    /// 检查点是否相邻海洋
    /// </summary>
    private static bool IsAdjacentToOcean(Vector2Int pos, int[,] mapData, Vector2Int mapSize)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (var dir in dirs)
        {
            var adjPos = pos + dir;
            if (adjPos.x >= 0 && adjPos.x < mapSize.x && adjPos.y >= 0 && adjPos.y < mapSize.y)
            {
                if ((mapData[adjPos.x, adjPos.y] & (int)MapData.Ocean) != 0)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 寻找最近的道路或居住点
    /// </summary>
    private static Vector2Int FindNearestRoadOrSettlement(Vector2Int from)
    {
        // 先找最近的道路
        Vector2Int nearestRoad = FindNearestInSet(_roadPoints, from);
        if (nearestRoad != Vector2Int.zero)
            return nearestRoad;
        // 再找最近的居住点
        return FindNearestInSet(_settlementArea, from);
    }

    /// <summary>
    /// 生成两点间的Bresenham直线路径
    /// </summary>
    private static List<Vector2Int> BresenhamLine(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            path.Add(new Vector2Int(x0, y0));
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
        return path;
    }


    // ------------------------------ 通用工具方法 ------------------------------

    /// <summary>
    /// 找到列表中离目标最近的点
    /// </summary>
    private static Vector2Int FindNearestInList(List<Vector2Int> list, Vector2Int target)
    {
        if (list.Count == 0) return Vector2Int.zero;
        return list.OrderBy(p => Vector2.Distance(p, target)).First();
    }

    /// <summary>
    /// 找到集合中离目标最近的点
    /// </summary>
    private static Vector2Int FindNearestInSet(HashSet<Vector2Int> set, Vector2Int target)
    {
        if (set.Count == 0) return Vector2Int.zero;
        return set.OrderBy(p => Vector2.Distance(p, target)).First();
    }

    /// <summary>
    /// 找到区域中离目标最近的点
    /// </summary>
    private static Vector2Int FindNearestInArea(Vector2Int target, HashSet<Vector2Int> area)
    {
        if (area.Count == 0) return Vector2Int.zero;
        return area.OrderBy(p => Vector2.Distance(p, target)).First();
    }

    /// <summary>
    /// 找到两个点集之间最近的一对点
    /// </summary>
    private static (Vector2Int a, Vector2Int b) FindClosestPair(List<Vector2Int> allPoints, HashSet<Vector2Int> connectedPoints)
    {
        Vector2Int closestUnconnected = Vector2Int.zero;
        Vector2Int closestConnected = Vector2Int.zero;
        float minDist = float.MaxValue;

        foreach (var unconnected in allPoints.Where(p => !connectedPoints.Contains(p)))
        {
            foreach (var connected in connectedPoints)
            {
                float dist = Vector2.Distance(unconnected, connected);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestUnconnected = unconnected;
                    closestConnected = connected;
                }
            }
        }
        return (closestUnconnected, closestConnected);
    }

    /// <summary>
    /// 获取点集的中心点
    /// </summary>
    private static Vector2Int GetCenterPoint(List<Vector2Int> points)
    {
        int avgX = (int)points.Average(p => p.x);
        int avgY = (int)points.Average(p => p.y);
        return new Vector2Int(avgX, avgY);
    }

    /// <summary>
    /// 获取圆形范围内的点（用于搜索）
    /// </summary>
    private static IEnumerable<Vector2Int> GetCirclePoints(Vector2Int center, int radius)
    {
        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                if (Vector2.Distance(new Vector2(x, y), center) <= radius)
                    yield return new Vector2Int(x, y);
            }
        }
    }
}
