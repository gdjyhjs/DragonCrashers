#if VFX_OUTPUTEVENT_PHYSICS
using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{

    /// <summary>
    /// Visual Effect Graph����¼����崦�����ı༭���Զ�����
    /// �����Զ���VFXOutputEventRigidBody�����Unity�༭���еļ��������
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventRigidBody))]
    class VFXOutputEventRigidBodyEditor : VFXOutputEventHandlerEditor
    { // ���л����ԣ������������
        SerializedProperty m_RigidBody;
        // ���л����ԣ����Կռ�����
        SerializedProperty m_AttributeSpace;
        // ���л����ԣ������¼�����
        SerializedProperty m_EventType;
        /// <summary>
        /// �༭����ʼ��ʱ���ã����ڻ�ȡ���л����Ե�����
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // ��ȡĿ������� rigidBody ����
            m_RigidBody = serializedObject.FindProperty(nameof(VFXOutputEventRigidBody.rigidBody));
            // ��ȡĿ������� attributeSpace ����
            m_AttributeSpace = serializedObject.FindProperty(nameof(VFXOutputEventRigidBody.attributeSpace));
            // ��ȡĿ������� eventType ����
            m_EventType = serializedObject.FindProperty(nameof(VFXOutputEventRigidBody.eventType));
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

            // ���Ƹ�����������ֶ�
            EditorGUILayout.PropertyField(m_RigidBody);
            // �������Կռ����������ֶ�
            EditorGUILayout.PropertyField(m_AttributeSpace);
            // ���Ƹ����¼����������ֶ�
            EditorGUILayout.PropertyField(m_EventType);

            // ���ݲ�ͬ���¼��������ɰ����ı�
            var helpText = string.Empty;
            switch ((VFXOutputEventRigidBody.RigidBodyEventType)(m_EventType.intValue))
            {
                default:
                case VFXOutputEventRigidBody.RigidBodyEventType.Impulse:
                    helpText = "- velocity : ������";
                    break;
                case VFXOutputEventRigidBody.RigidBodyEventType.Explosion:
                    helpText = "- velocity : ��Ϊ����С����ֵ \n - position : ��ը���� \n - size : ��ը�뾶";
                    break;
                case VFXOutputEventRigidBody.RigidBodyEventType.VelocityChange:
                    helpText = "- velocity : ��������ٶ�";
                    break;
            }
            // ���ư�����Ϣ��˵�� VFX ���Ե���;
            HelpBox("Attribute Usage", helpText);

            if (EditorGUI.EndChangeCheck())
            {
                // ��ȡ�µĸ�������
                var newRigidBody = m_RigidBody.objectReferenceValue;
                // ��֤�����Ƿ�ΪԤ����ʵ����δ���ӵ�Ԥ������Դ
                if (newRigidBody != null
                && PrefabUtility.GetPrefabAssetType(newRigidBody) != PrefabAssetType.NotAPrefab
                && PrefabUtility.GetPrefabInstanceStatus(newRigidBody) != PrefabInstanceStatus.Connected)
                    // �����δ���ӵ�Ԥ����ʵ�������������
                    m_RigidBody.objectReferenceValue = null;

                // Ӧ���޸ĵ����л�����
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif