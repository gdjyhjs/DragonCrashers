using UnityEngine;

[System.Serializable]
public class TerrainGrassData
{
    public GameObject prefab;
    public Vector2 startPos;
    public Vector2 endPos;
    public int density;
}

public class TerrainGrassSetter
{
    private TerrainData terrainData;
    private int[,] mapData;
    private TerrainGrassData[] grassData;

    public TerrainGrassSetter(TerrainData terrainData, int[,] mapData, TerrainGrassData[] grassData, LayerType[] layerOrder)
    {
        this.terrainData = terrainData;
        this.mapData = mapData;
        this.grassData= grassData;
    }

    //public void SetGrass()
    //{
    //    // 配置草的细节原型
    //    _terrainData.detailPrototypes = _grassPrototypes;

    //    int detailRes = _terrainData.detailResolution;
    //    int[,] grassData = new int[detailRes, detailRes];

    //    int mapWidth = _mapData.GetLength(0);
    //    int mapHeight = _mapData.GetLength(1);
    //    float terrainSizeX = _terrainData.size.x;
    //    float terrainSizeZ = _terrainData.size.z;

    //    for (int z = 0; z < detailRes; z++)
    //    {
    //        for (int x = 0; x < detailRes; x++)
    //        {
    //            float xNorm = (float)x / (detailRes - 1);
    //            float zNorm = (float)z / (detailRes - 1);

    //            float worldX = xNorm * terrainSizeX;
    //            float worldZ = zNorm * terrainSizeZ;

    //            int mapX = Mathf.Clamp(Mathf.FloorToInt(worldX / (terrainSizeX / mapWidth)), 0, mapWidth - 1);
    //            int mapZ = Mathf.Clamp(Mathf.FloorToInt(worldZ / (terrainSizeZ / mapHeight)), 0, mapHeight - 1);

    //            int dataValue = _mapData[mapX, mapZ];

    //            // 平原和森林生成草
    //            bool isPlain = IsType(dataValue, MapData.Plain);
    //            bool isForest = IsType(dataValue, MapData.Forest);
    //            grassData[x, z] = (isPlain || isForest) ? 1 : 0;
    //        }
    //    }

    //    // 应用草分布（细节层索引0）
    //    _terrainData.SetDetailLayer(0, 0, 0, grassData);
    //    Debug.Log("地形草设置完成！");
    //}

    private bool IsType(int data, MapData mapData)
    {
        return (data & (int)mapData) != 0;
    }
}