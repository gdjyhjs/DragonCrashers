#if VFX_OUTPUTEVENT_PHYSICS
using UnityEngine.Events;

namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph����¼��������������ڽ�VFX�¼�ת��Ϊ����������
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventRigidBody : VFXOutputEventAbstractHandler
    {
        // ָʾ�Ƿ���ڱ༭��ģʽ��ִ�У��˴����ã�
        public override bool canExecuteInEditor => false;

        // ����ռ�����
        public enum Space
        {
            Local,
            World
        }

        // VFX����ID
        static readonly int k_Position = Shader.PropertyToID("position");
        static readonly int k_Size = Shader.PropertyToID("size");
        static readonly int k_Velocity = Shader.PropertyToID("velocity");

        // Ҫʩ�����ĸ���
        [Tooltip("Ҫʩ�����ĸ���")]
        public Rigidbody rigidBody;

        // ���Կռ�����
        [Tooltip("VFX����ֵ�Ŀռ�����")]
        public Space attributeSpace;

        // �����¼�����
        public enum RigidBodyEventType
        {
            Impulse,        // ����
            Explosion,      // ��ը��
            VelocityChange  // �ٶȱ仯
        }

        [Tooltip("���¼�����ʱʩ�ӵ������˲ʱ�����ͣ�\n" +
                 "- Impulse��ʹ��Velocity����\n" +
                 "- Explosion����Position����λ��ʩ�ӣ�ʹ��Size��Ϊ�뾶��Velocity���ԵĴ�С��Ϊǿ��\n" +
                 "- VelocityChange��ʹ��Velocity����ֱ�Ӹı��ٶ�")]
        public RigidBodyEventType eventType;

        /// <summary>
        /// ��Visual Effect Graph��������¼�ʱ����
        /// </summary>
        /// <param name="eventAttribute">�¼���������</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            if (rigidBody == null)
                return;

            // ��VFX�¼��л�ȡ����ֵ
            var position = eventAttribute.GetVector3(k_Position);
            var size = eventAttribute.GetFloat(k_Size);
            var velocity = eventAttribute.GetVector3(k_Velocity);

            // ����Ǿֲ��ռ䣬ת��Ϊ����ռ�
            if (attributeSpace == Space.Local)
            {
                position = transform.localToWorldMatrix.MultiplyPoint(position);
                velocity = transform.localToWorldMatrix.MultiplyVector(velocity);
                // ����size��任��X������������ұ任�Ǿ��ȵ�
                size = transform.localToWorldMatrix.MultiplyVector(Vector3.right * size).magnitude;
            }

            // �����¼�����ʩ�Ӳ�ͬ����
            switch (eventType)
            {
                case RigidBodyEventType.Impulse:
                    rigidBody.AddForce(velocity, ForceMode.Impulse);
                    break;
                case RigidBodyEventType.Explosion:
                    rigidBody.AddExplosionForce(velocity.magnitude, position, size);
                    break;
                case RigidBodyEventType.VelocityChange:
                    rigidBody.AddForce(velocity, ForceMode.VelocityChange);
                    break;
                default:
                    break;
            }
        }
    }
}
#endif