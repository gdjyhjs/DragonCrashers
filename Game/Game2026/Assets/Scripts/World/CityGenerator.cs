using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 城市生成器 - 优化生成顺序：城墙→主干道→商铺→豪宅→辅路→民房→地形→装饰
/// </summary>
public class CityGenerator : MonoBehaviour
{
    // 地图尺寸配置
    [Header("地图配置")]
    public int mapWidth = 100;
    public int mapHeight = 100;

    // 瓦片资源配置
    [Header("瓦片资源")]
    public TileBase grassTile;
    public TileBase cityWallTile;
    public TileBase fenceTile;
    public TileBase houseTile;
    public TileBase mountainTile;
    public TileBase riverTile;
    public TileBase roadTile;
    public TileBase abyssTile;
    public TileBase obstacleTile;
    public TileBase bridgeTile;
    public TileBase pondTile;

    // 建筑数量配置
    [Header("建筑数量")]
    public int mansionCount = 10;
    public int shopCount = 10;
    public int houseCount = 40;

    // 城墙配置
    [Header("城墙配置")]
    public int cityWallWidth = 2;
    public int cityWallPadding = 5;

    // Tilemap组件引用
    [Header("Tilemap引用")]
    public Tilemap groundTilemap;
    public Tilemap buildingTilemap;
    public Tilemap wallTilemap;
    public Tilemap decorationTilemap;

    // 地图数据
    private bool[,] groundMap;
    private bool[,] buildingMap;

    // 尝试次数计数器
    private int attempts = 0;

