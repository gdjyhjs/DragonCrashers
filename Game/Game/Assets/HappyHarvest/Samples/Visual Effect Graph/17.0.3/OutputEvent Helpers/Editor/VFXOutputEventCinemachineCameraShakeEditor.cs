#if VFX_OUTPUTEVENT_CINEMACHINE_2_6_0_OR_NEWER
using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph输出事件相机抖动处理器的编辑器自定义类
    /// 用于自定义VFXOutputEventCinemachineCameraShake组件在Unity编辑器中的检查器界面
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventCinemachineCameraShake))]
    class VFXOutputEventCinemachineCameraShakeEditor : VFXOutputEventHandlerEditor
    {
        // 序列化属性：Cinemachine冲击源引用
        SerializedProperty m_CinemachineImpulseSource;
        // 序列化属性：属性空间类型
        SerializedProperty m_AttributeSpace;
        /// <summary>
        /// 编辑器初始化时调用，用于获取序列化属性的引用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // 获取目标组件的 cinemachineImpulseSource 属性
            m_CinemachineImpulseSource = serializedObject.FindProperty(nameof(VFXOutputEventCinemachineCameraShake.cinemachineImpulseSource));
            // 获取目标组件的 attributeSpace 属性
            m_AttributeSpace = serializedObject.FindProperty(nameof(VFXOutputEventCinemachineCameraShake.attributeSpace));
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

            // 绘制 Cinemachine 冲击源属性字段
            EditorGUILayout.PropertyField(m_CinemachineImpulseSource);
            // 绘制属性空间类型属性字段
            EditorGUILayout.PropertyField(m_AttributeSpace);
            // 绘制帮助信息框，说明 VFX 属性的用途
            HelpBox("Attribute Usage", "- position : 相机冲击的位置 \n- velocity : 冲击速度");

            // 如果检测到 GUI 变化，应用修改到序列化对象
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif