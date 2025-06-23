using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ����ϵͳ����������������ʱ������ҳ��������� WeatherSystemElement��
    /// ����ƥ�䵱ǰ������Ԫ�أ����ò�ƥ���Ԫ�ء�
    /// </summary>
    public class WeatherSystem : MonoBehaviour
    {
        // �������ͣ�ʹ��λ��־ö�٣�
        [Flags]
        public enum WeatherType
        {
            Sun = 0x1,
            Rain = 0x2,
            Thunder = 0x4
        }
        // ��ʼ��������
        public WeatherType StartingWeather;
        // ��ǰ��������
        private WeatherType m_CurrentWeatherType;
        // ע�������Ԫ���б�
        private List<WeatherSystemElement> m_Elements = new List<WeatherSystemElement>();
        private void Awake()
        {
            GameManager.Instance.WeatherSystem = this;
        }
        void Start()
        {
            // ������������Ԫ�ز����ó�ʼ����
            FindAllElements();
            ChangeWeather(StartingWeather);
        }
        /// <summary>
        /// ע������Ԫ��
        /// </summary>
        public static void UnregisterElement(WeatherSystemElement element)
        {
#if UNITY_EDITOR
            // �ڱ༭��������״̬�£��ֶ�����ʵ����֧��Ԥ��
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
        /// ������������
        /// </summary>
        public void ChangeWeather(WeatherType newType)
        {
            m_CurrentWeatherType = newType;
            // �л�����Ԫ����ƥ�䵱ǰ����
            SwitchAllElementsToCurrentWeather();
            // ���� UI ����ͼ��
            UIHandler.UpdateWeatherIcons(newType);
        }
        /// <summary>
        /// ���ҳ�������������Ԫ��
        /// </summary>
        void FindAllElements()
        {
            // ʹ�� FindObjectsByType ������������Ԫ�أ��������õĶ���
            // �༭���ж�����ܱ����ã��޷�ͨ�� Awake/Start ��ע�ᣬ�����������
            m_Elements = new(GameObject.FindObjectsByType<WeatherSystemElement>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        }
        /// <summary>
        /// �л�����Ԫ����ƥ�䵱ǰ����
        /// </summary>
        void SwitchAllElementsToCurrentWeather()
        {
            foreach (var element in m_Elements)
            {
                // �����뵱ǰ����ƥ���Ԫ�أ����ò�ƥ���Ԫ��
                element.gameObject.SetActive(element.WeatherType.HasFlag(m_CurrentWeatherType));
            }
        }
#if UNITY_EDITOR
        // ���ڱ༭�������ڲ�������ϵͳ
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
    /// ����ϵͳ�༭����չ
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
                Debug.Log("��������");
                (target as WeatherSystem).EditorWeatherUpdate();
            }
        }
    }
#endif
}