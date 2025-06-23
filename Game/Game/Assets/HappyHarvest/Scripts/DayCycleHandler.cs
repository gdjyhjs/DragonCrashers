using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace HappyHarvest
{

    /// <summary>
    /// ������Ϸ�е���ҹѭ��ϵͳ��������Ҫ��ʱ��仯�Ķ��󶼻�ע�ᵽ�����������
    /// �����ڸ���ʱͳһ��������Ӱʵ�������ղ�ֵ���ȣ���
    /// ��ϵͳ�ĸ��¿��Ա���ͣ��������Ϸ��ͣ���������ʱ�����á�
    /// </summary>
    [DefaultExecutionOrder(10)]
    public class DayCycleHandler : MonoBehaviour
    {
        // �ƹ���ڵ�
        public Transform LightsRoot;

        [Header("�չ�����")]
        // ���չ�
        public Light2D DayLight;
        // �չ���ɫ����
        public Gradient DayLightGradient;

        [Header("ҹ������")]
        // �¹�
        public Light2D NightLight;
        // �¹���ɫ����
        public Gradient NightLightGradient;

        [Header("����������")]
        // ������
        public Light2D AmbientLight;
        // ��������ɫ����
        public Gradient AmbientLightGradient;

        [Header("��Ե������")]
        // ̫����Ե��
        public Light2D SunRimLight;
        // ̫����Ե����ɫ����
        public Gradient SunRimLightGradient;
        // ������Ե��
        public Light2D MoonRimLight;
        // ������Ե����ɫ����
        public Gradient MoonRimLightGradient;

        [Tooltip("�Ƕ�0=���ϣ�˳ʱ����ת����ʱ��仯")]
        // ��Ӱ�Ƕ���ʱ��仯����
        public AnimationCurve ShadowAngle;
        [Tooltip("������Ӱ���ȵ����ű���(0��1)����ʱ��仯")]
        // ��Ӱ������ʱ��仯����
        public AnimationCurve ShadowLength;

        // ע�����Ӱʵ���б�
        private List<ShadowInstance> m_Shadows = new();
        // ע��ĵƹ��ֵ���б�
        private List<LightInterpolator> m_LightBlenders = new();

        private void Awake()
        {
            GameManager.Instance.DayCycleHandler = this;
        }

        /// <summary>
        /// ʹ����ʽ��Tick��������Update���Ա�GameManager���Կ���ʱ�����Ż���ͣ
        /// </summary>
        public void Tick()
        {
            UpdateLight(GameManager.Instance.CurrentDayRatio);
        }

        /// <summary>
        /// ����ʱ������������еƹ����Ӱ
        /// </summary>
        public void UpdateLight(float ratio)
        {
            // ���¸��ƹ���ɫ
            DayLight.color = DayLightGradient.Evaluate(ratio);
            NightLight.color = NightLightGradient.Evaluate(ratio);

#if UNITY_EDITOR
            // �ڱ༭�����������ʱû������ĳЩ�ƹ�
            if (AmbientLight != null)
#endif
                AmbientLight.color = AmbientLightGradient.Evaluate(ratio);

#if UNITY_EDITOR
            if (SunRimLight != null)
#endif
                SunRimLight.color = SunRimLightGradient.Evaluate(ratio);

#if UNITY_EDITOR
            if (MoonRimLight != null)
#endif
                MoonRimLight.color = MoonRimLightGradient.Evaluate(ratio);

            // ��ת�ƹ���ڵ㣬ģ��̫�����������ƶ�
            LightsRoot.rotation = Quaternion.Euler(0, 0, 360.0f * ratio);

            // ������Ӱ
            UpdateShadow(ratio);
        }

        /// <summary>
        /// ����ʱ�����������Ӱ
        /// </summary>
        void UpdateShadow(float ratio)
        {
            // ��ȡ��ǰ��Ӱ�ǶȺͳ���
            var currentShadowAngle = ShadowAngle.Evaluate(ratio);
            var currentShadowLength = ShadowLength.Evaluate(ratio);

            // �����෴����ĽǶ�
            var opposedAngle = currentShadowAngle + 0.5f;
            while (currentShadowAngle > 1.0f)
                currentShadowAngle -= 1.0f;

            // ��������ע�����Ӱʵ��
            foreach (var shadow in m_Shadows)
            {
                var t = shadow.transform;
                // ������Ӱ�ǶȺͳ���
                t.eulerAngles = new Vector3(0, 0, currentShadowAngle * 360.0f);
                t.localScale = new Vector3(1, 1f * shadow.BaseLength * currentShadowLength, 1);
            }

            // ��������ע��ĵƹ��ֵ��
            foreach (var handler in m_LightBlenders)
            {
                handler.SetRatio(ratio);
            }
        }

        /// <summary>
        /// ������ҹѭ������
        /// </summary>
        public void Save(ref DayCycleHandlerSaveData data)
        {
            //data.TimeOfTheDay = m_CurrentTimeOfTheDay;
        }

        /// <summary>
        /// ������ҹѭ������
        /// </summary>
        public void Load(DayCycleHandlerSaveData data)
        {
            //m_CurrentTimeOfTheDay = data.TimeOfTheDay;
            //StartingTime = m_CurrentTimeOfTheDay;
        }

        /// <summary>
        /// ע����Ӱʵ��
        /// </summary>
        public static void RegisterShadow(ShadowInstance shadow)
        {
#if UNITY_EDITOR
            // �ڱ༭��������״̬�£��ֶ�����ʵ����֧��Ԥ��
            if (!Application.isPlaying)
            {
                var instance = GameObject.FindFirstObjectByType<DayCycleHandler>();
                if (instance != null)
                {
                    instance.m_Shadows.Add(shadow);
                }
            }
            else
            {
#endif
                GameManager.Instance.DayCycleHandler.m_Shadows.Add(shadow);
#if UNITY_EDITOR
            }
#endif
        }

        /// <summary>
        /// ע����Ӱʵ��
        /// </summary>
        public static void UnregisterShadow(ShadowInstance shadow)
        {
#if UNITY_EDITOR
            // �ڱ༭��������״̬�£��ֶ�����ʵ����֧��Ԥ��
            if (!Application.isPlaying)
            {
                var instance = GameObject.FindFirstObjectByType<DayCycleHandler>();
                if (instance != null)
                {
                    instance.m_Shadows.Remove(shadow);
                }
            }
            else
            {
#endif
                if (GameManager.Instance?.DayCycleHandler != null)
                    GameManager.Instance.DayCycleHandler.m_Shadows.Remove(shadow);
#if UNITY_EDITOR
            }
#endif
        }

        /// <summary>
        /// ע��ƹ��ֵ��
        /// </summary>
        public static void RegisterLightBlender(LightInterpolator interpolator)
        {
#if UNITY_EDITOR
            // �ڱ༭��������״̬�£��ֶ�����ʵ����֧��Ԥ��
            if (!Application.isPlaying)
            {
                var instance = FindFirstObjectByType<DayCycleHandler>();
                if (instance != null)
                {
                    instance.m_LightBlenders.Add(interpolator);
                }
            }
            else
            {
#endif
                GameManager.Instance.DayCycleHandler.m_LightBlenders.Add(interpolator);
#if UNITY_EDITOR
            }
#endif
        }

        /// <summary>
        /// ע���ƹ��ֵ��
        /// </summary>
        public static void UnregisterLightBlender(LightInterpolator interpolator)
        {
#if UNITY_EDITOR
            // �ڱ༭��������״̬�£��ֶ�����ʵ����֧��Ԥ��
            if (!Application.isPlaying)
            {
                var instance = FindFirstObjectByType<DayCycleHandler>();
                if (instance != null)
                {
                    instance.m_LightBlenders.Remove(interpolator);
                }
            }
            else
            {
#endif
                if (GameManager.Instance?.DayCycleHandler != null)
                    GameManager.Instance.DayCycleHandler.m_LightBlenders.Remove(interpolator);
#if UNITY_EDITOR
            }
#endif
        }
    }

    /// <summary>
    /// ��ҹѭ���������������ݽṹ
    /// </summary>
    [System.Serializable]
    public struct DayCycleHandlerSaveData
    {
        // ��ǰʱ���
        public float TimeOfTheDay;
    }


#if UNITY_EDITOR
    /// <summary>
    /// ��ҹѭ���༭����չ
    /// </summary>
    [CustomEditor(typeof(DayCycleHandler))]
    class DayCycleEditor : Editor
    {
        private DayCycleHandler m_Target;

        // �����Զ�������GUI
        public override VisualElement CreateInspectorGUI()
        {
            m_Target = target as DayCycleHandler;

            var root = new VisualElement();

            // ���Ĭ�ϼ��������
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            // ���ʱ����Ի���
            var slider = new Slider(0.0f, 1.0f);
            slider.label = "����ʱ�� 0:00";
            slider.RegisterValueChangedCallback(evt =>
            {
                m_Target.UpdateLight(evt.newValue);

                slider.label = $"����ʱ�� {GameManager.GetTimeAsString(evt.newValue)} ({evt.newValue:F2})";
                SceneView.RepaintAll();
            });

            // ע�����¼���ȷ���κοؼ����Ķ����¹���
            root.RegisterCallback<ClickEvent>(evt =>
            {
                m_Target.UpdateLight(slider.value);
                SceneView.RepaintAll();
            });

            root.Add(slider);

            return root;
        }
    }
#endif

}
