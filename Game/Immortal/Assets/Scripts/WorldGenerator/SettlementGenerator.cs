using UnityEngine;
using System.Collections.Generic;
using System.Drawing;

/// <summary>
/// 定居点生成工具类（宗门、城市、村庄、部落）
/// </summary>
public static class SettlementGenerator
{
    /// <summary>
    /// 生成各类定居点
    /// </summary>
    public static void Generate(int[,] mapData, MapGeneratorConfig config, List<Vector2Int> importantLocations)
    {
        int locationIndex = 0;

        // 生成宗门
        for (int i = 0; i < config.sectCount; i++)
        {
            int adjustedSize = Mathf.RoundToInt(Mathf.Sqrt(config.sectSize) * Random.Range(0.8f, 1.2f));
            Vector2Int position = FindSuitablePosition(mapData, config, adjustedSize, MapData.Sect, importantLocations, locationIndex);
            if (position.x != -1)
            {
                PlaceSettlement(mapData, config, position, adjustedSize, config.sectSize, MapData.Sect);
                locationIndex++;
                importantLocations.Add(position);
            }
        }
        int sectCount = locationIndex;

        // 生成城市
        for (int i = 0; i < config.cityCount; i++)
        {
            int adjustedSize = Mathf.RoundToInt(Mathf.Sqrt(config.citySize) * Random.Range(0.8f, 1.2f));
            Vector2Int position = FindSuitablePosition(mapData, config, adjustedSize, MapData.City, importantLocations, locationIndex);
            if (position.x != -1)
            {
                PlaceSettlement(mapData, config, position, adjustedSize, config.citySize, MapData.City);
                locationIndex++;
                importantLocations.Add(position);
            }
        }
        int cityCount = locationIndex - sectCount;

        // 生成部落
        for (int i = 0; i < config.tribeCount; i++)
        {
            int adjustedSize = Mathf.RoundToInt(Mathf.Sqrt(config.tribeSize) * Random.Range(0.8f, 1.2f));
            Vector2Int position = FindSuitablePosition(mapData, config, adjustedSize, MapData.Tribe, importantLocations, locationIndex);
            if (position.x != -1)
            {
                PlaceSettlement(mapData, config, position, adjustedSize, config.tribeSize, MapData.Tribe);
                locationIndex++;
                importantLocations.Add(position);
            }
        }
        int tribeCount = locationIndex - sectCount - sectCount;

        // 生成村庄
        for (int i = 0; i < config.villageCount; i++)
        {
            int adjustedSize = Mathf.RoundToInt(Mathf.Sqrt(config.villageSize) * Random.Range(0.8f, 1.2f));
            Vector2Int position = FindSuitablePosition(mapData, config, adjustedSize, MapData.Village, importantLocations, locationIndex);
            if (position.x != -1)
            {
                PlaceSettlement(mapData, config, position, adjustedSize, config.villageSize, MapData.Village);
                locationIndex++;
                importantLocations.Add(position);
            }
        }
        int villageCount = locationIndex - sectCount - sectCount - tribeCount;


        var ptherImportantLocations = new List<Vector2Int>(importantLocations);
        MapData[] randTypes = new MapData[] { MapData.Lake, MapData.Mountain, MapData.Forest};
        // 生成生态
        for (int i = 0; i < 300; i++)
        {
            int size = Random.Range(5, 30);
            int adjustedSize = Mathf.RoundToInt(Mathf.Sqrt(size));
            MapData randType = randTypes[Random.Range(0, randTypes.Length)];

            Vector2Int position = FindSuitablePosition(mapData, config, adjustedSize, randType, ptherImportantLocations, locationIndex);
            if (position.x != -1)
            {
                PlaceSettlement(mapData, config, position, adjustedSize, size, randType);
                locationIndex++;
                ptherImportantLocations.Add(position);
            }
        }
        int otherCount = locationIndex - sectCount - sectCount - tribeCount - villageCount;


        Debug.Log($"创建数量：宗门{sectCount}  城市{cityCount}  部落{tribeCount}  村庄：{villageCount}  生态数量{otherCount}");
    }

    /// <summary>
    /// 寻找适合放置定居点的位置
    /// </summary>
    private static Vector2Int FindSuitablePosition(int[,] mapData, MapGeneratorConfig config, int adjustedSize, MapData type, List<Vector2Int> importantLocations, int locationCount)
    {
        int maxAttempts = 100;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            attempts++;
            int x = Random.Range(adjustedSize, config.mapSize.x - adjustedSize);
            int y = Random.Range(adjustedSize, config.mapSize.y - adjustedSize);
            Vector2Int pos = new Vector2Int(x, y);

            // 检查索引合法性
            if (x < 0 || x >= mapData.GetLength(0) || y < 0 || y >= mapData.GetLength(1))
                continue;

            // 检查是否在陆地
            bool isLand = (mapData[x, y] & (int)(MapData.Continent | MapData.Island)) != 0;
            if (!isLand) continue;

            // 检查区域是否足够大
            if (!IsAreaSuitable(mapData, config.mapSize, pos, adjustedSize))
            {
                continue;
            }

            // 检查距离
            if (IsDistanceSuitable(pos, importantLocations, locationCount, type, config))
            {
                return pos;
            }
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// 检查区域是否适合放置定居点
    /// </summary>
    private static bool IsAreaSuitable(int[,] mapData, Vector2Int mapSize, Vector2Int pos, int size)
    {
        for (int i = pos.x - size; i <= pos.x + size; i++)
        {
            for (int j = pos.y - size; j <= pos.y + size; j++)
            {
                if (i < 0 || i >= mapSize.x || j < 0 || j >= mapSize.y)
                    return false;

                if ((mapData[i, j] & (int)(MapData.Continent | MapData.Island)) == 0)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检查距离是否符合要求
    /// </summary>
    private static bool IsDistanceSuitable(Vector2Int pos, List<Vector2Int> locations, int locationCount, MapData type, MapGeneratorConfig config)
    {
        float requiredDistance = (type == MapData.Sect || type == MapData.City)
            ? config.townSectMinDistance
            : config.villageTribeMinDistance;

        for (int i = 0; i < locationCount; i++)
        {
            Vector2Int loc = locations[i];
            if (loc.x == 0 && loc.y == 0) continue;

            if (Vector2.Distance(pos, loc) < requiredDistance)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 放置定居点
    /// </summary>
    private static void PlaceSettlement(int[,] mapData, MapGeneratorConfig config, Vector2Int position, int adjustedSize, int size, MapData type)
    {
        // 使用预设格子数的开方来估算定居点长度
        int curSize = 0;
        for (int x = position.x - adjustedSize; x <= position.x + adjustedSize; x++)
        {
            for (int y = position.y - adjustedSize; y <= position.y + adjustedSize; y++)
            {
                if (x < 0 || x >= config.mapSize.x || y < 0 || y >= config.mapSize.y)
                    continue;

                float distance = Vector2.Distance(new Vector2(x, y), position);
                if (distance <= adjustedSize)
                {
                    mapData[x, y] |= (int)type | (int)MapData.Plain;
                    curSize++;
                    if (curSize >= size)
                        return;
                }
            }
        }
    }
}