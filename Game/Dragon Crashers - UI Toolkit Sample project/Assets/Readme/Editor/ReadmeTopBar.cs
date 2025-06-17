using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;

// 在加载时初始化
[InitializeOnLoad]
public class ReadmeTopBar : Editor
{
    // 会话状态名称，用于记录是否已显示过 Readme
    static string kShowedReadmeSessionStateName = "ReadmeTopBar.showedReadme";

    // 静态构造函数
    static ReadmeTopBar()
    {
        // 延迟调用选择 Readme 的方法
        EditorApplication.delayCall += SelectReadmeAutomatically;
    }

    // 自动选择 Readme 的方法
    static void SelectReadmeAutomatically()
    {
        // 检查是否已经显示过 Readme
        if (!SessionState.GetBool(kShowedReadmeSessionStateName, false))
        {
            // 选择 Readme
            var readme = SelectReadme();
            // 标记为已显示
            SessionState.SetBool(kShowedReadmeSessionStateName, true);
        }
    }

    // 菜单项：选择欢迎窗口
    [MenuItem("Read Me/Select Welcome Window")]
    static ResourcesDataScriptable SelectReadme()
    {
        // 查找 WelcomeInspectorData 类型的资源数据可脚本化对象
        var ids = AssetDatabase.FindAssets("WelcomeInspectorData t:ResourcesDataScriptable");
        if (ids.Length == 1)
        {
            // 加载主资产
            var readmeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(ids[0]));
            // 设置选择的对象
            Selection.objects = new UnityEngine.Object[] { readmeObject };
            // 返回资源数据可脚本化对象
            return (ResourcesDataScriptable)readmeObject;
        }
        else
        {
            // 未找到 Readme 时输出日志
            Debug.Log("未找到 Readme");
            return null;
        }
    }

    // 菜单项：获取电子书
    [MenuItem("Read Me/Get the e-book")]
    static void OpenWebLink()
    {
        // 打开网页链接
        Application.OpenURL("https://resources.unity.com/games/user-interface-design-and-implementation-in-unity?isGated=false"); // 替换为你自己的链接
    }
}