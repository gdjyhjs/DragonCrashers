#if VFX_OUTPUTEVENT_CINEMACHINE_2_6_0_OR_NEWER
using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph����¼���������������ı༭���Զ�����
    /// �����Զ���VFXOutputEventCinemachineCameraShake�����Unity�༭���еļ��������
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventCinemachineCameraShake))]
    class VFXOutputEventCinemachineCameraShakeEditor : VFXOutputEventHandlerEditor
    {
        // ���л����ԣ�Cinemachine���Դ����
        SerializedProperty m_CinemachineImpulseSource;
        // ���л����ԣ����Կռ�����
        SerializedProperty m_AttributeSpace;
        /// <summary>
        /// �༭����ʼ��ʱ���ã����ڻ�ȡ���л����Ե�����
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // ��ȡĿ������� cinemachineImpulseSource ����
            m_CinemachineImpulseSource = serializedObject.FindProperty(nameof(VFXOutputEventCinemachineCameraShake.cinemachineImpulseSource));
            // ��ȡĿ������� attributeSpace ����
            m_AttributeSpace = serializedObject.FindProperty(nameof(VFXOutputEventCinemachineCameraShake.attributeSpace));
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

            // ���� Cinemachine ���Դ�����ֶ�
            EditorGUILayout.PropertyField(m_CinemachineImpulseSource);
            // �������Կռ����������ֶ�
            EditorGUILayout.PropertyField(m_AttributeSpace);
            // ���ư�����Ϣ��˵�� VFX ���Ե���;
            HelpBox("Attribute Usage", "- position : ��������λ�� \n- velocity : ����ٶ�");

            // �����⵽ GUI �仯��Ӧ���޸ĵ����л�����
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif