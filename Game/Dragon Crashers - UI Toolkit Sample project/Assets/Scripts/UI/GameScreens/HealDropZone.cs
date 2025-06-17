using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(UnitController))]
    public class HealDropZone : MonoBehaviour
    {
        [Tooltip("表示每个角色上方的血瓶掉落区域。")]
        [SerializeField]
        string m_SlotID;

        [SerializeField] UIDocument m_GameScreenDocument;

        [SerializeField] Vector2 m_WorldSize = new Vector2(1.0f, 1.0f);

        [Range(1, 100)] [SerializeField] float m_PercentHealthBoost;

        VisualElement m_Slot; // 插槽
        UnitController m_UnitController; // 单位控制器
        UnitHealthBehaviour m_UnitHealth; // 单位健康行为

        int m_MaxHealth; // 最大生命值
        int m_HealthBoost; // 生命值提升
        Camera m_MainCamera; // 主相机

        public static Action UseOnePotion; // 使用一个药水的事件

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

            // 注册GeometryChangedEvent以在布局完成后更新插槽位置
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
                Debug.Log("由于缺少引用，不更新插槽位置。");
                return;
            }

            StartCoroutine(UpdateSlotPositionWithDelay());
        }

        IEnumerator UpdateSlotPositionWithDelay()
        {
            yield return new WaitForEndOfFrame();
            // 相机在纵向和横向之间切换
            m_MainCamera = Camera.main;


            // 将插槽移动到跟随UnitController的位置
            MoveToWorldPosition(m_Slot, m_UnitController.transform.position, m_WorldSize);
        }


        // 将插槽移动到匹配世界位置，类似于血条
        void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 worldSize)
        {
            // 获取相对于面板的世界位置和大小矩形
            Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, worldSize,
                m_MainCamera);

            // 使用contentRect获取VisualElement的准确宽度和高度
            float elementWidth = element.contentRect.width;
            float elementHeight = element.contentRect.height;

            // 检查元素的宽度和高度是否已解析
            if (elementWidth <= 0 || elementHeight <= 0)
            {
                // Debug.LogWarning("元素尺寸未解析，跳过位置更新。");
                return;
            }

            // 调整位置，使元素的底部中心与世界位置对齐
            // 水平居中元素并对齐底部
            element.style.left = rect.xMin - (elementWidth / 2);
            element.style.top = rect.yMin - elementHeight;
        }

        // 事件处理方法


        void OnLayoutReady(GeometryChangedEvent evt)
        {
            m_Slot.UnregisterCallback<GeometryChangedEvent>(OnLayoutReady);
            UpdateSlotPosition();
        }

        void OnSlotHealed(VisualElement activeSlot)
        {
            // 治疗关联的单位
            if (activeSlot == m_Slot)
            {
                m_UnitHealth.ChangeHealth(m_HealthBoost);

                UseOnePotion?.Invoke();
            }
        }

        // 禁用死亡单位的治疗插槽
        void OnUnitDied(UnitController deadUnit)
        {
            if (deadUnit == m_UnitController)
            {
                EnableSlot(false);
            }
        }
    }
}