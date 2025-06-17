using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;


namespace UIToolkitDemo
{
    // 将主题样式表与字符串配对
    [Serializable]
    public struct ThemeSettings
    {
        public string theme; // 主题
        public ThemeStyleSheet tss; // 主题样式表
        public PanelSettings panelSettings; // 面板设置
    }

    // 此组件更改主题样式表（从设置屏幕或媒体查询）。
    // 用于一次更改多个USS样式表。可能的应用包括季节性变化（例如圣诞节、万圣节）或屏幕尺寸（纵向）。
    [ExecuteInEditMode]
    public class ThemeManager : MonoBehaviour
    {
        [Tooltip("对要更新主题的UI文档的引用")]
        [SerializeField] UIDocument m_Document;
        [Tooltip("主题是一个字符串键、主题设置和面板设置")]
        [SerializeField] List<ThemeSettings> m_ThemeSettings;
        [SerializeField] bool m_Debug;

        string m_CurrentTheme; // 当前主题

        void OnEnable()
        {
            if (m_ThemeSettings.Count == 0)
            {
                Debug.LogWarning("[ThemeManager]: 添加ThemeSettings以设置主题");
                return;
            }
            // 直接从SettingsScreen更改主题
            ThemeEvents.ThemeChanged += OnThemeChanged;

            // 通过视口大小更改主题
            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            // 默认使用第一个主题
            m_CurrentTheme = m_ThemeSettings[0].theme;
        }

        void OnDisable()
        {
            ThemeEvents.ThemeChanged -= OnThemeChanged;
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;
        }

        // 更改面板设置资产中的主题样式表
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
                    Debug.LogWarning("[ThemeManager] ApplyTheme: 未分配UI文档。");
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
                    Debug.Log("[ThemeManager] 应用主题样式表: " + tss.name);
                }
            }
            else if (m_Debug)
            {
                Debug.LogWarning("[ThemeManager] ApplyTheme: 未找到与 " + theme + " 匹配的主题样式表");
            }
        }

        // 将主题对应的面板设置应用到UI文档
        void SetPanelSettings(string theme)
        {
            PanelSettings panelSettings = GetPanelSettings(theme);

            if (panelSettings != null)
            {
                m_Document.panelSettings = panelSettings;
            }
            else if (m_Debug)
            {
                Debug.LogWarning("[ThemeManager] ApplyTheme: 未找到与 " + theme + " 匹配的面板设置");
            }
        }

        // 查找与给定字符串对应的主题样式表
        ThemeStyleSheet GetThemeStyleSheet(string themeName)
        {
            int index = GetThemeIndex(themeName);
            if (index < 0)
            {
                Debug.LogWarning("[ThemeManager] GetThemeStyleSheet: 无效的主题名称 " + themeName);
                return null;
            }
            return m_ThemeSettings[index].tss;
        }

        // 返回给定主题对应的面板设置
        PanelSettings GetPanelSettings(string themeName)
        {
            int index = GetThemeIndex(themeName);

            if (index < 0)
            {
                Debug.LogWarning("[ThemeManager] GetPanelSettings: 无效的主题名称 " + themeName);
                return null;
            }
            return m_ThemeSettings[index].panelSettings;
        }

        // 返回给定主题的对应索引
        int GetThemeIndex(string themeName)
        {
            if (string.IsNullOrEmpty(themeName))
                return -1;

            // 从ThemeSettings中返回索引（如果未找到则返回-1）
            int index = m_ThemeSettings.FindIndex(x => x.theme == themeName);

            return index;
        }

        public static string GetPrefix(string input, string delimiter)
        {
            int lastIndex = input.LastIndexOf(delimiter);
            if (lastIndex == -1)
            {
                return input; // 未找到分隔符，返回原始字符串
            }
            return input.Substring(0, lastIndex);
        }

        public static string GetSuffix(string input, string delimiter)
        {
            int lastIndex = input.LastIndexOf(delimiter);
            if (lastIndex == -1)
            {
                return string.Empty; // 未找到分隔符，返回空字符串
            }
            return input.Substring(lastIndex);
        }

        // 事件触发方法

        void OnThemeChanged(string newTheme)
        {
            ApplyTheme(newTheme);
            AudioManager.PlayAltButtonSound();

            if (m_Debug)
            {
                Debug.Log("[ThemeManager] OnThemeChanged: " + newTheme);
            }
        }

        // 在切换纵向和横向时重新应用主题
        void OnAspectRatioUpdated(MediaAspectRatio mediaAspectRatio)
        {
            // 保存后缀为Default、Christmas或Halloween
            string suffix = GetSuffix(m_CurrentTheme, "--");

            // 添加Portrait或Landscape作为基本名称
            string newThemeName = mediaAspectRatio.ToString() + suffix;

            ApplyTheme(newThemeName);

            if (m_Debug)
            {
                Debug.Log("[ThemeManager] OnAspectRatioUpdated: " + newThemeName);
            }
        }
    }
}