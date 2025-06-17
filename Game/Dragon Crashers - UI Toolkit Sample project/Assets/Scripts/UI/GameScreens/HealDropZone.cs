using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(UnitController))]
    public class HealDropZone : MonoBehaviour
    {
        [Tooltip("��ʾÿ����ɫ�Ϸ���Ѫƿ��������")]
        [SerializeField]
        string m_SlotID;

        [SerializeField] UIDocument m_GameScreenDocument;

        [SerializeField] Vector2 m_WorldSize = new Vector2(1.0f, 1.0f);

        [Range(1, 100)] [SerializeField] float m_PercentHealthBoost;

        VisualElement m_Slot; // ���
        UnitController m_UnitController; // ��λ������
        UnitHealthBehaviour m_UnitHealth; // ��λ������Ϊ

        int m_MaxHealth; // �������ֵ
        int m_HealthBoost; // ����ֵ����
        Camera m_MainCamera; // �����

        public static Action UseOnePotion; // ʹ��һ��ҩˮ���¼�

        void OnEnable()
        {
            GameplayEvents.SlotHealed += OnSlotHealed;

            UnitController.UnitDied += OnUnitDied;
        }

        void OnDisable()
        {
            GameplayEvents.SlotHealed -= OnSlotHealed;

            UnitController.UnitDied -= OnUnitDied;
        }

        void Start()
        {
            m_MainCamera = Camera.main;

            m_UnitController = GetComponent<UnitController>();
            m_UnitHealth = m_UnitController.healthBehaviour;
            m_MaxHealth = m_UnitController.data.totalHealth;
            m_HealthBoost = (int)(m_MaxHealth * m_PercentHealthBoost / 100f);
            SetVisualElements();
        }

        void SetVisualElements()
        {
            VisualElement rootElement = m_GameScreenDocument.rootVisualElement;

            m_Slot = rootElement.Query<VisualElement>(m_SlotID);

            EnableSlot(true);

            // ע��GeometryChangedEvent���ڲ�����ɺ���²��λ��
            m_Slot.RegisterCallback<GeometryChangedEvent>(OnLayoutReady);
        }

        void EnableSlot(bool state)
        {
            if (m_Slot == null)
                return;

            m_Slot.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void UpdateSlotPosition()
        {
            if (m_Slot == null || m_MainCamera == null || m_UnitController == null)
            {
                Debug.Log("����ȱ�����ã������²��λ�á�");
                return;
            }

            StartCoroutine(UpdateSlotPositionWithDelay());
        }

        IEnumerator UpdateSlotPositionWithDelay()
        {
            yield return new WaitForEndOfFrame();
            // ���������ͺ���֮���л�
            m_MainCamera = Camera.main;


            // ������ƶ�������UnitController��λ��
            MoveToWorldPosition(m_Slot, m_UnitController.transform.position, m_WorldSize);
        }


        // ������ƶ���ƥ������λ�ã�������Ѫ��
        void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 worldSize)
        {
            // ��ȡ�������������λ�úʹ�С����
            Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, worldSize,
                m_MainCamera);

            // ʹ��contentRect��ȡVisualElement��׼ȷ��Ⱥ͸߶�
            float elementWidth = element.contentRect.width;
            float elementHeight = element.contentRect.height;

            // ���Ԫ�صĿ�Ⱥ͸߶��Ƿ��ѽ���
            if (elementWidth <= 0 || elementHeight <= 0)
            {
                // Debug.LogWarning("Ԫ�سߴ�δ����������λ�ø��¡�");
                return;
            }

            // ����λ�ã�ʹԪ�صĵײ�����������λ�ö���
            // ˮƽ����Ԫ�ز�����ײ�
            element.style.left = rect.xMin - (elementWidth / 2);
            element.style.top = rect.yMin - elementHeight;
        }

        // �¼�������


        void OnLayoutReady(GeometryChangedEvent evt)
        {
            m_Slot.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
            UpdateSlotPosition();
        }

        void OnSlotHealed(VisualElement activeSlot)
        {
            // ���ƹ����ĵ�λ
            if (activeSlot == m_Slot)
            {
                m_UnitHealth.ChangeHealth(m_HealthBoost);

                UseOnePotion?.Invoke();
            }
        }

        // ����������λ�����Ʋ��
        void OnUnitDied(UnitController deadUnit)
        {
            if (deadUnit == m_UnitController)
            {
                EnableSlot(false);
            }
        }
    }
}