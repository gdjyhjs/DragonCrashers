#if VFX_OUTPUTEVENT_CINEMACHINE_2_6_0_OR_NEWER
#if VFX_OUTPUTEVENT_CINEMACHINE_3_0_0_OR_NEWER
using Unity.Cinemachine;
#else
using Cinemachine;
#endif

namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph ����¼��������������������ͨ�� VFX �¼����� Cinemachine �������Ч��
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventCinemachineCameraShake : VFXOutputEventAbstractHandler
    {
        // �����ڱ༭��ģʽ��ִ��
        public override bool canExecuteInEditor => true;

        // ����ռ�����
        public enum Space
        {
            Local, // �ֲ��ռ�
            World // ����ռ�
        }

        // VFX ���� ID
        static readonly int k_Position = Shader.PropertyToID("position");
        static readonly int k_Velocity = Shader.PropertyToID("velocity");

        [Tooltip("���ڷ��ͳ���� Cinemachine ���Դ")]
        public CinemachineImpulseSource cinemachineImpulseSource;

        [Tooltip("λ�ú��ٶ�����ֵ�Ķ���ռ䣨����� VFX �ľֲ��ռ������ռ䣩")]
        public Space attributeSpace;

        /// <summary>
        /// �� Visual Effect Graph ��������¼�ʱ���ã���������������
        /// </summary>
        /// <param name="eventAttribute">�¼���������</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            if (cinemachineImpulseSource != null)
            {
                // �� VFX �¼��л�ȡλ�ú��ٶ�����
                Vector3 pos = eventAttribute.GetVector3(k_Position);
                Vector3 vel = eventAttribute.GetVector3(k_Velocity);

                // ����Ǿֲ��ռ䣬ת��Ϊ����ռ�
                if (attributeSpace == Space.Local)
                {
                    pos = transform.localToWorldMatrix.MultiplyPoint(pos);
                    vel = transform.localToWorldMatrix.MultiplyVector(vel);
                }

                // ͨ�� Cinemachine ���Դ�����������
                cinemachineImpulseSource.GenerateImpulseAt(pos, vel);
            }
        }
    }
}
#endif