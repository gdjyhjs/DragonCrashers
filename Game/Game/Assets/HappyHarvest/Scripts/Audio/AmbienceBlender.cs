using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 处理昼夜环境音效之间的混合过渡
    /// </summary>
    public class AmbienceBlender : MonoBehaviour
    {
        // 混合状态枚举
        enum State
        {
//            混合到夜间，
//混合到白天，
//播放中
            BlendToNight,
            BlendToDay,
            Playing
        }

        // 白天环境音效源
        public AudioSource DayAmbienceSource;
        // 夜间环境音效源
        public AudioSource NightAmbienceSource;

        // 当前混合状态
        private State m_CurrentState;
        // 当前混合比例（0.0-1.0）
        private float m_CurrentBlendRatio = 0.0f;

        private void Start()
        {
            // 初始化音量为 0
            DayAmbienceSource.volume = 0.0f;
            NightAmbienceSource.volume = 0.0f;

            m_CurrentState = State.Playing;
        }

        private void Update()
        {
            // 非播放状态时执行混合过渡
            if (m_CurrentState != State.Playing)
            {
                bool isFinished = AdvanceBlending();
                switch (m_CurrentState)
                {
                    case State.BlendToDay:
                        // 白天音量 = 混合比例，夜间音量 = 1 - 混合比例
                        DayAmbienceSource.volume = m_CurrentBlendRatio;
                        NightAmbienceSource.volume = 1.0f - m_CurrentBlendRatio;
                        break;
                    case State.BlendToNight:
                        // 夜间音量 = 混合比例，白天音量 = 1 - 混合比例
                        NightAmbienceSource.volume = m_CurrentBlendRatio;
                        DayAmbienceSource.volume = 1.0f - m_CurrentBlendRatio;
                        break;
                }

                // 混合完成后恢复播放状态
                if (isFinished)
                {
                    m_CurrentState = State.Playing;
                }
            }
        }

        // 推进混合进度并返回是否完成
        bool AdvanceBlending()
        {
            m_CurrentBlendRatio = Mathf.Clamp01(m_CurrentBlendRatio + Time.deltaTime);
            return Mathf.Approximately(m_CurrentBlendRatio, 1.0f);
        }

        // 开始混合到白天环境音效（可从编辑器事件或其他脚本调用）
        public void BlendToDay()
        {
            m_CurrentState = State.BlendToDay;
            m_CurrentBlendRatio = 0.0f;
        }

        // 开始混合到夜间环境音效（可从编辑器事件或其他脚本调用）
        public void BlendToNight()
        {
            m_CurrentState = State.BlendToNight;
            m_CurrentBlendRatio = 0.0f;
        }
    }
}