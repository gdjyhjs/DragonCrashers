using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

[System.Serializable]
public class TerrainTreeData
{
    public GameObject prefab; // 树预制体
    public Vector2 startPos = new Vector2(); // 出现在地图中的开始坐标比例
    public Vector2 endPos = new Vector2(1, 1); // 出现在地图中的结束坐标比例
    public float density = 1; // 种树密度
    public float minScale = 1; // 最小缩放比例
    public float maxScale = 1; // 最大缩放比例
}

public class TerrainTreeSetter
{
    private TerrainData terrainData;
    private int[,] mapData;
    private TerrainTreeData[] treesData;

    public TerrainTreeSetter(TerrainData terrainData, int[,] mapData, TerrainTreeData[] treesData)
    {
        this.terrainData = terrainData;
        this.mapData = mapData;
        this.treesData = treesData;
    }

    public void SetTrees()
    {
        // 配置树原型
        int mapWidth = mapData.GetLength(0);
        int mapHeight = mapData.GetLength(1);

        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;
        int heightmapRes = terrainData.heightmapResolution;

        float heightmapScaleX = terrainSizeX / (heightmapRes - 1);
        float heightmapScaleZ = terrainSizeZ / (heightmapRes - 1);
        float mapCellSizeX = terrainSizeX / mapWidth;
        float mapCellSizeZ = terrainSizeZ / mapHeight;

        List<TerrainTreeData> treesData = new List<TerrainTreeData>();

        int forestCount = 0;
        int forestMax = 0;
        for (int mapX = 0; mapX < mapWidth; mapX++)
        {
            for (int mapY = 0; mapY < mapHeight; mapY++)
            {
                int dataValue = mapData[mapX, mapY];
                if (!IsType(dataValue, MapData.Forest))
                {
                    continue;
                }
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

                var xPos = mapX * 1f / mapWidth;
                var yPos = mapY * 1f / mapHeight;

                foreach (TerrainTreeData treeData in this.treesData)
                {
                    if(xPos < treeData.startPos.x || xPos > treeData.endPos.x ||
                       yPos < treeData.startPos.y || yPos > treeData.endPos.y)
                    {
                        continue;
                    }
                    treesData.Add(treeData);
                        // 生成树实例
                        TreeInstance tree = new TreeInstance
                        {
                            position = new Vector3(xPos, 0, yPos), // 本地坐标0~1
                            prototypeIndex = 0, // 假设仅一种树
                            color = Color.green,
                            lightmapColor = Color.white,
                            widthScale = Random.Range(treeData.minScale, treeData.maxScale),
                            heightScale = Random.Range(treeData.minScale, treeData.maxScale)
                        };
                }
                if (treesData.Count == 0)
                {
                    continue;
                }

                for (int x = heightX0; x <= heightX1; x++)
                {
                    for (int z = heightZ0; z <= heightZ1; z++)
                    {
                        float height = terrainData.GetHeight(x, z);
                        foreach (var treeData in treesData)
                        {
                            float density = treeData.density / treesData.Count;
                            if (density > 0)
                            {
                                for (int j = 0; j < density; j++)
                                {
                                    CreateTree(treeData.prefab, height, x, z);
                                }
                            }
                            else
                            {
                                if (Random.Range(0f, 1f) <= density)
                                {
                                    CreateTree(treeData.prefab, height, x, z);
                                }
                            }
                        }
                    }
                }
                treesData.Clear();
                forestCount++;
            }
        }
        Debug.Log("森林数量："+ forestCount+" / "+ forestMax);
    }

    private void CreateTree(GameObject prefab, float height, int x, int z)
    {
        var go = GameObject.Instantiate(prefab);
        go.transform.position = new Vector3(x + Random.Range(-0.5f, 0.5f), height, z + Random.Range(-0.5f, 0.5f));
    }

    private bool IsType(int data, MapData mapData)
    {
        return (data & (int)mapData) != 0;
    }
}