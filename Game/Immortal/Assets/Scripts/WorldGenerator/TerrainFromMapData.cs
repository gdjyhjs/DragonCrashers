using UnityEngine;
using System.IO;

public class TerrainFromMapData : MonoBehaviour
{
    public Terrain targetTerrain;
    public int[,] mapData;

    public TerrainLayer layerCity;
    public TerrainLayer layerForest;
    public TerrainLayer layerMountain;
    public TerrainLayer layerPlain;
    public TerrainLayer layerRoad;

    private TerrainLayer[] terrainLayers;

    enum LayerType
    {
        City,
        Forest,
        Mountain,
        Plain,
        Road,
    }

    public float textureScale = 10.0f; // 纹理平铺缩放

    private void Awake()
    {
        // 按照LayerType的顺序
        terrainLayers = new TerrainLayer[] { layerCity, layerForest, layerMountain, layerPlain, layerRoad };

        string loadPath = Path.Combine(Application.dataPath, "GeneratedMaps", "world.txt");
        mapData = MapGeneratorUtility.LoadMapFromFile(loadPath, out Vector2Int _);
        if (targetTerrain == null)
        {
            targetTerrain = GetComponent<Terrain>();
        }
    }

    void Start()
    {
        if (targetTerrain == null || mapData == null)
        {
            Debug.LogError("地形或地图数据未设置！");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;

        // 1. 设置地形纹理层（替代过时的SplatPrototype）
        SetupTerrainLayers(terrainData);

        // 2. 设置地形高度（保留原有逻辑）
        SetTerrainHeights(terrainData);

        // 3. 根据地图数据设置纹理混合（Splat Map）
        SetTerrainSplatMaps(terrainData);

        Debug.Log("地形设置完成！");
    }

    // ---------------------------
    // 1. 配置地形纹理层（使用TerrainLayer）
    // ---------------------------
    void SetupTerrainLayers(TerrainData terrainData)
    {
        // 应用到地形
        terrainData.terrainLayers = terrainLayers;
    }

    // ---------------------------
    // 2. 原有高度设置逻辑（保持不变，仅抽离方法）
    // ---------------------------
    void SetTerrainHeights(TerrainData terrainData)
    {
        int mapWidth = mapData.GetLength(0);
        int mapHeight = mapData.GetLength(1);
        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;
        int heightmapRes = terrainData.heightmapResolution;
        float heightmapScaleX = terrainSizeX / (heightmapRes - 1);
        float heightmapScaleZ = terrainSizeZ / (heightmapRes - 1);
        float mapCellSizeX = terrainSizeX / mapWidth;
        float mapCellSizeZ = terrainSizeZ / mapHeight;

        float[,] heightMap = terrainData.GetHeights(0, 0, heightmapRes, heightmapRes);

        for (int mapX = 0; mapX < mapWidth; mapX++)
        {
            for (int mapY = 0; mapY < mapHeight; mapY++)
            {
                int dataValue = mapData[mapX, mapY];
                float worldX0 = mapX * mapCellSizeX;
                float worldZ0 = mapY * mapCellSizeZ;
                float worldX1 = (mapX + 1) * mapCellSizeX;
                float worldZ1 = (mapY + 1) * mapCellSizeZ;

                int heightX0 = Mathf.FloorToInt(worldX0 / heightmapScaleX);
                int heightZ0 = Mathf.FloorToInt(worldZ0 / heightmapScaleZ);
                int heightX1 = Mathf.CeilToInt(worldX1 / heightmapScaleX);
                int heightZ1 = Mathf.CeilToInt(worldZ1 / heightmapScaleZ);

                heightX0 = Mathf.Clamp(heightX0, 0, heightmapRes - 1);
                heightX1 = Mathf.Clamp(heightX1, 0, heightmapRes - 1);
                heightZ0 = Mathf.Clamp(heightZ0, 0, heightmapRes - 1);
                heightZ1 = Mathf.Clamp(heightZ1, 0, heightmapRes - 1);

                float targetHeight = 0f;
                if (IsType(dataValue, MapData.Mountain))
                {
                    targetHeight = 103f;   // 山
                }
                else if (IsType(dataValue, MapData.Ocean))
                {
                    targetHeight = 90f; // 水
                }
                else if (IsType(dataValue, MapData.Lake))
                {
                    targetHeight = 99.5f; // 水
                }
                else
                {
                    targetHeight = 100.5f;   // 陆地
                }

                float normalizedHeight = targetHeight / terrainData.heightmapScale.y;

                for (int x = heightX0; x <= heightX1; x++)
                {
                    for (int z = heightZ0; z <= heightZ1; z++)
                    {
                        heightMap[x, z] = normalizedHeight;
                    }
                }
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
        Debug.Log("地形高度设置完成！");
    }

    // ---------------------------
    // 3. 纹理混合（Splat Map）逻辑（核心逻辑不变，仅适配层类型）
    // ---------------------------
    void SetTerrainSplatMaps(TerrainData terrainData)
    {
        int mapWidth = mapData.GetLength(0);
        int mapHeight = mapData.GetLength(1);
        int splatMapResolution = terrainData.alphamapResolution;
        // 三维数组：[x, z, 层索引]，值为该层的混合权重
        float[,,] splatmapData = new float[splatMapResolution, splatMapResolution, terrainData.terrainLayers.Length];

        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        for (int z = 0; z < splatMapResolution; z++) // 注意：地形z轴对应循环的y！
        {
            for (int x = 0; x < splatMapResolution; x++)
            {
                // 将splat map坐标转换为0~1范围
                float xNorm = (float)x / (splatMapResolution - 1);
                float zNorm = (float)z / (splatMapResolution - 1);

                // 转换为世界坐标
                float worldX = xNorm * terrainSizeX;
                float worldZ = zNorm * terrainSizeZ;

                // 转换为mapData的格子索引
                int mapX = Mathf.Clamp(Mathf.FloorToInt(worldX / (terrainSizeX / mapWidth)), 0, mapWidth - 1);
                int mapZ = Mathf.Clamp(Mathf.FloorToInt(worldZ / (terrainSizeZ / mapHeight)), 0, mapHeight - 1);

                int dataValue = mapData[mapX, mapZ];

                // 重置所有层的权重（避免叠加错误）
                for (int layer = 0; layer < splatmapData.GetLength(2); layer++)
                {
                    splatmapData[x, z, layer] = 0;
                }

                // 根据地图数据设置对应层的权重为1（简单示例，可扩展混合逻辑）
                if (IsType(dataValue, MapData.City) || IsType(dataValue, MapData.Sect)|| IsType(dataValue, MapData.Village)|| IsType(dataValue, MapData.Tribe))
                {
                    splatmapData[x, z, GetLayerIndex(LayerType.City)] = 1.0f;
                }
                else if (IsType(dataValue, MapData.Road))
                {
                    splatmapData[x, z, GetLayerIndex(LayerType.Road)] = 1.0f;
                }
                else if (IsType(dataValue, MapData.Forest))
                {
                    splatmapData[x, z, GetLayerIndex(LayerType.Forest)] = 1.0f;
                }
                else if (IsType(dataValue, MapData.Mountain))
                {
                    splatmapData[x, z, GetLayerIndex(LayerType.Mountain)] = 1.0f;
                }
                else if (IsType(dataValue, MapData.Plain))
                {
                    splatmapData[x, z, GetLayerIndex(LayerType.Plain)] = 1.0f;
                }
                else
                {
                    splatmapData[x, z, GetLayerIndex(LayerType.Plain)] = 1.0f;
                }
            }
        }

        // 应用纹理混合数据
        terrainData.SetAlphamaps(0, 0, splatmapData);
        Debug.Log("地形纹理混合设置完成！");
    }

    bool IsType(int data, MapData mapData)
    {
        return (data & (int)mapData) != 0;
    }

    int GetLayerIndex(LayerType layer)
    {
        return (int)layer;
    }
}