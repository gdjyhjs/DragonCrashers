using UnityEngine;
using System.IO;

public class TerrainFromMapData : MonoBehaviour
{
    public Terrain targetTerrain;
    public int[,] mapData;

    // 纹理层（Inspector中按LayerType顺序赋值）
    public TerrainLayer layerCity;
    public TerrainLayer layerForest;
    public TerrainLayer layerMountain;
    public TerrainLayer layerPlain;
    public TerrainLayer layerRoad;

    // 草与树的原型（Inspector中赋值）
    public TerrainGrassData[] grassData; // 草纹理
    public TerrainTreeData[] treesData; // 树

    private LayerType[] _layerOrder = { LayerType.City, LayerType.Forest, LayerType.Mountain, LayerType.Plain, LayerType.Road };
    private TerrainLayer[] _terrainLayers;

    private void Awake()
    {
        // 初始化纹理层数组
        _terrainLayers = new TerrainLayer[] { layerCity, layerForest, layerMountain, layerPlain, layerRoad };

        // 加载地图数据
        string loadPath = Path.Combine(Application.dataPath, "GeneratedMaps", "world.txt");
        mapData = MapGeneratorUtility.LoadMapFromFile(loadPath, out Vector2Int _);

        // 自动查找地形
        if (targetTerrain == null) targetTerrain = GetComponent<Terrain>();
    }

    void Start()
    {
        if (targetTerrain == null || mapData == null)
        {
            Debug.LogError("地形或地图数据未设置！");
            return;
        }

        TerrainData terrainData = targetTerrain.terrainData;

        // 1. 设置地形高度
        new TerrainHeightSetter(terrainData, mapData).SetHeights();

        // 2. 设置地面纹理
        new TerrainTextureSetter(terrainData, mapData, _terrainLayers, _layerOrder).SetTextures();

        // 3. 种草（平原/森林）
        //new TerrainGrassSetter(terrainData, mapData, grassData, _layerOrder).SetGrass();

        // 4. 种树（森林）
        new TerrainTreeSetter(terrainData, mapData, treesData).SetTrees();

        Debug.Log("地形所有设置完成！");
    }
}