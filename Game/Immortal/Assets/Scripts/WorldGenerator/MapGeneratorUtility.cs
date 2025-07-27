using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

/// <summary>
/// 地图生成主入口工具类
/// </summary>
public static class MapGeneratorUtility
{
    /// <summary>
    /// 生成地图的主方法
    /// </summary>
    public static int[,] GenerateMap(MapGeneratorConfig config)
    {
        if (config == null)
        {
            Debug.LogError("地图生成配置不能为空！");
            return null;
        }

        // 记录开始时间
        System.DateTime startTime = System.DateTime.Now;

        // 初始化地图数据
        int[,] mapData = new int[config.mapSize.x, config.mapSize.y];
        List<Vector2Int> importantLocations = new List<Vector2Int>();

        // 生成大陆地形
        OceanContinentGenerator.Generate(mapData, config);

        // 生成定居点和生态群
        SettlementGenerator.Generate(mapData, config, importantLocations);

        // 生成平原特征
        NaturalFeatureGenerator.Generate(mapData, config.mapSize);

        // 生成河流
        RiverGenerator.Generate(mapData, config.mapSize);

        //// 生成道路和航线
        //RoadRouteGenerator.Generate(mapData, config, importantLocations);

        // 计算总耗时
        System.DateTime endTime = System.DateTime.Now;
        System.TimeSpan totalTime = endTime - startTime;
        Debug.Log($"地图生成完成！总耗时: {totalTime.TotalMilliseconds:F2}ms ({totalTime.TotalSeconds:F2}秒)");

        return mapData;
    }

    /// <summary>
    /// 将地图数据保存到txt文件
    /// </summary>
    /// <param name="mapData">地图数据</param>
    /// <param name="mapSize">地图大小</param>
    /// <param name="filePath">文件路径</param>
    public static void SaveMapToFile(int[,] mapData, Vector2Int mapSize, string filePath)
    {
        if (mapData == null)
        {
            Debug.LogError("地图数据为空，无法保存！");
            return;
        }

        try
        {
            // 确保目录存在
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    string line = "";
                    for (int x = 0; x < mapSize.x; x++)
                    {
                        line += mapData[x, y];
                        if (x < mapSize.x - 1)
                        {
                            line += " ";
                        }
                    }
                    writer.WriteLine(line);
                }
            }
            Debug.Log("地图保存成功: " + filePath);
        }
        catch (Exception e)
        {
            Debug.LogError("保存地图失败: " + e.Message);
        }
    }

    /// <summary>
    /// 从txt文件读取地图数据
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="mapSize">输出地图大小</param>
    /// <returns>读取的地图数据</returns>
    public static int[,] LoadMapFromFile(string filePath, out Vector2Int mapSize)
    {
        mapSize = Vector2Int.zero;

        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("文件不存在: " + filePath);
                return null;
            }

            string[] lines = File.ReadAllLines(filePath);
            int width = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
            int height = lines.Length;

            mapSize = new Vector2Int(width, height);
            int[,] mapData = new int[width, height];

            for (int y = 0; y < height; y++)
            {
                string[] values = lines[y].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < width; x++)
                {
                    if (int.TryParse(values[x], out int value))
                    {
                        mapData[x, y] = value;
                    }
                    else
                    {
                        Debug.LogWarning($"解析地图数据失败，位置: ({x}, {y})，值: {values[x]}");
                        mapData[x, y] = (int)MapData.None;
                    }
                }
            }

            Debug.Log("地图加载成功: " + filePath);
            return mapData;
        }
        catch (Exception e)
        {
            Debug.LogError("加载地图失败: " + e.Message);
            return null;
        }
    }
}