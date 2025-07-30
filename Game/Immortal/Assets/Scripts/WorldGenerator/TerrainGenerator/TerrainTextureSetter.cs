using UnityEngine;

public enum LayerType { City, Forest, Mountain, Plain, Road }

public class TerrainTextureSetter
{
    private TerrainData _terrainData;
    private int[,] _mapData;
    private TerrainLayer[] _terrainLayers;
    private LayerType[] _layerOrder;

    public TerrainTextureSetter(TerrainData terrainData, int[,] mapData, TerrainLayer[] terrainLayers, LayerType[] layerOrder)
    {
        _terrainData = terrainData;
        _mapData = mapData;
        _terrainLayers = terrainLayers;
        _layerOrder = layerOrder;
    }

    public void SetTextures()
    {
        // 配置地形纹理层
        _terrainData.terrainLayers = _terrainLayers;

        int mapWidth = _mapData.GetLength(0);
        int mapHeight = _mapData.GetLength(1);
        int splatRes = _terrainData.alphamapResolution;
        float[,,] splatmapData = new float[splatRes, splatRes, _terrainLayers.Length];

        float terrainSizeX = _terrainData.size.x;
        float terrainSizeZ = _terrainData.size.z;

        for (int z = 0; z < splatRes; z++)
        {
            for (int x = 0; x < splatRes; x++)
            {
                float xNorm = (float)x / (splatRes - 1);
                float zNorm = (float)z / (splatRes - 1);

                float worldX = xNorm * terrainSizeX;
                float worldZ = zNorm * terrainSizeZ;

                int mapX = Mathf.Clamp(Mathf.FloorToInt(worldX / (terrainSizeX / mapWidth)), 0, mapWidth - 1);
                int mapZ = Mathf.Clamp(Mathf.FloorToInt(worldZ / (terrainSizeZ / mapHeight)), 0, mapHeight - 1);

                int dataValue = _mapData[mapX, mapZ];

                // 重置权重
                for (int layer = 0; layer < splatmapData.GetLength(2); layer++)
                {
                    splatmapData[x, z, layer] = 0;
                }

                // 按地图数据分配纹理权重
                if (IsType(dataValue, MapData.City) || IsType(dataValue, MapData.Sect) || IsType(dataValue, MapData.Village) || IsType(dataValue, MapData.Tribe))
                {
                    SetLayerWeight(splatmapData, x, z, LayerType.City);
                }
                else if (IsType(dataValue, MapData.Road))
                {
                    SetLayerWeight(splatmapData, x, z, LayerType.Road);
                }
                else if (IsType(dataValue, MapData.Forest))
                {
                    SetLayerWeight(splatmapData, x, z, LayerType.Forest);
                }
                else if (IsType(dataValue, MapData.Mountain))
                {
                    SetLayerWeight(splatmapData, x, z, LayerType.Mountain);
                }
                else
                {
                    SetLayerWeight(splatmapData, x, z, LayerType.Plain);
                }
            }
        }

        _terrainData.SetAlphamaps(0, 0, splatmapData);
        Debug.Log("地形纹理混合设置完成！");
    }

    private void SetLayerWeight(float[,,] splatmapData, int x, int z, LayerType layerType)
    {
        int layerIndex = System.Array.IndexOf(_layerOrder, layerType);
        if (layerIndex != -1) splatmapData[x, z, layerIndex] = 1f;
    }

    private bool IsType(int data, MapData mapData)
    {
        return (data & (int)mapData) != 0;
    }
}