    private void Start()
    {
        GenerateCity();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            GenerateCity();
        }
    }

    /// <summary>
    /// 按新顺序生成城市：城墙→主干道→商铺→豪宅→辅路→民房→地形→装饰
    /// </summary>
    public void GenerateCity()
    {
        attempts = 0;
        InitializeMaps();                 // 初始化地图
        GenerateCityWalls();              // 生成城墙
        GenerateMainRoads();              // 生成主干道
        GenerateShopsOnMainRoads();       // 主干道旁生成商铺
        GenerateMansionsWithRoads();      // 生成豪宅并连接道路
        GenerateSideRoads();              // 生成辅路
        GenerateHouses();                 // 生成民房
        GenerateTerrain();                // 生成地形（山脉等）
        GenerateDecorations();            // 生成装饰元素

        Debug.Log("城市生成完成!");
    }

    /// <summary>
    /// 初始化地图数据结构和清空现有瓦片
    /// </summary>
    private void InitializeMaps()
    {
        // 初始化地图占用数组
        groundMap = new bool[mapWidth, mapHeight];
        buildingMap = new bool[mapWidth, mapHeight];

        // 清空所有Tilemap上的现有瓦片
        groundTilemap.ClearAllTiles();
        buildingTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        decorationTilemap.ClearAllTiles();

        // 用草地瓦片填充整个地图作为基础
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
            }
        }
    }

    /// <summary>
    /// 生成环绕整个城市的城墙
    /// </summary>
    private void GenerateCityWalls()
    {
        // 城墙内边界
        int innerXMin = cityWallPadding;
        int innerXMax = mapWidth - cityWallPadding - 1;
        int innerYMin = cityWallPadding;
        int innerYMax = mapHeight - cityWallPadding - 1;

        // 城墙外边界
        int outerXMin = innerXMin - cityWallWidth;
        int outerXMax = innerXMax + cityWallWidth;
        int outerYMin = innerYMin - cityWallWidth;
        int outerYMax = innerYMax + cityWallWidth;

        // 确保边界在有效范围内
        outerXMin = Mathf.Max(0, outerXMin);
        outerXMax = Mathf.Min(mapWidth - 1, outerXMax);
        outerYMin = Mathf.Max(0, outerYMin);
        outerYMax = Mathf.Min(mapHeight - 1, outerYMax);

        // 填充城墙区域
        for (int x = outerXMin; x <= outerXMax; x++)
        {
            for (int y = outerYMin; y <= outerYMax; y++)
            {
                // 检查是否在城墙范围内但不在内部区域
                if ((x < innerXMin || x > innerXMax || y < innerYMin || y > innerYMax))
                {
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), cityWallTile);
                    groundMap[x, y] = true;  // 标记为已占用
                    buildingMap[x, y] = true; // 标记为已占用
                }
            }
        }

        // 生成城门（简单地在四个方向各开一个门）
        GenerateCityGates(innerXMin, innerXMax, innerYMin, innerYMax);
    }

    /// <summary>
    /// 生成城门
    /// </summary>
    private void GenerateCityGates(int innerXMin, int innerXMax, int innerYMin, int innerYMax)
    {
        int gateWidth = 3;  // 城门宽度

        // 南城门
        int southGateX = (innerXMin + innerXMax) / 2;
        for (int x = southGateX - gateWidth / 2; x <= southGateX + gateWidth / 2; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, innerYMin - 1, 0), null);  // 移除城墙瓦片
            wallTilemap.SetTile(new Vector3Int(x, innerYMin, 0), null);      // 移除城墙瓦片
            groundMap[x, innerYMin - 1] = false;  // 标记为未占用
            groundMap[x, innerYMin] = false;      // 标记为未占用
            buildingMap[x, innerYMin - 1] = false; // 标记为未占用
            buildingMap[x, innerYMin] = false;     // 标记为未占用
        }

        // 北城门
        int northGateX = (innerXMin + innerXMax) / 2;
        for (int x = northGateX - gateWidth / 2; x <= northGateX + gateWidth / 2; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, innerYMax, 0), null);      // 移除城墙瓦片
            wallTilemap.SetTile(new Vector3Int(x, innerYMax + 1, 0), null);  // 移除城墙瓦片
            groundMap[x, innerYMax] = false;  // 标记为未占用
            groundMap[x, innerYMax + 1] = false;  // 标记为未占用
            buildingMap[x, innerYMax] = false; // 标记为未占用
            buildingMap[x, innerYMax + 1] = false; // 标记为未占用
        }

        // 东城门
        int eastGateY = (innerYMin + innerYMax) / 2;
        for (int y = eastGateY - gateWidth / 2; y <= eastGateY + gateWidth / 2; y++)
        {
            wallTilemap.SetTile(new Vector3Int(innerXMax, y), null);      // 移除城墙瓦片
            wallTilemap.SetTile(new Vector3Int(innerXMax + 1, y), null);  // 移除城墙瓦片
            groundMap[innerXMax, y] = false;  // 标记为未占用
            groundMap[innerXMax + 1, y] = false;  // 标记为未占用
            buildingMap[innerXMax, y] = false; // 标记为未占用
            buildingMap[innerXMax + 1, y] = false; // 标记为未占用
        }

        // 西城门
        int westGateY = (innerYMin + innerYMax) / 2;
        for (int y = westGateY - gateWidth / 2; y <= westGateY + gateWidth / 2; y++)
        {
            wallTilemap.SetTile(new Vector3Int(innerXMin - 1, y), null);  // 移除城墙瓦片
            wallTilemap.SetTile(new Vector3Int(innerXMin, y), null);      // 移除城墙瓦片
            groundMap[innerXMin - 1, y] = false;  // 标记为未占用
            groundMap[innerXMin, y] = false;      // 标记为未占用
            buildingMap[innerXMin - 1, y] = false; // 标记为未占用
            buildingMap[innerXMin, y] = false;     // 标记为未占用
        }

        // 在城门位置生成道路连接
        PlaceRoad(southGateX, innerYMin - 1);
        PlaceRoad(northGateX, innerYMax + 1);
        PlaceRoad(innerXMax + 1, eastGateY);
        PlaceRoad(innerXMin - 1, westGateY);
    }

    /// <summary>
    /// 生成城市主干道网络
    /// </summary>
    private void GenerateMainRoads()
    {
        // 计算地图中心位置
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // 生成十字形主干道（横向和纵向）
        for (int x = cityWallPadding + 5; x < mapWidth - cityWallPadding - 5; x++)
        {
            PlaceRoad(x, centerY);  // 横向主干道
        }

        for (int y = cityWallPadding + 5; y < mapHeight - cityWallPadding - 5; y++)
        {
            PlaceRoad(centerX, y);  // 纵向主干道
        }
    }

    /// <summary>
    /// 在主干道旁生成商铺（优先沿主干道布局）
    /// </summary>
    private void GenerateShopsOnMainRoads()
    {
        int placedShops = 0;
        int maxAttempts = shopCount * 50;
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // 优先在横向主干道生成商铺
        for (int x = cityWallPadding + 10; x < mapWidth - cityWallPadding - 10; x += 4)
        {
            for (int dy = -1; dy <= 1; dy += 2)  // 主干道上下两侧
            {
                if (placedShops >= shopCount || attempts >= maxAttempts) break;

                int y = centerY + dy;
                int shopWidth = 2, shopHeight = 3;

                if (CanPlaceStructure(x, y, shopWidth, shopHeight, buildingMap))
                {
                    PlaceHouse(x, y, shopWidth, shopHeight);
                    MarkAreaAsOccupied(x, y, shopWidth, shopHeight, buildingMap);
                    placedShops++;
                }
            }
        }

        // 在纵向主干道生成商铺
        for (int y = cityWallPadding + 10; y < mapHeight - cityWallPadding - 10; y += 4)
        {
            for (int dx = -1; dx <= 1; dx += 2)  // 主干道左右两侧
            {
                if (placedShops >= shopCount || attempts >= maxAttempts) break;

                int x = centerX + dx;
                int shopWidth = 2, shopHeight = 3;

                if (CanPlaceStructure(x, y, shopWidth, shopHeight, buildingMap))
                {
                    PlaceHouse(x, y, shopWidth, shopHeight);
                    MarkAreaAsOccupied(x, y, shopWidth, shopHeight, buildingMap);
                    placedShops++;
                }
            }
        }

        // 随机补充剩余商铺
        while (placedShops < shopCount && attempts < maxAttempts)
        {
            attempts++;
            int x = Random.Range(cityWallPadding + 10, mapWidth - cityWallPadding - 10);
            int y = Random.Range(cityWallPadding + 10, mapHeight - cityWallPadding - 10);

            if (IsNearMainRoad(x, y) && CanPlaceStructure(x, y, 2, 3, buildingMap))
            {
                PlaceHouse(x, y, 2, 3);
                MarkAreaAsOccupied(x, y, 2, 3, buildingMap);
                placedShops++;
            }
        }
    }

    /// <summary>
    /// 检查是否靠近主干道
    /// </summary>
    private bool IsNearMainRoad(int x, int y)
    {
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // 检查是否在主干道附近
        return (Mathf.Abs(y - centerY) <= 2 && x >= cityWallPadding + 5 && x <= mapWidth - cityWallPadding - 5) ||
               (Mathf.Abs(x - centerX) <= 2 && y >= cityWallPadding + 5 && y <= mapHeight - cityWallPadding - 5);
    }

    /// <summary>
    /// 生成豪宅并创建到主干道的连接道路
    /// </summary>
    private void GenerateMansionsWithRoads()
    {
        int placedMansions = 0;
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;
        int innerPadding = cityWallPadding + 15; // 豪宅与城墙的最小距离

        while (placedMansions < mansionCount && attempts < 1000)
        {
            attempts++;
            int x = Random.Range(innerPadding, mapWidth - innerPadding - 10);
            int y = Random.Range(innerPadding, mapHeight - innerPadding - 10);
            int mansionWidth = 8, mansionHeight = 9;

            if (CanPlaceStructure(x, y, mansionWidth, mansionHeight, buildingMap))
            {
                // 放置豪宅围墙和房屋
                PlaceMansionWalls(x, y, mansionWidth, mansionHeight);
                PlaceMansionHouses(x, y);
                MarkAreaAsOccupied(x, y, mansionWidth, mansionHeight, buildingMap);

                // 豪宅大门位置
                int doorX = x + mansionWidth / 2;
                int doorY = y - 1;

                // 创建豪宅到主干道的连接道路
                CreateRoadToMainroad(doorX, doorY, centerX, centerY);

                placedMansions++;
            }
        }
    }

    /// <summary>
    /// 创建从指定点到主干道的直线道路
    /// </summary>
    private void CreateRoadToMainroad(int fromX, int fromY, int mainroadX, int mainroadY)
    {
        // 计算路径点
        List<Vector2Int> path = new List<Vector2Int>();

        // 水平连接段
        int currentX = fromX;
        int currentY = fromY;

        while (currentX != mainroadX)
        {
            path.Add(new Vector2Int(currentX, currentY));
            currentX += (mainroadX > currentX) ? 1 : -1;
        }

        // 垂直连接段
        while (currentY != mainroadY)
        {
            path.Add(new Vector2Int(currentX, currentY));
            currentY += (mainroadY > currentY) ? 1 : -1;
        }

        // 放置道路瓦片
        foreach (var point in path)
        {
            if (!IsOccupied(point.x, point.y, groundMap))
            {
                PlaceRoad(point.x, point.y);
            }
        }
    }

    /// <summary>
    /// 生成辅路（在主干道之间创建次级道路）
    /// </summary>
    private void GenerateSideRoads()
    {
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // 生成横向辅路
        int horizontalRoads = Random.Range(3, 6);
        for (int i = 0; i < horizontalRoads; i++)
        {
            int y = centerY + Random.Range(-20, 20);
            y = Mathf.Clamp(y, cityWallPadding + 10, mapHeight - cityWallPadding - 10);

            for (int x = cityWallPadding + 5; x < mapWidth - cityWallPadding - 5; x++)
            {
                if (!IsOccupied(x, y, groundMap))
                {
                    PlaceRoad(x, y);
                }
            }
        }

        // 生成纵向辅路
        int verticalRoads = Random.Range(3, 6);
        for (int i = 0; i < verticalRoads; i++)
        {
            int x = centerX + Random.Range(-20, 20);
            x = Mathf.Clamp(x, cityWallPadding + 10, mapWidth - cityWallPadding - 10);

            for (int y = cityWallPadding + 5; y < mapHeight - cityWallPadding - 5; y++)
            {
                if (!IsOccupied(x, y, groundMap))
                {
                    PlaceRoad(x, y);
                }
            }
        }
    }

    /// <summary>
    /// 生成民房（沿辅路和次级道路布局）
    /// </summary>
    private void GenerateHouses()
    {
        int placedHouses = 0;
        int maxAttempts = houseCount * 20;

        while (placedHouses < houseCount && attempts < maxAttempts)
        {
            attempts++;
            int x = Random.Range(cityWallPadding + 10, mapWidth - cityWallPadding - 10);
            int y = Random.Range(cityWallPadding + 10, mapHeight - cityWallPadding - 10);
            int houseWidth = 2, houseHeight = 3;

            // 优先沿道路放置
            if (IsNearRoad(x, y, houseWidth, houseHeight) &&
                CanPlaceStructure(x, y, houseWidth, houseHeight, buildingMap))
            {
                PlaceHouse(x, y, houseWidth, houseHeight);
                MarkAreaAsOccupied(x, y, houseWidth, houseHeight, buildingMap);
                placedHouses++;
            }
        }
    }

    /// <summary>
    /// 生成地形（山脉、深渊等，确保在建筑之后生成）
    /// </summary>
    private void GenerateTerrain()
    {
        GenerateMountains();      // 生成山脉
        GenerateAbysses();       // 生成深渊
        GenerateRivers();        // 生成河流
        GeneratePonds();         // 生成池塘
    }

    /// <summary>
    /// 生成山脉地形（只在城墙外生成）
    /// </summary>
    private void GenerateMountains()
    {
        // 只在城墙外生成山脉
        int mountainPadding = cityWallPadding + 10;

        // 生成随机数量的山脉集群
        int mountainCount = Random.Range(3, 6);

        for (int i = 0; i < mountainCount; i++)
        {
            // 随机决定山脉位置（城墙外）
            int side = Random.Range(0, 4); // 0:上, 1:右, 2:下, 3:左
            int x, y;

            switch (side)
            {
                case 0: // 上
                    x = Random.Range(0, mapWidth);
                    y = Random.Range(mapHeight - mountainPadding, mapHeight);
                    break;
                case 1: // 右
                    x = Random.Range(mapWidth - mountainPadding, mapWidth);
                    y = Random.Range(0, mapHeight);
                    break;
                case 2: // 下
                    x = Random.Range(0, mapWidth);
                    y = Random.Range(0, mountainPadding);
                    break;
                case 3: // 左
                    x = Random.Range(0, mountainPadding);
                    y = Random.Range(0, mapHeight);
                    break;
                default:
                    x = 0; y = 0; // 不会执行
                    break;
            }

            int size = Random.Range(5, 15);  // 山脉大小
            GenerateMountainCluster(x, y, size);
        }
    }

    /// <summary>
    /// 生成山脉集群（基于中心点和大小生成圆形分布的山脉）
    /// </summary>
    private void GenerateMountainCluster(int centerX, int centerY, int size)
    {
        for (int x = centerX - size; x <= centerX + size; x++)
        {
            for (int y = centerY - size; y <= centerY + size; y++)
            {
                if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                {
                    // 计算与中心点的距离，越远概率越低
                    float distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                    if (distance <= size && Random.value < (1 - distance / size) * 0.8f)
                    {
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), mountainTile);
                        groundMap[x, y] = true;  // 标记为已占用
                    }
                }
            }
        }
    }

    /// <summary>
    /// 生成深渊地形（不可通行的深沟或悬崖）
    /// </summary>
    private void GenerateAbysses()
    {
        // 生成随机数量的深渊区域
        int abyssCount = Random.Range(1, 4);

        for (int i = 0; i < abyssCount; i++)
        {
            int x = Random.Range(cityWallPadding + 5, mapWidth - cityWallPadding - 15);
            int y = Random.Range(cityWallPadding + 5, mapHeight - cityWallPadding - 15);
            int width = Random.Range(5, 15);
            int height = Random.Range(5, 15);

            GenerateAbyss(x, y, width, height);
        }
    }

    /// <summary>
    /// 生成矩形深渊区域
    /// </summary>
    private void GenerateAbyss(int x, int y, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int worldX = x + i;
                int worldY = y + j;

                if (worldX >= 0 && worldX < mapWidth && worldY >= 0 && worldY < mapHeight)
                {
                    groundTilemap.SetTile(new Vector3Int(worldX, worldY, 0), abyssTile);
                    groundMap[worldX, worldY] = true;  // 标记为已占用
                }
            }
        }
    }

    /// <summary>
    /// 生成河流地形
    /// </summary>
    private void GenerateRivers()
    {
        // 生成随机数量的河流
        int riverCount = Random.Range(1, 3);

        for (int i = 0; i < riverCount; i++)
        {
            // 随机决定河流是横向还是纵向
            bool isHorizontal = Random.value > 0.5f;

            if (isHorizontal)
            {
                int y = Random.Range(cityWallPadding + 10, mapHeight - cityWallPadding - 10);
                GenerateHorizontalRiver(y);  // 生成横向河流
            }
            else
            {
                int x = Random.Range(cityWallPadding + 10, mapWidth - cityWallPadding - 10);
                GenerateVerticalRiver(x);  // 生成纵向河流
            }
        }
    }

    /// <summary>
    /// 生成横向河流
    /// </summary>
    private void GenerateHorizontalRiver(int y)
    {
        int width = 3;  // 河流宽度

        for (int x = 0; x < mapWidth; x++)
        {
            for (int dy = 0; dy < width; dy++)
            {
                int worldY = y + dy - width / 2;

                if (worldY >= 0 && worldY < mapHeight)
                {
                    groundTilemap.SetTile(new Vector3Int(x, worldY, 0), riverTile);
                    groundMap[x, worldY] = true;  // 标记为已占用
                }
            }

            // 处理道路与河流交叉的情况，自动放置桥梁
            if (IsRoad(x, y))
            {
                for (int dy = 0; dy < width; dy++)
                {
                    int worldY = y + dy - width / 2;
                    if (worldY >= 0 && worldY < mapHeight)
                    {
                        groundTilemap.SetTile(new Vector3Int(x, worldY, 0), bridgeTile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 生成纵向河流
    /// </summary>
    private void GenerateVerticalRiver(int x)
    {
        int width = 3;  // 河流宽度

        for (int y = 0; y < mapHeight; y++)
        {
            for (int dx = 0; dx < width; dx++)
            {
                int worldX = x + dx - width / 2;

                if (worldX >= 0 && worldX < mapWidth)
                {
                    groundTilemap.SetTile(new Vector3Int(worldX, y, 0), riverTile);
                    groundMap[worldX, y] = true;  // 标记为已占用
                }
            }

            // 处理道路与河流交叉的情况，自动放置桥梁
            if (IsRoad(x, y))
            {
                for (int dx = 0; dx < width; dx++)
                {
                    int worldX = x + dx - width / 2;
                    if (worldX >= 0 && worldX < mapWidth)
                    {
                        groundTilemap.SetTile(new Vector3Int(worldX, y, 0), bridgeTile);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 生成池塘地形
    /// </summary>
    private void GeneratePonds()
    {
        // 生成随机数量的池塘
        int pondCount = Random.Range(3, 8);

        for (int i = 0; i < pondCount; i++)
        {
            int x = Random.Range(cityWallPadding + 5, mapWidth - cityWallPadding - 15);
            int y = Random.Range(cityWallPadding + 5, mapHeight - cityWallPadding - 15);
            int size = Random.Range(3, 8);  // 池塘大小

            GeneratePond(x, y, size);
        }
    }

    /// <summary>
    /// 生成圆形池塘
    /// </summary>
    private void GeneratePond(int centerX, int centerY, int size)
    {
        for (int x = centerX - size; x <= centerX + size; x++)
        {
            for (int y = centerY - size; y <= centerY + size; y++)
            {
                if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                {
                    // 计算与中心点的距离，越远概率越低
                    float distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                    if (distance <= size && Random.value < (1 - distance / size) * 0.9f)
                    {
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), pondTile);
                        groundMap[x, y] = true;  // 标记为已占用
                    }
                }
            }
        }
    }

    /// <summary>
    /// 生成装饰元素（最后生成，避免遮挡建筑）
    /// </summary>
    private void GenerateDecorations()
    {
        GenerateObstacles();      // 生成障碍物
    }

    /// <summary>
    /// 生成随机分布的障碍物
    /// </summary>
    private void GenerateObstacles()
    {
        // 生成随机数量的障碍物
        int obstacleCount = Random.Range(50, 100);

        for (int i = 0; i < obstacleCount; i++)
        {
            int x = Random.Range(cityWallPadding + 5, mapWidth - cityWallPadding - 5);
            int y = Random.Range(cityWallPadding + 5, mapHeight - cityWallPadding - 5);

            // 只在未被占用的草地上放置障碍物
            if (!IsOccupied(x, y, groundMap) && !IsOccupied(x, y, buildingMap) &&
                !IsRoad(x, y) && Random.value < 0.7f)
            {
                decorationTilemap.SetTile(new Vector3Int(x, y, 0), obstacleTile);
            }
        }
    }

    /// <summary>
    /// 在指定位置放置道路瓦片
    /// </summary>
    private void PlaceRoad(int x, int y)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            groundTilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
            groundMap[x, y] = true;  // 标记为已占用
        }
    }

    /// <summary>
    /// 检查指定位置是否被占用
    /// </summary>
    private bool IsOccupied(int x, int y, bool[,] map)
    {
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            return true;  // 超出地图边界视为已占用

        return map[x, y];
    }

    /// <summary>
    /// 检查指定区域是否可以放置建筑（是否未被占用）
    /// </summary>
    private bool CanPlaceStructure(int x, int y, int width, int height, bool[,] map)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int worldX = x + i;
                int worldY = y + j;

                if (worldX < 0 || worldX >= mapWidth || worldY < 0 || worldY >= mapHeight ||
                    map[worldX, worldY])
                {
                    return false;  // 只要有一个点被占用或超出边界，就不能放置
                }
            }
        }

        return true;  // 所有点都未被占用，可以放置
    }

    /// <summary>
    /// 标记指定区域为已占用
    /// </summary>
    private void MarkAreaAsOccupied(int x, int y, int width, int height, bool[,] map)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int worldX = x + i;
                int worldY = y + j;

                if (worldX >= 0 && worldX < mapWidth && worldY >= 0 && worldY < mapHeight)
                {
                    map[worldX, worldY] = true;  // 标记为已占用
                }
            }
        }
    }

    /// <summary>
    /// 检查指定区域是否靠近道路
    /// </summary>
    private bool IsNearRoad(int x, int y, int width, int height)
    {
        // 检查建筑周围一圈是否有道路
        for (int i = -1; i <= width; i++)
        {
            for (int j = -1; j <= height; j++)
            {
                int worldX = x + i;
                int worldY = y + j;

                if (worldX >= 0 && worldX < mapWidth && worldY >= 0 && worldY < mapHeight &&
                    IsRoad(worldX, worldY))
                {
                    return true;  // 只要找到一个道路瓦片，就返回true
                }
            }
        }

        return false;  // 周围没有道路
    }

    /// <summary>
    /// 检查指定位置是否是道路
    /// </summary>
    private bool IsRoad(int x, int y)
    {
        return groundTilemap.GetTile(new Vector3Int(x, y, 0)) == roadTile;
    }

    /// <summary>
    /// 放置豪宅的围墙（使用1格宽的围墙瓦片）
    /// </summary>
    private void PlaceMansionWalls(int x, int y, int width, int height)
    {
        // 放置顶部和底部围墙
        for (int i = 0; i < width; i++)
        {
            wallTilemap.SetTile(new Vector3Int(x + i, y, 0), fenceTile);                   // 底部围墙
            wallTilemap.SetTile(new Vector3Int(x + i, y + height - 1, 0), fenceTile);       // 顶部围墙
        }

        // 放置左侧和右侧围墙
        for (int i = 0; i < height; i++)
        {
            wallTilemap.SetTile(new Vector3Int(x, y + i, 0), fenceTile);                   // 左侧围墙
            wallTilemap.SetTile(new Vector3Int(x + width - 1, y + i, 0), fenceTile);       // 右侧围墙
        }

        // 留出大门位置
        wallTilemap.SetTile(new Vector3Int(x + width / 2, y, 0), null);           // 移除中间位置的围墙作为大门
        wallTilemap.SetTile(new Vector3Int(x + width / 2 - 1, y, 0), null);       // 移除中间偏左位置的围墙作为大门
    }

    /// <summary>
    /// 放置豪宅内部的房屋（两个2x3的侧房和一个2x7的主房）
    /// </summary>
    private void PlaceMansionHouses(int x, int y)
    {
        // 放置左侧房屋 (2x3)
        PlaceHouse(x + 1, y + 3, 2, 3);

        // 放置右侧房屋 (2x3)
        PlaceHouse(x + 5, y + 3, 2, 3);

        // 放置底部主房 (2x7)
        PlaceHouse(x + 2, y + 1, 2, 7);
    }

    /// <summary>
    /// 放置普通房屋（在指定位置填充房屋瓦片）
    /// </summary>
    private void PlaceHouse(int x, int y, int width, int height)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                buildingTilemap.SetTile(new Vector3Int(x + i, y + j, 0), houseTile);
            }
        }
    }
}