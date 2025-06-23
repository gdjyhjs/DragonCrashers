using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph 输出事件处理器的编辑器抽象基类
    /// 为所有 VFX 输出事件处理器的自定义编辑器提供通用功能和界面结构
    /// </summary>
    public abstract class VFXOutputEventHandlerEditor : Editor
    {
        // 序列化属性：是否在编辑器模式下执行
        protected SerializedProperty m_ExecuteInEditor;
        // 序列化属性：输出事件配置
        protected SerializedProperty m_OutputEvent;
        // 目标事件处理器组件引用
        protected VFXOutputEventAbstractHandler m_TargetHandler;

        /// <summary>
        /// 编辑器初始化时调用，用于获取序列化属性的引用
        /// </summary>
        protected virtual void OnEnable()
        {
            // 获取目标事件处理器组件
            m_TargetHandler = serializedObject.targetObject as VFXOutputEventAbstractHandler;
            // 获取 outputEvent 属性
            m_OutputEvent = serializedObject.FindProperty(nameof(VFXOutputEventAbstractHandler.outputEvent));
            // 获取 executeInEditor 属性
            m_ExecuteInEditor = serializedObject.FindProperty(nameof(VFXOutputEventAbstractHandler.executeInEditor));
        }

        /// <summary>
        /// 绘制输出事件的通用属性界面
        /// </summary>
        protected void DrawOutputEventProperties()
        {
            // 如果处理器支持在编辑器模式下执行，则显示执行开关
            if (m_TargetHandler.canExecuteInEditor)
                EditorGUILayout.PropertyField(m_ExecuteInEditor);
            else
                // 否则显示提示信息
                EditorGUILayout.HelpBox($"此 VFX 输出事件处理器无法在编辑模式下预览。如果需要预览其行为，请进入播放模式。", MessageType.Info);

            // 显示输出事件配置属性
            EditorGUILayout.PropertyField(m_OutputEvent);
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

            // 绘制输出事件通用属性
            DrawOutputEventProperties();

            // 如果检测到 GUI 变化，应用修改到序列化对象
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制帮助信息框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="body">内容</param>
        public void HelpBox(string title, string body)
        {
            using (new GUILayout.VerticalScope(Styles.helpBox))
            {
                GUILayout.Label(title, Styles.helpBoxTitle);
                GUILayout.Label(body, Styles.helpBoxBody);
            }
        }

        /// <summary>
        /// 自定义样式类，用于绘制帮助信息框
        /// </summary>
        static class Styles
        {
            public static GUIStyle helpBox;
            public static GUIStyle helpBoxTitle;
            public static GUIStyle helpBoxBody;

            static Styles()
            {
                // 初始化帮助框样式
                helpBox = new GUIStyle(EditorStyles.helpBox);
                helpBox.margin = new RectOffset(0, 0, 12, 0);

                // 初始化标题样式
                helpBoxTitle = new GUIStyle(EditorStyles.boldLabel);
                helpBoxTitle.margin = new RectOffset(0, 0, 0, 4);

                // 初始化内容样式
                helpBoxBody = new GUIStyle(EditorStyles.label);
                helpBoxBody.wordWrap = true;
                helpBoxBody.padding = new RectOffset(8, 0, 0, 0);
            }
        }
    }
}