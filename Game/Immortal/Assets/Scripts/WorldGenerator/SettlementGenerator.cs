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
    public static void Generate(int[,] mapData, MapGeneratorConfig config, List<Vector2Int> landList, List<Vector2Int> cityList, List<Vector2Int> cityPoints, List<List<Vector2Int>> isLandList)
    {
        List<Vector2Int> canUsePoint = new List<Vector2Int>(landList);
        int locationIndex = 0;

        // 生成宗门
        for (int i = 0; i < config.sectCount; i++)
        {
            int size = Mathf.RoundToInt(config.sectSize * Random.Range(0.8f, 1.2f));
            Vector2Int position = FindSuitablePosition(mapData, config, size, MapData.Sect, locationIndex, canUsePoint, cityPoints, isLandList, config.townSectMinDistance);
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
    /// 寻找适合放置定居点的位置，在陆地上，距离其他不能太近
    /// </summary>
    private static Vector2Int FindSuitablePosition(int[,] mapData, MapGeneratorConfig config, int size, MapData type, int locationCount, List<Vector2Int> canUsePoint, List<Vector2Int> cityPoints, List<List<Vector2Int>> isLandList, int nearDis)
    {
        // 打乱位置列表增加随机性
        List<Vector2Int> shuffledPositions = new List<Vector2Int>(canUsePoint);
        for (int i = 0; i < shuffledPositions.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledPositions.Count);
            Vector2Int temp = shuffledPositions[i];
            shuffledPositions[i] = shuffledPositions[randomIndex];
            shuffledPositions[randomIndex] = temp;
        }

        foreach (var pos in shuffledPositions)
        {
            // 检查距离
            if (IsDistanceSuitable(pos, cityPoints, nearDis))
            {
                continue;
            }

            // 检查是否在岛上
            bool isLand = (mapData[pos.x, pos.y] & (int)(MapData.Island)) != 0;
            if (!isLand)
            {
                // 陆地直接可用
                return pos;
            }

            // 岛屿需要检查区域是否足够大
            if (!IsAreaSuitable(pos, isLandList, size))
            {
                continue;
            }
            return pos;
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// 检查区域是否适合放置定居点
    /// </summary>
    private static bool IsAreaSuitable(Vector2Int pos, List<List<Vector2Int>> isLandList, int size)
    {
        foreach (var isLandPoint in isLandList)
        {
            if (isLandPoint.Contains(isLandPoint))
            {
                return isLandPoint.Count >= size;
            }
        }
        return false;
    }

    /// <summary>
    /// 判断位置是否靠近其他居住点
    /// </summary>
    private static bool IsDistanceSuitable(Vector2Int pos, List<Vector2Int> cityPoints, int nearDis)
    {
        return AreaExpander.IsNearLandOrIsland(pos, cityPoints, nearDis);
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