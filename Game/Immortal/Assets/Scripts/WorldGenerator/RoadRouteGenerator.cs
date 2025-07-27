using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 道路和航线生成工具类
/// </summary>
public static class RoadRouteGenerator
{
    /// <summary>
    /// 生成道路和航线
    /// </summary>
    public static void Generate(int[,] mapData, MapGeneratorConfig config, Vector2Int[] importantLocations)
    {
        // 生成主道路（连接城市和宗门）
        List<Vector2Int> majorLocations = GetMajorLocations(mapData, importantLocations);
        GenerateMainRoads(mapData, config.mapSize, majorLocations);

        // 生成小道路（连接村庄、部落到主道路）
        List<Vector2Int> minorLocations = GetMinorLocations(mapData, importantLocations);
        GenerateMinorRoads(mapData, config.mapSize, minorLocations, majorLocations);

        // 生成码头和航线（岛屿连接）
        GenerateDocksAndRoutes(mapData, config.mapSize, importantLocations);
    }

    /// <summary>
    /// 获取主要地点（城市、宗门）
    /// </summary>
    private static List<Vector2Int> GetMajorLocations(int[,] mapData, Vector2Int[] importantLocations)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int loc in importantLocations)
        {
            if (loc.x == -1 || (loc.x == 0 && loc.y == 0)) continue;

            int cell = mapData[loc.x, loc.y];
            if ((cell & (int)(MapData.City | MapData.Sect)) != 0)
                result.Add(loc);
        }
        return result;
    }

    /// <summary>
    /// 获取次要地点（村庄、部落）
    /// </summary>
    private static List<Vector2Int> GetMinorLocations(int[,] mapData, Vector2Int[] importantLocations)
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (Vector2Int loc in importantLocations)
        {
            if (loc.x == -1 || (loc.x == 0 && loc.y == 0)) continue;

            int cell = mapData[loc.x, loc.y];
            if ((cell & (int)(MapData.Village | MapData.Tribe)) != 0)
                result.Add(loc);
        }
        return result;
    }

    /// <summary>
    /// 生成主道路（最小生成树算法）
    /// </summary>
    private static void GenerateMainRoads(int[,] mapData, Vector2Int mapSize, List<Vector2Int> majorLocations)
    {
        if (majorLocations.Count <= 0) return;

        List<Vector2Int> connected = new List<Vector2Int> { majorLocations[0] };

        while (connected.Count < majorLocations.Count)
        {
            float minDist = float.MaxValue;
            Vector2Int a = Vector2Int.zero;
            Vector2Int b = Vector2Int.zero;

            foreach (Vector2Int locA in connected)
            {
                foreach (Vector2Int locB in majorLocations)
                {
                    if (!connected.Contains(locB))
                    {
                        float dist = Vector2.Distance(locA, locB);
                        if (dist < minDist)
                        {
                            minDist = dist;
                            a = locA;
                            b = locB;
                        }
                    }
                }
            }

            GenerateRoad(mapData, mapSize, a, b, 2); // 2-3格宽
            connected.Add(b);
        }
    }

    /// <summary>
    /// 生成小道路
    /// </summary>
    private static void GenerateMinorRoads(int[,] mapData, Vector2Int mapSize, List<Vector2Int> minorLocations, List<Vector2Int> majorLocations)
    {
        foreach (Vector2Int loc in minorLocations)
        {
            Vector2Int target = FindNearestRoadOrLocation(mapData, mapSize, loc, majorLocations);
            if (target != Vector2Int.zero)
                GenerateRoad(mapData, mapSize, loc, target, 1); // 1格宽
        }
    }

    /// <summary>
    /// 寻找最近的道路或主要地点
    /// </summary>
    private static Vector2Int FindNearestRoadOrLocation(int[,] mapData, Vector2Int mapSize, Vector2Int loc, List<Vector2Int> majorLocations)
    {
        float minDist = float.MaxValue;
        Vector2Int target = Vector2Int.zero;

        // 检查主要地点
        foreach (Vector2Int majorLoc in majorLocations)
        {
            float dist = Vector2.Distance(loc, majorLoc);
            if (dist < minDist)
            {
                minDist = dist;
                target = majorLoc;
            }
        }

        // 检查现有道路
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if ((mapData[x, y] & (int)MapData.Road) != 0)
                {
                    float dist = Vector2.Distance(loc, new Vector2Int(x, y));
                    if (dist < minDist)
                    {
                        minDist = dist;
                        target = new Vector2Int(x, y);
                    }
                }
            }
        }

        return target;
    }

    /// <summary>
    /// 生成道路
    /// </summary>
    private static void GenerateRoad(int[,] mapData, Vector2Int mapSize, Vector2Int start, Vector2Int end, int width)
    {
        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            // 绘制道路
            for (int w = -width / 2; w <= width / 2; w++)
            {
                for (int h = -width / 2; h <= width / 2; h++)
                {
                    int x = x0 + w;
                    int y = y0 + h;

                    if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y)
                    {
                        mapData[x, y] |= (int)MapData.Road;
                        if ((mapData[x, y] & (int)MapData.River) != 0)
                            mapData[x, y] |= (int)MapData.Bridge;
                    }
                }
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }

    /// <summary>
    /// 生成码头和航线
    /// </summary>
    private static void GenerateDocksAndRoutes(int[,] mapData, Vector2Int mapSize, Vector2Int[] importantLocations)
    {
        List<Vector2Int> islandDocks = new List<Vector2Int>();
        List<Vector2Int> continentDocks = new List<Vector2Int>();

        // 为岛屿定居点创建码头
        foreach (Vector2Int loc in importantLocations)
        {
            if (loc.x == -1 || (loc.x == 0 && loc.y == 0)) continue;

            bool isOnIsland = (mapData[loc.x, loc.y] & (int)MapData.Island) != 0;
            if (isOnIsland && !HasRoadAccess(mapData, mapSize, loc))
            {
                Vector2Int dockPos = FindEdgePosition(mapData, mapSize, loc, MapData.Island);
                if (dockPos.x != -1)
                {
                    mapData[dockPos.x, dockPos.y] |= (int)MapData.Dock;
                    islandDocks.Add(dockPos);
                    GenerateRoad(mapData, mapSize, loc, dockPos, 1);
                }
            }
        }

        // 连接岛屿码头和大陆码头
        foreach (Vector2Int islandDock in islandDocks)
        {
            Vector2Int nearestContinent = FindNearestContinentPoint(mapData, mapSize, islandDock);
            if (nearestContinent.x == -1) continue;

            Vector2Int continentDock = FindEdgePosition(mapData, mapSize, nearestContinent, MapData.Continent);
            if (continentDock.x == -1) continue;

            mapData[continentDock.x, continentDock.y] |= (int)MapData.Dock;
            continentDocks.Add(continentDock);
            ConnectDockToRoadNetwork(mapData, mapSize, continentDock, importantLocations);
            GenerateRoute(mapData, mapSize, islandDock, continentDock);
        }
    }

    /// <summary>
    /// 检查是否有道路连接
    /// </summary>
    private static bool HasRoadAccess(int[,] mapData, Vector2Int mapSize, Vector2Int loc)
    {
        for (int x = loc.x - 5; x <= loc.x + 5; x++)
        {
            for (int y = loc.y - 5; y <= loc.y + 5; y++)
            {
                if (x >= 0 && x < mapSize.x && y >= 0 && y < mapSize.y)
                    if ((mapData[x, y] & (int)MapData.Road) != 0)
                        return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 找到地形边缘位置
    /// </summary>
    private static Vector2Int FindEdgePosition(int[,] mapData, Vector2Int mapSize, Vector2Int center, MapData terrainType)
    {
        int searchRadius = 10;
        for (int r = 1; r <= searchRadius; r++)
        {
            for (int x = center.x - r; x <= center.x + r; x++)
            {
                for (int y = center.y - r; y <= center.y + r; y++)
                {
                    if (x < 0 || x >= mapSize.x || y < 0 || y >= mapSize.y) continue;
                    if ((mapData[x, y] & (int)terrainType) == 0) continue;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx < 0 || nx >= mapSize.x || ny < 0 || ny >= mapSize.y) continue;

                            if ((mapData[nx, ny] & (int)MapData.Ocean) != 0)
                                return new Vector2Int(x, y);
                        }
                    }
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// 找到最近的大陆点
    /// </summary>
    private static Vector2Int FindNearestContinentPoint(int[,] mapData, Vector2Int mapSize, Vector2Int point)
    {
        int searchRadius = 20;
        float minDist = float.MaxValue;
        Vector2Int nearest = new Vector2Int(-1, -1);

        for (int x = Mathf.Max(0, point.x - searchRadius); x <= Mathf.Min(mapSize.x - 1, point.x + searchRadius); x++)
        {
            for (int y = Mathf.Max(0, point.y - searchRadius); y <= Mathf.Min(mapSize.y - 1, point.y + searchRadius); y++)
            {
                if ((mapData[x, y] & (int)MapData.Continent) != 0)
                {
                    float dist = Vector2.Distance(point, new Vector2Int(x, y));
                    if (dist < minDist)
                    {
                        minDist = dist;
                        nearest = new Vector2Int(x, y);
                    }
                }
            }
        }
        return nearest;
    }

    /// <summary>
    /// 将码头连接到道路网络
    /// </summary>
    private static void ConnectDockToRoadNetwork(int[,] mapData, Vector2Int mapSize, Vector2Int dockPos, Vector2Int[] importantLocations)
    {
        Vector2Int target = Vector2Int.zero;
        float minDist = float.MaxValue;

        foreach (Vector2Int loc in importantLocations)
        {
            if (loc.x == -1 || (loc.x == 0 && loc.y == 0)) continue;

            float dist = Vector2.Distance(dockPos, loc);
            if (dist < minDist)
            {
                minDist = dist;
                target = loc;
            }
        }

        if (target != Vector2Int.zero)
            GenerateRoad(mapData, mapSize, dockPos, target, 1);
    }

    /// <summary>
    /// 生成航线
    /// </summary>
    private static void GenerateRoute(int[,] mapData, Vector2Int mapSize, Vector2Int start, Vector2Int end)
    {
        int x0 = start.x, y0 = start.y;
        int x1 = end.x, y1 = end.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            if ((mapData[x0, y0] & (int)MapData.Ocean) != 0)
                mapData[x0, y0] |= (int)MapData.Route;

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
}