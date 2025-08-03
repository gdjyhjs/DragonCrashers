using UnityEngine;

public class TerrainHeightSetter
{
    private TerrainData _terrainData;
    private int[,] _mapData;

    // 高度配置（可扩展为外部参数）
    private float _mountainHeight = 100f;
    private float _oceanHeight = 95.6f;
    private float _lakeHeight = 97.2f;
    private float _riverHeight = 98.4f;
    private float _defaultHeight = 100f;

    public TerrainHeightSetter(TerrainData terrainData, int[,] mapData)
    {
        _terrainData = terrainData;
        _mapData = mapData;
    }

    public void SetHeights()
    {
        int mapWidth = _mapData.GetLength(0);
        int mapHeight = _mapData.GetLength(1);
        float terrainSizeX = _terrainData.size.x;
        float terrainSizeZ = _terrainData.size.z;
        int heightmapRes = _terrainData.heightmapResolution;

        float heightmapScaleX = terrainSizeX / (heightmapRes - 1);
        float heightmapScaleZ = terrainSizeZ / (heightmapRes - 1);
        float mapCellSizeX = terrainSizeX / mapWidth;
        float mapCellSizeZ = terrainSizeZ / mapHeight;

        float[,] heightMap = _terrainData.GetHeights(0, 0, heightmapRes, heightmapRes);

        for (int mapX = 0; mapX < mapWidth; mapX++)
        {
            for (int mapY = 0; mapY < mapHeight; mapY++)
            {
                int dataValue = _mapData[mapX, mapY];
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

                float targetHeight = GetTargetHeight(dataValue);
                float normalizedHeight = targetHeight / _terrainData.heightmapScale.y;

                for (int x = heightX0; x <= heightX1; x++)
                {
                    for (int z = heightZ0; z <= heightZ1; z++)
                    {
                        heightMap[x, z] = normalizedHeight;
                    }
                }
            }
        }

        _terrainData.SetHeights(0, 0, heightMap);
        Debug.Log("地形高度设置完成！");
    }

    private float GetTargetHeight(int dataValue)
    {
        if (IsType(dataValue, MapData.River)) return _defaultHeight;

        if (IsType(dataValue, MapData.Ocean)) return _oceanHeight;
        if (IsType(dataValue, MapData.Lake)) return _lakeHeight;
        if (IsType(dataValue, MapData.River)) return _riverHeight;

        if (IsType(dataValue, MapData.City) || IsType(dataValue, MapData.Sect) || IsType(dataValue, MapData.Village) || IsType(dataValue, MapData.Tribe)) return _defaultHeight;
        if (IsType(dataValue, MapData.Road)) return _defaultHeight;
        //if (IsType(dataValue, MapData.Forest)) return _defaultHeight;
        //if (IsType(dataValue, MapData.Plain)) return _defaultHeight;

        if (IsType(dataValue, MapData.Mountain)) return _mountainHeight;
        return _defaultHeight;
    }

    private bool IsType(int data, MapData mapData)
    {
        return (data & (int)mapData) != 0;
    }
}