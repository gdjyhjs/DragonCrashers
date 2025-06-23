using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ������ҹ������Ч֮��Ļ�Ϲ���
    /// </summary>
    public class AmbienceBlender : MonoBehaviour
    {
        // ���״̬ö��
        enum State
        {
//            ��ϵ�ҹ�䣬
//��ϵ����죬
//������
            BlendToNight,
            BlendToDay,
            Playing
        }

        // ���컷����ЧԴ
        public AudioSource DayAmbienceSource;
        // ҹ�价����ЧԴ
        public AudioSource NightAmbienceSource;

        // ��ǰ���״̬
        private State m_CurrentState;
        // ��ǰ��ϱ�����0.0-1.0��
        private float m_CurrentBlendRatio = 0.0f;

        private void Start()
        {
            // ��ʼ������Ϊ 0
            DayAmbienceSource.volume = 0.0f;
            NightAmbienceSource.volume = 0.0f;

            m_CurrentState = State.Playing;
        }

        private void Update()
        {
            // �ǲ���״̬ʱִ�л�Ϲ���
            if (m_CurrentState != State.Playing)
            {
                bool isFinished = AdvanceBlending();
                switch (m_CurrentState)
                {
                    case State.BlendToDay:
                        // �������� = ��ϱ�����ҹ������ = 1 - ��ϱ���
                        DayAmbienceSource.volume = m_CurrentBlendRatio;
                        NightAmbienceSource.volume = 1.0f - m_CurrentBlendRatio;
                        break;
                    case State.BlendToNight:
                        // ҹ������ = ��ϱ������������� = 1 - ��ϱ���
                        NightAmbienceSource.volume = m_CurrentBlendRatio;
                        DayAmbienceSource.volume = 1.0f - m_CurrentBlendRatio;
                        break;
                }

                // �����ɺ�ָ�����״̬
                if (isFinished)
                {
                    m_CurrentState = State.Playing;
                }
            }
        }

        // �ƽ���Ͻ��Ȳ������Ƿ����
        bool AdvanceBlending()
        {
            m_CurrentBlendRatio = Mathf.Clamp01(m_CurrentBlendRatio + Time.deltaTime);
            return Mathf.Approximately(m_CurrentBlendRatio, 1.0f);
        }

        // ��ʼ��ϵ����컷����Ч���ɴӱ༭���¼��������ű����ã�
        public void BlendToDay()
        {
            m_CurrentState = State.BlendToDay;
            m_CurrentBlendRatio = 0.0f;
        }

        // ��ʼ��ϵ�ҹ�价����Ч���ɴӱ༭���¼��������ű����ã�
        public void BlendToNight()
        {
            m_CurrentState = State.BlendToNight;
            m_CurrentBlendRatio = 0.0f;
        }
    }
}