using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 将GameObject的位置对齐到指定的VisualElement
    /// </summary>
    public class PositionToVisualElement : MonoBehaviour
    {
        [Header("变换")]
        // 要移动的游戏对象
        [SerializeField] GameObject m_ObjectToMove;

        [Header("相机参数")]
        // 相机
        [SerializeField] Camera m_Camera;
        // 深度
        [SerializeField] float m_Depth = 10f;

        [Header("UI目标")]
        // UI文档
        [SerializeField] UIDocument m_Document;
        // 元素名称
        [SerializeField] string m_ElementName;

        // 目标元素
        VisualElement m_TargetElement;

        void OnEnable()
        {
            if (m_Document == null)
            {
                Debug.LogError("[PositionToVisualElement]: 未指定UIDocument。");
                return;
            }

            VisualElement root = m_Document.rootVisualElement;
            m_TargetElement = root.Q<VisualElement>(name: m_ElementName);

            ThemeEvents.CameraUpdated += OnCameraUpdated;

            if (m_TargetElement == null)
            {
                Debug.LogError($"[PositionToVisualElement]: 未找到元素 '{m_ElementName}'。");
                return;
            }

            m_TargetElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        void OnDisable()
        {
            ThemeEvents.CameraUpdated -= OnCameraUpdated;

            if (m_TargetElement != null)
            {
                m_TargetElement.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            }
        }

        void Start()
        {
            MoveToElement();
        }

        public void MoveToElement()
        {
            if (m_Camera == null)
            {
                Debug.LogError("[PositionToVisualElement] MoveToElement: 未指定相机。");
                return;
            }

            if (m_ObjectToMove == null)
            {
                Debug.LogError("[PositionToVisualElement] MoveToElement: 未指定要移动的对象。");
                return;
            }

            // 定位UI Toolkit中的屏幕中心位置
            Rect worldBound = m_TargetElement.worldBound;
            Vector2 centerPosition = new Vector2(worldBound.x + worldBound.width / 2, worldBound.y + worldBound.height / 2);

            // 使用扩展方法转换为像素坐标
            Vector2 screenPos = centerPosition.GetScreenCoordinate(m_Document.rootVisualElement);

            // 使用扩展方法转换为世界位置
            Vector3 worldPosition = screenPos.ScreenPosToWorldPos(m_Camera, m_Depth);

            if (m_ObjectToMove != null)
            {
                m_ObjectToMove.transform.position = worldPosition;
            }
        }

        // 更新相机以适应纵向/横向主题
        void OnCameraUpdated(Camera camera)
        {
            m_Camera = camera;
            MoveToElement();
        }

        // 每当UI元素设置或移动时移动GameObject
        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            MoveToElement();
        }
    }
}