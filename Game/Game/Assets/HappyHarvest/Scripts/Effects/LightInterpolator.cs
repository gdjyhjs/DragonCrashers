using System;
using System.Collections.Generic;
using System.Linq;
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
    /// 该类将在一天中多个 Light2D 之间进行混合过渡。这允许创建移动的阴影，
    /// 例如用于房屋或谷仓的阴影效果。
    /// </summary>
    [DefaultExecutionOrder(999)]
    [ExecuteInEditMode]
    public class LightInterpolator : MonoBehaviour
    {
        [Serializable]
        public class LightFrame
        {
            // 参考灯光
            public Light2D ReferenceLight;
            // 标准化时间（0-1）
            public float NormalizedTime;
        }

        [Tooltip("其形状将根据定义的帧进行更改的灯光")]
        // 目标灯光
        public Light2D TargetLight;
        // 灯光帧数组
        public LightFrame[] LightFrames;

        private void OnEnable()
        {
            // 注册到昼夜循环处理器
            DayCycleHandler.RegisterLightBlender(this);
        }

        private void OnDisable()
        {
            // 从昼夜循环处理器注销
            DayCycleHandler.UnregisterLightBlender(this);
        }

        /// <summary>
        /// 设置时间比例并更新灯光插值
        /// </summary>
        /// <param name="t">标准化时间（0-1）</param>
        public void SetRatio(float t)
        {
            if (LightFrames.Length == 0)
                return;

            int startFrame = 0;

            // 查找起始帧
            while (startFrame < LightFrames.Length - 1 && LightFrames[startFrame + 1].NormalizedTime < t)
            {
                startFrame += 1;
            }

            if (startFrame == LightFrames.Length - 1)
            {
                // 最后一帧无需插值，直接使用最后设置
                Interpolate(LightFrames[startFrame].ReferenceLight, LightFrames[startFrame].ReferenceLight, 0.0f);
            }
            else
            {
                // 计算帧长度和当前帧内的标准化值
                float frameLength = LightFrames[startFrame + 1].NormalizedTime - LightFrames[startFrame].NormalizedTime;
                float frameValue = t - LightFrames[startFrame].NormalizedTime;
                float normalizedFrame = frameValue / frameLength;

                // 在两帧之间进行插值
                Interpolate(LightFrames[startFrame].ReferenceLight, LightFrames[startFrame + 1].ReferenceLight, normalizedFrame);
            }
        }

        /// <summary>
        /// 在两个灯光之间进行插值
        /// </summary>
        /// <param name="start">起始灯光</param>
        /// <param name="end">结束灯光</param>
        /// <param name="t">插值系数（0-1）</param>
        void Interpolate(Light2D start, Light2D end, float t)
        {
            // 插值颜色和强度
            TargetLight.color = Color.Lerp(start.color, end.color, t);
            TargetLight.intensity = Mathf.Lerp(start.intensity, end.intensity, t);

            // 插值灯光形状路径
            var startPath = start.shapePath;
            var endPath = end.shapePath;

            var newPath = new Vector3[startPath.Length];

            for (int i = 0; i < startPath.Length; ++i)
            {
                newPath[i] = Vector3.Lerp(startPath[i], endPath[i], t);
            }

            TargetLight.SetShapePath(newPath);
        }

        /// <summary>
        /// 禁用所有参考灯光
        /// </summary>
        public void DisableAllLights()
        {
            foreach (var frame in LightFrames)
            {
                frame.ReferenceLight?.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 灯光插值器编辑器扩展
    /// </summary>
    [CustomEditor(typeof(LightInterpolator))]
    public class LightInterpolatorEditor : Editor
    {
        private LightInterpolator m_Interpolator;
        private DayCycleHandler m_DayCycleHandler;

        private Slider m_PreviewSlider;

        private void OnEnable()
        {
            m_Interpolator = target as LightInterpolator;
            m_Interpolator.DisableAllLights();
            m_Interpolator.SetRatio(0);

            m_DayCycleHandler = GameObject.FindFirstObjectByType<DayCycleHandler>();
            if (m_DayCycleHandler != null)
            {
                m_DayCycleHandler.UpdateLight(0.0f);
            }

            EditorApplication.update += SceneUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= SceneUpdate;
        }

        // 场景更新回调
        void SceneUpdate()
        {
            if (m_PreviewSlider == null ||
            m_Interpolator == null ||
            m_DayCycleHandler == null)
                return;

            m_Interpolator.SetRatio(m_PreviewSlider.value);
            m_DayCycleHandler.UpdateLight(m_PreviewSlider.value);

            SceneView.RepaintAll();
        }

        // 创建自定义检查器界面
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            CustomUI(root);

            m_PreviewSlider = new Slider("预览时间 00:00 (0)", 0.0f, 1.0f);
            m_PreviewSlider.RegisterValueChangedCallback(evt =>
            {
                m_Interpolator.SetRatio(m_PreviewSlider.value);
                if (m_DayCycleHandler != null)
                {
                    m_PreviewSlider.label = $"预览时间 {GameManager.GetTimeAsString(m_PreviewSlider.value)} ({m_PreviewSlider.value:F2})";
                    m_DayCycleHandler.UpdateLight(m_PreviewSlider.value);
                }
            });

            root.Add(m_PreviewSlider);

            return root;
        }

        // 创建自定义 UI 元素
        void CustomUI(VisualElement root)
        {
            var targetLight = serializedObject.FindProperty(nameof(LightInterpolator.TargetLight));
            var frames = serializedObject.FindProperty(nameof(LightInterpolator.LightFrames));

            var list = new ListView();

            list.showFoldoutHeader = false;
            list.showBoundCollectionSize = false;
            list.reorderable = true;
            list.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            list.reorderMode = ListViewReorderMode.Animated;
            list.showBorder = true;
            list.headerTitle = "标题";
            list.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            list.selectionType = SelectionType.Single;
            list.showAddRemoveFooter = true;

            list.style.flexGrow = 1.0f;

            list.Bind(serializedObject);
            list.bindingPath = frames.propertyPath;

            list.makeItem += () =>
            {
                var foldout = new Foldout();
                var refLight = new PropertyField()
                {
                    name = "ReferenceLightEntry"
                };
                var timeEntry = new PropertyField()
                {
                    name = "TimeEntry"
                };

                foldout.Add(refLight);
                foldout.Add(timeEntry);

                return foldout;
            };

            list.bindItem += (element, i) =>
            {
                var entry = frames.GetArrayElementAtIndex(i);

                (element as Foldout).text = entry.displayName;

                var refLight = entry.FindPropertyRelative(nameof(LightInterpolator.LightFrame.ReferenceLight));

                element.Q<PropertyField>("ReferenceLightEntry")
                .BindProperty(refLight);
                element.Q<PropertyField>("TimeEntry")
                .BindProperty(entry.FindPropertyRelative(nameof(LightInterpolator.LightFrame.NormalizedTime)));
            };

            list.unbindItem += (element, i) =>
            {
                if (element.userData != null)
                {
                    DestroyImmediate(element.userData as Editor);
                }
            };

            list.selectionChanged += objs =>
            {
                if (!objs.Any())
                    return;

                var first = objs.First() as SerializedProperty;
                m_PreviewSlider.value = first.FindPropertyRelative(nameof(LightInterpolator.LightFrame.NormalizedTime)).floatValue;
            };

            list.itemsChosen += objects =>
            {
                var first = objects.First() as SerializedProperty;

                var target = first.FindPropertyRelative(nameof(LightInterpolator.LightFrame.ReferenceLight))
                .objectReferenceValue as Light2D;

                if (target != null) target.gameObject.SetActive(true);

                Selection.activeObject = target;
            };

            root.Add(list);
        }
    }
#endif

}