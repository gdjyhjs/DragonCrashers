using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 粒子发射控制器，用于在特定时机触发粒子系统发射粒子
    /// </summary>
    public class EmitParticles : MonoBehaviour
    {
        // 要控制的粒子系统
        [SerializeField] private ParticleSystem particles;
        // 每次发射的粒子数量
        [SerializeField] private int particleCount = 1;

        /// <summary>
        /// 触发粒子系统发射指定数量的粒子
        /// 可通过Unity事件系统或其他脚本调用
        /// </summary>
        public void Emit()
        {
            particles.Emit(particleCount);
        }
    }
}