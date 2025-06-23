using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph����¼�UnityEvent�������ı༭���Զ�����
    /// �����Զ���VFXOutputEventUnityEvent�����Unity�༭���еļ��������
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventUnityEvent))]
    class VFXOutputEventUnityEventEditor : VFXOutputEventHandlerEditor
    {
        // ���л����ԣ�VFX�¼�������UnityEvent
        SerializedProperty m_OnEvent;
        /// <summary>
        /// �༭����ʼ��ʱ���ã����ڻ�ȡ���л����Ե�����
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // ��ȡĿ������� onEvent ����
            m_OnEvent = serializedObject.FindProperty(nameof(VFXOutputEventUnityEvent.onEvent));
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
            // ���� UnityEvent �����ֶ�
            EditorGUILayout.PropertyField(m_OnEvent);
            // ���ư�����Ϣ��˵���ô�������ʹ�� VFX ����
            HelpBox("������;", "������¼���������ʹ�� VFX ����");

            // �����⵽ GUI �仯��Ӧ���޸ĵ����л�����
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}