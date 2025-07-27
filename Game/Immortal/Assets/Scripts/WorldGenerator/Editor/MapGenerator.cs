using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// 地图生成器编辑器扩展，用于在Unity菜单中添加地图生成选项
/// </summary>
public static class MapGeneratorEditor
{
    // 菜单路径
    private const string MENU_PATH = "地图/生成随机世界地图";

    /// <summary>
    /// 菜单选项的回调方法，用于触发地图生成
    /// </summary>
    //[MenuItem("地图/生成随机世界地图", false, 100)]
    public static void GenerateMapFromMenu()
    {
        // 查找项目中所有的地图配置文件
        string[] configGuids = AssetDatabase.FindAssets("t:MapGeneratorConfig");

        MapGeneratorConfig config = null;

        if (configGuids.Length > 0)
        {
            // 如果找到配置文件，使用第一个
            string configPath = AssetDatabase.GUIDToAssetPath(configGuids[0]);
            config = AssetDatabase.LoadAssetAtPath<MapGeneratorConfig>(configPath);
        }
        else
        {
            // 如果没有找到配置文件
            Debug.LogError("未找到地图配置文件");
            return;
        }

        if (config != null)
        {
            // 调用地图生成器生成地图
            int[,] mapData = MapGeneratorUtility.GenerateMap(config);

            if (mapData != null)
            {
                // 保存地图到文件
                string saveDirectory = Path.Combine(Application.dataPath, "GeneratedMaps");
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                // 生成带时间戳的文件名，避免覆盖
                string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string savePath = Path.Combine(saveDirectory, $"map_{timestamp}.txt");

                MapGeneratorUtility.SaveMapToFile(mapData, config.mapSize, savePath);

                // 刷新AssetDatabase以在Project窗口中显示新文件
                AssetDatabase.Refresh();

                EditorUtility.DisplayDialog(
                    "地图生成成功",
                    $"地图已成功生成并保存到:\n{savePath}",
                    "确定"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "地图生成失败",
                    "地图生成过程中出现错误，请查看控制台日志。",
                    "确定"
                );
            }
        }
        else
        {
            EditorUtility.DisplayDialog(
                "配置文件错误",
                "无法加载或创建地图配置文件，无法生成地图。",
                "确定"
            );
        }
    }

    /// <summary>
    /// 验证菜单选项是否可用
    /// </summary>
    [MenuItem(MENU_PATH, true)]
    public static bool ValidateGenerateMap()
    {
        // 菜单选项始终可用
        return true;
    }
}
