using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
#if UNITY_EDITOR 
using HappyHarvest;
using UnityEditor; 
using UnityEditor.UIElements;
#endif
namespace HappyHarvest
{
    /// <summary>
    /// ��������һ����ָ��ʱ�䴥�����¼���ÿ���¼��п�ʼ�ͽ���ʱ�䷶Χ��
    /// ���ڽ�����뿪�÷�Χʱ������Ӧ�ĺ���
    /// </summary>
    [DefaultExecutionOrder(999)]
    public class DayEventHandler : MonoBehaviour
    {
        /// <summary>
        /// �ռ��¼��ṹ������ʱ�䷶Χ�Ͷ�Ӧ�¼�
        /// </summary>
        [System.Serializable]
        public class DayEvent
        {
            // �¼���ʼʱ�䣨0-1 ��׼��ʱ�䣩
            public float StartTime = 0.0f;
            // �¼�����ʱ�䣨0-1 ��׼��ʱ�䣩
            public float EndTime = 1.0f;

            // ����ʱ�䷶Χʱ�������¼�
            public UnityEvent OnEvents;
            // �뿪ʱ�䷶Χʱ�������¼�
            public UnityEvent OffEvent;

            /// <summary>
            /// �жϵ�ǰʱ���Ƿ����¼���Χ��
            /// </summary>
            /// <param name="t">��׼��ʱ�䣨0-1��</param>
            /// <returns>�Ƿ��ڷ�Χ��</returns>
            public bool IsInRange(float t)
            {
                return t >= StartTime && t <= EndTime;
            }
        }

        // �ռ��¼�����
        public DayEvent[] Events;

        private void Start()
        {
            // ע�ᵽ��Ϸ���������¼�������
            GameManager.RegisterEventHandler(this);
        }

        private void OnDisable()
        {
            // ����Ϸ������ע���¼�������
            GameManager.RemoveEventHandler(this);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// �ռ��¼����Գ��루�༭����չ��
    /// </summary>
    [CustomPropertyDrawer(typeof(DayEventHandler.DayEvent))]
    public class DayEventDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // ��ȡ��ҹѭ��������ʵ��
            var dayHandler = GameObject.FindFirstObjectByType<DayCycleHandler>();

            // ������������
            var container = new VisualElement();

            if (dayHandler != null)
            {
                // ���ҿ�ʼʱ��ͽ���ʱ������
                var minProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.StartTime));
                var maxProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.EndTime));

                // ����ʱ�䷶Χ����
                var slider = new MinMaxSlider(
                $"�ռ䷶Χ {GameManager.GetTimeAsString(minProperty.floatValue)} - {GameManager.GetTimeAsString(maxProperty.floatValue)}",
                minProperty.floatValue, maxProperty.floatValue, 0.0f, 1.0f);

                // ע�Ử��ֵ�仯�ص�
                slider.RegisterValueChangedCallback(evt =>
                {
                    minProperty.floatValue = evt.newValue.x;
                    maxProperty.floatValue = evt.newValue.y;

                    property.serializedObject.ApplyModifiedProperties();

                    // ���»����ǩ��ʾ
                    slider.label =
                    $"�ռ䷶Χ {GameManager.GetTimeAsString(minProperty.floatValue)} - {GameManager.GetTimeAsString(maxProperty.floatValue)}";
                });

                // �����¼�����
                var evtOnProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.OnEvents));
                var evtOffProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.OffEvent));

                // ��ӿؼ�������
                container.Add(slider);
                container.Add(new PropertyField(evtOnProperty, "�����¼�"));
                container.Add(new PropertyField(evtOffProperty, "�뿪�¼�"));
            }
            else
            {
                // ������û����ҹѭ��������ʱ����ʾ
                container.Add(new Label("������û�� DayCycleHandler�����Ǳ�������"));
            }

            return container;
        }
    }
#endif
}