using UnityEditor;
using UnityEngine;
using System.IO;
using Codice.Client.BaseCommands;
using System.Collections.Generic;
using log4net.Util;
using TreeEditor;

public class MapEditorWindow : EditorWindow
{
    private MapGeneratorConfig mapConfig;
    private string savePath = "";
    private string loadPath = "";
    private int[,] currentMapData;
    private Vector2Int currentMapSize;
    private Texture2D mapTexture;
    private Vector2 scrollPosition;
    private bool showGeneratedMap = false;
    private float lastWindowWidth = 0;

    // 颜色配置
    private static Color bridgeColor = new Color(0.54f, 0.27f, 0.07f); // 褐色
    private static Color sectColor = new Color(0.87f, 0.72f, 0.53f); // 土黄色
    private static Color tribeColor = new Color(0.98f, 0.92f, 0.70f); // 浅黄色
    private static Color cityColor = new Color(0.25f, 0.5f, 0.75f); // 蓝色
    private static Color villageColor = new Color(0.53f, 0.81f, 0.98f); // 浅蓝色
    private static Color dockColor = new Color(0.8f, 0.2f, 0.2f); // 红色
    private static Color routeColor = new Color(0.9f, 0.5f, 0.7f); // 粉红色
    private static Color riverColor = new Color(1, 1, 0f); // 黄色
    private static Color roadColor = new Color(1f, 0f, 0f); // 大红色
    private static Color forestColor = new Color(0.0f, 0.3f, 0.0f); // 墨绿色
    private static Color mountainColor = new Color(0.22f, 0.15f, 0.10f); // 深褐色
    private static Color plainColor = new Color(0.6f, 0.9f, 0.4f); // 淡绿色
    private static Color lakeColor = new Color(0.2f, 0.8f, 0.8f); // 青色
    private static Color oceanColor = new Color(0.0f, 0.3f, 0.5f); // 暗青色
    private int customTextureWidth = -1; // 新增变量，用于记录自定义的图片宽度

    private TerrainFromMapData terrainData;

    // 新增：地形类型与颜色、说明的映射，用于右侧显示
    private Dictionary<int, (Color, string)> terrainColorInfo = new Dictionary<int, (Color, string)>()
    {
        [(int)MapData.Bridge] = (bridgeColor, "桥"),
        [(int)MapData.Sect] = (sectColor, "门派"),
        [(int)MapData.Tribe] = (tribeColor, "部落"),
        [(int)MapData.City] = (cityColor, "城市"),
        [(int)MapData.Village] = (villageColor, "村庄"),
        [(int)MapData.Dock] = (dockColor, "码头"),
        [(int)MapData.Route] = (routeColor, "航线"),
        [(int)MapData.River] = (riverColor, "河流"),
        [(int)MapData.Road] = (roadColor, "道路"),
        [(int)MapData.Forest] = (forestColor, "森林"),
        [(int)MapData.Mountain] = (mountainColor, "山脉"),
        [(int)MapData.Lake] = (lakeColor, "湖泊"),
        [(int)MapData.Plain] = (plainColor, "平原"),
        [(int)MapData.Ocean] = (oceanColor, "海洋"),
    };

    [MenuItem("地图/游戏世界生成器")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindow>("游戏世界生成器");
    }

    private void OnEnable()
    {
        // 尝试加载默认配置
        string[] configGuids = AssetDatabase.FindAssets("t:MapGeneratorConfig");
        if (configGuids.Length > 0)
        {
            string configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
            mapConfig = AssetDatabase.LoadAssetAtPath<MapGeneratorConfig>(configPath);
        }

        // 设置默认保存路径
        savePath = Path.Combine(Application.dataPath, "GeneratedMaps", "new_map.txt");

        // 记录初始窗口宽度
        lastWindowWidth = position.width;
    }

