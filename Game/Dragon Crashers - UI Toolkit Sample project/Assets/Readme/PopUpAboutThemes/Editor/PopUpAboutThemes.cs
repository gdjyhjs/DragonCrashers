using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ����༭�����ڽű��� UI Builder ���ڴ�ʱ��ʾһ���������ڣ������û�ѡ�� UI Toolkit ���⡣
/// �ű�ʹ�� PlayerPrefs �����ٵ��������Ƿ��Ѿ���ʾ������ȷ����ֻ����һ�Ρ�
/// </summary>
[InitializeOnLoad]
public class PopUpAboutThemes : EditorWindow
{
    // PlayerPrefs �����ڼ�¼���������Ƿ�����ʾ�ļ�
    const string k_PopUpKey = "PopUpThemesShown";
    // UI Builder ���ڵı���
    const string k_UIBuilderTitle = "UI Builder";
    // �������ڵ� XML �ʲ�·��
    const string k_AssetXML = "Assets/ReadMe/PopUpAboutThemes/PopUpAboutThemes.uxml";

    // ��̬���캯��
    static PopUpAboutThemes()
    {
        // ע��༭�������¼�
        EditorApplication.update += CheckEditorChanges;
        // ��ʼ���������ڼ�
        InitializePopUpKey();
    }

    // ��ʼ���������ڼ��ķ���
    private static void InitializePopUpKey()
    {
        // ��� PlayerPrefs ���Ƿ���ڸü�
        if (!PlayerPrefs.HasKey(k_PopUpKey))
        {
            // ���ü�ֵΪ 0
            PlayerPrefs.SetInt(k_PopUpKey, 0);
            // ���� PlayerPrefs
            PlayerPrefs.Save();
        }
    }

    // ���༭�����ڱ仯�ķ���
    static void CheckEditorChanges()
    {
        // ��� PlayerPrefs ���Ƿ���ڸü�
        if (!PlayerPrefs.HasKey(k_PopUpKey))
        {
            // ����ֵ�Ƿ�Ϊ 0
            if (PlayerPrefs.GetInt(k_PopUpKey) == 0)
            {
                // ��ȡ��ǰ�۽��ı༭������
                EditorWindow currentWindow = EditorWindow.focusedWindow;

                // ��鴰���Ƿ�����ұ����Ƿ�Ϊ UI Builder
                if (currentWindow != null && currentWindow.titleContent != null && currentWindow.titleContent.text == k_UIBuilderTitle)
                {
                    // ���ü�ֵΪ 1
                    PlayerPrefs.SetInt(k_PopUpKey, 1);
                    // ���� PlayerPrefs
                    PlayerPrefs.Save();

                    // ��ʾ��������
                    ShowPopUp();
                    // �Ƴ��༭�������¼�
                    EditorApplication.update -= CheckEditorChanges;
                }
            }
        }
    }

    // �˵���������Ļ����
    [MenuItem("Read Me/Ѱ���ʵ�")]
    static void ShowPopUp()
    {
        // ��ȡ��������ʵ��
        PopUpAboutThemes wnd = GetWindow<PopUpAboutThemes>();
        // ���ô��ڱ���
        wnd.titleContent = new GUIContent("Ѱ���ʵ�");
        // ���� XML �ʲ�
        VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(k_AssetXML);

        if (uiAsset != null)
        {
            // ʵ���� UI Ԫ��
            VisualElement ui = uiAsset.Instantiate();
            // �� UI Ԫ�ز��뵽���ڸ�Ԫ����
            wnd.rootVisualElement.Insert(0, ui);
        }
        else
        {
            // δ�ҵ� XML �ʲ�ʱ���������־
            Debug.LogWarning("[PopUpAboutThemes] ShowPopUp: δ�ҵ� VisualTreeAsset��");
        }
    }
}