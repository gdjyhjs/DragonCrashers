using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��GameObject��λ�ö��뵽ָ����VisualElement
    /// </summary>
    public class PositionToVisualElement : MonoBehaviour
    {
        [Header("�任")]
        // Ҫ�ƶ�����Ϸ����
        [SerializeField] GameObject m_ObjectToMove;

        [Header("�������")]
        // ���
        [SerializeField] Camera m_Camera;
        // ���
        [SerializeField] float m_Depth = 10f;

        [Header("UIĿ��")]
        // UI�ĵ�
        [SerializeField] UIDocument m_Document;
        // Ԫ������
        [SerializeField] string m_ElementName;

        // Ŀ��Ԫ��
        VisualElement m_TargetElement;

        void OnEnable()
        {
            if (m_Document == null)
            {
                Debug.LogError("[PositionToVisualElement]: δָ��UIDocument��");
                return;
            }

            VisualElement root = m_Document.rootVisualElement;
            m_TargetElement = root.Q<VisualElement>(name: m_ElementName);

            ThemeEvents.CameraUpdated += OnCameraUpdated;

            if (m_TargetElement == null)
            {
                Debug.LogError($"[PositionToVisualElement]: δ�ҵ�Ԫ�� '{m_ElementName}'��");
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
                Debug.LogError("[PositionToVisualElement] MoveToElement: δָ�������");
                return;
            }

            if (m_ObjectToMove == null)
            {
                Debug.LogError("[PositionToVisualElement] MoveToElement: δָ��Ҫ�ƶ��Ķ���");
                return;
            }

            // ��λUI Toolkit�е���Ļ����λ��
            Rect worldBound = m_TargetElement.worldBound;
            Vector2 centerPosition = new Vector2(worldBound.x + worldBound.width / 2, worldBound.y + worldBound.height / 2);

            // ʹ����չ����ת��Ϊ��������
            Vector2 screenPos = centerPosition.GetScreenCoordinate(m_Document.rootVisualElement);

            // ʹ����չ����ת��Ϊ����λ��
            Vector3 worldPosition = screenPos.ScreenPosToWorldPos(m_Camera, m_Depth);

            if (m_ObjectToMove != null)
            {
                m_ObjectToMove.transform.position = worldPosition;
            }
        }

        // �����������Ӧ����/��������
        void OnCameraUpdated(Camera camera)
        {
            m_Camera = camera;
            MoveToElement();
        }

        // ÿ��UIԪ�����û��ƶ�ʱ�ƶ�GameObject
        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            MoveToElement();
        }
    }
}