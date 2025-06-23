using System;
using Template2DCommon;
using UnityEngine;
using Random = UnityEngine.Random;
namespace HappyHarvest
{
    /// <summary>
    /// ��ʵ����Ϸ�����ڸ��� 2D ��ײ���߽��� "����" �Ĺ���
    /// </summary>
    public class BasicAnimalMovement : MonoBehaviour
    {
        // ��������� 2D ��ײ��
        public Collider2D Area;

        // ��С Idle ʱ�䣨�룩
        [Min(0)] public float MinIdleTime;
        // ��� Idle ʱ�䣨�룩
        [Min(0)] public float MaxIdleTime;

        // �ƶ��ٶ�
        [Min(0)] public float Speed = 2.0f;

        [Header("��Ƶ����")]
        // ��������Ƭ������
        public AudioClip[] AnimalSound;
        // ��С����������ʱ�䣨�룩
        public float MinRandomSoundTime;
        // �������������ʱ�䣨�룩
        public float MaxRandomSoundTime;

        // Idle ��ʱ��
        private float m_IdleTimer;
        // Ŀ�� Idle ʱ��
        private float m_CurrentIdleTarget;

        // ������ʱ��
        private float m_SoundTimer;

        // ��ǰ�ƶ�Ŀ��λ��
        private Vector3 m_CurrentTarget;

        // �Ƿ��� Idle ״̬
        private bool m_IsIdle;

        // ����������
        private Animator m_Animator;
        // "Speed" �����Ĺ�ϣֵ�����ڶ������ƣ�
        private int SpeedHash = Animator.StringToHash("Speed");

        private void Start()
        {
            // ȷ����� Idle ʱ�������С Idle ʱ��
            if (MaxIdleTime <= MinIdleTime)
                MaxIdleTime = MinIdleTime + 0.1f;

            // ��ȡ�Ӷ����еĶ���������
            m_Animator = GetComponentInChildren<Animator>();

            // ��ʼ��������ʱ�������ֵ��
            m_SoundTimer = Random.Range(MinRandomSoundTime, MaxRandomSoundTime);

            m_IsIdle = true;
            // ѡ���µ� Idle ʱ��
            PickNewIdleTime();
        }

        private void Update()
        {
            // ����������ʱ��
            m_SoundTimer -= Time.deltaTime;
            // ����ʱ������ʱ���������������
            if (m_SoundTimer <= 0.0f)
            {
                SoundManager.Instance.PlaySFXAt(transform.position, AnimalSound[Random.Range(0, AnimalSound.Length)],
                true);
                m_SoundTimer = Random.Range(MinRandomSoundTime, MaxRandomSoundTime);
            }

            // ���� Idle ״̬
            if (m_IsIdle)
            {
                m_IdleTimer += Time.deltaTime;

                // �� Idle ʱ��ﵽĿ��ֵʱ��ѡ���µ��ƶ�Ŀ��
                if (m_IdleTimer >= m_CurrentIdleTarget)
                {
                    PickNewTarget();
                }
            }
            // �����ƶ�״̬
            else
            {
                // ��Ŀ��λ���ƶ�
                transform.position = Vector3.MoveTowards(transform.position, m_CurrentTarget, Speed * Time.deltaTime);
                // ����Ŀ��λ�ú���� Idle ״̬
                if (transform.position == m_CurrentTarget)
                {
                    PickNewIdleTime();
                }
            }
        }

        /// <summary>
        /// ѡ���µ� Idle ʱ�䲢���¶���״̬
        /// </summary>
        void PickNewIdleTime()
        {
            // �������ٶ���Ϊ 0��Idle ״̬��
            if (m_Animator != null)
                m_Animator.SetFloat(SpeedHash, 0.0f);

            m_IsIdle = true;
            // ������� Idle ʱ��
            m_CurrentIdleTarget = Random.Range(MinIdleTime, MaxIdleTime);
            m_IdleTimer = 0.0f;
        }

        /// <summary>
        /// ѡ���µ��ƶ�Ŀ�겢���¶��ﳯ��
        /// </summary>
        void PickNewTarget()
        {
            m_IsIdle = false;
            // �����������
            var dir = Quaternion.Euler(0, 0, 360.0f * Random.Range(0.0f, 1.0f)) * Vector2.up;

            dir *= Random.Range(1.0f, 10.0f);

            var pos = (Vector2)transform.position;
            var pts = pos + (Vector2)dir;

            // ���Ŀ��λ���Ƿ��ڻ�����ڣ�����ʹ�������
            if (!Area.OverlapPoint(pts))
            {
                pts = Area.ClosestPoint(pts);
            }

            m_CurrentTarget = pts;
            var toTarget = m_CurrentTarget - transform.position;

            // ����Ŀ��λ�õ������ﳯ��
            bool flipped = toTarget.x < 0;
            transform.localScale = new Vector3(flipped ? -1 : 1, 1, 1);

            // ���ö����ٶ�
            if (m_Animator != null)
                m_Animator.SetFloat(SpeedHash, Speed);
        }
    }
}