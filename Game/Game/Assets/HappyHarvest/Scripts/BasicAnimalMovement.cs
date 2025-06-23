using System;
using Template2DCommon;
using UnityEngine;
using Random = UnityEngine.Random;
namespace HappyHarvest
{
    /// <summary>
    /// 简单实现游戏对象在给定 2D 碰撞器边界内 "漫游" 的功能
    /// </summary>
    public class BasicAnimalMovement : MonoBehaviour
    {
        // 动物活动区域的 2D 碰撞器
        public Collider2D Area;

        // 最小 Idle 时间（秒）
        [Min(0)] public float MinIdleTime;
        // 最大 Idle 时间（秒）
        [Min(0)] public float MaxIdleTime;

        // 移动速度
        [Min(0)] public float Speed = 2.0f;

        [Header("音频设置")]
        // 动物声音片段数组
        public AudioClip[] AnimalSound;
        // 最小随机声音间隔时间（秒）
        public float MinRandomSoundTime;
        // 最大随机声音间隔时间（秒）
        public float MaxRandomSoundTime;

        // Idle 计时器
        private float m_IdleTimer;
        // 目标 Idle 时间
        private float m_CurrentIdleTarget;

        // 声音计时器
        private float m_SoundTimer;

        // 当前移动目标位置
        private Vector3 m_CurrentTarget;

        // 是否处于 Idle 状态
        private bool m_IsIdle;

        // 动画控制器
        private Animator m_Animator;
        // "Speed" 参数的哈希值（用于动画控制）
        private int SpeedHash = Animator.StringToHash("Speed");

        private void Start()
        {
            // 确保最大 Idle 时间大于最小 Idle 时间
            if (MaxIdleTime <= MinIdleTime)
                MaxIdleTime = MinIdleTime + 0.1f;

            // 获取子对象中的动画控制器
            m_Animator = GetComponentInChildren<Animator>();

            // 初始化声音计时器（随机值）
            m_SoundTimer = Random.Range(MinRandomSoundTime, MaxRandomSoundTime);

            m_IsIdle = true;
            // 选择新的 Idle 时间
            PickNewIdleTime();
        }

        private void Update()
        {
            // 减少声音计时器
            m_SoundTimer -= Time.deltaTime;
            // 当计时器归零时播放随机动物声音
            if (m_SoundTimer <= 0.0f)
            {
                SoundManager.Instance.PlaySFXAt(transform.position, AnimalSound[Random.Range(0, AnimalSound.Length)],
                true);
                m_SoundTimer = Random.Range(MinRandomSoundTime, MaxRandomSoundTime);
            }

            // 处理 Idle 状态
            if (m_IsIdle)
            {
                m_IdleTimer += Time.deltaTime;

                // 当 Idle 时间达到目标值时，选择新的移动目标
                if (m_IdleTimer >= m_CurrentIdleTarget)
                {
                    PickNewTarget();
                }
            }
            // 处理移动状态
            else
            {
                // 向目标位置移动
                transform.position = Vector3.MoveTowards(transform.position, m_CurrentTarget, Speed * Time.deltaTime);
                // 到达目标位置后进入 Idle 状态
                if (transform.position == m_CurrentTarget)
                {
                    PickNewIdleTime();
                }
            }
        }

        /// <summary>
        /// 选择新的 Idle 时间并更新动画状态
        /// </summary>
        void PickNewIdleTime()
        {
            // 将动画速度设为 0（Idle 状态）
            if (m_Animator != null)
                m_Animator.SetFloat(SpeedHash, 0.0f);

            m_IsIdle = true;
            // 生成随机 Idle 时间
            m_CurrentIdleTarget = Random.Range(MinIdleTime, MaxIdleTime);
            m_IdleTimer = 0.0f;
        }

        /// <summary>
        /// 选择新的移动目标并更新动物朝向
        /// </summary>
        void PickNewTarget()
        {
            m_IsIdle = false;
            // 生成随机方向
            var dir = Quaternion.Euler(0, 0, 360.0f * Random.Range(0.0f, 1.0f)) * Vector2.up;

            dir *= Random.Range(1.0f, 10.0f);

            var pos = (Vector2)transform.position;
            var pts = pos + (Vector2)dir;

            // 检查目标位置是否在活动区域内，否则使用最近点
            if (!Area.OverlapPoint(pts))
            {
                pts = Area.ClosestPoint(pts);
            }

            m_CurrentTarget = pts;
            var toTarget = m_CurrentTarget - transform.position;

            // 根据目标位置调整动物朝向
            bool flipped = toTarget.x < 0;
            transform.localScale = new Vector3(flipped ? -1 : 1, 1, 1);

            // 设置动画速度
            if (m_Animator != null)
                m_Animator.SetFloat(SpeedHash, Speed);
        }
    }
}