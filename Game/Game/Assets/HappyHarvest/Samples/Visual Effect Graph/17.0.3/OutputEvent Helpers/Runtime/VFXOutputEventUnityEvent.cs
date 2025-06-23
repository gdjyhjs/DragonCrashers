using UnityEngine.Events;

namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph����¼�������������ڽ�VFX�¼�ת��ΪUnityEvent
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventUnityEvent : VFXOutputEventAbstractHandler
    {
        // ָʾ�Ƿ���ڱ༭��ģʽ��ִ�У��˴����ã�
        public override bool canExecuteInEditor => false;

        // ��VFX�����¼�ʱ���õ�UnityEvent
        public UnityEvent onEvent;

        /// <summary>
        /// ��Visual Effect Graph��������¼�ʱ����
        /// </summary>
        /// <param name="eventAttribute">�¼���������</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            // ����UnityEvent
            onEvent?.Invoke();
        }
    }
}