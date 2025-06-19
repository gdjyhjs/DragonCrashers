using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class FileIDExtractor
{
    [MenuItem("�༭������/��ȡͼƬ�ļ�ID")]
    static void GetTextureFileID()
    {
        // ��ȡѡ�е�ͼƬ��Դ
        Object selectedObject = Selection.activeObject;
        if (selectedObject == null || !(selectedObject is Texture2D))
        {
            Debug.LogError("��ѡ��һ�� Texture2D ��Դ");
            return;
        }

        // ��ȡ��Դ·���� GUID
        string assetPath = AssetDatabase.GetAssetPath(selectedObject);
        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        Debug.Log($"��Դ·��: {assetPath}");
        Debug.Log($"GUID: {guid}");

        // ��ȡ��Ӧ�� .meta �ļ�·��
        string metaPath = assetPath + ".meta";
        if (!File.Exists(metaPath))
        {
            Debug.LogError($"δ�ҵ� .meta �ļ�: {metaPath}");
            return;
        }

        // ���� .meta �ļ��е� fileID��ע�⣺Texture �� fileID ͨ�������������ļ��в������壩
        string metaContent = File.ReadAllText(metaPath);
        Match fileIDMatch = Regex.Match(metaContent, @"fileID: (\d+)");
        if (fileIDMatch.Success)
        {
            string fileID = fileIDMatch.Groups[1].Value;
            Debug.Log($"Texture Ĭ�� fileID: {fileID}");
        }
        else
        {
            Debug.Log("δ�� .meta �ļ����ҵ� fileID");
        }

        // �����Ҫ���ض������ļ��в��� fileID������ prefab �� scriptableObject��
        // ����ʹ�� SerializedObject �� SerializedProperty ��һ������
    }
}