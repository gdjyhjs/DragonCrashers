using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIToolkitDemo
{
    // 药水数据类
    public class PotionData : MonoBehaviour
    {
        // 最大治疗药水数量
        [SerializeField] uint m_MaxHealingPotions = 3;

        uint m_HealingPotionCount; // 治疗药水数量

        public uint HealingPotionCount => m_HealingPotionCount; // 治疗药水数量属性

        void OnEnable()
        {
            // 监听使用一瓶药水事件
            HealDropZone.UseOnePotion += OnUseOnePotion;
        }

        void OnDisable()
        {
            // 取消监听使用一瓶药水事件
            HealDropZone.UseOnePotion -= OnUseOnePotion;
        }

        void Start()
        {
            // 从最大值开始，并通知UI
            m_HealingPotionCount = m_MaxHealingPotions;

            // 触发治疗药水更新事件，通知UI更新药水数量
            GameplayEvents.HealingPotionUpdated?.Invoke((int)m_HealingPotionCount);
        }

        // 使用药水
        void UsePotion()
        {
            m_HealingPotionCount--;

            // 通知UI更新药水数量
            GameplayEvents.HealingPotionUpdated?.Invoke((int)m_HealingPotionCount);

            // 播放药水掉落音效
            AudioManager.PlayPotionDropSound();
        }

        // 事件处理方法
        void OnUseOnePotion()
        {
            UsePotion();
        }
    }
}