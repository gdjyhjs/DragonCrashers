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
    /// 处理游戏中的昼夜循环系统。所有需要随时间变化的对象都会注册到这个处理器，
    /// 它会在更新时统一处理（如阴影实例、光照插值器等）。
    /// 该系统的更新可以被暂停，这在游戏暂停或过场动画时很有用。
    /// </summary>
    [DefaultExecutionOrder(10)]
    public class DayCycleHandler : MonoBehaviour
    {
        // 灯光根节点
        public Transform LightsRoot;

        [Header("日光设置")]
        // 主日光
        public Light2D DayLight;
        // 日光颜色渐变
        public Gradient DayLightGradient;

        [Header("夜光设置")]
        // 月光
        public Light2D NightLight;
        // 月光颜色渐变
        public Gradient NightLightGradient;

        [Header("环境光设置")]
        // 环境光
        public Light2D AmbientLight;
        // 环境光颜色渐变
        public Gradient AmbientLightGradient;

        [Header("边缘光设置")]
        // 太阳边缘光
        public Light2D SunRimLight;
        // 太阳边缘光颜色渐变
        public Gradient SunRimLightGradient;
        // 月亮边缘光
        public Light2D MoonRimLight;
        // 月亮边缘光颜色渐变
        public Gradient MoonRimLightGradient;

        [Tooltip("角度0=向上，顺时针旋转，随时间变化")]
        // 阴影角度随时间变化曲线
        public AnimationCurve ShadowAngle;
        [Tooltip("正常阴影长度的缩放比例(0到1)，随时间变化")]
        // 阴影长度随时间变化曲线
        public AnimationCurve ShadowLength;

        // 注册的阴影实例列表
        private List<ShadowInstance> m_Shadows = new();
        // 注册的灯光插值器列表
        private List<LightInterpolator> m_LightBlenders = new();

        private void Awake()
        {
            GameManager.Instance.DayCycleHandler = this;
        }

        /// <summary>
        /// 使用显式的Tick函数而非Update，以便GameManager可以控制时间流逝或暂停
        /// </summary>
        public void Tick()
        {
            UpdateLight(GameManager.Instance.CurrentDayRatio);
        }

        /// <summary>
        /// 根据时间比例更新所有灯光和阴影
        /// </summary>
        public void UpdateLight(float ratio)
        {
            // 更新各灯光颜色
            DayLight.color = DayLightGradient.Evaluate(ratio);
            NightLight.color = NightLightGradient.Evaluate(ratio);

#if UNITY_EDITOR
            // 在编辑器中允许测试时没有设置某些灯光
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

            // 旋转灯光根节点，模拟太阳和月亮的移动
            LightsRoot.rotation = Quaternion.Euler(0, 0, 360.0f * ratio);

            // 更新阴影
            UpdateShadow(ratio);
        }

        /// <summary>
        /// 根据时间比例更新阴影
        /// </summary>
        void UpdateShadow(float ratio)
        {
            // 获取当前阴影角度和长度
            var currentShadowAngle = ShadowAngle.Evaluate(ratio);
            var currentShadowLength = ShadowLength.Evaluate(ratio);

            // 计算相反方向的角度
            var opposedAngle = currentShadowAngle + 0.5f;
            while (currentShadowAngle > 1.0f)
                currentShadowAngle -= 1.0f;

            // 更新所有注册的阴影实例
            foreach (var shadow in m_Shadows)
            {
                var t = shadow.transform;
                // 设置阴影角度和长度
                t.eulerAngles = new Vector3(0, 0, currentShadowAngle * 360.0f);
                t.localScale = new Vector3(1, 1f * shadow.BaseLength * currentShadowLength, 1);
            }

            // 更新所有注册的灯光插值器
            foreach (var handler in m_LightBlenders)
            {
                handler.SetRatio(ratio);
            }
        }

        /// <summary>
        /// 保存昼夜循环数据
        /// </summary>
        public void Save(ref DayCycleHandlerSaveData data)
        {
            //data.TimeOfTheDay = m_CurrentTimeOfTheDay;
        }

        /// <summary>
        /// 加载昼夜循环数据
        /// </summary>
        public void Load(DayCycleHandlerSaveData data)
        {
            //m_CurrentTimeOfTheDay = data.TimeOfTheDay;
            //StartingTime = m_CurrentTimeOfTheDay;
        }

        /// <summary>
        /// 注册阴影实例
        /// </summary>
        public static void RegisterShadow(ShadowInstance shadow)
        {
#if UNITY_EDITOR
            // 在编辑器非运行状态下，手动查找实例以支持预览
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
        /// 注销阴影实例
        /// </summary>
        public static void UnregisterShadow(ShadowInstance shadow)
        {
#if UNITY_EDITOR
            // 在编辑器非运行状态下，手动查找实例以支持预览
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
        /// 注册灯光插值器
        /// </summary>
        public static void RegisterLightBlender(LightInterpolator interpolator)
        {
#if UNITY_EDITOR
            // 在编辑器非运行状态下，手动查找实例以支持预览
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
        /// 注销灯光插值器
        /// </summary>
        public static void UnregisterLightBlender(LightInterpolator interpolator)
        {
#if UNITY_EDITOR
            // 在编辑器非运行状态下，手动查找实例以支持预览
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
    /// 昼夜循环处理器保存数据结构
    /// </summary>
    [System.Serializable]
    public struct DayCycleHandlerSaveData
    {
        // 当前时间点
        public float TimeOfTheDay;
    }


#if UNITY_EDITOR
    /// <summary>
    /// 昼夜循环编辑器扩展
    /// </summary>
    [CustomEditor(typeof(DayCycleHandler))]
    class DayCycleEditor : Editor
    {
        private DayCycleHandler m_Target;

        // 创建自定义检查器GUI
        public override VisualElement CreateInspectorGUI()
        {
            m_Target = target as DayCycleHandler;

            var root = new VisualElement();

            // 添加默认检查器属性
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            // 添加时间测试滑块
            var slider = new Slider(0.0f, 1.0f);
            slider.label = "测试时间 0:00";
            slider.RegisterValueChangedCallback(evt =>
            {
                m_Target.UpdateLight(evt.newValue);

                slider.label = $"测试时间 {GameManager.GetTimeAsString(evt.newValue)} ({evt.newValue:F2})";
                SceneView.RepaintAll();
            });

            // 注册点击事件，确保任何控件更改都更新光照
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
