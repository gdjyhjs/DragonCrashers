#if VFX_OUTPUTEVENT_AUDIO
using UnityEngine;
using UnityEngine.VFX.Utility;
namespace UnityEditor.VFX.Utility
{

    /// <summary>
    /// Visual Effect Graph����¼���Ƶ�������ı༭���Զ�����
    /// �����Զ���VFXOutputEventPlayAudio�����Unity�༭���еļ��������
    /// </summary>
    [CustomEditor(typeof(VFXOutputEventPlayAudio))]
    class VFXOutputEventPlayAudioEditor : VFXOutputEventHandlerEditor
    {
        // ���л����ԣ�AudioSource�������
        SerializedProperty m_AudioSource;
        /// <summary>
        /// �༭����ʼ��ʱ���ã����ڻ�ȡ���л����Ե�����
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            // ��ȡĿ������� audioSource ����
            m_AudioSource = serializedObject.FindProperty(nameof(VFXOutputEventPlayAudio.audioSource));
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

            // ���� AudioSource �����ֶ�
            EditorGUILayout.PropertyField(m_AudioSource);
            // ���ư�����Ϣ��˵���ô�������ʹ�� VFX ����
            HelpBox("Attribute Usage", "������¼���������ʹ�� VFX ����");

            if (EditorGUI.EndChangeCheck())
            {
                // ��ȡ�µ� AudioSource ����
                var newAudioSource = m_AudioSource.objectReferenceValue;
                // ��֤�����Ƿ�ΪԤ����ʵ����δ���ӵ�Ԥ������Դ
                if (newAudioSource != null
                && PrefabUtility.GetPrefabAssetType(newAudioSource) != PrefabAssetType.NotAPrefab
                && PrefabUtility.GetPrefabInstanceStatus(newAudioSource) != PrefabInstanceStatus.Connected)
                    // �����δ���ӵ�Ԥ����ʵ�������������
                    m_AudioSource.objectReferenceValue = null;

                // Ӧ���޸ĵ����л�����
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif