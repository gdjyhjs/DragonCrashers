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
    /// 允许定义在一天中指定时间触发的事件。每个事件有开始和结束时间范围，
    /// 并在进入或离开该范围时触发相应的函数
    /// </summary>
    [DefaultExecutionOrder(999)]
    public class DayEventHandler : MonoBehaviour
    {
        /// <summary>
        /// 日间事件结构，包含时间范围和对应事件
        /// </summary>
        [System.Serializable]
        public class DayEvent
        {
            // 事件开始时间（0-1 标准化时间）
            public float StartTime = 0.0f;
            // 事件结束时间（0-1 标准化时间）
            public float EndTime = 1.0f;

            // 进入时间范围时触发的事件
            public UnityEvent OnEvents;
            // 离开时间范围时触发的事件
            public UnityEvent OffEvent;

            /// <summary>
            /// 判断当前时间是否在事件范围内
            /// </summary>
            /// <param name="t">标准化时间（0-1）</param>
            /// <returns>是否在范围内</returns>
            public bool IsInRange(float t)
            {
                return t >= StartTime && t <= EndTime;
            }
        }

        // 日间事件数组
        public DayEvent[] Events;

        private void Start()
        {
            // 注册到游戏管理器的事件处理器
            GameManager.RegisterEventHandler(this);
        }

        private void OnDisable()
        {
            // 从游戏管理器注销事件处理器
            GameManager.RemoveEventHandler(this);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 日间事件属性抽屉（编辑器扩展）
    /// </summary>
    [CustomPropertyDrawer(typeof(DayEventHandler.DayEvent))]
    public class DayEventDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // 获取昼夜循环处理器实例
            var dayHandler = GameObject.FindFirstObjectByType<DayCycleHandler>();

            // 创建属性容器
            var container = new VisualElement();

            if (dayHandler != null)
            {
                // 查找开始时间和结束时间属性
                var minProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.StartTime));
                var maxProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.EndTime));

                // 创建时间范围滑块
                var slider = new MinMaxSlider(
                $"日间范围 {GameManager.GetTimeAsString(minProperty.floatValue)} - {GameManager.GetTimeAsString(maxProperty.floatValue)}",
                minProperty.floatValue, maxProperty.floatValue, 0.0f, 1.0f);

                // 注册滑块值变化回调
                slider.RegisterValueChangedCallback(evt =>
                {
                    minProperty.floatValue = evt.newValue.x;
                    maxProperty.floatValue = evt.newValue.y;

                    property.serializedObject.ApplyModifiedProperties();

                    // 更新滑块标签显示
                    slider.label =
                    $"日间范围 {GameManager.GetTimeAsString(minProperty.floatValue)} - {GameManager.GetTimeAsString(maxProperty.floatValue)}";
                });

                // 查找事件属性
                var evtOnProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.OnEvents));
                var evtOffProperty = property.FindPropertyRelative(nameof(DayEventHandler.DayEvent.OffEvent));

                // 添加控件到容器
                container.Add(slider);
                container.Add(new PropertyField(evtOnProperty, "进入事件"));
                container.Add(new PropertyField(evtOffProperty, "离开事件"));
            }
            else
            {
                // 场景中没有昼夜循环处理器时的提示
                container.Add(new Label("场景中没有 DayCycleHandler，这是必需的组件"));
            }

            return container;
        }
    }
#endif
}