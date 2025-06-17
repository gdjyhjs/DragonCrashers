using System.Collections.Generic;
using UnityEngine;
using System;


namespace UIToolkitDemo
{
    // 将主题样式表与字符串配对
    [Serializable]
    public struct CameraTheme
    {
        public Camera camera; // 相机
        public string theme; // 主题
    }

    /// <summary>
    /// 此组件将特定相机与特定主题配对，在切换时启用相应的相机。
    /// </summary>
    [ExecuteInEditMode]
    public class ActiveThemeCamera : MonoBehaviour
    {
        [Tooltip("将相机与主题配对。")]
        [SerializeField] List<CameraTheme> m_CameraThemes;
        [Tooltip("发送主题事件以通知其他组件相机已更新。")]
        [SerializeField] bool m_SendEvent;
        [Tooltip("在控制台记录调试消息。")]
        [SerializeField] bool m_Debug;

        string m_CurrentTheme; // 当前主题
        Camera m_ActiveCamera; // 活动相机

        public List<CameraTheme> CameraThemes => m_CameraThemes;

        public Camera ActiveCamera => m_ActiveCamera;

        void OnEnable()
        {
            if (m_CameraThemes.Count == 0)
            {
                Debug.LogWarning("[ActiveThemeCamera]: 添加CameraThemes以切换主题相机");
                return;
            }

            ThemeEvents.ThemeChanged += OnThemeChanged;

            MediaQueryEvents.AspectRatioUpdated += OnAspectRatioUpdated;

            m_ActiveCamera = m_CameraThemes[0].camera;
            m_CurrentTheme = m_CameraThemes[0].theme;
        }


        void OnDisable()
        {
            ThemeEvents.ThemeChanged -= OnThemeChanged;
            MediaQueryEvents.AspectRatioUpdated -= OnAspectRatioUpdated;

        }

        public void ShowCamera(int index)
        {
            for (int i = 0; i < m_CameraThemes.Count; i++)
            {
                m_CameraThemes[i].camera.gameObject.SetActive(false);

                if (index == i)
                    m_ActiveCamera = m_CameraThemes[i].camera;
            }

            m_ActiveCamera.gameObject.SetActive(true);

            if (m_Debug)
                Debug.Log("[Active Theme Camera]: " + m_ActiveCamera.name);

            if (m_SendEvent)
                ThemeEvents.CameraUpdated?.Invoke(m_ActiveCamera);

        }

        // 事件处理方法

        void OnThemeChanged(string themeName)
        {
            int index = m_CameraThemes.FindIndex(x => x.theme == themeName);
            ShowCamera(index);
        }

        // 应用横向或纵向主题样式表
        void OnAspectRatioUpdated(MediaAspectRatio mediaAspectRatio)
        {
            // 保存后缀为Default、Christmas或Halloween
            string suffix = ThemeManager.GetSuffix(m_CurrentTheme, "--");

            // 添加Portrait或Landscape作为基本名称
            string newThemeName = mediaAspectRatio.ToString() + suffix;

            int index = m_CameraThemes.FindIndex(x => x.theme == newThemeName);


            ShowCamera(index);
        }
    }
}