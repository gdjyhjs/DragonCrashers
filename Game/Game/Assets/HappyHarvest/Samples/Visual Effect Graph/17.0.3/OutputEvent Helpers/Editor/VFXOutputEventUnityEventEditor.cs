using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph输出事件UnityEvent处理器的编辑器自定义类
    /// 用于自定义VFXOutputEventUnityEvent组件在Unity编辑器中的检查器界面
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventUnityEvent))]
    class VFXOutputEventUnityEventEditor : VFXOutputEventHandlerEditor
    {
        // 序列化属性：VFX事件触发的UnityEvent
        SerializedProperty m_OnEvent;
        /// <summary>
        /// 编辑器初始化时调用，用于获取序列化属性的引用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // 获取目标组件的 onEvent 属性
            m_OnEvent = serializedObject.FindProperty(nameof(VFXOutputEventUnityEvent.onEvent));
        }

        /// <summary>
        /// 绘制自定义检查器界面
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 开始更新序列化对象
            serializedObject.Update();
            // 开始检查 GUI 变化
            EditorGUI.BeginChangeCheck();

            // 绘制基础的输出事件属性
            DrawOutputEventProperties();
            // 绘制 UnityEvent 属性字段
            EditorGUILayout.PropertyField(m_OnEvent);
            // 绘制帮助信息框，说明该处理器不使用 VFX 属性
            HelpBox("属性用途", "此输出事件处理器不使用 VFX 属性");

            // 如果检测到 GUI 变化，应用修改到序列化对象
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}