#if VFX_OUTPUTEVENT_AUDIO
using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{

    /// <summary>
    /// Visual Effect Graph输出事件音频播放器的编辑器自定义类
    /// 用于自定义VFXOutputEventPlayAudio组件在Unity编辑器中的检查器界面
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventPlayAudio))]
    class VFXOutputEventPlayAudioEditor : VFXOutputEventHandlerEditor
    {
        // 序列化属性：AudioSource组件引用
        SerializedProperty m_AudioSource;
        /// <summary>
        /// 编辑器初始化时调用，用于获取序列化属性的引用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // 获取目标组件的 audioSource 属性
            m_AudioSource = serializedObject.FindProperty(nameof(VFXOutputEventPlayAudio.audioSource));
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

            // 绘制 AudioSource 属性字段
            EditorGUILayout.PropertyField(m_AudioSource);
            // 绘制帮助信息框，说明该处理器不使用 VFX 属性
            HelpBox("Attribute Usage", "此输出事件处理器不使用 VFX 属性");

            if (EditorGUI.EndChangeCheck())
            {
                // 获取新的 AudioSource 引用
                var newAudioSource = m_AudioSource.objectReferenceValue;
                // 验证引用是否为预制体实例且未连接到预制体资源
                if (newAudioSource != null
                && PrefabUtility.GetPrefabAssetType(newAudioSource) != PrefabAssetType.NotAPrefab
                && PrefabUtility.GetPrefabInstanceStatus(newAudioSource) != PrefabInstanceStatus.Connected)
                    // 如果是未连接的预制体实例，则清空引用
                    m_AudioSource.objectReferenceValue = null;

                // 应用修改到序列化对象
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif