#if VFX_OUTPUTEVENT_PHYSICS 
namespace UnityEngine.VFX.Utility
{
    ///
    /// Visual Effect Graph输出事件预制体属性处理器，用于设置刚体速度 /// 当VFX触发事件时，将事件中的速度属性应用到刚体上 ///
    [RequireComponent(typeof(Rigidbody))]
    class VFXOutputEventPrefabAttributeRigidBodyVelocityHandler : VFXOutputEventPrefabAttributeAbstractHandler
    {
        // 刚体组件引用
        Rigidbody m_RigidBody;
        // 坐标空间枚举（局部或世界空间）
        public enum Space
        {
            Local, // 局部空间
            World // 世界空间
        }

        // 事件属性的坐标空间
        public Space attributeSpace;

        // VFX 属性 ID：速度
        static readonly int k_Velocity = Shader.PropertyToID("velocity");

        /// <summary>
        /// 当 VFX 触发输出事件时调用，处理刚体速度设置
        /// </summary>
        /// <param name="eventAttribute">VFX 事件属性数据</param>
        /// <param name="visualEffect">Visual Effect 组件引用</param>
        public override void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect)
        {
            // 从 VFX 事件中获取速度属性
            var velocity = eventAttribute.GetVector3(k_Velocity);

            // 如果是局部空间，转换为世界空间
            if (attributeSpace == Space.Local)
                velocity = visualEffect.transform.localToWorldMatrix.MultiplyVector(velocity);

            // 获取刚体组件
            if (TryGetComponent<Rigidbody>(out m_RigidBody))
            {
                // 唤醒刚体（如果处于休眠状态）
                m_RigidBody.WakeUp();
                // 设置刚体线速度
                m_RigidBody.linearVelocity = velocity;
            }
        }
    }
}
#endif