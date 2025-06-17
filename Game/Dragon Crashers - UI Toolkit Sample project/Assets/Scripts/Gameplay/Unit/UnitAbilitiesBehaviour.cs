using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using Utilities.Inspector;
using UIToolkitDemo;

// 单位技能行为类
public class UnitAbilitiesBehaviour : MonoBehaviour
{
    // 单位技能行为数组
    public UnitAbilityBehaviour[] abilities;

    // 技能队列，只读，用于存储待执行的技能ID
    [SerializeField]
    [ReadOnly] private List<int> abilityQueue;

    // 是否允许释放技能，只读
    [SerializeField]
    [ReadOnly] private bool abilityCastAllow;

    // 当前是否有技能正在激活，只读
    [SerializeField]
    [ReadOnly] private bool abilityCurrentlyActive;

    // 唤醒时调用，初始化技能队列并设置技能
    void Awake()
    {
        abilityQueue = new List<int>();
        SetupAbilities();
    }

    // 设置技能，为每个技能设置ID和冷却计时器
    void SetupAbilities()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].SetupID(i);
            abilities[i].SetupAbilityCooldownTimer();

        }
    }
    // 开始所有技能的冷却计时，可传入进度条组件用于显示冷却进度
    public void StartAbilityCooldowns(HealthBarComponent progressBar = null)
    {
        abilityCastAllow = true;
        abilityCurrentlyActive = false;

        for (int i = 0; i < abilities.Length; i++)
        {
            // TODO: 更新设计以处理多个冷却计时条。目前，仅显示最后一个技能的冷却计时条。
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

    // 将技能ID添加到技能队列，并检查是否可以开始下一个技能
    public void AddAbilityToQueue(int abilityID)
    {
        abilityQueue.Add(abilityID);
        CheckForNextAbility();
    }

    // 检查是否有下一个技能可以执行
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

    // 开始执行技能队列中的第一个技能
    void BeginAbility()
    {
        int currentAbility = abilityQueue[0];
        abilities[currentAbility].BeginAbilitySequence();
        abilityQueue.RemoveAt(0);

    }

    // 技能序列执行完成时调用，标记当前没有激活的技能并检查下一个技能
    public void AbilitySequenceFinished()
    {
        abilityCurrentlyActive = false;
        CheckForNextAbility();
    }

    // 停止所有技能，清空技能队列，禁止释放技能，并停止每个技能
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