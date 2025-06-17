using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理治疗药剂的拖放功能。允许用户拖动药剂图标并将其放置在
    /// 最近的有效治疗放置区域上，以便在游戏过程中治疗角色。
    /// </summary>

    [RequireComponent(typeof(UIDocument))]
    public class PotionScreen : MonoBehaviour
    {
        // 勾选以在拖动时显示治疗药剂放置槽。
        [Tooltip("勾选以在拖动时显示治疗药剂放置槽。")]
        [SerializeField] bool m_IsSlotVisible;

        // USS类名
        const string k_DropZoneClass = "healing-potion__slot";

        const string k_PotionIconActiveClass = "potion--active";
        const string k_PotionIconInactiveClass = "potion--inactive";

        // 游戏屏幕文档
        UIDocument m_Document;

        // 屏幕区域的可拖动部分
        VisualElement m_DragArea;

        // 开始拖动的元素
        VisualElement m_StartElement;

        // 作为指针的药剂图像
        VisualElement m_PointerIcon;

        // 为每个角色标记的“放置区域”
        List<VisualElement> m_HealDropZones;

        // 激活距离内最近的放置区域
        VisualElement m_ActiveZone;

        // 显示剩余药剂数量的文本元素
        Label m_HealPotionCount;

        // 指针当前是否处于活动状态
        bool m_IsDragging;

        // 是否还有一个或多个药剂可用？
        bool m_IsPotionAvailable;

        // 用于计算药剂图标和鼠标指针之间的偏移量
        Vector3 m_IconStartPosition;
        Vector3 m_PointerStartPosition;

        /// <summary>
        /// 订阅事件。
        /// </summary>
        void OnEnable()
        {
            GameplayEvents.HealingPotionUpdated += OnHealingPotionsUpdated;
        }

        /// <summary>
        /// 取消订阅事件。
        /// </summary>
        void OnDisable()
        {
            GameplayEvents.HealingPotionUpdated -= OnHealingPotionsUpdated;
        }

        /// <summary>
        /// 执行设置和初始化。
        /// </summary>
        void Awake()
        {
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();

            设置视觉元素();
            注册回调();
            隐藏拖动区域();
        }

        void 设置视觉元素()
        {
            m_Document = GetComponent<UIDocument>();
            VisualElement rootElement = m_Document.rootVisualElement;

            m_DragArea = rootElement.Q<VisualElement>("healing-potion__drag-area"); // 屏幕的交互式部分
            m_StartElement = rootElement.Q<VisualElement>("healing-potion__space"); // 可点击并拖动的UI元素
            m_PointerIcon = rootElement.Q<VisualElement>("healing-potion__image");  // 代表拖动药剂的光标图标
            m_HealPotionCount = rootElement.Q<Label>("healing-potion__count");  // 显示可用药剂的文本标签
            m_HealDropZones = rootElement.Query<VisualElement>(className: k_DropZoneClass).ToList(); // 放置药剂的位置列表
        }

        void 注册回调()
        {
            // 监听鼠标/触摸移动
            m_DragArea.RegisterCallback<PointerMoveEvent>(PointerMoveEventHandler);

            // 监听鼠标按钮/触摸按下
            m_StartElement.RegisterCallback<PointerDownEvent>(PointerDownEventHandler);

            // 监听鼠标按钮/触摸抬起
            m_DragArea.RegisterCallback<PointerUpEvent>(PointerUpEventHandler);

        }

        /// <summary>
        /// 处理指针移动事件并更新药剂图标的位置。
        /// 在阈值距离内激活最近的治疗放置区域。
        /// </summary>
        /// <param name="evt">指针移动事件数据。</param>
        void PointerMoveEventHandler(PointerMoveEvent evt)
        {
            if (m_IsDragging && m_DragArea.HasPointerCapture(evt.pointerId))
            {
                // 移动药剂图标
                移动药剂图标(evt.position);

                // 找到最近的放置区域
                float activationDistance = 100f; // 根据需要调整
                VisualElement closestZone = 找到最近的放置区域(evt.position, activationDistance);

                // 激活最近的放置区域
                激活最近的放置区域(closestZone);
            }
        }

        /// <summary>
        /// 根据指针移动更新药剂图标的位置。
        /// </summary>
        /// <param name="pointerPosition">当前指针位置。</param>
        private void 移动药剂图标(Vector2 pointerPosition)
        {
            float newX = m_IconStartPosition.x + (pointerPosition.x - m_PointerStartPosition.x);
            float newY = m_IconStartPosition.y + (pointerPosition.y - m_PointerStartPosition.y);

            m_PointerIcon.transform.position = new Vector2(newX, newY);
        }

        /// <summary>
        /// 找到距离当前指针位置在给定距离内最近的治疗放置区域。
        /// </summary>
        /// <param name="pointerPosition">当前指针位置。</param>
        /// <param name="activationDistance">激活的距离阈值。</param>
        /// <returns>如果在距离内则返回最近的治疗放置区域，否则返回null。</returns>
        VisualElement 找到最近的放置区域(Vector2 pointerPosition, float activationDistance)
        {
            float closestDistance = float.MaxValue;
            VisualElement closestZone = null;

            foreach (VisualElement slot in m_HealDropZones)
            {
                Vector2 slotCenter = slot.worldBound.center;
                float distance = Vector2.Distance(pointerPosition, slotCenter);

                if (distance < activationDistance && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestZone = slot;
                }
            }

            return closestZone;
        }

        /// <summary>
        /// 激活最近的治疗放置区域，并停用之前的活动区域（如果有）。
        /// </summary>
        /// <param name="closestZone">要激活的最近的治疗放置区域。</param>
        void 激活最近的放置区域(VisualElement closestZone)
        {
            if (m_ActiveZone != closestZone)
            {
                // 如果之前的区域存在，则停用它
                if (m_ActiveZone != null)
                {
                    m_ActiveZone.style.opacity = 0f;
                }

                // 激活最近的区域
                if (closestZone != null)
                {
                    m_ActiveZone = closestZone;
                    m_ActiveZone.style.opacity = 0.25f;
                }
                else
                {
                    m_ActiveZone = null;
                }
            }
        }

        /// <summary>
        /// 开始拖动。
        /// </summary>
        /// <param name="evt"></param>
        void PointerDownEventHandler(PointerDownEvent evt)
        {
            if (!m_IsPotionAvailable)
                return;

            // 启用拖动区域并隐藏放置槽
            m_DragArea.style.display = DisplayStyle.Flex;
            隐藏放置区域();

            // 将所有指针事件发送到DragArea元素
            m_DragArea.CapturePointer(evt.pointerId);

            // 设置图标和指针的起始位置
            m_IconStartPosition = m_PointerIcon.transform.position;
            m_PointerStartPosition = evt.position;

            m_IsDragging = true;
        }

        /// <summary>
        /// 释放指针。
        /// </summary>
        /// <param name="evt"></param>
        void PointerUpEventHandler(PointerUpEvent evt)
        {
            // 禁用拖动区域并释放指针
            m_DragArea.style.display = DisplayStyle.None;
            m_DragArea.ReleasePointer(evt.pointerId);
            m_IsDragging = false;

            // 恢复药剂图标
            m_PointerIcon.transform.position = m_IconStartPosition;

            // 发送带有选定槽位的消息
            if (m_ActiveZone != null)
            {
                // 通知GameHealDrop组件并重置
                GameplayEvents.SlotHealed?.Invoke(m_ActiveZone);
                m_ActiveZone = null;
            }
        }

        /// <summary>
        /// 通过将所有治疗放置区域的不透明度设置为零来隐藏它们。
        /// </summary>
        void 隐藏放置区域()
        {
            foreach (VisualElement slot in m_HealDropZones)
            {
                slot.style.opacity = 0f;
            }
        }

        /// <summary>
        /// 隐藏代表屏幕可拖动交互式部分的元素。
        /// </summary>
        void 隐藏拖动区域()
        {
            m_DragArea.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// 当可用药剂数量发生变化时更新药剂计数。
        /// </summary>
        /// <param name="potionCount">可用治疗药剂的数量。</param>
        void OnHealingPotionsUpdated(int potionCount)
        {

            m_IsPotionAvailable = (potionCount > 0);

            启用药剂图标(m_IsPotionAvailable);

            m_HealPotionCount.text = potionCount.ToString();
        }

        /// <summary>
        /// 根据药剂的可用性启用或禁用药剂图标。
        /// </summary>
        /// <param name="state">如果有药剂可用则为true，否则为false。</param>
        void 启用药剂图标(bool state)
        {
            if (state)
            {
                m_PointerIcon.RemoveFromClassList(k_PotionIconInactiveClass);
                m_PointerIcon.AddToClassList(k_PotionIconActiveClass);
            }
            else
            {
                m_PointerIcon.RemoveFromClassList(k_PotionIconActiveClass);
                m_PointerIcon.AddToClassList(k_PotionIconInactiveClass);
            }
        }

    }
}