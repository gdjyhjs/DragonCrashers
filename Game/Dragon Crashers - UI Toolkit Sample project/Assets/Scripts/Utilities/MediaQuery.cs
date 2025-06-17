using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UIToolkitDemo
{
    // 按宽高比分类
    public enum MediaAspectRatio
    {
        // 未定义
        Undefined,
        // 横向
        Landscape,
        // 纵向
        Portrait
    }

    [ExecuteInEditMode]
    public class MediaQuery : MonoBehaviour
    {
        [SerializeField] UIDocument m_Document;

        // 被认为是横向的最小宽高比
        public const float k_LandscapeMin = 1.2f;

        // 存储当前屏幕分辨率
        Vector2 m_CurrentResolution;

        // 横向、纵向或未定义
        MediaAspectRatio m_CurrentAspectRatio;

        public Vector2 CurrentResolution => m_CurrentResolution;

        void OnEnable()
        {
            if (m_Document == null)
            {
                Debug.Log("[MediaQuery]: 请指定UI文档。");
                return;
            }

            VisualElement root = m_Document.rootVisualElement;

            if (root != null)
                root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

            QueryResolution();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void Start()
        {
            QueryResolution();
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateResolution();
        }

        // 如果分辨率与之前不同则更新
        public void QueryResolution()
        {
            Vector2 newResolution = new Vector2(Screen.width, Screen.height);

            if (newResolution != m_CurrentResolution)
            {
                m_CurrentResolution = newResolution;
                MediaQueryEvents.ResolutionUpdated?.Invoke(newResolution);
            }

            MediaAspectRatio newAspectRatio = CalculateAspectRatio(newResolution);

            if (newAspectRatio != m_CurrentAspectRatio)
            {
                m_CurrentAspectRatio = newAspectRatio;
                MediaQueryEvents.AspectRatioUpdated?.Invoke(newAspectRatio);
            }
        }

        // 强制更新分辨率和宽高比
        public void UpdateResolution()
        {
            Vector2 newResolution = new Vector2(Screen.width, Screen.height);
            MediaQueryEvents.ResolutionUpdated?.Invoke(newResolution);
            MediaAspectRatio newAspectRatio = CalculateAspectRatio(newResolution);
            MediaQueryEvents.AspectRatioUpdated?.Invoke(newAspectRatio);
        }

        public static MediaAspectRatio CalculateAspectRatio(Vector2 resolution)
        {
            if (Math.Abs(resolution.y) < float.Epsilon)
            {
                Debug.LogWarning("[MediaQuery] CalculateAspectRatio: 高度为零。无法计算宽高比。");
                return MediaAspectRatio.Undefined;
            }

            float aspectRatio = resolution.x / resolution.y;

            if (aspectRatio >= k_LandscapeMin)
            {
                return MediaAspectRatio.Landscape;
            }
            else
            {
                return MediaAspectRatio.Portrait;
            }
        }

        public static MediaAspectRatio CalculateAspectRatio(float width, float height)
        {
            return CalculateAspectRatio(new Vector2(width, height));
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            UpdateResolution();
        }
    }
}