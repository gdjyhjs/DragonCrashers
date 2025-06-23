using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// ���ӷ�����������������ض�ʱ����������ϵͳ��������
    /// </summary>
    public class EmitParticles : MonoBehaviour
    {
        // Ҫ���Ƶ�����ϵͳ
        [SerializeField] private ParticleSystem particles;
        // ÿ�η������������
        [SerializeField] private int particleCount = 1;

        /// <summary>
        /// ��������ϵͳ����ָ������������
        /// ��ͨ��Unity�¼�ϵͳ�������ű�����
        /// </summary>
        public void Emit()
        {
            particles.Emit(particleCount);
        }
    }
}