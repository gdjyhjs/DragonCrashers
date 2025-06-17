using System.Collections;
using System.Collections.Generic;
using UIToolkitDemo;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Utilities.Inspector;
using System;

// 单位技能行为类
public class UnitAbilityBehaviour : MonoBehaviour
{
    // 冷却完成后的队列操作枚举
    public enum QueueActionAfterCooldown
    {
        Automatic, // 自动
        ManualButton // 手动按钮
    }

    // 设置部分
    [Header("设置")]
    public UnitAbilityData data;
    // 冷却完成后的队列操作
    public QueueActionAfterCooldown queueActionAfterCooldown;

    // 运行时ID部分
    [Header("运行时ID")]
    // 技能ID，只读
    [SerializeField]
    [ReadOnly] private int ID;

    // 冷却计时器
    public DurationTimer cooldownTimer;
    // 冷却进度条组件
    private HealthBarComponent m_CoolDownMeter;

    // 状态部分
    [Header("状态")]

    // 冷却是否激活，只读
    [SerializeField]
    [ReadOnly] public bool cooldownActive;

    // 技能是否准备好，只读
    [SerializeField]
    [ReadOnly] public bool abilityReady;

    // 是否等待添加到队列
    private bool waitToBeAddedToQueue;

    // 技能时间线部分
    [Header("技能时间线")]
    public PlayableDirector abilityTimeline;

    // 事件部分
    [Header("事件")]
    // 技能准备好加入队列的事件
    public UnityEvent<int> abilityReadyForQueue;
    // 对目标应用技能值的事件
    public UnityEvent<int, TargetType> applyAbilityValueToTargets;
    // 技能序列完成的事件
    public UnityEvent<int> abilitySequenceFinished;

    // 外部系统检测的委托（例如：单位的UI）
    public delegate void AbilityCooldownChangedEventHandler(float newCooldownAmount);
    public event AbilityCooldownChangedEventHandler AbilityCooldownChangedEvent;

    // 来自原始《Dragon Crashers》，未使用
    public delegate void AbilityReadyEventHandler();
    public event AbilityReadyEventHandler AbilityReadyEvent;

    // 放电延迟常量
    const float k_dischargeDelay = 1f;
    // 技能充能完成的静态事件
    public static Action<UnitAbilityBehaviour> AbilityCharged;
    // 技能放电完成的静态事件
    public static Action<UnitAbilityBehaviour> AbilityDischarged;

    // 设置技能ID
    public void SetupID(int newID)
    {
        ID = newID;
    }

    // 设置技能冷却计时器
    public void SetupAbilityCooldownTimer()
    {
        cooldownTimer = new DurationTimer(data.cooldownTime);
    }

    // 开始技能冷却计时，可传入进度条组件用于显示冷却进度
    public void StartAbilityCooldown(HealthBarComponent progressBar = null)
    {
        cooldownActive = true;
        abilityReady = false;
        if (progressBar != null)
        {
            m_CoolDownMeter = progressBar;
            m_CoolDownMeter.HealthData.MaximumHealth = Mathf.RoundToInt(data.cooldownTime);
        }
    }

    // 每帧更新时检查技能冷却状态
    void Update()
    {
        CheckAbilityCooldown();
    }

    // 检查技能冷却状态
    void CheckAbilityCooldown()
    {
        if (cooldownActive)
        {
            cooldownTimer.UpdateTimer();
            // 触发冷却时间变化事件
            DelegateEventAbilityCooldownChanged();

            if (m_CoolDownMeter != null)
            {
                m_CoolDownMeter.HealthData.CurrentHealth = Mathf.RoundToInt(cooldownTimer.GetPolledTime());
            }

            if (cooldownTimer.HasElapsed())
            {
                cooldownTimer.EndTimer();
                cooldownTimer.Reset();
                // 冷却完成处理
                AbilityCooldownFinished();
                return;
            }

        }
    }

    // 技能冷却完成时调用
    void AbilityCooldownFinished()
    {
        cooldownActive = false;
        abilityReady = true;

        // 通知帧特效
        AbilityCharged.Invoke(this);

        switch (queueActionAfterCooldown)
        {
            case QueueActionAfterCooldown.Automatic:
                // 添加技能到队列
                AddAbilityToQueue();
                break;

            case QueueActionAfterCooldown.ManualButton:
                // 原始《Dragon Crashers》，此处未使用
                DelegateEventAbilityReady();
                break;
        }
    }

    // 将技能添加到队列
    public void AddAbilityToQueue()
    {
        if (abilityReady)
        {
            abilityReadyForQueue.Invoke(ID);
        }
    }

    // 开始技能序列
    public void BeginAbilitySequence()
    {
        abilityTimeline.Play();
        abilityReady = false;

    }

    // 技能标记触发时调用
    public void AbilityMarkerHappened()
    {
        int abilityValue = data.GetRandomValueInRange();
        applyAbilityValueToTargets.Invoke(abilityValue, data.targetType);
    }

    // 技能序列完成时调用
    public void AbilitySequenceFinished()
    {
        cooldownActive = true;
        abilitySequenceFinished.Invoke(ID);

        // 关闭帧特效并重置冷却进度条
        AbilityDischarged.Invoke(this);

        if (m_CoolDownMeter != null)
        {
            m_CoolDownMeter.HealthData.CurrentHealth = 0;
        }
    }

    // 停止技能
    public void StopAbility()
    {
        cooldownActive = false;
        abilityReady = false;
    }

    // 触发冷却时间变化事件
    void DelegateEventAbilityCooldownChanged()
    {
        if (AbilityCooldownChangedEvent != null)
        {
            AbilityCooldownChangedEvent(cooldownTimer.GetPolledTime());
        }
    }

    // 触发技能准备好事件
    void DelegateEventAbilityReady()
    {
        if (AbilityReadyEvent != null)
        {
            AbilityReadyEvent();
        }
    }
}