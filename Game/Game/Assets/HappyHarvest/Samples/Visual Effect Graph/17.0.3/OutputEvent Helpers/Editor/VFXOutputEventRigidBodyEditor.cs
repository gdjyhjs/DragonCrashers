#if VFX_OUTPUTEVENT_PHYSICS
using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{

    /// <summary>
    /// Visual Effect Graph输出事件刚体处理器的编辑器自定义类
    /// 用于自定义VFXOutputEventRigidBody组件在Unity编辑器中的检查器界面
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventRigidBody))]
    class VFXOutputEventRigidBodyEditor : VFXOutputEventHandlerEditor
    { // 序列化属性：刚体组件引用
        SerializedProperty m_RigidBody;
        // 序列化属性：属性空间类型
        SerializedProperty m_AttributeSpace;
        // 序列化属性：刚体事件类型
        SerializedProperty m_EventType;
        /// <summary>
        /// 编辑器初始化时调用，用于获取序列化属性的引用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // 获取目标组件的 rigidBody 属性
            m_RigidBody = serializedObject.FindProperty(nameof(VFXOutputEventRigidBody.rigidBody));
            // 获取目标组件的 attributeSpace 属性
            m_AttributeSpace = serializedObject.FindProperty(nameof(VFXOutputEventRigidBody.attributeSpace));
            // 获取目标组件的 eventType 属性
            m_EventType = serializedObject.FindProperty(nameof(VFXOutputEventRigidBody.eventType));
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

            // 绘制刚体组件属性字段
            EditorGUILayout.PropertyField(m_RigidBody);
            // 绘制属性空间类型属性字段
            EditorGUILayout.PropertyField(m_AttributeSpace);
            // 绘制刚体事件类型属性字段
            EditorGUILayout.PropertyField(m_EventType);

            // 根据不同的事件类型生成帮助文本
            var helpText = string.Empty;
            switch ((VFXOutputEventRigidBody.RigidBodyEventType)(m_EventType.intValue))
            {
                default:
                case VFXOutputEventRigidBody.RigidBodyEventType.Impulse:
                    helpText = "- velocity : 冲量力";
                    break;
                case VFXOutputEventRigidBody.RigidBodyEventType.Explosion:
                    helpText = "- velocity : 作为力大小的量值 \n - position : 爆炸中心 \n - size : 爆炸半径";
                    break;
                case VFXOutputEventRigidBody.RigidBodyEventType.VelocityChange:
                    helpText = "- velocity : 刚体的新速度";
                    break;
            }
            // 绘制帮助信息框，说明 VFX 属性的用途
            HelpBox("Attribute Usage", helpText);

            if (EditorGUI.EndChangeCheck())
            {
                // 获取新的刚体引用
                var newRigidBody = m_RigidBody.objectReferenceValue;
                // 验证引用是否为预制体实例且未连接到预制体资源
                if (newRigidBody != null
                && PrefabUtility.GetPrefabAssetType(newRigidBody) != PrefabAssetType.NotAPrefab
                && PrefabUtility.GetPrefabInstanceStatus(newRigidBody) != PrefabInstanceStatus.Connected)
                    // 如果是未连接的预制体实例，则清空引用
                    m_RigidBody.objectReferenceValue = null;

                // 应用修改到序列化对象
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif