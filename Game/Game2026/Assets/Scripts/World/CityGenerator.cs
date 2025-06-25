using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ���������� - �Ż�����˳�򣺳�ǽ�����ɵ������̡���լ����·���񷿡����Ρ�װ��
/// </summary>
public class CityGenerator : MonoBehaviour
{
    // ��ͼ�ߴ�����
    [Header("��ͼ����")]
    public int mapWidth = 100;
    public int mapHeight = 100;

    // ��Ƭ��Դ����
    [Header("��Ƭ��Դ")]
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

    // ������������
    [Header("��������")]
    public int mansionCount = 10;
    public int shopCount = 10;
    public int houseCount = 40;

    // ��ǽ����
    [Header("��ǽ����")]
    public int cityWallWidth = 2;
    public int cityWallPadding = 5;

    // Tilemap�������
    [Header("Tilemap����")]
    public Tilemap groundTilemap;
    public Tilemap buildingTilemap;
    public Tilemap wallTilemap;
    public Tilemap decorationTilemap;

    // ��ͼ����
    private bool[,] groundMap;
    private bool[,] buildingMap;

    // ���Դ���������
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
    /// ����˳�����ɳ��У���ǽ�����ɵ������̡���լ����·���񷿡����Ρ�װ��
    /// </summary>
    public void GenerateCity()
    {
        attempts = 0;
        InitializeMaps();                 // ��ʼ����ͼ
        GenerateCityWalls();              // ���ɳ�ǽ
        GenerateMainRoads();              // �������ɵ�
        GenerateShopsOnMainRoads();       // ���ɵ�����������
        GenerateMansionsWithRoads();      // ���ɺ�լ�����ӵ�·
        GenerateSideRoads();              // ���ɸ�·
        GenerateHouses();                 // ������
        GenerateTerrain();                // ���ɵ��Σ�ɽ���ȣ�
        GenerateDecorations();            // ����װ��Ԫ��

        Debug.Log("�����������!");
    }

    /// <summary>
    /// ��ʼ����ͼ���ݽṹ�����������Ƭ
    /// </summary>
    private void InitializeMaps()
    {
        // ��ʼ����ͼռ������
        groundMap = new bool[mapWidth, mapHeight];
        buildingMap = new bool[mapWidth, mapHeight];

        // �������Tilemap�ϵ�������Ƭ
        groundTilemap.ClearAllTiles();
        buildingTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        decorationTilemap.ClearAllTiles();

        // �òݵ���Ƭ���������ͼ��Ϊ����
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
            }
        }
    }

    /// <summary>
    /// ���ɻ����������еĳ�ǽ
    /// </summary>
    private void GenerateCityWalls()
    {
        // ��ǽ�ڱ߽�
        int innerXMin = cityWallPadding;
        int innerXMax = mapWidth - cityWallPadding - 1;
        int innerYMin = cityWallPadding;
        int innerYMax = mapHeight - cityWallPadding - 1;

        // ��ǽ��߽�
        int outerXMin = innerXMin - cityWallWidth;
        int outerXMax = innerXMax + cityWallWidth;
        int outerYMin = innerYMin - cityWallWidth;
        int outerYMax = innerYMax + cityWallWidth;

        // ȷ���߽�����Ч��Χ��
        outerXMin = Mathf.Max(0, outerXMin);
        outerXMax = Mathf.Min(mapWidth - 1, outerXMax);
        outerYMin = Mathf.Max(0, outerYMin);
        outerYMax = Mathf.Min(mapHeight - 1, outerYMax);

        // ����ǽ����
        for (int x = outerXMin; x <= outerXMax; x++)
        {
            for (int y = outerYMin; y <= outerYMax; y++)
            {
                // ����Ƿ��ڳ�ǽ��Χ�ڵ������ڲ�����
                if ((x < innerXMin || x > innerXMax || y < innerYMin || y > innerYMax))
                {
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), cityWallTile);
                    groundMap[x, y] = true;  // ���Ϊ��ռ��
                    buildingMap[x, y] = true; // ���Ϊ��ռ��
                }
            }
        }

        // ���ɳ��ţ��򵥵����ĸ��������һ���ţ�
        GenerateCityGates(innerXMin, innerXMax, innerYMin, innerYMax);
    }

    /// <summary>
    /// ���ɳ���
    /// </summary>
    private void GenerateCityGates(int innerXMin, int innerXMax, int innerYMin, int innerYMax)
    {
        int gateWidth = 3;  // ���ſ��

        // �ϳ���
        int southGateX = (innerXMin + innerXMax) / 2;
        for (int x = southGateX - gateWidth / 2; x <= southGateX + gateWidth / 2; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, innerYMin - 1, 0), null);  // �Ƴ���ǽ��Ƭ
            wallTilemap.SetTile(new Vector3Int(x, innerYMin, 0), null);      // �Ƴ���ǽ��Ƭ
            groundMap[x, innerYMin - 1] = false;  // ���Ϊδռ��
            groundMap[x, innerYMin] = false;      // ���Ϊδռ��
            buildingMap[x, innerYMin - 1] = false; // ���Ϊδռ��
            buildingMap[x, innerYMin] = false;     // ���Ϊδռ��
        }

        // ������
        int northGateX = (innerXMin + innerXMax) / 2;
        for (int x = northGateX - gateWidth / 2; x <= northGateX + gateWidth / 2; x++)
        {
            wallTilemap.SetTile(new Vector3Int(x, innerYMax, 0), null);      // �Ƴ���ǽ��Ƭ
            wallTilemap.SetTile(new Vector3Int(x, innerYMax + 1, 0), null);  // �Ƴ���ǽ��Ƭ
            groundMap[x, innerYMax] = false;  // ���Ϊδռ��
            groundMap[x, innerYMax + 1] = false;  // ���Ϊδռ��
            buildingMap[x, innerYMax] = false; // ���Ϊδռ��
            buildingMap[x, innerYMax + 1] = false; // ���Ϊδռ��
        }

        // ������
        int eastGateY = (innerYMin + innerYMax) / 2;
        for (int y = eastGateY - gateWidth / 2; y <= eastGateY + gateWidth / 2; y++)
        {
            wallTilemap.SetTile(new Vector3Int(innerXMax, y), null);      // �Ƴ���ǽ��Ƭ
            wallTilemap.SetTile(new Vector3Int(innerXMax + 1, y), null);  // �Ƴ���ǽ��Ƭ
            groundMap[innerXMax, y] = false;  // ���Ϊδռ��
            groundMap[innerXMax + 1, y] = false;  // ���Ϊδռ��
            buildingMap[innerXMax, y] = false; // ���Ϊδռ��
            buildingMap[innerXMax + 1, y] = false; // ���Ϊδռ��
        }

        // ������
        int westGateY = (innerYMin + innerYMax) / 2;
        for (int y = westGateY - gateWidth / 2; y <= westGateY + gateWidth / 2; y++)
        {
            wallTilemap.SetTile(new Vector3Int(innerXMin - 1, y), null);  // �Ƴ���ǽ��Ƭ
            wallTilemap.SetTile(new Vector3Int(innerXMin, y), null);      // �Ƴ���ǽ��Ƭ
            groundMap[innerXMin - 1, y] = false;  // ���Ϊδռ��
            groundMap[innerXMin, y] = false;      // ���Ϊδռ��
            buildingMap[innerXMin - 1, y] = false; // ���Ϊδռ��
            buildingMap[innerXMin, y] = false;     // ���Ϊδռ��
        }

        // �ڳ���λ�����ɵ�·����
        PlaceRoad(southGateX, innerYMin - 1);
        PlaceRoad(northGateX, innerYMax + 1);
        PlaceRoad(innerXMax + 1, eastGateY);
        PlaceRoad(innerXMin - 1, westGateY);
    }

    /// <summary>
    /// ���ɳ������ɵ�����
    /// </summary>
    private void GenerateMainRoads()
    {
        // �����ͼ����λ��
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // ����ʮ�������ɵ������������
        for (int x = cityWallPadding + 5; x < mapWidth - cityWallPadding - 5; x++)
        {
            PlaceRoad(x, centerY);  // �������ɵ�
        }

        for (int y = cityWallPadding + 5; y < mapHeight - cityWallPadding - 5; y++)
        {
            PlaceRoad(centerX, y);  // �������ɵ�
        }
    }

    /// <summary>
    /// �����ɵ����������̣����������ɵ����֣�
    /// </summary>
    private void GenerateShopsOnMainRoads()
    {
        int placedShops = 0;
        int maxAttempts = shopCount * 50;
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // �����ں������ɵ���������
        for (int x = cityWallPadding + 10; x < mapWidth - cityWallPadding - 10; x += 4)
        {
            for (int dy = -1; dy <= 1; dy += 2)  // ���ɵ���������
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

        // ���������ɵ���������
        for (int y = cityWallPadding + 10; y < mapHeight - cityWallPadding - 10; y += 4)
        {
            for (int dx = -1; dx <= 1; dx += 2)  // ���ɵ���������
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

        // �������ʣ������
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
    /// ����Ƿ񿿽����ɵ�
    /// </summary>
    private bool IsNearMainRoad(int x, int y)
    {
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // ����Ƿ������ɵ�����
        return (Mathf.Abs(y - centerY) <= 2 && x >= cityWallPadding + 5 && x <= mapWidth - cityWallPadding - 5) ||
               (Mathf.Abs(x - centerX) <= 2 && y >= cityWallPadding + 5 && y <= mapHeight - cityWallPadding - 5);
    }

    /// <summary>
    /// ���ɺ�լ�����������ɵ������ӵ�·
    /// </summary>
    private void GenerateMansionsWithRoads()
    {
        int placedMansions = 0;
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;
        int innerPadding = cityWallPadding + 15; // ��լ���ǽ����С����

        while (placedMansions < mansionCount && attempts < 1000)
        {
            attempts++;
            int x = Random.Range(innerPadding, mapWidth - innerPadding - 10);
            int y = Random.Range(innerPadding, mapHeight - innerPadding - 10);
            int mansionWidth = 8, mansionHeight = 9;

            if (CanPlaceStructure(x, y, mansionWidth, mansionHeight, buildingMap))
            {
                // ���ú�լΧǽ�ͷ���
                PlaceMansionWalls(x, y, mansionWidth, mansionHeight);
                PlaceMansionHouses(x, y);
                MarkAreaAsOccupied(x, y, mansionWidth, mansionHeight, buildingMap);

                // ��լ����λ��
                int doorX = x + mansionWidth / 2;
                int doorY = y - 1;

                // ������լ�����ɵ������ӵ�·
                CreateRoadToMainroad(doorX, doorY, centerX, centerY);

                placedMansions++;
            }
        }
    }

    /// <summary>
    /// ������ָ���㵽���ɵ���ֱ�ߵ�·
    /// </summary>
    private void CreateRoadToMainroad(int fromX, int fromY, int mainroadX, int mainroadY)
    {
        // ����·����
        List<Vector2Int> path = new List<Vector2Int>();

        // ˮƽ���Ӷ�
        int currentX = fromX;
        int currentY = fromY;

        while (currentX != mainroadX)
        {
            path.Add(new Vector2Int(currentX, currentY));
            currentX += (mainroadX > currentX) ? 1 : -1;
        }

        // ��ֱ���Ӷ�
        while (currentY != mainroadY)
        {
            path.Add(new Vector2Int(currentX, currentY));
            currentY += (mainroadY > currentY) ? 1 : -1;
        }

        // ���õ�·��Ƭ
        foreach (var point in path)
        {
            if (!IsOccupied(point.x, point.y, groundMap))
            {
                PlaceRoad(point.x, point.y);
            }
        }
    }

    /// <summary>
    /// ���ɸ�·�������ɵ�֮�䴴���μ���·��
    /// </summary>
    private void GenerateSideRoads()
    {
        int centerX = mapWidth / 2;
        int centerY = mapHeight / 2;

        // ���ɺ���·
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

        // ��������·
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
    /// �����񷿣��ظ�·�ʹμ���·���֣�
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

            // �����ص�·����
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
    /// ���ɵ��Σ�ɽ������Ԩ�ȣ�ȷ���ڽ���֮�����ɣ�
    /// </summary>
    private void GenerateTerrain()
    {
        GenerateMountains();      // ����ɽ��
        GenerateAbysses();       // ������Ԩ
        GenerateRivers();        // ���ɺ���
        GeneratePonds();         // ���ɳ���
    }

    /// <summary>
    /// ����ɽ�����Σ�ֻ�ڳ�ǽ�����ɣ�
    /// </summary>
    private void GenerateMountains()
    {
        // ֻ�ڳ�ǽ������ɽ��
        int mountainPadding = cityWallPadding + 10;

        // �������������ɽ����Ⱥ
        int mountainCount = Random.Range(3, 6);

        for (int i = 0; i < mountainCount; i++)
        {
            // �������ɽ��λ�ã���ǽ�⣩
            int side = Random.Range(0, 4); // 0:��, 1:��, 2:��, 3:��
            int x, y;

            switch (side)
            {
                case 0: // ��
                    x = Random.Range(0, mapWidth);
                    y = Random.Range(mapHeight - mountainPadding, mapHeight);
                    break;
                case 1: // ��
                    x = Random.Range(mapWidth - mountainPadding, mapWidth);
                    y = Random.Range(0, mapHeight);
                    break;
                case 2: // ��
                    x = Random.Range(0, mapWidth);
                    y = Random.Range(0, mountainPadding);
                    break;
                case 3: // ��
                    x = Random.Range(0, mountainPadding);
                    y = Random.Range(0, mapHeight);
                    break;
                default:
                    x = 0; y = 0; // ����ִ��
                    break;
            }

            int size = Random.Range(5, 15);  // ɽ����С
            GenerateMountainCluster(x, y, size);
        }
    }

    /// <summary>
    /// ����ɽ����Ⱥ���������ĵ�ʹ�С����Բ�ηֲ���ɽ����
    /// </summary>
    private void GenerateMountainCluster(int centerX, int centerY, int size)
    {
        for (int x = centerX - size; x <= centerX + size; x++)
        {
            for (int y = centerY - size; y <= centerY + size; y++)
            {
                if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                {
                    // ���������ĵ�ľ��룬ԽԶ����Խ��
                    float distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                    if (distance <= size && Random.value < (1 - distance / size) * 0.8f)
                    {
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), mountainTile);
                        groundMap[x, y] = true;  // ���Ϊ��ռ��
                    }
                }
            }
        }
    }

    /// <summary>
    /// ������Ԩ���Σ�����ͨ�е�������£�
    /// </summary>
    private void GenerateAbysses()
    {
        // ���������������Ԩ����
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
    /// ���ɾ�����Ԩ����
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
                    groundMap[worldX, worldY] = true;  // ���Ϊ��ռ��
                }
            }
        }
    }

    /// <summary>
    /// ���ɺ�������
    /// </summary>
    private void GenerateRivers()
    {
        // ������������ĺ���
        int riverCount = Random.Range(1, 3);

        for (int i = 0; i < riverCount; i++)
        {
            // ������������Ǻ���������
            bool isHorizontal = Random.value > 0.5f;

            if (isHorizontal)
            {
                int y = Random.Range(cityWallPadding + 10, mapHeight - cityWallPadding - 10);
                GenerateHorizontalRiver(y);  // ���ɺ������
            }
            else
            {
                int x = Random.Range(cityWallPadding + 10, mapWidth - cityWallPadding - 10);
                GenerateVerticalRiver(x);  // �����������
            }
        }
    }

    /// <summary>
    /// ���ɺ������
    /// </summary>
    private void GenerateHorizontalRiver(int y)
    {
        int width = 3;  // �������

        for (int x = 0; x < mapWidth; x++)
        {
            for (int dy = 0; dy < width; dy++)
            {
                int worldY = y + dy - width / 2;

                if (worldY >= 0 && worldY < mapHeight)
                {
                    groundTilemap.SetTile(new Vector3Int(x, worldY, 0), riverTile);
                    groundMap[x, worldY] = true;  // ���Ϊ��ռ��
                }
            }

            // �����·����������������Զ���������
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
    /// �����������
    /// </summary>
    private void GenerateVerticalRiver(int x)
    {
        int width = 3;  // �������

        for (int y = 0; y < mapHeight; y++)
        {
            for (int dx = 0; dx < width; dx++)
            {
                int worldX = x + dx - width / 2;

                if (worldX >= 0 && worldX < mapWidth)
                {
                    groundTilemap.SetTile(new Vector3Int(worldX, y, 0), riverTile);
                    groundMap[worldX, y] = true;  // ���Ϊ��ռ��
                }
            }

            // �����·����������������Զ���������
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
    /// ���ɳ�������
    /// </summary>
    private void GeneratePonds()
    {
        // ������������ĳ���
        int pondCount = Random.Range(3, 8);

        for (int i = 0; i < pondCount; i++)
        {
            int x = Random.Range(cityWallPadding + 5, mapWidth - cityWallPadding - 15);
            int y = Random.Range(cityWallPadding + 5, mapHeight - cityWallPadding - 15);
            int size = Random.Range(3, 8);  // ������С

            GeneratePond(x, y, size);
        }
    }

    /// <summary>
    /// ����Բ�γ���
    /// </summary>
    private void GeneratePond(int centerX, int centerY, int size)
    {
        for (int x = centerX - size; x <= centerX + size; x++)
        {
            for (int y = centerY - size; y <= centerY + size; y++)
            {
                if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                {
                    // ���������ĵ�ľ��룬ԽԶ����Խ��
                    float distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                    if (distance <= size && Random.value < (1 - distance / size) * 0.9f)
                    {
                        groundTilemap.SetTile(new Vector3Int(x, y, 0), pondTile);
                        groundMap[x, y] = true;  // ���Ϊ��ռ��
                    }
                }
            }
        }
    }

    /// <summary>
    /// ����װ��Ԫ�أ�������ɣ������ڵ�������
    /// </summary>
    private void GenerateDecorations()
    {
        GenerateObstacles();      // �����ϰ���
    }

    /// <summary>
    /// ��������ֲ����ϰ���
    /// </summary>
    private void GenerateObstacles()
    {
        // ��������������ϰ���
        int obstacleCount = Random.Range(50, 100);

        for (int i = 0; i < obstacleCount; i++)
        {
            int x = Random.Range(cityWallPadding + 5, mapWidth - cityWallPadding - 5);
            int y = Random.Range(cityWallPadding + 5, mapHeight - cityWallPadding - 5);

            // ֻ��δ��ռ�õĲݵ��Ϸ����ϰ���
            if (!IsOccupied(x, y, groundMap) && !IsOccupied(x, y, buildingMap) &&
                !IsRoad(x, y) && Random.value < 0.7f)
            {
                decorationTilemap.SetTile(new Vector3Int(x, y, 0), obstacleTile);
            }
        }
    }

    /// <summary>
    /// ��ָ��λ�÷��õ�·��Ƭ
    /// </summary>
    private void PlaceRoad(int x, int y)
    {
        if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
        {
            groundTilemap.SetTile(new Vector3Int(x, y, 0), roadTile);
            groundMap[x, y] = true;  // ���Ϊ��ռ��
        }
    }

    /// <summary>
    /// ���ָ��λ���Ƿ�ռ��
    /// </summary>
    private bool IsOccupied(int x, int y, bool[,] map)
    {
        if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            return true;  // ������ͼ�߽���Ϊ��ռ��

        return map[x, y];
    }

    /// <summary>
    /// ���ָ�������Ƿ���Է��ý������Ƿ�δ��ռ�ã�
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
                    return false;  // ֻҪ��һ���㱻ռ�û򳬳��߽磬�Ͳ��ܷ���
                }
            }
        }

        return true;  // ���е㶼δ��ռ�ã����Է���
    }

    /// <summary>
    /// ���ָ������Ϊ��ռ��
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
                    map[worldX, worldY] = true;  // ���Ϊ��ռ��
                }
            }
        }
    }

    /// <summary>
    /// ���ָ�������Ƿ񿿽���·
    /// </summary>
    private bool IsNearRoad(int x, int y, int width, int height)
    {
        // ��齨����ΧһȦ�Ƿ��е�·
        for (int i = -1; i <= width; i++)
        {
            for (int j = -1; j <= height; j++)
            {
                int worldX = x + i;
                int worldY = y + j;

                if (worldX >= 0 && worldX < mapWidth && worldY >= 0 && worldY < mapHeight &&
                    IsRoad(worldX, worldY))
                {
                    return true;  // ֻҪ�ҵ�һ����·��Ƭ���ͷ���true
                }
            }
        }

        return false;  // ��Χû�е�·
    }

    /// <summary>
    /// ���ָ��λ���Ƿ��ǵ�·
    /// </summary>
    private bool IsRoad(int x, int y)
    {
        return groundTilemap.GetTile(new Vector3Int(x, y, 0)) == roadTile;
    }

    /// <summary>
    /// ���ú�լ��Χǽ��ʹ��1����Χǽ��Ƭ��
    /// </summary>
    private void PlaceMansionWalls(int x, int y, int width, int height)
    {
        // ���ö����͵ײ�Χǽ
        for (int i = 0; i < width; i++)
        {
            wallTilemap.SetTile(new Vector3Int(x + i, y, 0), fenceTile);                   // �ײ�Χǽ
            wallTilemap.SetTile(new Vector3Int(x + i, y + height - 1, 0), fenceTile);       // ����Χǽ
        }

        // ���������Ҳ�Χǽ
        for (int i = 0; i < height; i++)
        {
            wallTilemap.SetTile(new Vector3Int(x, y + i, 0), fenceTile);                   // ���Χǽ
            wallTilemap.SetTile(new Vector3Int(x + width - 1, y + i, 0), fenceTile);       // �Ҳ�Χǽ
        }

        // ��������λ��
        wallTilemap.SetTile(new Vector3Int(x + width / 2, y, 0), null);           // �Ƴ��м�λ�õ�Χǽ��Ϊ����
        wallTilemap.SetTile(new Vector3Int(x + width / 2 - 1, y, 0), null);       // �Ƴ��м�ƫ��λ�õ�Χǽ��Ϊ����
    }

    /// <summary>
    /// ���ú�լ�ڲ��ķ��ݣ�����2x3�Ĳ෿��һ��2x7��������
    /// </summary>
    private void PlaceMansionHouses(int x, int y)
    {
        // ������෿�� (2x3)
        PlaceHouse(x + 1, y + 3, 2, 3);

        // �����Ҳ෿�� (2x3)
        PlaceHouse(x + 5, y + 3, 2, 3);

        // ���õײ����� (2x7)
        PlaceHouse(x + 2, y + 1, 2, 7);
    }

    /// <summary>
    /// ������ͨ���ݣ���ָ��λ����䷿����Ƭ��
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