using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 河流生成工具类
/// </summary>
public static class RiverGenerator
{
    private static System.Random _random = new System.Random();

    /// <summary>
    /// 生成所有河流
    /// </summary>
    public static void Generate(int[,] mapData, List<Vector2Int> landList, List<List<Vector2Int>> islandList)
    {
        Vector2Int mapSize = new Vector2Int(mapData.GetLength(0), mapData.GetLength(1));

        // 计算大陆区域（所有陆地减去岛屿区域）
        HashSet<Vector2Int> allIslands = new HashSet<Vector2Int>();
        foreach (var island in islandList)
        {
            foreach (var pos in island)
            {
                allIslands.Add(pos);
            }
        }

        List<Vector2Int> continentList = landList.FindAll(pos => !allIslands.Contains(pos));

        // 生成3条大型贯穿河流（只在大陆上，避开山脉）
        GenerateMajorRivers(mapData, mapSize, continentList, 3);

        // 生成约30条小型河流（20%随机浮动）
        int minorRiverCount = (int)(30 * (1 + (float)(_random.NextDouble() - 0.5) * 0.4));
        minorRiverCount = Mathf.Clamp(minorRiverCount, 20, 40);
        GenerateMinorRivers(mapData, mapSize, landList, islandList, allIslands, continentList, minorRiverCount);
    }

    /// <summary>
    /// 生成大型贯穿河流（只在大陆上，避开山脉）
    /// </summary>
    private static void GenerateMajorRivers(int[,] mapData, Vector2Int mapSize, List<Vector2Int> continentList, int count)
    {
        if (continentList.Count == 0) return;

        for (int i = 0; i < count; i++)
        {
            // 大型河流使用两个控制点，确保贯穿大陆
            Vector2Int start = GetContinentEdgePoint(continentList, mapSize, mapData);
            Vector2Int end;

            // 确保起点和终点的距离足够长
            do
            {
                end = continentList[_random.Next(0, continentList.Count)];
            } while (start == end || Vector2Int.Distance(start, end) < 40);

            // 生成蜿蜒路径
            int minLength = (int)(Vector2Int.Distance(start, end) * 1.2f);
            int maxLength = (int)(Vector2Int.Distance(start, end) * 1.8f);
            List<Vector2Int> riverPath = GenerateLinearMeanderingPath(
                start, end, mapData, continentList, mapSize, minLength, maxLength, true);

            if (riverPath.Count < 20) continue;

            // 宽度配置
            int baseWidth = _random.Next(2, 4); // 2-3
            int wideSectionWidth = baseWidth + _random.Next(1, 3); // 1-2
            int wideSectionStart = (int)(riverPath.Count * (0.2f + _random.NextDouble() * 0.5));
            int wideSectionLength = Mathf.Max(8, (int)(riverPath.Count * (0.15f + _random.NextDouble() * 0.25)));

            MarkRiverOnMapWithWidthVariation(mapData, riverPath, baseWidth, wideSectionWidth, wideSectionStart, wideSectionLength);
        }
    }

