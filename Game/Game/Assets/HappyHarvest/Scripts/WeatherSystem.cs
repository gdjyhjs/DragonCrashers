using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 天气系统：当设置天气类型时，会查找场景中所有 WeatherSystemElement，
    /// 启用匹配当前天气的元素，禁用不匹配的元素。
    /// </summary>
    public class WeatherSystem : MonoBehaviour
    {
        // 天气类型（使用位标志枚举）
        [Flags]
        public enum WeatherType
        {
            Sun = 0x1,
            Rain = 0x2,
            Thunder = 0x4
        }
        // 初始天气类型
        public WeatherType StartingWeather;
        // 当前天气类型
        private WeatherType m_CurrentWeatherType;
        // 注册的天气元素列表
        private List<WeatherSystemElement> m_Elements = new List<WeatherSystemElement>();
        private void Awake()
        {
            GameManager.Instance.WeatherSystem = this;
        }
        void Start()
        {
            // 查找所有天气元素并设置初始天气
            FindAllElements();
            ChangeWeather(StartingWeather);
        }
        /// <summary>
        /// 注销天气元素
        /// </summary>
        public static void UnregisterElement(WeatherSystemElement element)
        {
#if UNITY_EDITOR
            // 在编辑器非运行状态下，手动查找实例以支持预览
            if (!Application.isPlaying)
            {
                var instance = GameObject.FindFirstObjectByType<WeatherSystem>();
                if (instance != null)
                {
                    instance.m_Elements.Remove(element);
                }
            }
            else
            {
#endif
                GameManager.Instance?.WeatherSystem?.m_Elements.Remove(element);
#if UNITY_EDITOR
            }
#endif
        }
        /// <summary>
        /// 更改天气类型
        /// </summary>
        public void ChangeWeather(WeatherType newType)
        {
            m_CurrentWeatherType = newType;
            // 切换所有元素以匹配当前天气
            SwitchAllElementsToCurrentWeather();
            // 更新 UI 天气图标
            UIHandler.UpdateWeatherIcons(newType);
        }
        /// <summary>
        /// 查找场景中所有天气元素
        /// </summary>
        void FindAllElements()
        {
            // 使用 FindObjectsByType 查找所有天气元素（包括禁用的对象）
            // 编辑器中对象可能被禁用，无法通过 Awake/Start 自注册，因此主动查找
            m_Elements = new(GameObject.FindObjectsByType<WeatherSystemElement>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        }
        /// <summary>
        /// 切换所有元素以匹配当前天气
        /// </summary>
        void SwitchAllElementsToCurrentWeather()
        {
            foreach (var element in m_Elements)
            {
                // 启用与当前天气匹配的元素，禁用不匹配的元素
                element.gameObject.SetActive(element.WeatherType.HasFlag(m_CurrentWeatherType));
            }
        }
#if UNITY_EDITOR
        // 仅在编辑器中用于测试天气系统
        public void EditorWeatherUpdate()
        {
            m_CurrentWeatherType = StartingWeather;
            FindAllElements();
            SwitchAllElementsToCurrentWeather();
        }
#endif
    }
#if UNITY_EDITOR
    /// <summary>
    /// 天气系统编辑器扩展
    /// </summary>
    [CustomEditor(typeof(WeatherSystem))]
    public class WeatherSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("更新天气");
                (target as WeatherSystem).EditorWeatherUpdate();
            }
        }
    }
#endif
}