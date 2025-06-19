using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

// 自定义编辑器，用于 ResourcesDataScriptable 类型的对象
[CustomEditor(typeof(ResourcesDataScriptable))]
public class WelcomeInspectorEditor : Editor
{
    // 检查器的 XML 模板
    public VisualTreeAsset m_InspectorXML;
    // 块模板的 XML
    public VisualTreeAsset m_BlockTemplateXML;
    // 根视觉元素
    private VisualElement v_Root;
    // 资源数据的可脚本化对象
    public ResourcesDataScriptable resourcesSO;

    // 重写检查器头部的 GUI 绘制方法
    protected override void OnHeaderGUI()
    {
        // 获取目标对象
        var data = (ResourcesDataScriptable)target;
        // 计算图标宽度
        var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);
        // 标题样式
        GUIStyle m_TitleStyle;
        // 副标题样式
        GUIStyle m_SubTitleStyle;

        // 初始化标题样式
        m_TitleStyle = new GUIStyle(EditorStyles.label);
        m_TitleStyle.fontSize = 24;
        m_TitleStyle.wordWrap = true;
        // 初始化副标题样式
        m_SubTitleStyle = new GUIStyle(EditorStyles.label);
        m_SubTitleStyle.fontSize = 14;
        m_SubTitleStyle.wordWrap = true;

        // 开始水平布局
        GUILayout.BeginHorizontal();
        {
            // 显示窗口横幅
            GUILayout.Label(data.windowBanner, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
            // 开始垂直布局
            GUILayout.BeginVertical();
            {
                // 添加间距
                GUILayout.Space(5);
                // 显示窗口标题
                GUILayout.Label(data.windowTitle, m_TitleStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth / 1.5f));
                // 显示窗口副标题
                GUILayout.Label(data.windowSubTitle, m_SubTitleStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth / 1.5f));
            }
            // 结束垂直布局
            GUILayout.EndVertical();
        }
        // 结束水平布局
        GUILayout.EndHorizontal();
    }

    //重写创建检查器 GUI 的方法
    public override VisualElement CreateInspectorGUI()
    {
        // 创建一个新的视觉元素作为检查器 UI 的根
        VisualElement myInspector = new VisualElement();
        // 克隆 XML 模板到检查器
        m_InspectorXML.CloneTree(myInspector);
        // 设置介绍标签的数据源
        myInspector.Q<Label>("intro").dataSource = resourcesSO;

        // 遍历资源数据中的信息块
        for (int i = 0; i < resourcesSO.infoBlock.Count; i++)
        {
            // 获取当前信息块
            BlockDataScriptable b = resourcesSO.infoBlock[i];
            // 实例化块模板
            VisualElement v_Block = m_BlockTemplateXML.Instantiate();
            // 设置块信息按钮的数据源
            v_Block.Q<Button>("button-block-info").dataSource = b;
            // 注册按钮点击事件
            v_Block.Q<Button>("button-block-info").RegisterCallback<ClickEvent>(evt =>
            {
                // 点击按钮时打开资源链接
                Application.OpenURL("https://www.yellowshange.com");
            });

            // 将块添加到资源根元素
            myInspector.Q<VisualElement>("root-resources").Add(v_Block);
        }

        /* // 如果想直接扫描资源文件夹中的可脚本化对象以获取块信息
        var AllAvailableBlocksData = AssetDatabase.FindAssets("t:BlockDataScriptable");

        foreach (string b in AllAvailableBlocksData)
        {
            //Debug.Log("BlockDataScriptable found: " + b);
            VisualElement v_Block = m_BlockTemplateXML.Instantiate();
            string AssetPath = AssetDatabase.GUIDToAssetPath(b);
            var blockDataSO = (BlockDataScriptable)AssetDatabase.LoadAssetAtPath(AssetPath, typeof(BlockDataScriptable));

            v_Block.Q<Button>("button-block-info").dataSource = blockDataSO;
            v_Block.Q<Button>("button-block-info").RegisterCallback<ClickEvent>(evt =>
            {
                // 点击按钮时打开资源链接
                Application.OpenURL("https://www.yellowshange.com");
            });

            myInspector.Q<VisualElement>("root-resources").Add(v_Block);
        }*/

        // 返回完成的检查器 UI
        return myInspector;
    }

    // 在加载时初始化方法
    [InitializeOnLoadMethod]
    public static void RegisterConverters()
    {
        // 创建一个转换器组，用于调整块大小
        ConverterGroup groupSize = new("ConvertToPix", "调整块大小", "将整数乘以 150 像素并加上边距像素，这样宽块与下面的窄块在开始和结束处对齐");
        // 添加转换器
        groupSize.AddConverter((ref int v) => new StyleLength(new Length(v * 150 + (v - 1) * 6, LengthUnit.Pixel))); // 大小乘以 150 像素并加上边距像素，这样宽块与下面的窄块在开始和结束处对齐
        // 注册转换器组
        ConverterGroups.RegisterConverterGroup(groupSize);

        // 创建一个转换器组，用于将绑定的文本转换为大写
        ConverterGroup groupUpper = new("Uppercase", "绑定项大写", "为了美观目的将绑定的文本转换为大写");
        // 添加转换器
        groupUpper.AddConverter((ref string v) => v.ToUpper()); // 因为文本是绑定的，所以需要用转换器将其转换为大写或添加富标签
        // 注册转换器组
        ConverterGroups.RegisterConverterGroup(groupUpper);

        // 创建一个转换器组，用于将布尔值转换为可见性样式
        ConverterGroup groupDisplay = new("Bool to Visibility");
        // 添加转换器
        groupDisplay.AddConverter((ref bool v) =>
        {
            return v switch
            {
                true => new StyleFloat(65f),
                false => new StyleFloat(0f)
            };
        }); // 从布尔值转换为样式
        // 注册转换器组
        ConverterGroups.RegisterConverterGroup(groupDisplay);
    }
}