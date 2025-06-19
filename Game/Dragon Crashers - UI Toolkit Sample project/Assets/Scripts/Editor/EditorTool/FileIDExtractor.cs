using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class FileIDExtractor
{
    [MenuItem("编辑器工具/获取图片文件ID")]
    static void GetTextureFileID()
    {
        // 获取选中的图片资源
        Object selectedObject = Selection.activeObject;
        if (selectedObject == null || !(selectedObject is Texture2D))
        {
            Debug.LogError("请选择一个 Texture2D 资源");
            return;
        }

        // 获取资源路径和 GUID
        string assetPath = AssetDatabase.GetAssetPath(selectedObject);
        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        Debug.Log($"资源路径: {assetPath}");
        Debug.Log($"GUID: {guid}");

        // 获取对应的 .meta 文件路径
        string metaPath = assetPath + ".meta";
        if (!File.Exists(metaPath))
        {
            Debug.LogError($"未找到 .meta 文件: {metaPath}");
            return;
        }

        // 解析 .meta 文件中的 fileID（注意：Texture 的 fileID 通常在引用它的文件中才有意义）
        string metaContent = File.ReadAllText(metaPath);
        Match fileIDMatch = Regex.Match(metaContent, @"fileID: (\d+)");
        if (fileIDMatch.Success)
        {
            string fileID = fileIDMatch.Groups[1].Value;
            Debug.Log($"Texture 默认 fileID: {fileID}");
        }
        else
        {
            Debug.Log("未在 .meta 文件中找到 fileID");
        }

        // 如果需要在特定引用文件中查找 fileID（例如 prefab 或 scriptableObject）
        // 可以使用 SerializedObject 和 SerializedProperty 进一步解析
    }
}