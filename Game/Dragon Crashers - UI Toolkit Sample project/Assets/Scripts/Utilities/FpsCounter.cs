using System;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// �򵥵�֡�ʼ�������
/// </summary>
namespace UIToolkitDemo
{
    public class FpsCounter : MonoBehaviour
    {
        // Ŀ��֡�ʣ��ƶ���Ϊ60��-1��ʾ�����ܿ�
        public const int k_TargetFrameRate = 60;
        // ������֡��
        public const int k_BufferSize = 50;

        [SerializeField] UIDocument m_Document;

        // ��ǰ֡��ֵ
        float m_FpsValue;
        // ��ǰ����
        int m_CurrentIndex;
        // �洢ÿ֡ʱ��Ļ�����
        float[] m_DeltaTimeBuffer;

        // ��ʾ֡�ʵı�ǩ
        Label m_FpsLabel;
        // �Ƿ�����֡�ʼ�����
        bool m_IsEnabled;

        public float FpsValue => m_FpsValue;

        // MonoBehaviour�¼���Ϣ
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
                Debug.LogWarning("[FPSCounter]: ��ʾ��ǩΪ�ա�");
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

                m_FpsLabel.text = $"֡��: {m_FpsValue}";
            }
        }

        // ����
        float CalculateFps()
        {
            float totalTime = 0f;
            foreach (float deltaTime in m_DeltaTimeBuffer)
            {
                totalTime += deltaTime;
            }

            return m_DeltaTimeBuffer.Length / totalTime;
        }

        // �¼�������
        void OnFpsCounterToggled(bool state)
        {
            m_IsEnabled = state;
            m_FpsLabel.style.visibility = (state) ? Visibility.Visible : Visibility.Hidden;
        }

        // ����Ŀ��֡��:  -1 = �����ܿ� (PC) �� 60/30 ֡/�� (�ƶ���) 
        void OnTargetFrameRateSet(int newFrameRate)
        {
            Application.targetFrameRate = newFrameRate;
        }
    }
}