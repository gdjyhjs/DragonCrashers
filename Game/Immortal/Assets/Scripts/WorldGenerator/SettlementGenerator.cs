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
    public static void Generate(int[,] mapData, MapGeneratorConfig config, List<Vector2Int> landList, List<Vector2Int> cityList, List<Vector2Int> cityPoints, List<List<Vector2Int>> isLandList, List<Vector2Int> canUsePoint)
    {
        int locationIndex = 0;

        // 生成宗门
        for (int i = 0; i < config.sectCount; i++)
        {
            int size = Mathf.RoundToInt(config.sectSize * Random.Range(0.8f, 1.2f));
            Vector2Int center = FindSuitablePosition(mapData, size, MapData.Sect, locationIndex, canUsePoint, cityPoints, isLandList, config.townSectMinDistance);
            if (center.x != -1)
            {
                PlaceSettlement(mapData, center, size, MapData.Sect, canUsePoint, cityList, cityPoints, config.townSectMinDistance);
                locationIndex++;
            }
        }
        int sectCount = locationIndex;

        // 生成城市
        for (int i = 0; i < config.cityCount; i++)
        {
            int size = Mathf.RoundToInt(config.citySize * Random.Range(0.8f, 1.2f));
            Vector2Int center = FindSuitablePosition(mapData, size, MapData.City, locationIndex, canUsePoint, cityPoints, isLandList, config.townSectMinDistance);
            if (center.x != -1)
            {
                PlaceSettlement(mapData, center, size, MapData.City, canUsePoint, cityList, cityPoints, config.townSectMinDistance);
                locationIndex++;
            }
        }
        int cityCount = locationIndex - sectCount;

        // 生成部落
        for (int i = 0; i < config.tribeCount; i++)
        {
            int size = Mathf.RoundToInt(config.tribeSize * Random.Range(0.8f, 1.2f));
            Vector2Int center = FindSuitablePosition(mapData, size, MapData.Tribe, locationIndex, canUsePoint, cityPoints, isLandList, config.villageTribeMinDistance);
            if (center.x != -1)
            {
                PlaceSettlement(mapData, center, size, MapData.Tribe, canUsePoint, cityList, cityPoints, config.villageTribeMinDistance);
                locationIndex++;
            }
        }
        int tribeCount = locationIndex - sectCount - cityCount;

        // 生成村庄
        for (int i = 0; i < config.villageCount; i++)
        {
            int size = Mathf.RoundToInt(config.villageSize * Random.Range(0.8f, 1.2f));
            Vector2Int center = FindSuitablePosition(mapData, size, MapData.Village, locationIndex, canUsePoint, cityPoints, isLandList, config.villageTribeMinDistance);
            if (center.x != -1)
            {
                PlaceSettlement(mapData, center, size, MapData.Village, canUsePoint, cityList, cityPoints, config.villageTribeMinDistance);
                locationIndex++;
            }
        }
        int villageCount = locationIndex - sectCount - cityCount - tribeCount;


        Debug.Log($"创建数量：宗门{sectCount}  城市{cityCount}  部落{tribeCount}  村庄：{villageCount}");
    }

    /// <summary>
    /// 寻找适合放置定居点的位置，在陆地上，距离其他不能太近
    /// </summary>
    private static Vector2Int FindSuitablePosition(int[,] mapData, int size, MapData type, int locationCount, List<Vector2Int> canUsePoint, List<Vector2Int> cityPoints, List<List<Vector2Int>> isLandList, int nearDis)
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

            if (type == MapData.City || type == MapData.Sect)
            {
                // 宗门和城市不在岛屿上
                continue;
            }

            // 岛屿需要检查区域是否足够大
            if (!IsAreaSuitable(pos, isLandList, size, canUsePoint))
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
    private static bool IsAreaSuitable(Vector2Int pos, List<List<Vector2Int>> isLandList, int size, List<Vector2Int> canUsePoint)
    {
        foreach (var isLandPoint in isLandList)
        {
            if (isLandPoint.Contains(pos))
            {
                int canUseCount = 0;
                foreach (var p in isLandPoint)
                {
                    if (canUsePoint.Contains(p))
                        canUseCount++;
                }
                return canUseCount >= size;
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
    private static void PlaceSettlement(int[,] mapData, Vector2Int center, int size, MapData type, List<Vector2Int> canUsePoint, List<Vector2Int> cityList, List<Vector2Int> cityPoints, int nearDis)
    {
        var list = AreaExpander.ExpandPointToArea(center, default, (pos) =>
        {
            return canUsePoint.Contains(pos) && !IsDistanceSuitable(pos, cityPoints, nearDis);
        }, size);
        cityList.Add(center);
        foreach (var p in list)
        {
            mapData[p.x, p.y] = mapData[p.x, p.y] |(int)type;
            cityPoints.Add(p);
            canUsePoint.Remove(p);
        }
    }
}