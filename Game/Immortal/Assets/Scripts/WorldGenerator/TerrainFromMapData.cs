using UnityEngine;
using System.IO;

public class TerrainFromMapData : MonoBehaviour
{
    public Terrain targetTerrain;
    public int[,] mapData;

    private void Awake()
    {
        string loadPath = Path.Combine(Application.dataPath, "GeneratedMaps", "new_map.txt");
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

        // mapData 尺寸（200x200）
        int mapWidth = mapData.GetLength(0);
        int mapHeight = mapData.GetLength(1);

        // 地形参数（物理尺寸 & 高度图分辨率）
        float terrainSizeX = terrainData.size.x;   // 1000 米
        float terrainSizeZ = terrainData.size.z;   // 1000 米
        int heightmapRes = terrainData.heightmapResolution; // 513

        // 关键：计算高度图每个采样点的物理间隔（米/采样点）
        float heightmapScaleX = terrainSizeX / (heightmapRes - 1);
        float heightmapScaleZ = terrainSizeZ / (heightmapRes - 1);

        // 计算 mapData 每个格子对应的物理尺寸（米）
        float mapCellSizeX = terrainSizeX / mapWidth;   // 1000 / 200 = 5 米/格
        float mapCellSizeZ = terrainSizeZ / mapHeight;  // 1000 / 200 = 5 米/格

        Debug.Log($"地形尺寸：{new Vector2(terrainSizeX, terrainSizeZ)}   数据尺寸：{new Vector2Int(mapWidth, mapHeight)}   一个格子大小：{new Vector2(mapCellSizeX, mapCellSizeZ)}");

        // 获取高度图数据（513x513）
        float[,] heightMap = terrainData.GetHeights(0, 0, heightmapRes, heightmapRes);

        // 遍历 mapData，逐格映射到地形
        for (int mapX = 0; mapX < mapWidth; mapX++)
        {
            for (int mapY = 0; mapY < mapHeight; mapY++)
            {
                int dataValue = mapData[mapX, mapY];

                // Step 1: 计算当前 mapData 格子的物理范围（米）
                float worldX0 = mapX * mapCellSizeX;          // 格子左边界
                float worldZ0 = mapY * mapCellSizeZ;          // 格子下边界
                float worldX1 = (mapX + 1) * mapCellSizeX;    // 格子右边界
                float worldZ1 = (mapY + 1) * mapCellSizeZ;    // 格子上边界

                // Step 2: 转换为高度图的采样点范围（索引）
                int heightX0 = Mathf.FloorToInt(worldX0 / heightmapScaleX);
                int heightZ0 = Mathf.FloorToInt(worldZ0 / heightmapScaleZ);
                int heightX1 = Mathf.CeilToInt(worldX1 / heightmapScaleX);
                int heightZ1 = Mathf.CeilToInt(worldZ1 / heightmapScaleZ);

                // 限制范围在高度图内
                heightX0 = Mathf.Clamp(heightX0, 0, heightmapRes - 1);
                heightX1 = Mathf.Clamp(heightX1, 0, heightmapRes - 1);
                heightZ0 = Mathf.Clamp(heightZ0, 0, heightmapRes - 1);
                heightZ1 = Mathf.Clamp(heightZ1, 0, heightmapRes - 1);

                // Step 3: 根据 mapData 设置高度
                float targetHeight = 0f;
                if (IsType(dataValue, MapData.Mountain))
                {
                    targetHeight = 103f;   // 山
                }
                else if (IsType(dataValue, MapData.Ocean))
                {
                    targetHeight = 90f; // 大海
                }
                else if (IsType(dataValue, MapData.Lake))
                {
                    targetHeight = 99.5f;   // 河流
                }
                else
                {
                    targetHeight = 100.5f;   // 陆地
                }

                // 归一化到高度图范围（0~1）
                float normalizedHeight = targetHeight / terrainData.heightmapScale.y;

                // Step 4: 填充高度图区域
                for (int x = heightX0; x <= heightX1; x++)
                {
                    for (int z = heightZ0; z <= heightZ1; z++)
                    {
                        heightMap[x, z] = normalizedHeight;
                    }
                }
            }
        }

        // 应用高度图
        terrainData.SetHeights(0, 0, heightMap);
        Debug.Log("地形高度设置完成！");
    }

    bool IsType(int data, MapData mapData)
    {
        return (data & (int)mapData) != 0;
    }
}