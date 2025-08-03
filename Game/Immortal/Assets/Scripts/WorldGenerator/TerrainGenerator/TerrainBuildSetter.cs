using System;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuildData
{

}

internal class TerrainBuildSetter
{
    private TerrainData _terrainData;
    private int[,] _mapData;
    private TerrainBuildData[] buildsData;

    private Transform root;

    public TerrainBuildSetter(TerrainData terrainData, int[,] mapData, TerrainBuildData[] buildsData)
    {
        this._terrainData = terrainData;
        this._mapData = mapData;
        this.buildsData = buildsData;
    }

    internal void SetBuilds(Transform root)
    {
        this.root = root;

        Build();
    }

    void Build()
    {
        HashSet<Vector2Int> usedPoint = new HashSet<Vector2Int>(); // 已经使用的点


        int mapWidth = _mapData.GetLength(0);
        int mapHeight = _mapData.GetLength(1);

        // 先创建所有宗门
        for (int mapX = 0; mapX < mapWidth; mapX++)
        {
            for (int mapY = 0; mapY < mapHeight; mapY++)
            {
               if (true) // 是Sect并且格子不在usedPoint中
                {
                    // 遍历格子，找到地形为Sect的格子，然后获取周围（上下左右斜角）所有相连的Sect格子，添加到一个集合，然后调用 CreateSect 创建城市的建筑
                    HashSet<Vector2Int> SectArea;
                }
            }
        }

        // 创建宗门的附属城市、部落（世家）、村庄
        for (int mapX = 0; mapX < mapWidth; mapX++)
        {
            for (int mapY = 0; mapY < mapHeight; mapY++)
            {
                if (true) // 是city并且格子不在usedPoint中
                {
                    // 遍历格子，找到地形为City的格子，然后获取周围（上下左右斜角）所有相连的City格子，添加到一个集合，然后调用 CreateCity 创建城市的建筑
                    HashSet<Vector2Int> cityArea;


                }
                else if (true) // 是Village
                {
                }
                else if (true) // 是Tribe
                {
                }
            }
        }
    }

    /// <summary>
    /// 然后获取周围（上下左右斜角）所有相连的指定类型格子
    /// </summary>
    void 找到所有相连的同类型格子(Vector2Int point, HashSet<Vector2Int> typeArea, MapData gridType)
    {

    }

    /// <summary>
    /// 生成宗门地标、建筑和装饰
    /// </summary>
    void BuildSect(HashSet<Vector2Int> area)
    {
        // 从配置中获取一个未使用的宗门配置

        // 每种生成的建筑占一个格子，建筑与建筑之间至少距离一个格子。

        // 找到一块比较中心的格子，生成一个聚集点广场。

        // 根据配置将所需的必要建筑生成到范围内随机格子上

        // 为所有生成建筑指定所属宗门
    }

    /// <summary>
    /// 生成城市地标、建筑和装饰
    /// </summary>
    void BuildCity(HashSet<Vector2Int> area)
    {
        // 找到一块比较中心的格子，生成一个聚集点广场。

        // 获取所有宗门按与中心格子距离进行排序

        // 从配置中获取一个未使用的城市配置（优先选择最近宗门的附属城市，没有则取第二近的，以此类推）

        // 每种生成的建筑占一个格子，建筑与建筑之间至少距离一个格子。

        // 根据配置将所需的必要建筑生成到范围内随机格子上

        // 根据剩余空地数量随机生成民居，循环（area.Count/10）次随机位置并判断是否是否符合创建条件（如是否距离其他建筑过近）。如何条件则创建民居，不符合则跳过。

        // 为所有生成建筑指定所属城市
    }

    /// <summary>
    /// 生成村庄地标、建筑和装饰
    /// </summary>
    void Build是Village(HashSet<Vector2Int> area)
    {
        // 找到一块比较中心的格子，生成一个聚集点广场。

        // 获取所有宗门按与中心格子距离进行排序

        // 从配置中获取一个未使用的村庄配置（优先选择最近宗门的附属村庄，没有则取第二近的，以此类推）

        // 每种生成的建筑占一个格子，建筑与建筑之间至少距离一个格子。

        // 根据配置将所需的必要建筑生成到范围内随机格子上

        // 根据剩余空地数量随机生成民居，循环（area.Count/10）次随机位置并判断是否是否符合创建条件（如是否距离其他建筑过近）。如何条件则创建民居，不符合则跳过。

        // 为所有生成建筑指定所属村庄
    }

    /// <summary>
    /// 生成部落地标、建筑和装饰
    /// </summary>
    void Build是Tribe(HashSet<Vector2Int> area)
    {
        // 找到一块比较中心的格子，生成一个聚集点广场。

        // 获取所有宗门按与中心格子距离进行排序

        // 从配置中获取一个未使用的部落配置（优先选择最近宗门的附属部落，没有则取第二近的，以此类推）

        // 每种生成的建筑占一个格子，建筑与建筑之间至少距离一个格子。

        // 根据配置将所需的必要建筑生成到范围内随机格子上

        // 根据剩余空地数量随机生成民居，循环（area.Count/10）次随机位置并判断是否是否符合创建条件（如是否距离其他建筑过近）。如何条件则创建民居，不符合则跳过。

        // 为所有生成建筑指定所属部落
    }
}