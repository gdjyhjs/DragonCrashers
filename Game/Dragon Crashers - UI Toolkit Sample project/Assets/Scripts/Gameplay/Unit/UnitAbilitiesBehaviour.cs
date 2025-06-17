using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using Utilities.Inspector;
using UIToolkitDemo;

// ��λ������Ϊ��
public class UnitAbilitiesBehaviour : MonoBehaviour
{
    // ��λ������Ϊ����
    public UnitAbilityBehaviour[] abilities;

    // ���ܶ��У�ֻ�������ڴ洢��ִ�еļ���ID
    [SerializeField]
    [ReadOnly] private List<int> abilityQueue;

    // �Ƿ������ͷż��ܣ�ֻ��
    [SerializeField]
    [ReadOnly] private bool abilityCastAllow;

    // ��ǰ�Ƿ��м������ڼ��ֻ��
    [SerializeField]
    [ReadOnly] private bool abilityCurrentlyActive;

    // ����ʱ���ã���ʼ�����ܶ��в����ü���
    void Awake()
    {
        abilityQueue = new List<int>();
        SetupAbilities();
    }

    // ���ü��ܣ�Ϊÿ����������ID����ȴ��ʱ��
    void SetupAbilities()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].SetupID(i);
            abilities[i].SetupAbilityCooldownTimer();

        }
    }
    // ��ʼ���м��ܵ���ȴ��ʱ���ɴ�����������������ʾ��ȴ����
    public void StartAbilityCooldowns(HealthBarComponent progressBar = null)
    {
        abilityCastAllow = true;
        abilityCurrentlyActive = false;

        for (int i = 0; i < abilities.Length; i++)
        {
            // TODO: ��������Դ�������ȴ��ʱ����Ŀǰ������ʾ���һ�����ܵ���ȴ��ʱ����
            if (i == abilities.Length - 1)
            {
                abilities[i].StartAbilityCooldown(progressBar);
            }
            else
            {
                abilities[i].StartAbilityCooldown();
            }
        }
    }

    // ������ID��ӵ����ܶ��У�������Ƿ���Կ�ʼ��һ������
    public void AddAbilityToQueue(int abilityID)
    {
        abilityQueue.Add(abilityID);
        CheckForNextAbility();
    }

    // ����Ƿ�����һ�����ܿ���ִ��
    void CheckForNextAbility()
    {
        if (abilityQueue.Count > 0)
        {
            if (!abilityCurrentlyActive)
            {
                if (abilityCastAllow)
                {
                    BeginAbility();
                    abilityCurrentlyActive = true;
                }
            }
        }
    }

    // ��ʼִ�м��ܶ����еĵ�һ������
    void BeginAbility()
    {
        int currentAbility = abilityQueue[0];
        abilities[currentAbility].BeginAbilitySequence();
        abilityQueue.RemoveAt(0);

    }

    // ��������ִ�����ʱ���ã���ǵ�ǰû�м���ļ��ܲ������һ������
    public void AbilitySequenceFinished()
    {
        abilityCurrentlyActive = false;
        CheckForNextAbility();
    }

    // ֹͣ���м��ܣ���ռ��ܶ��У���ֹ�ͷż��ܣ���ֹͣÿ������
    public void StopAllAbilities()
    {
        abilityQueue.Clear();

        abilityCastAllow = false;

        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].StopAbility();
        }
    }
}