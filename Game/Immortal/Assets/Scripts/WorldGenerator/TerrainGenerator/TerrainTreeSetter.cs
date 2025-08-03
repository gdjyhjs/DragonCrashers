using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

/// <summary>
/// 树的配置数据类，存储单种树的生成参数
/// </summary>
[System.Serializable]
public class TerrainTreeData
{
    public MapData gridType = MapData.Forest; // 在什么地形上生成该对象
    public GameObject prefab; // 生成的的预制体
    public Vector2 startPos = new Vector2(); // 树在地图中生成的起始坐标比例（0-1范围）
    public Vector2 endPos = new Vector2(1, 1); // 树在地图中生成的结束坐标比例（0-1范围）
    public float minScale = 1; // 最小缩放比例
    public float maxScale = 1; // 最大缩放比例
    public float density = 1; // 生成密度（每1平方米生成多少个，小于0则按照概率生成）Density calculation method
    public DensityCalculationType densityCalculationType = DensityCalculationType.Shared; // 密度计算方式（独立或共享）
    public string flag = "Tree"; // 对象标识
}

/// <summary>
/// 地形树木生成器，负责根据配置在地形上生成树木
/// </summary>
public class TerrainTreeSetter
{
    private TerrainData terrainData; // 地形数据（包含地形尺寸、高度等信息）
    private int[,] mapData; // 地图数据（用于判断哪些区域需要生成树）
    private TerrainTreeData[] treesData; // 所有树的配置数据数组

    /// <summary>
    /// 构造函数，初始化地形、地图和树木配置数据
    /// </summary>
    /// <param name="terrainData">地形数据</param>
    /// <param name="mapData">地图数据（用于标记森林区域）</param>
    /// <param name="treesData">树木配置数据数组</param>
    public TerrainTreeSetter(TerrainData terrainData, int[,] mapData, TerrainTreeData[] treesData)
    {
        this.terrainData = terrainData;
        this.mapData = mapData;
        this.treesData = treesData;
    }

    /// <summary>
    /// 核心方法：在地形上生成树木
    /// </summary>
    public void SetTrees(Transform parent)
    {
        // 获取地图尺寸（宽和高的单元格数量）
        int mapWidth = mapData.GetLength(0);
        int mapHeight = mapData.GetLength(1);

        // 获取地形实际尺寸（世界坐标中的大小）
        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;
        // 地形高度图分辨率（用于计算高度采样坐标）
        int heightmapRes = terrainData.heightmapResolution;

        // 计算高度图每个格子对应的世界坐标缩放比例
        float heightmapScaleX = terrainSizeX / (heightmapRes - 1);
        float heightmapScaleZ = terrainSizeZ / (heightmapRes - 1);
        // 计算地图每个单元格对应的世界坐标大小
        float mapCellSizeX = terrainSizeX / mapWidth;
        float mapCellSizeZ = terrainSizeZ / mapHeight;

        // 临时存储符合条件的树配置（当前区域可生成的树）
        List<TerrainTreeData> currentTreesData = new List<TerrainTreeData>();

        int forestCount = 0; // 记录森林区域数量
        int forestMax = 0; // （原代码未使用，可能是预留变量）

        Debug.Log($"格子大小：{mapCellSizeX}，{mapCellSizeZ}   数据大小：{mapWidth}，{mapHeight}  heightmapRes:{heightmapRes}");

        var offset = new Vector2Int(mapWidth/2, mapHeight/2);
        // 遍历地图所有单元格
        for (int mapX = 0; mapX < mapWidth; mapX++)
        {
            for (int mapY = 0; mapY < mapHeight; mapY++)
            {
                // 获取当前单元格的地图数据值
                int dataValue = mapData[mapX, mapY];
                if (IsType(dataValue, MapData.Sect)) continue;
                if (IsType(dataValue, MapData.City)) continue;
                if (IsType(dataValue, MapData.Tribe)) continue;
                if (IsType(dataValue, MapData.Village)) continue;
                if (IsType(dataValue, MapData.Road)) continue;
                if (IsType(dataValue, MapData.Route)) continue;
                if (IsType(dataValue, MapData.Lake)) continue;
                if (IsType(dataValue, MapData.River)) continue;
                if (IsType(dataValue, MapData.Dock)) continue;
                if (IsType(dataValue, MapData.Bridge)) continue;
                if (IsType(dataValue, MapData.Ocean)) continue;

                var curPos = new Vector3(mapX * mapCellSizeX, 0, mapY * mapCellSizeZ);

                // 计算当前地图单元格在世界坐标中的范围
                float worldX = (mapX) * mapCellSizeX;
                float worldZ = (mapY) * mapCellSizeZ;

                // 计算当前单元格在地图中的相对比例（0-1范围）
                var xPos = 1 - mapX * 1f / mapWidth;
                var yPos = 1 - mapY * 1f / mapHeight;


                // 筛选当前位置符合生成条件的树配置
                foreach (TerrainTreeData treeData in this.treesData)
                {
                    // 检查当前位置是否在树的生成范围（startPos到endPos之间）
                    if (xPos < treeData.startPos.y || xPos > treeData.endPos.y ||
                        yPos < treeData.startPos.x || yPos > treeData.endPos.x)
                    {
                        continue; // 不在范围内，跳过
                    }
                    // 判断当前单元格是否为森林类型（通过位运算判断）
                    if (!IsType(dataValue, treeData.gridType))
                    {
                        continue; // 非指定生成区域，跳过
                    }
                    currentTreesData.Add(treeData); // 符合条件，加入列表

                    // 创建树实例数据（Terrain的TreeInstance）
                    TreeInstance tree = new TreeInstance
                    {
                        position = new Vector3(xPos, 0, yPos), // 地形本地坐标（0-1范围）
                        prototypeIndex = 0, // 树原型索引（默认第一种，实际应根据配置设置）
                        color = Color.green, // 树的颜色
                        lightmapColor = Color.white, // 光照图颜色
                        widthScale = Random.Range(treeData.minScale, treeData.maxScale), // 随机宽度缩放
                        heightScale = Random.Range(treeData.minScale, treeData.maxScale) // 随机高度缩放
                    };
                }

                // 如果没有符合条件的树，跳过后续生成逻辑
                if (currentTreesData.Count == 0)
                {
                    continue;
                }

                // 遍历当前区域的高度图范围，生成树木
                for (int i = 0; i < mapCellSizeX; i++)
                {
                    for (int j = 0; j < mapCellSizeZ; j++)
                    {
                        var x = worldX + i;
                        var z = worldZ + j;

                        // 遍历符合条件的树配置，按密度生成树木
                        foreach (var treeData in currentTreesData)
                        {
                            // 计算当前树的实际生成密度（平均分配给多种树）
                            float density = treeData.density / currentTreesData.Count;
                            int count = (int)density;
                            float other = density - count;
                            if (density > 1)
                            {
                                for (int k = 0; k < density; k++)
                                {
                                    CreateTree(treeData, x, z, parent);
                                }
                            }
                            
                            if(other > 0)
                            {
                                if (Random.Range(0f, 1f) <= other)
                                {
                                    CreateTree(treeData, x, z, parent);
                                }
                            }
                        }
                    }
                }

                currentTreesData.Clear(); // 清空当前区域的树配置列表，准备下一个区域
                forestCount++; // 森林区域数量+1
            }
        }

        // 输出森林区域数量信息
        Debug.Log("森林数量：" + forestCount + " / " + forestMax);
    }

