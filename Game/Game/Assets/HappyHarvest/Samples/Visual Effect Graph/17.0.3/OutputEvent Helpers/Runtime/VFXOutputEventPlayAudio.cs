#if VFX_OUTPUTEVENT_AUDIO
using UnityEngine.Events;
namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph ����¼���Ƶ�������������� VFX �¼�����ʱ������Ƶ
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventPlayAudio : VFXOutputEventAbstractHandler
    {
        // �����ڱ༭��ģʽ��ִ��
        public override bool canExecuteInEditor => true;

        // Ҫ������Ƶ�� AudioSource ���
        public AudioSource audioSource;

        /// <summary>
        /// �� Visual Effect Graph ��������¼�ʱ���ã�������Ƶ
        /// </summary>
        /// <param name="eventAttribute">�¼���������</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            if (audioSource != null)
                audioSource.Play();
        }
    }
}
#endif