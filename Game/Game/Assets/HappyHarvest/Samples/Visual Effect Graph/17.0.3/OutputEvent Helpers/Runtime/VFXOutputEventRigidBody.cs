#if VFX_OUTPUTEVENT_PHYSICS
using UnityEngine.Events;

namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph输出事件物理处理器，用于将VFX事件转换为刚体物理力
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventRigidBody : VFXOutputEventAbstractHandler
    {
        // 指示是否可在编辑器模式下执行（此处禁用）
        public override bool canExecuteInEditor => false;

        // 坐标空间类型
        public enum Space
        {
            Local,
            World
        }

        // VFX属性ID
        static readonly int k_Position = Shader.PropertyToID("position");
        static readonly int k_Size = Shader.PropertyToID("size");
        static readonly int k_Velocity = Shader.PropertyToID("velocity");

        // 要施加力的刚体
        [Tooltip("要施加力的刚体")]
        public Rigidbody rigidBody;

        // 属性空间类型
        [Tooltip("VFX属性值的空间类型")]
        public Space attributeSpace;

        // 刚体事件类型
        public enum RigidBodyEventType
        {
            Impulse,        // 冲量
            Explosion,      // 爆炸力
            VelocityChange  // 速度变化
        }

        [Tooltip("在事件触发时施加到刚体的瞬时力类型：\n" +
                 "- Impulse：使用Velocity属性\n" +
                 "- Explosion：在Position属性位置施加，使用Size作为半径，Velocity属性的大小作为强度\n" +
                 "- VelocityChange：使用Velocity属性直接改变速度")]
        public RigidBodyEventType eventType;

        /// <summary>
        /// 当Visual Effect Graph触发输出事件时调用
        /// </summary>
        /// <param name="eventAttribute">事件属性数据</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            if (rigidBody == null)
                return;

            // 从VFX事件中获取属性值
            var position = eventAttribute.GetVector3(k_Position);
            var size = eventAttribute.GetFloat(k_Size);
            var velocity = eventAttribute.GetVector3(k_Velocity);

            // 如果是局部空间，转换为世界空间
            if (attributeSpace == Space.Local)
            {
                position = transform.localToWorldMatrix.MultiplyPoint(position);
                velocity = transform.localToWorldMatrix.MultiplyVector(velocity);
                // 假设size与变换的X分量缩放相关且变换是均匀的
                size = transform.localToWorldMatrix.MultiplyVector(Vector3.right * size).magnitude;
            }

            // 根据事件类型施加不同的力
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