    private void OnGUI()
    {
        // 检查窗口宽度是否变化，如果变化则需要重新计算预览尺寸
        if (Mathf.Abs(position.width - lastWindowWidth) > 10)
        {
            lastWindowWidth = position.width;
        }

        // 开始滚动视图
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // 地图图像导出设置
        GUILayout.Label("地图图像导出设置", EditorStyles.boldLabel);
        customTextureWidth = EditorGUILayout.IntField("自定义宽度[-1为原尺寸]", customTextureWidth);

        // 地图配置
        GUILayout.Label("地图配置", EditorStyles.boldLabel);
        mapConfig = (MapGeneratorConfig)EditorGUILayout.ObjectField(
            "地图配置文件",
            mapConfig,
            typeof(MapGeneratorConfig),
            false
        );

        if (mapConfig == null)
        {
            EditorGUILayout.HelpBox("请指定地图配置文件，或创建新的配置文件。", MessageType.Warning);
            if (GUILayout.Button("创建默认配置"))
            {
                CreateDefaultConfig();
            }
        }

        // 地图生成
        EditorGUILayout.Space();
        GUILayout.Label("地图生成", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存路径:", GUILayout.Width(70));
        savePath = EditorGUILayout.TextField(savePath);
        if (GUILayout.Button("浏览...", GUILayout.Width(60)))
        {
            string defaultName = "map_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string path = EditorUtility.SaveFilePanel("保存地图", Application.dataPath, defaultName, "txt");
            if (!string.IsNullOrEmpty(path))
            {
                savePath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("生成并保存随机地图"))
        {
            GenerateAndSaveMap();
        }

        // 地图加载与可视化
        EditorGUILayout.Space();
        GUILayout.Label("地图加载与可视化", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("加载路径:", GUILayout.Width(70));
        loadPath = EditorGUILayout.TextField(loadPath);
        if (GUILayout.Button("浏览...", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFilePanel("加载地图", Application.dataPath, "txt");
            if (!string.IsNullOrEmpty(path))
            {
                loadPath = path;
                LoadAndVisualizeMap();
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("加载并显示地图"))
        {
            LoadAndVisualizeMap();
        }

        // 地图预览
        EditorGUILayout.Space();
        GUILayout.Label("地图预览", EditorStyles.boldLabel);

        if (mapTexture != null)
        {
            // 计算可用宽度（窗口宽度减去边距）
            float availableWidth = position.width - 40; // 减去左右边距

            // 计算预览尺寸，保持宽高比
            float aspectRatio = (float)mapTexture.height / mapTexture.width;
            float previewWidth = Mathf.Max(availableWidth, mapTexture.width);
            float previewHeight = previewWidth * aspectRatio;


            // 如果预览高度超过窗口高度的一半，调整尺寸
            float maxPreviewHeight = position.height * 0.6f; // 最大预览高度为窗口高度的60%
            if (previewHeight > maxPreviewHeight)
            {
                previewHeight = maxPreviewHeight;
                previewWidth = previewHeight / aspectRatio;
            }

            // 显示地图纹理
            GUILayout.BeginHorizontal();
            {
                // 地图预览占主要区域
                GUILayout.BeginVertical(GUILayout.Width(position.width * 0.7f));
                {
                    GUILayout.Label($"地图尺寸: {currentMapSize.x}x{currentMapSize.y} 像素   显示尺寸：{previewWidth}x{previewHeight}");
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(mapTexture, GUILayout.Width(previewWidth), GUILayout.Height(previewHeight));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    if (GUILayout.Button("保存地图图像"))
                    {
                        SaveMapTexture();
                    }
                }
                GUILayout.EndVertical();

                // 地形颜色说明区域，占右侧 30% 宽度
                GUILayout.BeginVertical(GUILayout.Width(position.width * 0.3f));
                {
                    GUILayout.Label("地形颜色说明", EditorStyles.boldLabel);
                    foreach (var kvp in terrainColorInfo)
                    {
                        GUILayout.BeginHorizontal();
                        // 绘制颜色块
                        GUILayout.Label("", GUILayout.Width(20), GUILayout.Height(20));
                        GUI.backgroundColor = kvp.Value.Item1;
                        if (GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20)))
                        {
                            // 可扩展点击逻辑，比如选中地形高亮等，这里先空着
                        }
                        GUI.backgroundColor = Color.white;
                        // 绘制地形名称
                        GUILayout.Label(kvp.Value.Item2, GUILayout.ExpandWidth(true));
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
        else if (showGeneratedMap)
        {
            EditorGUILayout.HelpBox("未生成地图纹理，请先加载或生成地图。", MessageType.Info);
        }
        terrainData = (TerrainFromMapData)EditorGUILayout.ObjectField(
            "地形处理组件",  // 字段标签
            terrainData,     // 当前值
            typeof(TerrainFromMapData),  // 允许的类型
            true  // 是否允许场景中的对象（如果是MonoBehaviour组件则需要设为true）
        );

        if (terrainData != null && GUILayout.Button("设置地形高度"))
        {
            TerrainData data = terrainData.targetTerrain.terrainData;
            new TerrainHeightSetter(data, terrainData.mapData).SetHeights();
        }

        if (terrainData != null && GUILayout.Button("设置地图纹理"))
        {
            TerrainData data = terrainData.targetTerrain.terrainData;
            new TerrainTextureSetter(data, terrainData.mapData, terrainData._terrainLayers, terrainData._layerOrder).SetTextures();
        }

        if (terrainData != null && GUILayout.Button("设置地形树木"))
        {
            TerrainData data = terrainData.targetTerrain.terrainData;
            new TerrainTreeSetter(data, terrainData.mapData, terrainData.treesData).SetTrees(terrainData.transform);
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreateDefaultConfig()
    {
        // 创建新的配置实例
        MapGeneratorConfig config = ScriptableObject.CreateInstance<MapGeneratorConfig>();

        // 设置默认值
        config.mapSize = new Vector2Int(100, 100);
        config.sectCount = 3;
        config.cityCount = 5;
        config.villageCount = 15;
        config.tribeCount = 10;
        config.sectSize = 5;
        config.citySize = 4;
        config.villageSize = 2;
        config.tribeSize = 2;
        config.townSectMinDistance = 10;
        config.villageTribeMinDistance = 6;

        // 确保保存目录存在
        string configDirectory = Path.Combine(Application.dataPath, "MapConfigs");
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        // 保存配置文件
        string configPath = Path.Combine(configDirectory, "DefaultMapConfig.asset");
        AssetDatabase.CreateAsset(config, configPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        mapConfig = config;
        EditorUtility.DisplayDialog("配置创建成功", "已创建默认地图配置文件。", "确定");
    }

    private void GenerateAndSaveMap()
    {
        if (mapConfig == null)
        {
            EditorUtility.DisplayDialog("错误", "请先指定或创建地图配置文件。", "确定");
            return;
        }

        if (string.IsNullOrEmpty(savePath))
        {
            EditorUtility.DisplayDialog("错误", "请指定保存路径。", "确定");
            return;
        }

        // 生成地图
        currentMapData = MapGeneratorUtility.GenerateMap(mapConfig);
        currentMapSize = mapConfig.mapSize;

        if (currentMapData != null)
        {
            // 保存地图
            MapGeneratorUtility.SaveMapToFile(currentMapData, currentMapSize, savePath);

            // 生成地图纹理
            GenerateMapTexture();
            showGeneratedMap = true;

            // 刷新资源窗口
            AssetDatabase.Refresh();

            Debug.Log("地图已生成并保存到:" + savePath);
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "地图生成失败，请查看控制台日志。", "确定");
        }
    }

    private void LoadAndVisualizeMap()
    {
        if (string.IsNullOrEmpty(loadPath) || !File.Exists(loadPath))
        {
            EditorUtility.DisplayDialog("错误", "请指定有效的地图文件路径。", "确定");
            return;
        }

        // 加载地图
        currentMapData = MapGeneratorUtility.LoadMapFromFile(loadPath, out currentMapSize);

        if (currentMapData != null)
        {
            // 生成地图纹理
            GenerateMapTexture();
            showGeneratedMap = true;
            EditorUtility.DisplayDialog("成功", "地图已加载，尺寸: " + currentMapSize.x + "x" + currentMapSize.y, "确定");
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "地图加载失败，请查看控制台日志。", "确定");
        }
    }

    private void GenerateMapTexture()
    {
        if (currentMapData == null || currentMapSize.x <= 0 || currentMapSize.y <= 0)
        {
            Debug.LogError("无法生成地图纹理，地图数据无效。");
            return;
        }

        int targetWidth = customTextureWidth;
        int targetHeight = currentMapSize.y;

        // 如果自定义宽度为 -1，使用原图尺寸
        if (targetWidth == -1)
        {
            targetWidth = currentMapSize.x;
            targetHeight = currentMapSize.y;
        }
        else
        {
            // 根据原图宽高比计算目标高度
            float aspectRatio = (float)currentMapSize.y / currentMapSize.x;
            targetHeight = Mathf.RoundToInt(targetWidth * aspectRatio);
        }

        // 创建与目标尺寸相同的纹理
        if (mapTexture != null)
        {
            DestroyImmediate(mapTexture);
        }

        mapTexture = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, false);
        mapTexture.filterMode = UnityEngine.FilterMode.Bilinear; // 使用双线性插值，让缩放后的纹理更平滑
        Color[] pixels = new Color[targetWidth * targetHeight];

        // 遍历每个目标像素，通过插值计算颜色
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                // 计算在原图中的对应坐标（浮点型，用于插值）
                float sourceX = (float)(targetWidth - 1 - x) / targetWidth * currentMapSize.x;
                float sourceY = (float)y / targetHeight * currentMapSize.y;

                // 取整得到周围像素坐标，进行双线性插值
                int x1 = Mathf.Clamp(Mathf.FloorToInt(sourceX), 0, currentMapSize.x - 1);
                int y1 = Mathf.Clamp(Mathf.FloorToInt(sourceY), 0, currentMapSize.y - 1);
                int x2 = Mathf.Clamp(x1 + 1, 0, currentMapSize.x - 1);
                int y2 = Mathf.Clamp(y1 + 1, 0, currentMapSize.y - 1);

                float dx = sourceX - x1;
                float dy = sourceY - y1;

                Color color11 = GetCellColor(x1, y1);
                Color color12 = GetCellColor(x1, y2);
                Color color21 = GetCellColor(x2, y1);
                Color color22 = GetCellColor(x2, y2);

                // 双线性插值计算最终颜色
                Color color = Color.Lerp(
                    Color.Lerp(color11, color21, dx),
                    Color.Lerp(color12, color22, dx),
                    dy
                );

                int index = y * targetWidth + x;
                pixels[index] = color;
            }
        }

        // 设置像素并应用
        mapTexture.SetPixels(pixels);
        mapTexture.Apply();
    }

    private Color GetCellColor(int x, int y)
    {
        // 检查索引是否有效
        if (x < 0 || x >= currentMapSize.x || y < 0 || y >= currentMapSize.y)
        {
            return Color.magenta; // 调试用的颜色
        }

        int cellData = currentMapData[x, y];
        Color color = Color.clear;

        // 按照优先级确定颜色
        if ((cellData & (int)MapData.Bridge) != 0)
        {
            color = bridgeColor;
        }
        else if ((cellData & (int)MapData.Sect) != 0)
        {
            color = sectColor;
        }
        else if ((cellData & (int)MapData.Tribe) != 0)
        {
            color = tribeColor;
        }
        else if ((cellData & (int)MapData.City) != 0)
        {
            color = cityColor;
        }
        else if ((cellData & (int)MapData.Village) != 0)
        {
            color = villageColor;
        }
        else if ((cellData & (int)MapData.Dock) != 0)
        {
            color = dockColor;
        }
        else if ((cellData & (int)MapData.Route) != 0)
        {
            color = routeColor;
        }
        else if ((cellData & (int)MapData.River) != 0)
        {
            color = riverColor;
        }
        else if ((cellData & (int)MapData.Road) != 0)
        {
            color = roadColor;
        }
        else if ((cellData & (int)MapData.Forest) != 0)
        {
            color = forestColor;
        }
        else if ((cellData & (int)MapData.Mountain) != 0)
        {
            color = mountainColor;
        }
        else if ((cellData & (int)MapData.Lake) != 0)
        {
            color = lakeColor;
        }
        else if ((cellData & (int)MapData.Plain) != 0)
        {
            color = plainColor;
        }
        else if ((cellData & (int)MapData.Ocean) != 0)
        {
            color = oceanColor;
        }
        // 大陆颜色（不应该出现）
        else if ((cellData & (int)MapData.Continent) != 0)
        {
            //Debug.Log(x + "," + y + " 不该出现的大陆颜色：" + cellData);
            color = Color.white;
        }
        // 岛屿颜色（不应该出现）
        else if ((cellData & (int)MapData.Island) != 0)
        {
            //Debug.Log(x + "," + y + " 不该出现的岛屿颜色：" + cellData);
            color = Color.gray;
        }
        // 默认颜色（不应该出现）
        else
        {
            color = Color.black;
        }

        return color;
    }

    private void SaveMapTexture()
    {
        if (mapTexture == null)
        {
            EditorUtility.DisplayDialog("错误", "没有可保存的地图图像。", "确定");
            return;
        }

        string defaultName = Path.GetFileNameWithoutExtension(loadPath ?? savePath) + ".png";
        string path = EditorUtility.SaveFilePanel("保存地图图像", Application.dataPath, defaultName, "png");

        if (!string.IsNullOrEmpty(path))
        {
            byte[] bytes = mapTexture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("成功", "地图图像已保存到:\n" + path, "确定");
        }
    }

    private void OnDestroy()
    {
        // 清理纹理资源
        if (mapTexture != null)
        {
            DestroyImmediate(mapTexture);
        }
    }

    // 当窗口尺寸变化时触发
    private void OnResize()
    {
        // 强制重绘窗口
        Repaint();
    }
}