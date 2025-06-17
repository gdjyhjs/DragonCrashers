using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIToolkitDemo
{
    // ҩˮ������
    public class PotionData : MonoBehaviour
    {
        // �������ҩˮ����
        [SerializeField] uint m_MaxHealingPotions = 3;

        uint m_HealingPotionCount; // ����ҩˮ����

        public uint HealingPotionCount => m_HealingPotionCount; // ����ҩˮ��������

        void OnEnable()
        {
            // ����ʹ��һƿҩˮ�¼�
            HealDropZone.UseOnePotion += OnUseOnePotion;
        }

        void OnDisable()
        {
            // ȡ������ʹ��һƿҩˮ�¼�
            HealDropZone.UseOnePotion -= OnUseOnePotion;
        }

        void Start()
        {
            // �����ֵ��ʼ����֪ͨUI
            m_HealingPotionCount = m_MaxHealingPotions;

            // ��������ҩˮ�����¼���֪ͨUI����ҩˮ����
            GameplayEvents.HealingPotionUpdated?.Invoke((int)m_HealingPotionCount);
        }

        // ʹ��ҩˮ
        void UsePotion()
        {
            m_HealingPotionCount--;

            // ֪ͨUI����ҩˮ����
            GameplayEvents.HealingPotionUpdated?.Invoke((int)m_HealingPotionCount);

            // ����ҩˮ������Ч
            AudioManager.PlayPotionDropSound();
        }

        // �¼�������
        void OnUseOnePotion()
        {
            UsePotion();
        }
    }
}