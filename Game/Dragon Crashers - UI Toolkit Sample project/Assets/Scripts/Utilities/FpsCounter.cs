using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 简单的帧率计数器。
/// </summary>
namespace UIToolkitDemo
{
    public class FpsCounter : MonoBehaviour
    {
        // 目标帧率，移动端为60，-1表示尽可能快
        public const int k_TargetFrameRate = 60;
        // 采样的帧数
        public const int k_BufferSize = 50;

        [SerializeField] UIDocument m_Document;

        // 当前帧率值
        float m_FpsValue;
        // 当前索引
        int m_CurrentIndex;
        // 存储每帧时间的缓冲区
        float[] m_DeltaTimeBuffer;

        // 显示帧率的标签
        Label m_FpsLabel;
        // 是否启用帧率计数器
        bool m_IsEnabled;

        public float FpsValue => m_FpsValue;

        // MonoBehaviour事件消息
        void Awake()
        {
            m_DeltaTimeBuffer = new float[k_BufferSize];
            Application.targetFrameRate = k_TargetFrameRate;
        }

        void OnEnable()
        {
            SettingsEvents.FpsCounterToggled += OnFpsCounterToggled;
            SettingsEvents.TargetFrameRateSet += OnTargetFrameRateSet;

            var root = m_Document.rootVisualElement;

            m_FpsLabel = root.Q<Label>("fps-counter");

            if (m_FpsLabel == null)
            {
                Debug.LogWarning("[FPSCounter]: 显示标签为空。");
                return;
            }
        }

        void OnDisable()
        {
            SettingsEvents.FpsCounterToggled -= OnFpsCounterToggled;
            SettingsEvents.TargetFrameRateSet -= OnTargetFrameRateSet;
        }

        void Update()
        {
            if (m_IsEnabled)
            {
                m_DeltaTimeBuffer[m_CurrentIndex] = Time.deltaTime;
                m_CurrentIndex = (m_CurrentIndex + 1) % m_DeltaTimeBuffer.Length;
                m_FpsValue = Mathf.RoundToInt(CalculateFps());

                m_FpsLabel.text = $"帧率: {m_FpsValue}";
            }
        }

        // 方法
        float CalculateFps()
        {
            float totalTime = 0f;
            foreach (float deltaTime in m_DeltaTimeBuffer)
            {
                totalTime += deltaTime;
            }

            return m_DeltaTimeBuffer.Length / totalTime;
        }

        // 事件处理方法
        void OnFpsCounterToggled(bool state)
        {
            m_IsEnabled = state;
            m_FpsLabel.style.visibility = (state) ? Visibility.Visible : Visibility.Hidden;
        }

        // 设置目标帧率:  -1 = 尽可能快 (PC) 或 60/30 帧/秒 (移动端) 
        void OnTargetFrameRateSet(int newFrameRate)
        {
            Application.targetFrameRate = newFrameRate;
        }
    }
}