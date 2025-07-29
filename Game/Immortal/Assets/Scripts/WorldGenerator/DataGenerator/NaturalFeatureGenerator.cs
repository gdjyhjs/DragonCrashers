using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.STP;

/// <summary>
/// 自然特征生成工具类（平原）
/// </summary>
public static class NaturalFeatureGenerator
{
    /// <summary>
    /// 生成自然特征
    /// </summary>
    public static void Generate(int[,] mapData, List<Vector2Int> landList, List<List<Vector2Int>> isLandList
        , out List<Vector2Int> mountainUsedList, out List<Vector2Int> forestUsedList, out List<Vector2Int> lakeUsedList, out List<Vector2Int> plainUsedList)
    {
        // 生成山脉
        int maxMountainCount = Random.Range(7, 12);
        mountainUsedList = new List<Vector2Int>(); // 山脉占点列表
        List<Vector2Int> mountainCanUseList = new List<Vector2Int>(landList); // 山脉选点可用列表
        int expectMountainGrid = 0; // 预期山脉格子数
        for (int i = 0; i < maxMountainCount; i++)
        {
            int size = Random.Range(96, 1000);
            expectMountainGrid += size;
            int nearDis = Mathf.CeilToInt(Mathf.Sqrt(size));
            Vector2Int center = FindSuitablePosition(mapData, size, MapData.Mountain, mountainCanUseList, mountainUsedList, isLandList, nearDis);
            if (center.x != -1)
            {
                Vector2Int[] dirList;
                int dirType = Random.Range(0, 6); // 随机山脉走向
                switch (dirType)
                {
                    case 0:
                        dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.left, Vector2Int.right, Vector2Int.left, Vector2Int.right, Vector2Int.left, Vector2Int.right, Vector2Int.left, Vector2Int.right };
                        break;
                    case 1:
                        dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.up, Vector2Int.down, Vector2Int.up, Vector2Int.down, Vector2Int.up, Vector2Int.down, Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                        break;
                    case 2:
                        dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.left, Vector2Int.up, Vector2Int.left, Vector2Int.up, Vector2Int.left, Vector2Int.up, Vector2Int.left, Vector2Int.down, Vector2Int.right };
                        break;
                    case 3:
                        dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.up, Vector2Int.right, Vector2Int.up, Vector2Int.right, Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
                        break;
                    case 4:
                        dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.down, Vector2Int.left, Vector2Int.down, Vector2Int.left, Vector2Int.down, Vector2Int.left, Vector2Int.right };
                        break;
                    case 5:
                        dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.down, Vector2Int.right, Vector2Int.down, Vector2Int.right, Vector2Int.down, Vector2Int.right, Vector2Int.left };
                        break;
                    default:
                        dirList = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
                        break;
                }
                PlaceSettlement(mapData, center, dirList, size, MapData.Mountain, mountainCanUseList, mountainUsedList, nearDis);
            }
        }
        // 生成森林
        int maxForestCount = Random.Range(9, 15);
        forestUsedList = new List<Vector2Int>(); // 森林占点列表
        List<Vector2Int> forestCanUseList = new List<Vector2Int>(landList); // 森林选点可用列表
        int expectForestGrid = 0; // 预期森林格子数
        for (int i = 0; i < maxForestCount; i++)
        {
            int size = Random.Range(49, 500);
            expectForestGrid += size;
            int nearDis = Mathf.CeilToInt(Mathf.Sqrt(size));
            Vector2Int center = FindSuitablePosition(mapData, size, MapData.Forest, forestCanUseList, forestUsedList, isLandList, nearDis);
            if (center.x != -1)
            {
                PlaceSettlement(mapData, center, default, size, MapData.Forest, forestCanUseList, forestUsedList, nearDis);
            }
        }

        // 生成湖泊
        int maxLakeCount = Random.Range(13, 17);
        lakeUsedList = new List<Vector2Int>(); // 湖泊占点列表
        List<Vector2Int> lakeCanUseList = new List<Vector2Int>(landList); // 湖泊选点可用列表
        int expectLakeGrid = 0; // 预期湖泊格子数
        for (int i = 0; i < maxLakeCount; i++)
        {
            int size = Random.Range(13, 200);
            expectLakeGrid += size;
            int nearDis = Mathf.CeilToInt(Mathf.Sqrt(size));
            Vector2Int center = FindSuitablePosition(mapData, size, MapData.Lake, lakeCanUseList, lakeUsedList, isLandList, nearDis);
            if (center.x != -1)
            {
                PlaceSettlement(mapData, center, default, size, MapData.Lake, lakeCanUseList, lakeUsedList, nearDis);
            }
        }

        plainUsedList = new List<Vector2Int>();
        foreach (Vector2Int p in landList)
        {
            if (!mountainUsedList.Contains(p) && !forestUsedList.Contains(p) && !lakeUsedList.Contains(p))
            {
                mapData[p.x, p.y] = mapData[p.x, p.y] | (int)MapData.Plain;
                plainUsedList.Add(p);
            }
        }

        Debug.Log($"陆地格子数：{landList.Count}  山脉{mountainUsedList.Count}/{expectMountainGrid}  森林{forestUsedList.Count}/{expectForestGrid}  湖泊{lakeUsedList.Count}/{expectLakeGrid}  平原{plainUsedList.Count}");
    }

    /// <summary>
    /// 寻找适合放置生态点，在陆地上
    /// </summary>
    private static Vector2Int FindSuitablePosition(int[,] mapData, int size, MapData type, List<Vector2Int> canUsePoint, List<Vector2Int> mountainList, List<List<Vector2Int>> isLandList, int nearDis)
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
            if (IsDistanceSuitable(pos, mountainList, nearDis))
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
            if (!IsAreaSuitable(pos, isLandList, size, canUsePoint))
            {
                continue;
            }
            return pos;
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// 判断位置是否靠近其他生态点
    /// </summary>
    private static bool IsDistanceSuitable(Vector2Int pos, List<Vector2Int> otherPoints, int nearDis)
    {
        return AreaExpander.IsNearLandOrIsland(pos, otherPoints, nearDis);
    }

    /// <summary>
    /// 检查区域是否适合放置生态点
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
    /// 放置生态点
    /// </summary>
    private static void PlaceSettlement(int[,] mapData, Vector2Int center, Vector2Int[] dirList, int size, MapData type, List<Vector2Int> canUsePoint, List<Vector2Int> otherPoints, int nearDis)
    {
        var list = AreaExpander.ExpandPointToArea(center, dirList, (pos) =>
        {
            return canUsePoint.Contains(pos) && !IsDistanceSuitable(pos, otherPoints, nearDis);
        }, size);
        foreach (var p in list)
        {
            mapData[p.x, p.y] = mapData[p.x, p.y] | (int)type;
            otherPoints.Add(p);
            canUsePoint.Remove(p);
        }
    }
}