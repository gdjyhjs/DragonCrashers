using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph ����¼��������ı༭���������
    /// Ϊ���� VFX ����¼����������Զ���༭���ṩͨ�ù��ܺͽ���ṹ
    /// </summary>
    public abstract class VFXOutputEventHandlerEditor : Editor
    {
        // ���л����ԣ��Ƿ��ڱ༭��ģʽ��ִ��
        protected SerializedProperty m_ExecuteInEditor;
        // ���л����ԣ�����¼�����
        protected SerializedProperty m_OutputEvent;
        // Ŀ���¼��������������
        protected VFXOutputEventAbstractHandler m_TargetHandler;

        /// <summary>
        /// �༭����ʼ��ʱ���ã����ڻ�ȡ���л����Ե�����
        /// </summary>
        protected virtual void OnEnable()
        {
            // ��ȡĿ���¼����������
            m_TargetHandler = serializedObject.targetObject as VFXOutputEventAbstractHandler;
            // ��ȡ outputEvent ����
            m_OutputEvent = serializedObject.FindProperty(nameof(VFXOutputEventAbstractHandler.outputEvent));
            // ��ȡ executeInEditor ����
            m_ExecuteInEditor = serializedObject.FindProperty(nameof(VFXOutputEventAbstractHandler.executeInEditor));
        }

        /// <summary>
        /// ��������¼���ͨ�����Խ���
        /// </summary>
        protected void DrawOutputEventProperties()
        {
            // ���������֧���ڱ༭��ģʽ��ִ�У�����ʾִ�п���
            if (m_TargetHandler.canExecuteInEditor)
                EditorGUILayout.PropertyField(m_ExecuteInEditor);
            else
                // ������ʾ��ʾ��Ϣ
                EditorGUILayout.HelpBox($"�� VFX ����¼��������޷��ڱ༭ģʽ��Ԥ���������ҪԤ������Ϊ������벥��ģʽ��", MessageType.Info);

            // ��ʾ����¼���������
            EditorGUILayout.PropertyField(m_OutputEvent);
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

            // ��������¼�ͨ������
            DrawOutputEventProperties();

            // �����⵽ GUI �仯��Ӧ���޸ĵ����л�����
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// ���ư�����Ϣ��
        /// </summary>
        /// <param name="title">����</param>
        /// <param name="body">����</param>
        public void HelpBox(string title, string body)
        {
            using (new GUILayout.VerticalScope(Styles.helpBox))
            {
                GUILayout.Label(title, Styles.helpBoxTitle);
                GUILayout.Label(body, Styles.helpBoxBody);
            }
        }

        /// <summary>
        /// �Զ�����ʽ�࣬���ڻ��ư�����Ϣ��
        /// </summary>
        static class Styles
        {
            public static GUIStyle helpBox;
            public static GUIStyle helpBoxTitle;
            public static GUIStyle helpBoxBody;

            static Styles()
            {
                // ��ʼ����������ʽ
                helpBox = new GUIStyle(EditorStyles.helpBox);
                helpBox.margin = new RectOffset(0, 0, 12, 0);

                // ��ʼ��������ʽ
                helpBoxTitle = new GUIStyle(EditorStyles.boldLabel);
                helpBoxTitle.margin = new RectOffset(0, 0, 0, 4);

                // ��ʼ��������ʽ
                helpBoxBody = new GUIStyle(EditorStyles.label);
                helpBoxBody.wordWrap = true;
                helpBoxBody.padding = new RectOffset(8, 0, 0, 0);
            }
        }
    }
}