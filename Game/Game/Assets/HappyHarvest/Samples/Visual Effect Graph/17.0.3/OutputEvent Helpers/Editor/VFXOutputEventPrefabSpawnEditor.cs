using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph 输出事件预制体生成器的编辑器自定义类
    /// 用于自定义 VFXOutputEventPrefabSpawn 组件在 Unity 编辑器中的检查器界面
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventPrefabSpawn))]
    class VFXOutputEventPrefabSpawnEditor : VFXOutputEventHandlerEditor
    {
        // 目标预制体生成处理器组件引用
        VFXOutputEventPrefabSpawn m_PrefabSpawnHandler;

        // 序列化属性：可同时激活的预制体实例最大数量
        SerializedProperty m_InstanceCount;
        // 序列化属性：要生成的预制体
        SerializedProperty m_PrefabToSpawn;
        // 序列化属性：生成的实例是否作为当前对象的子对象
        SerializedProperty m_ParentInstances;
        // 序列化属性：是否使用 position 属性设置生成预制体的位置
        SerializedProperty m_UsePosition;
        // 序列化属性：是否使用 angle 属性设置生成预制体的旋转
        SerializedProperty m_UseAngle;
        // 序列化属性：是否使用 scale 属性设置生成预制体的本地缩放
        SerializedProperty m_UseScale;
        // 序列化属性：是否使用 lifetime 属性确定预制体的生存时间
        SerializedProperty m_UseLifetime;

        /// <summary>
        /// 编辑器初始化时调用，用于获取序列化属性的引用
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // 获取目标预制体生成处理器组件
            m_PrefabSpawnHandler = serializedObject.targetObject as VFXOutputEventPrefabSpawn;

            // 获取各属性的序列化引用
            m_InstanceCount = serializedObject.FindProperty("m_InstanceCount");
            m_PrefabToSpawn = serializedObject.FindProperty("m_PrefabToSpawn");
            m_ParentInstances = serializedObject.FindProperty("m_ParentInstances");
            m_UsePosition = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.usePosition));
            m_UseAngle = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.useAngle));
            m_UseScale = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.useScale));
            m_UseLifetime = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.useLifetime));
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

            // 显示编辑器模式下的提示信息
            if (m_ExecuteInEditor.boolValue)
                EditorGUILayout.HelpBox($"在编辑器中预览预制体生成时，附加到预制体的某些属性处理器无法执行，除非在播放模式下运行。", MessageType.Info);

            // 绘制预制体实例设置区域
            EditorGUILayout.LabelField("预制体实例", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope(1))
            {
                // 绘制预制体引用属性字段
                EditorGUILayout.PropertyField(m_PrefabToSpawn);
                // 绘制实例数量属性字段
                EditorGUILayout.PropertyField(m_InstanceCount);
                // 绘制父子关系属性字段
                EditorGUILayout.PropertyField(m_ParentInstances);
            }

            // 绘制事件属性使用设置区域
            EditorGUILayout.LabelField("事件属性使用", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope(1))
            {
                // 绘制各属性使用开关字段
                EditorGUILayout.PropertyField(m_UsePosition);
                EditorGUILayout.PropertyField(m_UseAngle);
                EditorGUILayout.PropertyField(m_UseScale);
                EditorGUILayout.PropertyField(m_UseLifetime);
            }

            // 绘制帮助信息框，说明组件功能和属性用途
            HelpBox("帮助", @" 从给定实例数量的预制体池中生成预制体。可以通过在预制体中使用 VFXOutputEventPrefabAttributeHandler 脚本来捕获事件属性。

属性用途:

position : 在给定位置生成预制体
angle : 以给定角度生成预制体
scale : 以给定缩放生成预制体
lifetime : 在给定生存时间后销毁预制体
");
            if (EditorGUI.EndChangeCheck())
            {
                // 验证预制体引用是否为当前对象的子对象（避免无限层级递归）
                if (m_PrefabToSpawn.objectReferenceValue != null)
                {
                    var prefab = m_PrefabToSpawn.objectReferenceValue as GameObject;
                    var self = m_PrefabSpawnHandler.gameObject;
                    if (prefab.transform.IsChildOf(self.transform))
                        m_PrefabToSpawn.objectReferenceValue = null;
                }
                // 应用修改到序列化对象
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}