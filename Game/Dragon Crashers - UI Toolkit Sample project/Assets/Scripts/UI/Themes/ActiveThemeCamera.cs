using System.Collections.Generic;
using UnityEngine;
using System;


namespace UIToolkitDemo
{
    // ��������ʽ�����ַ������
    [Serializable]
    public struct CameraTheme
    {
        public Camera camera; // ���
        public string theme; // ����
    }

    /// <summary>
    /// ��������ض�������ض�������ԣ����л�ʱ������Ӧ�������
    /// </summary>
    [ExecuteInEditMode]
    public class ActiveThemeCamera : MonoBehaviour
    {
        [Tooltip("�������������ԡ�")]
        [SerializeField] List<CameraTheme> m_CameraThemes;
        [Tooltip("���������¼���֪ͨ�����������Ѹ��¡�")]
        [SerializeField] bool m_SendEvent;
        [Tooltip("�ڿ���̨��¼������Ϣ��")]
        [SerializeField] bool m_Debug;

        string m_CurrentTheme; // ��ǰ����
        Camera m_ActiveCamera; // ����

        public List<CameraTheme> CameraThemes => m_CameraThemes;

        public Camera ActiveCamera => m_ActiveCamera;

        void OnEnable()
        {
            if (m_CameraThemes.Count == 0)
            {
                Debug.LogWarning("[ActiveThemeCamera]: ���CameraThemes���л��������");
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

        // �¼�������

        void OnThemeChanged(string themeName)
        {
            int index = m_CameraThemes.FindIndex(x => x.theme == themeName);
            ShowCamera(index);
        }

        // Ӧ�ú��������������ʽ��
        void OnAspectRatioUpdated(MediaAspectRatio mediaAspectRatio)
        {
            // �����׺ΪDefault��Christmas��Halloween
            string suffix = ThemeManager.GetSuffix(m_CurrentTheme, "--");

            // ���Portrait��Landscape��Ϊ��������
            string newThemeName = mediaAspectRatio.ToString() + suffix;

            int index = m_CameraThemes.FindIndex(x => x.theme == newThemeName);


            ShowCamera(index);
        }
    }
}