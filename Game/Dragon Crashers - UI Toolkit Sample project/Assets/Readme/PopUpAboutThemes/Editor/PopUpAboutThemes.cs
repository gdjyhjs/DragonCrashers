using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 这个编辑器窗口脚本在 UI Builder 窗口打开时显示一个弹出窗口，提醒用户选择 UI Toolkit 主题。
/// 脚本使用 PlayerPrefs 来跟踪弹出窗口是否已经显示过，以确保它只出现一次。
/// </summary>
[InitializeOnLoad]
public class PopUpAboutThemes : EditorWindow
{
    // PlayerPrefs 中用于记录弹出窗口是否已显示的键
    const string k_PopUpKey = "PopUpThemesShown";
    // UI Builder 窗口的标题
    const string k_UIBuilderTitle = "UI Builder";
    // 弹出窗口的 XML 资产路径
    const string k_AssetXML = "Assets/ReadMe/PopUpAboutThemes/PopUpAboutThemes.uxml";

    // 静态构造函数
    static PopUpAboutThemes()
    {
        // 注册编辑器更新事件
        EditorApplication.update += CheckEditorChanges;
        // 初始化弹出窗口键
        InitializePopUpKey();
    }

    // 初始化弹出窗口键的方法
    private static void InitializePopUpKey()
    {
        // 检查 PlayerPrefs 中是否存在该键
        if (!PlayerPrefs.HasKey(k_PopUpKey))
        {
            // 设置键值为 0
            PlayerPrefs.SetInt(k_PopUpKey, 0);
            // 保存 PlayerPrefs
            PlayerPrefs.Save();
        }
    }

    // 检查编辑器窗口变化的方法
    static void CheckEditorChanges()
    {
        // 检查 PlayerPrefs 中是否存在该键
        if (!PlayerPrefs.HasKey(k_PopUpKey))
        {
            // 检查键值是否为 0
            if (PlayerPrefs.GetInt(k_PopUpKey) == 0)
            {
                // 获取当前聚焦的编辑器窗口
                EditorWindow currentWindow = EditorWindow.focusedWindow;

                // 检查窗口是否存在且标题是否为 UI Builder
                if (currentWindow != null && currentWindow.titleContent != null && currentWindow.titleContent.text == k_UIBuilderTitle)
                {
                    // 设置键值为 1
                    PlayerPrefs.SetInt(k_PopUpKey, 1);
                    // 保存 PlayerPrefs
                    PlayerPrefs.Save();

                    // 显示弹出窗口
                    ShowPopUp();
                    // 移除编辑器更新事件
                    EditorApplication.update -= CheckEditorChanges;
                }
            }
        }
    }

    // 菜单项：主题和屏幕比例
    [MenuItem("Read Me/寻仙问道")]
    static void ShowPopUp()
    {
        // 获取弹出窗口实例
        PopUpAboutThemes wnd = GetWindow<PopUpAboutThemes>();
        // 设置窗口标题
        wnd.titleContent = new GUIContent("寻仙问道");
        // 加载 XML 资产
        VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AssetXML);

        if (uiAsset != null)
        {
            // 实例化 UI 元素
            VisualElement ui = uiAsset.Instantiate();
            // 将 UI 元素插入到窗口根元素中
            wnd.rootVisualElement.Insert(0, ui);
        }
        else
        {
            // 未找到 XML 资产时输出警告日志
            Debug.LogWarning("[PopUpAboutThemes] ShowPopUp: 未找到 VisualTreeAsset。");
        }
    }
}