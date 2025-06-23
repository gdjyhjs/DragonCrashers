using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.U2D;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Rendering.Universal;
namespace HappyHarvest
{
    /// <summary>
    /// �ƹ���˸Ч��������������ʵ�� 2D �ƹ����Ȼ����Ч��
    /// ����λ�ö�������ת������ǿ�ȱ仯
    /// </summary>
    public class LightFlicker : MonoBehaviour
    {
        // λ�ö������ȣ����Ƶƹ�λ�����ƫ�Ƶķ�Χ��
        [SerializeField] public float m_PositionJitterScale;

        // ��ת�������ȣ����Ƶƹ���ת�Ƕ����ƫ�Ƶķ�Χ��
        [SerializeField] public float m_RotationJitterScale;

        // ǿ�ȶ������ȣ����Ƶƹ�ǿ������仯�ķ�Χ��
        [SerializeField] public float m_IntensityJitterScale;

        // Ч���仯��ʱ��߶ȣ����Ʋ����ٶȣ�
        [SerializeField] public float m_Timescale;

        // ��ʼλ�ã����ڼ��㶶��ƫ�ƣ�
        private Vector3 m_InitialPosition;
        // ��ʼǿ�ȣ����ڼ���ǿ�ȱ仯��
        private float m_InitialIntensity;
        // ��ʼ��ת�����ڼ�����תƫ�ƣ�
        private Quaternion m_InitialRotation;

        // �������ɵ�����ֵ��ȷ��ÿ���ƹ��Ч��Ψһ��
        private float m_XSeed;
        private float m_YSeed;
        private float m_ZSeed;

        // 2D �ƹ��������
        private Light2D m_Light;

        // ǿ��ƫ������������ǿ��ƫ�ƣ�
        private float m_flickerIntensityOffset = 1f;
        // ǿ��ƫ�������ԣ�ֻ����
        public float flickerIntensityOffset => m_flickerIntensityOffset;
        // �������ǿ�ȣ���ʼǿ�� + ƫ������
        public float modifiedIntensity => m_InitialIntensity + m_flickerIntensityOffset;

        void Start()
        {
            // ʹ�ö���ʵ�� ID ��ʼ���������������ȷ��ÿ��ʵ��Ч��Ψһ
            Random.InitState(gameObject.GetInstanceID());
            m_XSeed = Random.value * 248;
            m_YSeed = Random.value * 248;
            m_ZSeed = Random.value * 248;

            // ��ȡ 2D �ƹ����
            m_Light = GetComponent<Light2D>();
            // �����ʼ״̬
            m_InitialIntensity = m_Light.intensity;
            m_InitialPosition = transform.position;
            m_InitialRotation = transform.rotation;
        }

        void Update()
        {
            // ����ʱ�������ֵ������������
            float x = Time.time * m_Timescale + m_XSeed;
            float y = Time.time * m_Timescale + m_YSeed;
            float z = Time.time * m_Timescale + m_ZSeed;

            // ������ά������������Χ 0-1����ת��Ϊ - 1 �� 1 �ķ�Χ
            Vector3 Noise = PerlinNoise3D(new Vector3(x, y, z), 2, 1);
            Noise = Noise * 2 - Vector3.one;

            // Ӧ���������ƹ��λ�ú���ת
            transform.SetPositionAndRotation(m_InitialPosition + Noise * m_PositionJitterScale, m_InitialRotation * Quaternion.Euler(Noise * m_RotationJitterScale));

            // Ӧ���������ƹ�ǿ��
            m_flickerIntensityOffset = Noise.x * m_IntensityJitterScale;
            m_Light.intensity = modifiedIntensity;
        }

        /// <summary>
        /// ������ά��������
        /// </summary>
        /// <param name="uv">������������</param>
        /// <param name="Octaves">�������Ӳ�����Ӱ��ϸ�ڣ�</param>
        /// <param name="freq">����Ƶ�ʣ�Ӱ�첨�����ȣ�</param>
        /// <returns>��ά��������</returns>
        private Vector3 PerlinNoise3D(Vector3 uv, int Octaves, float freq)
        {
            Vector3 output = Vector3.zero;
            for (int i = 0; i < Octaves; i++)
            {
                // ������ά�ȷֱ�����һά����������
                output.x += Mathf.PerlinNoise1D(uv.x * freq * (i + 1));
                output.y += Mathf.PerlinNoise1D(uv.y * freq * (i + 1));
                output.z += Mathf.PerlinNoise1D(uv.z * freq * (i + 1));
            }

            return output;
        }
    }
}