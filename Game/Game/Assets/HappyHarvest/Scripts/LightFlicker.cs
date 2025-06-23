using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.U2D;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Rendering.Universal;
namespace HappyHarvest
{
    /// <summary>
    /// 灯光闪烁效果控制器，用于实现 2D 灯光的自然波动效果
    /// 包括位置抖动、旋转抖动和强度变化
    /// </summary>
    public class LightFlicker : MonoBehaviour
    {
        // 位置抖动幅度（控制灯光位置随机偏移的范围）
        [SerializeField] public float m_PositionJitterScale;

        // 旋转抖动幅度（控制灯光旋转角度随机偏移的范围）
        [SerializeField] public float m_RotationJitterScale;

        // 强度抖动幅度（控制灯光强度随机变化的范围）
        [SerializeField] public float m_IntensityJitterScale;

        // 效果变化的时间尺度（控制波动速度）
        [SerializeField] public float m_Timescale;

        // 初始位置（用于计算抖动偏移）
        private Vector3 m_InitialPosition;
        // 初始强度（用于计算强度变化）
        private float m_InitialIntensity;
        // 初始旋转（用于计算旋转偏移）
        private Quaternion m_InitialRotation;

        // 噪声生成的种子值（确保每个灯光的效果唯一）
        private float m_XSeed;
        private float m_YSeed;
        private float m_ZSeed;

        // 2D 灯光组件引用
        private Light2D m_Light;

        // 强度偏移量（计算后的强度偏移）
        private float m_flickerIntensityOffset = 1f;
        // 强度偏移量属性（只读）
        public float flickerIntensityOffset => m_flickerIntensityOffset;
        // 修正后的强度（初始强度 + 偏移量）
        public float modifiedIntensity => m_InitialIntensity + m_flickerIntensityOffset;

        void Start()
        {
            // 使用对象实例 ID 初始化随机数生成器，确保每个实例效果唯一
            Random.InitState(gameObject.GetInstanceID());
            m_XSeed = Random.value * 248;
            m_YSeed = Random.value * 248;
            m_ZSeed = Random.value * 248;

            // 获取 2D 灯光组件
            m_Light = GetComponent<Light2D>();
            // 保存初始状态
            m_InitialIntensity = m_Light.intensity;
            m_InitialPosition = transform.position;
            m_InitialRotation = transform.rotation;
        }

        void Update()
        {
            // 基于时间和种子值计算噪声输入
            float x = Time.time * m_Timescale + m_XSeed;
            float y = Time.time * m_Timescale + m_YSeed;
            float z = Time.time * m_Timescale + m_ZSeed;

            // 生成三维柏林噪声（范围 0-1）并转换为 - 1 到 1 的范围
            Vector3 Noise = PerlinNoise3D(new Vector3(x, y, z), 2, 1);
            Noise = Noise * 2 - Vector3.one;

            // 应用噪声到灯光的位置和旋转
            transform.SetPositionAndRotation(m_InitialPosition + Noise * m_PositionJitterScale, m_InitialRotation * Quaternion.Euler(Noise * m_RotationJitterScale));

            // 应用噪声到灯光强度
            m_flickerIntensityOffset = Noise.x * m_IntensityJitterScale;
            m_Light.intensity = modifiedIntensity;
        }

        /// <summary>
        /// 生成三维柏林噪声
        /// </summary>
        /// <param name="uv">噪声输入坐标</param>
        /// <param name="Octaves">噪声叠加层数（影响细节）</param>
        /// <param name="freq">噪声频率（影响波动幅度）</param>
        /// <returns>三维噪声向量</returns>
        private Vector3 PerlinNoise3D(Vector3 uv, int Octaves, float freq)
        {
            Vector3 output = Vector3.zero;
            for (int i = 0; i < Octaves; i++)
            {
                // 对三个维度分别生成一维噪声并叠加
                output.x += Mathf.PerlinNoise1D(uv.x * freq * (i + 1));
                output.y += Mathf.PerlinNoise1D(uv.y * freq * (i + 1));
                output.z += Mathf.PerlinNoise1D(uv.z * freq * (i + 1));
            }

            return output;
        }
    }
}