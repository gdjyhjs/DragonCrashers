#if VFX_OUTPUTEVENT_PHYSICS 
namespace UnityEngine.VFX.Utility
{
    ///
    /// Visual Effect Graph����¼�Ԥ�������Դ��������������ø����ٶ� /// ��VFX�����¼�ʱ�����¼��е��ٶ�����Ӧ�õ������� ///
    [RequireComponent(typeof(Rigidbody))]
    class VFXOutputEventPrefabAttributeRigidBodyVelocityHandler : VFXOutputEventPrefabAttributeAbstractHandler
    {
        // �����������
        Rigidbody m_RigidBody;
        // ����ռ�ö�٣��ֲ�������ռ䣩
        public enum Space
        {
            Local, // �ֲ��ռ�
            World // ����ռ�
        }

        // �¼����Ե�����ռ�
        public Space attributeSpace;

        // VFX ���� ID���ٶ�
        static readonly int k_Velocity = Shader.PropertyToID("velocity");

        /// <summary>
        /// �� VFX ��������¼�ʱ���ã���������ٶ�����
        /// </summary>
        /// <param name="eventAttribute">VFX �¼���������</param>
        /// <param name="visualEffect">Visual Effect �������</param>
        public override void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect)
        {
            // �� VFX �¼��л�ȡ�ٶ�����
            var velocity = eventAttribute.GetVector3(k_Velocity);

            // ����Ǿֲ��ռ䣬ת��Ϊ����ռ�
            if (attributeSpace == Space.Local)
                velocity = visualEffect.transform.localToWorldMatrix.MultiplyVector(velocity);

            // ��ȡ�������
            if (TryGetComponent<Rigidbody>(out m_RigidBody))
            {
                // ���Ѹ��壨�����������״̬��
                m_RigidBody.WakeUp();
                // ���ø������ٶ�
                m_RigidBody.linearVelocity = velocity;
            }
        }
    }
}
#endif