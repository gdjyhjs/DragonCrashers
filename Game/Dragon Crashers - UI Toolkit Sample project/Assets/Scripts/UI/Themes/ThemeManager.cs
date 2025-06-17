using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;


namespace UIToolkitDemo
{
    // ��������ʽ�����ַ������
    [Serializable]
    public struct ThemeSettings
    {
        public string theme; // ����
        public ThemeStyleSheet tss; // ������ʽ��
        public PanelSettings panelSettings; // �������
    }

    // ���������������ʽ����������Ļ��ý���ѯ����
    // ����һ�θ��Ķ��USS��ʽ�����ܵ�Ӧ�ð��������Ա仯������ʥ���ڡ���ʥ�ڣ�����Ļ�ߴ磨���򣩡�
    [ExecuteInEditMode]
    public class ThemeManager : MonoBehaviour
    {
        [Tooltip("��Ҫ���������UI�ĵ�������")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("������һ���ַ��������������ú��������")]
        [SerializeField] List<ThemeSettings> m_ThemeSettings;
        [SerializeField] bool m_Debug;

        string m_CurrentTheme; // ��ǰ����

        void OnEnable()
        {
            if (m_ThemeSettings.Count == 0)
            {
                Debug.LogWarning("[ThemeManager]: ���ThemeSettings����������");
                return;
            }
            // ֱ�Ӵ�SettingsScreen��������
            ThemeEvents.ThemeChanged += OnThemeChanged;

            // ͨ���ӿڴ�С��������
            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            // Ĭ��ʹ�õ�һ������
            m_CurrentTheme = m_ThemeSettings[0].theme;
        }

        void OnDisable()
        {
            ThemeEvents.ThemeChanged -= OnThemeChanged;
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;
        }

        // ������������ʲ��е�������ʽ��
        public void ApplyTheme(string theme)
        {
            if (m_Document == null)
            {
                m_Document = FindFirstObjectByType<UIDocument>();
            }

            if (m_Document == null)
            {
                if (m_Debug)
                {
                    Debug.LogWarning("[ThemeManager] ApplyTheme: δ����UI�ĵ���");
                }
                return;
            }

            SetPanelSettings(theme);

            SetThemeStyleSheet(theme);

            m_CurrentTheme = theme;
        }

        void SetThemeStyleSheet(string theme)
        {
            ThemeStyleSheet tss = GetThemeStyleSheet(theme);

            if (tss != null)
            {
                m_Document.panelSettings.themeStyleSheet = tss;

                if (m_Debug)
                {
                    Debug.Log("[ThemeManager] Ӧ��������ʽ��: " + tss.name);
                }
            }
            else if (m_Debug)
            {
                Debug.LogWarning("[ThemeManager] ApplyTheme: δ�ҵ��� " + theme + " ƥ���������ʽ��");
            }
        }

        // �������Ӧ���������Ӧ�õ�UI�ĵ�
        void SetPanelSettings(string theme)
        {
            PanelSettings panelSettings = GetPanelSettings(theme);

            if (panelSettings != null)
            {
                m_Document.panelSettings = panelSettings;
            }
            else if (m_Debug)
            {
                Debug.LogWarning("[ThemeManager] ApplyTheme: δ�ҵ��� " + theme + " ƥ����������");
            }
        }

        // ����������ַ�����Ӧ��������ʽ��
        ThemeStyleSheet GetThemeStyleSheet(string themeName)
        {
            int index = GetThemeIndex(themeName);
            if (index < 0)
            {
                Debug.LogWarning("[ThemeManager] GetThemeStyleSheet: ��Ч���������� " + themeName);
                return null;
            }
            return m_ThemeSettings[index].tss;
        }

        // ���ظ��������Ӧ���������
        PanelSettings GetPanelSettings(string themeName)
        {
            int index = GetThemeIndex(themeName);

            if (index < 0)
            {
                Debug.LogWarning("[ThemeManager] GetPanelSettings: ��Ч���������� " + themeName);
                return null;
            }
            return m_ThemeSettings[index].panelSettings;
        }

        // ���ظ�������Ķ�Ӧ����
        int GetThemeIndex(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
                return -1;

            // ��ThemeSettings�з������������δ�ҵ��򷵻�-1��
            int index = m_ThemeSettings.FindIndex(x => x.theme == themeName);

            return index;
        }

        public static string GetPrefix(string input, string delimiter)
        {
            int lastIndex = input.LastIndexOf(delimiter);
            if (lastIndex == -1)
            {
                return input; // δ�ҵ��ָ���������ԭʼ�ַ���
            }
            return input.Substring(0, lastIndex);
        }

        public static string GetSuffix(string input, string delimiter)
        {
            int lastIndex = input.LastIndexOf(delimiter);
            if (lastIndex == -1)
            {
                return string.Empty; // δ�ҵ��ָ��������ؿ��ַ���
            }
            return input.Substring(lastIndex);
        }

        // �¼���������

        void OnThemeChanged(string newTheme)
        {
            ApplyTheme(newTheme);
            AudioManager.PlayAltButtonSound();

            if (m_Debug)
            {
                Debug.Log("[ThemeManager] OnThemeChanged: " + newTheme);
            }
        }

        // ���л�����ͺ���ʱ����Ӧ������
        void OnAspectRatioUpdated(MediaAspectRatio mediaAspectRatio)
        {
            // �����׺ΪDefault��Christmas��Halloween
            string suffix = GetSuffix(m_CurrentTheme, "--");

            // ���Portrait��Landscape��Ϊ��������
            string newThemeName = mediaAspectRatio.ToString() + suffix;

            ApplyTheme(newThemeName);

            if (m_Debug)
            {
                Debug.Log("[ThemeManager] OnAspectRatioUpdated: " + newThemeName);
            }
        }
    }
}