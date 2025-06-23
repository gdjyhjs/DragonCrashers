using System;
using System.Collections;
using System.Collections.Generic;
using HappyHarvest;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ���������ͽ�����ҿ������Ĳ����������¼�ʹ�ô˹��������繤�߶����ڼ�ֹͣ�ƶ���
    /// �����Ӧ��ӵ�����������������ͬһ�����ϣ��Ա���ն����¼���
    /// </summary>
    public class CharacterAnimationEventHandler : MonoBehaviour
    {
        // ��ҿ���������
        private PlayerController m_Controller;

        private void Awake()
        {
            // �Ӹ������л�ȡ����������Ϊ�����λ�ڴ��ж���������Ϸ�����ϣ���������λ�ڽ�ɫ������
            m_Controller = GetComponentInParent<PlayerController>();
        }

        /// <summary>
        /// ������ҿ��ƣ�ͨ�������¼����ã�
        /// </summary>
        void LockControl()
        {
            m_Controller.ToggleControl(false);
        }

        /// <summary>
        /// ������ҿ��ƣ�ͨ�������¼����ã�
        /// </summary>
        void UnlockControl()
        {
            m_Controller.ToggleControl(true);
        }
    }
}