    /// <summary>
    /// 实例化树预制体并设置位置
    /// </summary>
    /// <param name="prefab">树的预制体</param>
    /// <param name="height">地形高度（Y轴位置）</param>
    /// <param name="x">高度图X坐标</param>
    /// <param name="z">高度图Z坐标</param>
    private void CreateTree(TerrainTreeData data, float x1, float z1,Transform root)
    {
        var parent = root.Find(data.flag);
        if(parent == null)
        {
            var flag = new GameObject(data.flag);
            flag.transform.SetParent(root, false);
            parent = flag.transform;
        }

        // 数据需要转换坐标
        var x = z1;
        var z = x1;

        // 射线起点：从(x, 500, z)位置开始
        Vector3 localRayOrigin = new Vector3(x, 500f, z);
        // 转换为世界坐标
        Vector3 worldRayOrigin = parent.TransformPoint(localRayOrigin);
        // 射线方向：向下
        Vector3 rayDirection = Vector3.down;
        // 射线最大距离（足够覆盖从500高度到地面的距离）
        float maxDistance = 1000f;
        // 获取Ground层的索引
        int groundLayerIndex = LayerMask.NameToLayer("Ground");

        RaycastHit hit;

        // 射线检测所有层（layerMask设为-1表示检测所有层）
        if (Physics.Raycast(worldRayOrigin, rayDirection, out hit, maxDistance, -1))
        {
            // 检查碰撞到的是否是Ground层
            if (hit.collider.gameObject.layer != groundLayerIndex)
            {
                // 不是Ground层，说明地面有其他东西，直接返回
                Debug.Log($"位置 ({x}, {z}) 的地面被其他物体覆盖，不生成树木");
                return;
            }
        }
        else
        {
            // 没有检测到任何碰撞
            Debug.LogWarning($"位置 ({x}, {z}) 未检测到任何物体，不生成树木");
            return;
        }

        // 如果通过检测，获取地面高度
        float height = hit.point.y - root.transform.position.y;

        // 实例化树预制体
        GameObject go = GameObject.Instantiate(data.prefab, parent);
        // 设置树的位置（在高度图坐标基础上添加随机偏移，避免整齐排列）
        go.transform.localPosition = new Vector3(x + Random.Range(0f, 1f), height, z + Random.Range(0f, 1f));
        go.transform.localScale = Vector3.one * Random.Range(data.minScale, data.maxScale);
        go.name = data.flag + ":" + x + "," + z;
    }

    /// <summary>
    /// 判断地图数据是否包含目标类型（通过位运算实现，支持多类型标记）
    /// </summary>
    /// <param name="data">地图单元格数据</param>
    /// <param name="mapData">目标类型</param>
    /// <returns>是否包含目标类型</returns>
    private bool IsType(int data, MapData mapData)
    {
        return (data & (int)mapData) != 0;
    }
}