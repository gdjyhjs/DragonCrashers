using System.Collections;
using System.Collections.Generic;
using UIToolkitDemo;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Utilities.Inspector;
using System;

// ��λ������Ϊ��
public class UnitAbilityBehaviour : MonoBehaviour
{
    // ��ȴ��ɺ�Ķ��в���ö��
    public enum QueueActionAfterCooldown
    {
        Automatic, // �Զ�
        ManualButton // �ֶ���ť
    }

    // ���ò���
    [Header("����")]
    public UnitAbilityData data;
    // ��ȴ��ɺ�Ķ��в���
    public QueueActionAfterCooldown queueActionAfterCooldown;

    // ����ʱID����
    [Header("����ʱID")]
    // ����ID��ֻ��
    [SerializeField]
    [ReadOnly] private int ID;

    // ��ȴ��ʱ��
    public DurationTimer cooldownTimer;
    // ��ȴ���������
    private HealthBarComponent m_CoolDownMeter;

    // ״̬����
    [Header("״̬")]

    // ��ȴ�Ƿ񼤻ֻ��
    [SerializeField]
    [ReadOnly] public bool cooldownActive;

    // �����Ƿ�׼���ã�ֻ��
    [SerializeField]
    [ReadOnly] public bool abilityReady;

    // �Ƿ�ȴ���ӵ�����
    private bool waitToBeAddedToQueue;

    // ����ʱ���߲���
    [Header("����ʱ����")]
    public PlayableDirector abilityTimeline;

    // �¼�����
    [Header("�¼�")]
    // ����׼���ü�����е��¼�
    public UnityEvent<int> abilityReadyForQueue;
    // ��Ŀ��Ӧ�ü���ֵ���¼�
    public UnityEvent<int, TargetType> applyAbilityValueToTargets;
    // ����������ɵ��¼�
    public UnityEvent<int> abilitySequenceFinished;

    // �ⲿϵͳ����ί�У����磺��λ��UI��
    public delegate void AbilityCooldownChangedEventHandler(float newCooldownAmount);
    public event AbilityCooldownChangedEventHandler AbilityCooldownChangedEvent;

    // ����ԭʼ��Dragon Crashers����δʹ��
    public delegate void AbilityReadyEventHandler();
    public event AbilityReadyEventHandler AbilityReadyEvent;

    // �ŵ��ӳٳ���
    const float k_dischargeDelay = 1f;
    // ���ܳ�����ɵľ�̬�¼�
    public static Action<UnitAbilityBehaviour> AbilityCharged;
    // ���ܷŵ���ɵľ�̬�¼�
    public static Action<UnitAbilityBehaviour> AbilityDischarged;

    // ���ü���ID
    public void SetupID(int newID)
    {
        ID = newID;
    }

    // ���ü�����ȴ��ʱ��
    public void SetupAbilityCooldownTimer()
    {
        cooldownTimer = new DurationTimer(data.cooldownTime);
    }

    // ��ʼ������ȴ��ʱ���ɴ�����������������ʾ��ȴ����
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

    // ÿ֡����ʱ��鼼����ȴ״̬
    void Update()
    {
        CheckAbilityCooldown();
    }

    // ��鼼����ȴ״̬
    void CheckAbilityCooldown()
    {
        if (cooldownActive)
        {
            cooldownTimer.UpdateTimer();
            // ������ȴʱ��仯�¼�
            DelegateEventAbilityCooldownChanged();

            if (m_CoolDownMeter != null)
            {
                m_CoolDownMeter.HealthData.CurrentHealth = Mathf.RoundToInt(cooldownTimer.GetPolledTime());
            }

            if (cooldownTimer.HasElapsed())
            {
                cooldownTimer.EndTimer();
                cooldownTimer.Reset();
                // ��ȴ��ɴ���
                AbilityCooldownFinished();
                return;
            }

        }
    }

    // ������ȴ���ʱ����
    void AbilityCooldownFinished()
    {
        cooldownActive = false;
        abilityReady = true;

        // ֪ͨ֡��Ч
        AbilityCharged.Invoke(this);

        switch (queueActionAfterCooldown)
        {
            case QueueActionAfterCooldown.Automatic:
                // ��Ӽ��ܵ�����
                AddAbilityToQueue();
                break;

            case QueueActionAfterCooldown.ManualButton:
                // ԭʼ��Dragon Crashers�����˴�δʹ��
                DelegateEventAbilityReady();
                break;
        }
    }

    // ��������ӵ�����
    public void AddAbilityToQueue()
    {
        if (abilityReady)
        {
            abilityReadyForQueue.Invoke(ID);
        }
    }

    // ��ʼ��������
    public void BeginAbilitySequence()
    {
        abilityTimeline.Play();
        abilityReady = false;

    }

    // ���ܱ�Ǵ���ʱ����
    public void AbilityMarkerHappened()
    {
        int abilityValue = data.GetRandomValueInRange();
        applyAbilityValueToTargets.Invoke(abilityValue, data.targetType);
    }

    // �����������ʱ����
    public void AbilitySequenceFinished()
    {
        cooldownActive = true;
        abilitySequenceFinished.Invoke(ID);

        // �ر�֡��Ч��������ȴ������
        AbilityDischarged.Invoke(this);

        if (m_CoolDownMeter != null)
        {
            m_CoolDownMeter.HealthData.CurrentHealth = 0;
        }
    }

    // ֹͣ����
    public void StopAbility()
    {
        cooldownActive = false;
        abilityReady = false;
    }

    // ������ȴʱ��仯�¼�
    void DelegateEventAbilityCooldownChanged()
    {
        if (AbilityCooldownChangedEvent != null)
        {
            AbilityCooldownChangedEvent(cooldownTimer.GetPolledTime());
        }
    }

    // ��������׼�����¼�
    void DelegateEventAbilityReady()
    {
        if (AbilityReadyEvent != null)
        {
            AbilityReadyEvent();
        }
    }
}