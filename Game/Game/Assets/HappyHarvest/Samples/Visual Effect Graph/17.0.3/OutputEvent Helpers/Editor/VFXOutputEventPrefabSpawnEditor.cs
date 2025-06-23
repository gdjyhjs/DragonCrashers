using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph ����¼�Ԥ�����������ı༭���Զ�����
    /// �����Զ��� VFXOutputEventPrefabSpawn ����� Unity �༭���еļ��������
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventPrefabSpawn))]
    class VFXOutputEventPrefabSpawnEditor : VFXOutputEventHandlerEditor
    {
        // Ŀ��Ԥ�������ɴ������������
        VFXOutputEventPrefabSpawn m_PrefabSpawnHandler;

        // ���л����ԣ���ͬʱ�����Ԥ����ʵ���������
        SerializedProperty m_InstanceCount;
        // ���л����ԣ�Ҫ���ɵ�Ԥ����
        SerializedProperty m_PrefabToSpawn;
        // ���л����ԣ����ɵ�ʵ���Ƿ���Ϊ��ǰ������Ӷ���
        SerializedProperty m_ParentInstances;
        // ���л����ԣ��Ƿ�ʹ�� position ������������Ԥ�����λ��
        SerializedProperty m_UsePosition;
        // ���л����ԣ��Ƿ�ʹ�� angle ������������Ԥ�������ת
        SerializedProperty m_UseAngle;
        // ���л����ԣ��Ƿ�ʹ�� scale ������������Ԥ����ı�������
        SerializedProperty m_UseScale;
        // ���л����ԣ��Ƿ�ʹ�� lifetime ����ȷ��Ԥ���������ʱ��
        SerializedProperty m_UseLifetime;

        /// <summary>
        /// �༭����ʼ��ʱ���ã����ڻ�ȡ���л����Ե�����
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // ��ȡĿ��Ԥ�������ɴ��������
            m_PrefabSpawnHandler = serializedObject.targetObject as VFXOutputEventPrefabSpawn;

            // ��ȡ�����Ե����л�����
            m_InstanceCount = serializedObject.FindProperty("m_InstanceCount");
            m_PrefabToSpawn = serializedObject.FindProperty("m_PrefabToSpawn");
            m_ParentInstances = serializedObject.FindProperty("m_ParentInstances");
            m_UsePosition = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.usePosition));
            m_UseAngle = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.useAngle));
            m_UseScale = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.useScale));
            m_UseLifetime = serializedObject.FindProperty(nameof(VFXOutputEventPrefabSpawn.useLifetime));
        }

        /// <summary>
        /// �����Զ�����������
        /// </summary>
        public override void OnInspectorGUI()
        {
            // ��ʼ�������л�����
            serializedObject.Update();
            // ��ʼ��� GUI �仯
            EditorGUI.BeginChangeCheck();

            // ���ƻ���������¼�����
            DrawOutputEventProperties();

            // ��ʾ�༭��ģʽ�µ���ʾ��Ϣ
            if (m_ExecuteInEditor.boolValue)
                EditorGUILayout.HelpBox($"�ڱ༭����Ԥ��Ԥ��������ʱ�����ӵ�Ԥ�����ĳЩ���Դ������޷�ִ�У������ڲ���ģʽ�����С�", MessageType.Info);

            // ����Ԥ����ʵ����������
            EditorGUILayout.LabelField("Ԥ����ʵ��", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope(1))
            {
                // ����Ԥ�������������ֶ�
                EditorGUILayout.PropertyField(m_PrefabToSpawn);
                // ����ʵ�����������ֶ�
                EditorGUILayout.PropertyField(m_InstanceCount);
                // ���Ƹ��ӹ�ϵ�����ֶ�
                EditorGUILayout.PropertyField(m_ParentInstances);
            }

            // �����¼�����ʹ����������
            EditorGUILayout.LabelField("�¼�����ʹ��", EditorStyles.boldLabel);

            using (new EditorGUI.IndentLevelScope(1))
            {
                // ���Ƹ�����ʹ�ÿ����ֶ�
                EditorGUILayout.PropertyField(m_UsePosition);
                EditorGUILayout.PropertyField(m_UseAngle);
                EditorGUILayout.PropertyField(m_UseScale);
                EditorGUILayout.PropertyField(m_UseLifetime);
            }

            // ���ư�����Ϣ��˵��������ܺ�������;
            HelpBox("����", @" �Ӹ���ʵ��������Ԥ�����������Ԥ���塣����ͨ����Ԥ������ʹ�� VFXOutputEventPrefabAttributeHandler �ű��������¼����ԡ�

������;:

position : �ڸ���λ������Ԥ����
angle : �Ը����Ƕ�����Ԥ����
scale : �Ը�����������Ԥ����
lifetime : �ڸ�������ʱ�������Ԥ����
");
            if (EditorGUI.EndChangeCheck())
            {
                // ��֤Ԥ���������Ƿ�Ϊ��ǰ������Ӷ��󣨱������޲㼶�ݹ飩
                if (m_PrefabToSpawn.objectReferenceValue != null)
                {
                    var prefab = m_PrefabToSpawn.objectReferenceValue as GameObject;
                    var self = m_PrefabSpawnHandler.gameObject;
                    if (prefab.transform.IsChildOf(self.transform))
                        m_PrefabToSpawn.objectReferenceValue = null;
                }
                // Ӧ���޸ĵ����л�����
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}