using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

// �Զ���༭�������� ResourcesDataScriptable ���͵Ķ���
[CustomEditor(typeof(ResourcesDataScriptable))]
public class WelcomeInspectorEditor : Editor
{
    // ������� XML ģ��
    public VisualTreeAsset m_InspectorXML;
    // ��ģ��� XML
    public VisualTreeAsset m_BlockTemplateXML;
    // ���Ӿ�Ԫ��
    private VisualElement v_Root;
    // ��Դ���ݵĿɽű�������
    public ResourcesDataScriptable resourcesSO;

    // ��д�����ͷ���� GUI ���Ʒ���
    protected override void OnHeaderGUI()
    {
        // ��ȡĿ�����
        var data = (ResourcesDataScriptable)target;
        // ����ͼ����
        var iconWidth = Mathf.Min(EditorGUIUtility.currentViewWidth / 3f - 20f, 128f);
        // ������ʽ
        GUIStyle m_TitleStyle;
        // ��������ʽ
        GUIStyle m_SubTitleStyle;

        // ��ʼ��������ʽ
        m_TitleStyle = new GUIStyle(EditorStyles.label);
        m_TitleStyle.fontSize = 24;
        m_TitleStyle.wordWrap = true;
        // ��ʼ����������ʽ
        m_SubTitleStyle = new GUIStyle(EditorStyles.label);
        m_SubTitleStyle.fontSize = 14;
        m_SubTitleStyle.wordWrap = true;

        // ��ʼˮƽ����
        GUILayout.BeginHorizontal();
        {
            // ��ʾ���ں��
            GUILayout.Label(data.windowBanner, GUILayout.Width(iconWidth), GUILayout.Height(iconWidth));
            // ��ʼ��ֱ����
            GUILayout.BeginVertical();
            {
                // ��Ӽ��
                GUILayout.Space(5);
                // ��ʾ���ڱ���
                GUILayout.Label(data.windowTitle, m_TitleStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth / 1.5f));
                // ��ʾ���ڸ�����
                GUILayout.Label(data.windowSubTitle, m_SubTitleStyle, GUILayout.Width(EditorGUIUtility.currentViewWidth / 1.5f));
            }
            // ������ֱ����
            GUILayout.EndVertical();
        }
        // ����ˮƽ����
        GUILayout.EndHorizontal();
    }

    //��д��������� GUI �ķ���
    public override VisualElement CreateInspectorGUI()
    {
        // ����һ���µ��Ӿ�Ԫ����Ϊ����� UI �ĸ�
        VisualElement myInspector = new VisualElement();
        // ��¡ XML ģ�嵽�����
        m_InspectorXML.CloneTree(myInspector);
        // ���ý��ܱ�ǩ������Դ
        myInspector.Q<Label>("intro").dataSource = resourcesSO;

        // ������Դ�����е���Ϣ��
        for (int i = 0; i < resourcesSO.infoBlock.Count; i++)
        {
            // ��ȡ��ǰ��Ϣ��
            BlockDataScriptable b = resourcesSO.infoBlock[i];
            // ʵ������ģ��
            VisualElement v_Block = m_BlockTemplateXML.Instantiate();
            // ���ÿ���Ϣ��ť������Դ
            v_Block.Q<Button>("button-block-info").dataSource = b;
            // ע�ᰴť����¼�
            v_Block.Q<Button>("button-block-info").RegisterCallback<ClickEvent>(evt =>
            {
                // �����ťʱ����Դ����
                Application.OpenURL("https://www.yellowshange.com");
            });

            // ������ӵ���Դ��Ԫ��
            myInspector.Q<VisualElement>("root-resources").Add(v_Block);
        }

        /* // �����ֱ��ɨ����Դ�ļ����еĿɽű��������Ի�ȡ����Ϣ
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
                // �����ťʱ����Դ����
                Application.OpenURL("https://www.yellowshange.com");
            });

            myInspector.Q<VisualElement>("root-resources").Add(v_Block);
        }*/

        // ������ɵļ���� UI
        return myInspector;
    }

    // �ڼ���ʱ��ʼ������
    [InitializeOnLoadMethod]
    public static void RegisterConverters()
    {
        // ����һ��ת�����飬���ڵ������С
        ConverterGroup groupSize = new("ConvertToPix", "�������С", "���������� 150 ���ز����ϱ߾����أ���������������խ���ڿ�ʼ�ͽ���������");
        // ���ת����
        groupSize.AddConverter((ref int v) => new StyleLength(new Length(v * 150 + (v - 1) * 6, LengthUnit.Pixel))); // ��С���� 150 ���ز����ϱ߾����أ���������������խ���ڿ�ʼ�ͽ���������
        // ע��ת������
        ConverterGroups.RegisterConverterGroup(groupSize);

        // ����һ��ת�����飬���ڽ��󶨵��ı�ת��Ϊ��д
        ConverterGroup groupUpper = new("Uppercase", "�����д", "Ϊ������Ŀ�Ľ��󶨵��ı�ת��Ϊ��д");
        // ���ת����
        groupUpper.AddConverter((ref string v) => v.ToUpper()); // ��Ϊ�ı��ǰ󶨵ģ�������Ҫ��ת��������ת��Ϊ��д����Ӹ���ǩ
        // ע��ת������
        ConverterGroups.RegisterConverterGroup(groupUpper);

        // ����һ��ת�����飬���ڽ�����ֵת��Ϊ�ɼ�����ʽ
        ConverterGroup groupDisplay = new("Bool to Visibility");
        // ���ת����
        groupDisplay.AddConverter((ref bool v) =>
        {
            return v switch
            {
                true => new StyleFloat(65f),
                false => new StyleFloat(0f)
            };
        }); // �Ӳ���ֵת��Ϊ��ʽ
        // ע��ת������
        ConverterGroups.RegisterConverterGroup(groupDisplay);
    }
}