    /// <summary>
    /// 生成小型河流（确保在同一区域：大陆或同一岛屿）
    /// </summary>
    private static void GenerateMinorRivers(int[,] mapData, Vector2Int mapSize, List<Vector2Int> landList,
                                           List<List<Vector2Int>> islandList, HashSet<Vector2Int> allIslands,
                                           List<Vector2Int> continentList, int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 确定河流生成区域：80%大陆，20%岛屿
            bool isOnIsland = _random.NextDouble() < 0.2 && islandList.Count > 0;
            List<Vector2Int> targetLandList = null;
            int targetIslandIndex = -1;

            if (isOnIsland)
            {
                // 选择一个足够大的岛屿
                do
                {
                    targetIslandIndex = _random.Next(islandList.Count);
                    targetLandList = islandList[targetIslandIndex];
                } while (targetLandList.Count < 15); // 确保岛屿足够大以容纳河流
            }
            else
            {
                // 选择大陆
                targetLandList = continentList;
                if (targetLandList.Count < 15) continue; // 大陆太小则跳过
            }

            // 随机选择起点和终点，确保在同一区域
            Vector2Int startPoint = GetRandomLandPoint(targetLandList, mapData);
            Vector2Int endPoint;

            // 确保终点与起点在同一区域且有足够距离
            int attempts = 0;
            do
            {
                attempts++;
                endPoint = GetRandomLandPoint(targetLandList, mapData);
            } while (startPoint == endPoint || (Vector2Int.Distance(startPoint, endPoint) < 5 && attempts < 10));

            // 根据区域大小和两点距离确定河流长度
            int minLength = (int)(Vector2Int.Distance(startPoint, endPoint) * 1.1f);
            int maxLength = (int)(Vector2Int.Distance(startPoint, endPoint) * 1.6f);
            int maxPossibleLength = isOnIsland ? targetLandList.Count * 2 : 80;

            minLength = Mathf.Clamp(minLength, 5, maxPossibleLength);
            maxLength = Mathf.Clamp(maxLength, minLength, maxPossibleLength);

            // 生成两点之间的蜿蜒路径
            List<Vector2Int> path = GenerateLinearMeanderingPath(
                startPoint, endPoint, mapData, targetLandList, mapSize, minLength, maxLength, false);

            if (path.Count < 5) continue;

            // 宽度配置
            int baseWidth = _random.Next(1, 3); // 1-2
            int wideSectionWidth = baseWidth + 1;
            int wideSectionStart = (int)(path.Count * (0.2f + _random.NextDouble() * 0.5));
            int wideSectionLength = Mathf.Max(3, (int)(path.Count * (0.15f + _random.NextDouble() * 0.2)));

            MarkRiverOnMapWithWidthVariation(mapData, path, baseWidth, wideSectionWidth, wideSectionStart, wideSectionLength);
        }
    }

    /// <summary>
    /// 生成线性蜿蜒路径
    /// </summary>
    private static List<Vector2Int> GenerateLinearMeanderingPath(Vector2Int start, Vector2Int end, int[,] mapData,
                                                                List<Vector2Int> landList, Vector2Int mapSize,
                                                                int minLength, int maxLength, bool avoidMountain)
    {
        List<Vector2Int> path = new List<Vector2Int> { start };
        HashSet<Vector2Int> visited = new HashSet<Vector2Int> { start };

        Vector2Int current = start;
        int steps = 0;
        int targetSteps = _random.Next(minLength, maxLength + 1);

        // 主方向向量（归一化）
        Vector2 dirVector = new Vector2(end.x - start.x, end.y - start.y).normalized;
        Vector2Int mainDir = new Vector2Int(
            Mathf.RoundToInt(dirVector.x),
            Mathf.RoundToInt(dirVector.y)
        );

        // 如果主方向为零，随机一个方向
        if (mainDir == Vector2Int.zero)
        {
            mainDir = new Vector2Int(_random.Next(3) - 1, _random.Next(3) - 1);
            if (mainDir == Vector2Int.zero) mainDir = Vector2Int.right;
        }

        while (steps < targetSteps && Vector2Int.Distance(current, end) > 1)
        {
            steps++;
            Vector2Int nextDir = GetNextDirection(current, end, mainDir);

            Vector2Int nextPos = current + nextDir;

            if (IsValidRiverPosition(nextPos, mapSize, mapData, landList, false) &&
                !visited.Contains(nextPos))
            {
                current = nextPos;
                path.Add(current);
                visited.Add(current);
            }
            else
            {
                // 尝试其他方向
                foreach (var dir in GetPossibleDirections(mainDir))
                {
                    nextPos = current + dir;
                    if (IsValidRiverPosition(nextPos, mapSize, mapData, landList, false) &&
                        !visited.Contains(nextPos))
                    {
                        current = nextPos;
                        path.Add(current);
                        visited.Add(current);
                        break;
                    }
                }
            }
        }

        // 确保到达终点附近
        if (Vector2Int.Distance(current, end) > 1)
        {
            // 添加直线到达终点
            Vector2Int temp = current;
            while (Vector2Int.Distance(temp, end) > 1)
            {
                Vector2Int dir = new Vector2Int(
                    end.x > temp.x ? 1 : (end.x < temp.x ? -1 : 0),
                    end.y > temp.y ? 1 : (end.y < temp.y ? -1 : 0)
                );
                temp += dir;
                if (!visited.Contains(temp) && IsValidRiverPosition(temp, mapSize, mapData, landList, false))
                {
                    path.Add(temp);
                    visited.Add(temp);
                }
                else
                {
                    break; // 如果无法到达终点，则停止
                }
            }
        }

        return path;
    }

    /// <summary>
    /// 获取下一个移动方向（带蜿蜒效果）
    /// </summary>
    private static Vector2Int GetNextDirection(Vector2Int current, Vector2Int target, Vector2Int mainDir)
    {
        // 计算到目标的方向
        Vector2Int toTargetDir = new Vector2Int(
            target.x > current.x ? 1 : (target.x < current.x ? -1 : 0),
            target.y > current.y ? 1 : (target.y < current.y ? -1 : 0)
        );

        // 70%概率沿主方向或朝向目标，30%概率随机偏移（蜿蜒效果）
        if (_random.NextDouble() < 0.7)
        {
            return toTargetDir != Vector2Int.zero ? toTargetDir : mainDir;
        }
        else
        {
            // 随机偏移方向，但偏向主方向
            List<Vector2Int> possibleDirs = new List<Vector2Int> { mainDir };

            // 添加垂直于主方向的方向作为可能的偏移
            if (mainDir.x != 0)
            {
                possibleDirs.Add(Vector2Int.up);
                possibleDirs.Add(Vector2Int.down);
                possibleDirs.Add(mainDir); // 增加主方向权重
            }
            else if (mainDir.y != 0)
            {
                possibleDirs.Add(Vector2Int.left);
                possibleDirs.Add(Vector2Int.right);
                possibleDirs.Add(mainDir); // 增加主方向权重
            }

            return possibleDirs[_random.Next(possibleDirs.Count)];
        }
    }

    /// <summary>
    /// 获取可能的移动方向
    /// </summary>
    private static List<Vector2Int> GetPossibleDirections(Vector2Int mainDir)
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        // 移除与主方向相反的方向，避免回流
        directions.Remove(new Vector2Int(-mainDir.x, -mainDir.y));

        // 打乱顺序但保持主方向优先
        Vector2Int temp = directions[0];
        int mainIndex = directions.IndexOf(mainDir);
        if (mainIndex != -1)
        {
            directions[0] = mainDir;
            directions[mainIndex] = temp;
        }

        return directions;
    }

    /// <summary>
    /// 标记河流在地图数据上，包含宽度变化
    /// </summary>
    private static void MarkRiverOnMapWithWidthVariation(int[,] mapData, List<Vector2Int> riverPath,
                                                       int baseWidth, int wideSectionWidth,
                                                       int wideSectionStart, int wideSectionLength)
    {
        int mapWidth = mapData.GetLength(0);
        int mapHeight = mapData.GetLength(1);

        for (int i = 0; i < riverPath.Count; i++)
        {
            var pos = riverPath[i];
            // 确定当前位置的河流宽度
            int currentWidth;

            // 检查是否在宽段范围内
            if (i >= wideSectionStart && i < wideSectionStart + wideSectionLength)
            {
                currentWidth = wideSectionWidth;
            }
            else
            {
                currentWidth = baseWidth;
            }

            // 绘制当前宽度的河流
            for (int w = -currentWidth / 2; w <= currentWidth / 2; w++)
            {
                for (int h = -currentWidth / 2; h <= currentWidth / 2; h++)
                {
                    int x = pos.x + w;
                    int y = pos.y + h;

                    // 检查边界
                    if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                    {
                        // 不在海洋上才标记为河流
                        if ((mapData[x, y] & (int)MapData.Ocean) == 0)
                        {
                            mapData[x, y] |= (int)MapData.River;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 检查位置是否适合作为河流
    /// </summary>
    private static bool IsValidRiverPosition(Vector2Int pos, Vector2Int mapSize, int[,] mapData,
                                            List<Vector2Int> landList, bool allowOcean = false)
    {
        // 检查是否在地图范围内
        if (pos.x < 0 || pos.x >= mapSize.x || pos.y < 0 || pos.y >= mapSize.y)
            return false;

        // 检查是否是目标陆地区域的一部分
        return landList.Contains(pos) || (allowOcean && (mapData[pos.x, pos.y] & (int)MapData.Ocean) != 0);
    }

    /// <summary>
    /// 获取大陆边缘点
    /// </summary>
    private static Vector2Int GetContinentEdgePoint(List<Vector2Int> continentList, Vector2Int mapSize, int[,] mapData)
    {
        // 优先选择大陆边缘点
        List<Vector2Int> edgeCandidates = new List<Vector2Int>();

        foreach (var pos in continentList)
        {
            if (IsEdgePoint(pos, continentList))
            {
                edgeCandidates.Add(pos);
            }
        }

        // 如果有边缘点，从中选择；否则从所有大陆点中选择
        return edgeCandidates.Count > 0
            ? edgeCandidates[_random.Next(edgeCandidates.Count)]
            : GetRandomLandPoint(continentList, mapData);
    }

    /// <summary>
    /// 检查是否为边缘点（至少有一个相邻单元格不是陆地）
    /// </summary>
    private static bool IsEdgePoint(Vector2Int pos, List<Vector2Int> landList)
    {
        Vector2Int[] neighbors = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down,
            Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in neighbors)
        {
            if (!landList.Contains(pos + dir))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 从指定陆地列表中随机获取一个点
    /// </summary>
    private static Vector2Int GetRandomLandPoint(List<Vector2Int> landList, int[,] mapData)
    {
        if (landList.Count == 0)
            return Vector2Int.zero;

        Vector2Int point = landList[_random.Next(landList.Count)];
        return point;
    }
}
