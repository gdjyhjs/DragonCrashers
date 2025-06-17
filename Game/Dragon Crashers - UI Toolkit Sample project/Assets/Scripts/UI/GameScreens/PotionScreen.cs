using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��������ҩ�����ϷŹ��ܡ������û��϶�ҩ��ͼ�겢���������
    /// �������Ч���Ʒ��������ϣ��Ա�����Ϸ���������ƽ�ɫ��
    /// </summary>

    [RequireComponent(typeof(UIDocument))]
    public class PotionScreen : MonoBehaviour
    {
        // ��ѡ�����϶�ʱ��ʾ����ҩ�����òۡ�
        [Tooltip("��ѡ�����϶�ʱ��ʾ����ҩ�����òۡ�")]
        [SerializeField] bool m_IsSlotVisible;

        // USS����
        const string k_DropZoneClass = "healing-potion__slot";

        const string k_PotionIconActiveClass = "potion--active";
        const string k_PotionIconInactiveClass = "potion--inactive";

        // ��Ϸ��Ļ�ĵ�
        UIDocument m_Document;

        // ��Ļ����Ŀ��϶�����
        VisualElement m_DragArea;

        // ��ʼ�϶���Ԫ��
        VisualElement m_StartElement;

        // ��Ϊָ���ҩ��ͼ��
        VisualElement m_PointerIcon;

        // Ϊÿ����ɫ��ǵġ���������
        List<VisualElement> m_HealDropZones;

        // �������������ķ�������
        VisualElement m_ActiveZone;

        // ��ʾʣ��ҩ���������ı�Ԫ��
        Label m_HealPotionCount;

        // ָ�뵱ǰ�Ƿ��ڻ״̬
        bool m_IsDragging;

        // �Ƿ���һ������ҩ�����ã�
        bool m_IsPotionAvailable;

        // ���ڼ���ҩ��ͼ������ָ��֮���ƫ����
        Vector3 m_IconStartPosition;
        Vector3 m_PointerStartPosition;

        /// <summary>
        /// �����¼���
        /// </summary>
        void OnEnable()
        {
            GameplayEvents.HealingPotionUpdated += OnHealingPotionsUpdated;
        }

        /// <summary>
        /// ȡ�������¼���
        /// </summary>
        void OnDisable()
        {
            GameplayEvents.HealingPotionUpdated -= OnHealingPotionsUpdated;
        }

        /// <summary>
        /// ִ�����úͳ�ʼ����
        /// </summary>
        void Awake()
        {
            if (m_Document == null)
                m_Document = GetComponent<UIDocument>();

            �����Ӿ�Ԫ��();
            ע��ص�();
            �����϶�����();
        }

        void �����Ӿ�Ԫ��()
        {
            m_Document = GetComponent<UIDocument>();
            VisualElement rootElement = m_Document.rootVisualElement;

            m_DragArea = rootElement.Q<VisualElement>("healing-potion__drag-area"); // ��Ļ�Ľ���ʽ����
            m_StartElement = rootElement.Q<VisualElement>("healing-potion__space"); // �ɵ�����϶���UIԪ��
            m_PointerIcon = rootElement.Q<VisualElement>("healing-potion__image");  // �����϶�ҩ���Ĺ��ͼ��
            m_HealPotionCount = rootElement.Q<Label>("healing-potion__count");  // ��ʾ����ҩ�����ı���ǩ
            m_HealDropZones = rootElement.Query<VisualElement>(className: k_DropZoneClass).ToList(); // ����ҩ����λ���б�
        }

        void ע��ص�()
        {
            // �������/�����ƶ�
            m_DragArea.RegisterCallback<PointerMoveEvent>(PointerMoveEventHandler);

            // ������갴ť/��������
            m_StartElement.RegisterCallback<PointerDownEvent>(PointerDownEventHandler);

            // ������갴ť/����̧��
            m_DragArea.RegisterCallback<PointerUpEvent>(PointerUpEventHandler);

        }

        /// <summary>
        /// ����ָ���ƶ��¼�������ҩ��ͼ���λ�á�
        /// ����ֵ�����ڼ�����������Ʒ�������
        /// </summary>
        /// <param name="evt">ָ���ƶ��¼����ݡ�</param>
        void PointerMoveEventHandler(PointerMoveEvent evt)
        {
            if (m_IsDragging && m_DragArea.HasPointerCapture(evt.pointerId))
            {
                // �ƶ�ҩ��ͼ��
                �ƶ�ҩ��ͼ��(evt.position);

                // �ҵ�����ķ�������
                float activationDistance = 100f; // ������Ҫ����
                VisualElement closestZone = �ҵ�����ķ�������(evt.position, activationDistance);

                // ��������ķ�������
                ��������ķ�������(closestZone);
            }
        }

        /// <summary>
        /// ����ָ���ƶ�����ҩ��ͼ���λ�á�
        /// </summary>
        /// <param name="pointerPosition">��ǰָ��λ�á�</param>
        private void �ƶ�ҩ��ͼ��(Vector2 pointerPosition)
        {
            float newX = m_IconStartPosition.x + (pointerPosition.x - m_PointerStartPosition.x);
            float newY = m_IconStartPosition.y + (pointerPosition.y - m_PointerStartPosition.y);

            m_PointerIcon.transform.position = new Vector2(newX, newY);
        }

        /// <summary>
        /// �ҵ����뵱ǰָ��λ���ڸ�����������������Ʒ�������
        /// </summary>
        /// <param name="pointerPosition">��ǰָ��λ�á�</param>
        /// <param name="activationDistance">����ľ�����ֵ��</param>
        /// <returns>����ھ������򷵻���������Ʒ������򣬷��򷵻�null��</returns>
        VisualElement �ҵ�����ķ�������(Vector2 pointerPosition, float activationDistance)
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
        /// ������������Ʒ������򣬲�ͣ��֮ǰ�Ļ��������У���
        /// </summary>
        /// <param name="closestZone">Ҫ�������������Ʒ�������</param>
        void ��������ķ�������(VisualElement closestZone)
        {
            if (m_ActiveZone != closestZone)
            {
                // ���֮ǰ��������ڣ���ͣ����
                if (m_ActiveZone != null)
                {
                    m_ActiveZone.style.opacity = 0f;
                }

                // �������������
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
        /// ��ʼ�϶���
        /// </summary>
        /// <param name="evt"></param>
        void PointerDownEventHandler(PointerDownEvent evt)
        {
            if (!m_IsPotionAvailable)
                return;

            // �����϶��������ط��ò�
            m_DragArea.style.display = DisplayStyle.Flex;
            ���ط�������();

            // ������ָ���¼����͵�DragAreaԪ��
            m_DragArea.CapturePointer(evt.pointerId);

            // ����ͼ���ָ�����ʼλ��
            m_IconStartPosition = m_PointerIcon.transform.position;
            m_PointerStartPosition = evt.position;

            m_IsDragging = true;
        }

        /// <summary>
        /// �ͷ�ָ�롣
        /// </summary>
        /// <param name="evt"></param>
        void PointerUpEventHandler(PointerUpEvent evt)
        {
            // �����϶������ͷ�ָ��
            m_DragArea.style.display = DisplayStyle.None;
            m_DragArea.ReleasePointer(evt.pointerId);
            m_IsDragging = false;

            // �ָ�ҩ��ͼ��
            m_PointerIcon.transform.position = m_IconStartPosition;

            // ���ʹ���ѡ����λ����Ϣ
            if (m_ActiveZone != null)
            {
                // ֪ͨGameHealDrop���������
                GameplayEvents.SlotHealed?.Invoke(m_ActiveZone);
                m_ActiveZone = null;
            }
        }

        /// <summary>
        /// ͨ�����������Ʒ�������Ĳ�͸��������Ϊ�����������ǡ�
        /// </summary>
        void ���ط�������()
        {
            foreach (VisualElement slot in m_HealDropZones)
            {
                slot.style.opacity = 0f;
            }
        }

        /// <summary>
        /// ���ش�����Ļ���϶�����ʽ���ֵ�Ԫ�ء�
        /// </summary>
        void �����϶�����()
        {
            m_DragArea.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// ������ҩ�����������仯ʱ����ҩ��������
        /// </summary>
        /// <param name="potionCount">��������ҩ����������</param>
        void OnHealingPotionsUpdated(int potionCount)
        {

            m_IsPotionAvailable = (potionCount > 0);

            ����ҩ��ͼ��(m_IsPotionAvailable);

            m_HealPotionCount.text = potionCount.ToString();
        }

        /// <summary>
        /// ����ҩ���Ŀ��������û����ҩ��ͼ�ꡣ
        /// </summary>
        /// <param name="state">�����ҩ��������Ϊtrue������Ϊfalse��</param>
        void ����ҩ��ͼ��(bool state)
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