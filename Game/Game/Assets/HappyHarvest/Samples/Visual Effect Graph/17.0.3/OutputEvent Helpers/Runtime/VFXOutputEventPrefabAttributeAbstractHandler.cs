namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph����¼�Ԥ�������Դ��������� 
    /// ���ڴ���VFX�¼�����ʱԤ����ʵ������������
    /// </summary>
    public abstract class VFXOutputEventPrefabAttributeAbstractHandler : MonoBehaviour
    {
        /// <summary>
        /// ��VFX��������¼�ʱ���õĳ��󷽷�
        /// ������ʵ�־�������Դ����߼�
        /// </summary>
        /// <param name="eventAttribute">VFX�¼���������</param>
        /// <param name="visualEffect">�����¼���VisualEffect���</param>
        public abstract void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect);
    }
}
