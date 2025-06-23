#if VFX_OUTPUTEVENT_CINEMACHINE_2_6_0_OR_NEWER
#if VFX_OUTPUTEVENT_CINEMACHINE_3_0_0_OR_NEWER
using Unity.Cinemachine;
#else
using Cinemachine;
#endif

namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph 输出事件相机抖动处理器，用于通过 VFX 事件触发 Cinemachine 相机抖动效果
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventCinemachineCameraShake : VFXOutputEventAbstractHandler
    {
        // 允许在编辑器模式下执行
        public override bool canExecuteInEditor => true;

        // 坐标空间类型
        public enum Space
        {
            Local, // 局部空间
            World // 世界空间
        }

        // VFX 属性 ID
        static readonly int k_Position = Shader.PropertyToID("position");
        static readonly int k_Velocity = Shader.PropertyToID("velocity");

        [Tooltip("用于发送冲击的 Cinemachine 冲击源")]
        public CinemachineImpulseSource cinemachineImpulseSource;

        [Tooltip("位置和速度属性值的定义空间（相对于 VFX 的局部空间或世界空间）")]
        public Space attributeSpace;

        /// <summary>
        /// 当 Visual Effect Graph 触发输出事件时调用，生成相机抖动冲击
        /// </summary>
        /// <param name="eventAttribute">事件属性数据</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            if (cinemachineImpulseSource != null)
            {
                // 从 VFX 事件中获取位置和速度属性
                Vector3 pos = eventAttribute.GetVector3(k_Position);
                Vector3 vel = eventAttribute.GetVector3(k_Velocity);

                // 如果是局部空间，转换为世界空间
                if (attributeSpace == Space.Local)
                {
                    pos = transform.localToWorldMatrix.MultiplyPoint(pos);
                    vel = transform.localToWorldMatrix.MultiplyVector(vel);
                }

                // 通过 Cinemachine 冲击源生成相机抖动
                cinemachineImpulseSource.GenerateImpulseAt(pos, vel);
            }
        }
    }
}
#endif