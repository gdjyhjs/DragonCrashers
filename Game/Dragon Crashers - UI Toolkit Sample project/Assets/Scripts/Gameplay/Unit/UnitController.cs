using System;
using System.Collections.Generic;
using UIToolkitDemo;
using UnityEngine;
using UnityEngine.UIElements;

// 单位控制器类
public class UnitController : MonoBehaviour
{
    // 数据设置部分
    [Header("数据")]
    // 单位信息数据
    public UnitInfoData data;

    // 健康设置部分
    [Header("健康设置")]
    // 单位健康行为组件
    public UnitHealthBehaviour healthBehaviour;
    // 单位是否存活的标志
    private bool unitIsAlive;
    // 健康条控制器组件
    public HealthBarController healthBarBehaviour;

    // 目标设置部分
    [Header("目标设置")]
    // 单位目标行为组件
    public UnitTargetsBehaviour targetsBehaviour;

    // 技能设置部分
    [Header("技能设置")]
    // 单位技能行为组件
    public UnitAbilitiesBehaviour abilitiesBehaviour;

    // 动画设置部分
    [Header("动画设置")]
    // 单位角色动画行为组件
    public UnitCharacterAnimationBehaviour characterAnimationBehaviour;

    // 音频设置部分
    [Header("音频设置")]
    // 单位音频行为组件
    public UnitAudioBehaviour audioBehaviour;

    // UI设置部分
    [Header("UI")]
    // 游戏屏幕对象
    [SerializeField] private GameScreen m_GameScreen;
    // 角色可视化树资产
    [SerializeField] private VisualTreeAsset m_CharacterVisualTree;

    // 调试设置部分
    [Header("调试")]
    // 是否初始化自身的标志
    public bool initializeSelf;

    // 定义单位死亡事件处理程序委托
    public delegate void UnitDiedEventHandler(UnitController unit);
    // 单位死亡事件
    public event UnitDiedEventHandler UnitDiedEvent;

    // 特殊技能充能事件
    public static Action<UnitController> SpecialCharged;
    // 特殊技能释放事件
    public static Action<UnitController> SpecialDischarged;
    // 单位死亡静态事件
    public static Action<UnitController> UnitDied;

    // 特殊技能行为组件
    private UnitAbilityBehaviour m_SpecialAbility;
    // 角色卡片组件
    private CharacterCard m_CharacterCard;
    // 角色卡片属性访问器
    public CharacterCard CharacterCard => m_CharacterCard;

    // 共享的健康数据实例
    private HealthData m_HealthData;

    // 开始方法，游戏开始时调用
    void Start()
    {
        if (initializeSelf)
        {
            // 设置单位为存活状态
            SetAlive();
            // 战斗开始
            BattleStarted();
        }
    }

    // 启用时调用，用于注册事件
    void OnEnable()
    {
        if (healthBehaviour != null)
            // 注册健康值改变事件
            healthBehaviour.HealthChanged += OnHealthChanged;

        // 注册游戏暂停事件
        GameplayEvents.GamePaused += OnGamePaused;
        // 注册游戏恢复事件
        GameplayEvents.GameResumed += OnGameResumed;

        // 注册技能充能事件
        UnitAbilityBehaviour.AbilityCharged += OnAbilityCharged;
        // 注册技能释放事件
        UnitAbilityBehaviour.AbilityDischarged += OnAbilityDischarged;
    }

    // 禁用时调用，用于注销事件
    void OnDisable()
    {
        if (healthBehaviour != null)
            // 注销健康值改变事件
            healthBehaviour.HealthChanged -= OnHealthChanged;

        // 注销游戏暂停事件
        GameplayEvents.GamePaused -= OnGamePaused;
        // 注销游戏恢复事件
        GameplayEvents.GameResumed -= OnGameResumed;

        // 注销技能充能事件
        UnitAbilityBehaviour.AbilityCharged -= OnAbilityCharged;
        // 注销技能释放事件
        UnitAbilityBehaviour.AbilityDischarged -= OnAbilityDischarged;
    }

    // 游戏暂停事件处理方法
    void OnGamePaused(float delay)
    {
        // 隐藏健康条
        healthBarBehaviour.DisplayHealthBar(false);
    }

    // 游戏恢复事件处理方法
    void OnGameResumed()
    {
        // 显示健康条
        healthBarBehaviour.DisplayHealthBar(true);
    }

    // 健康值改变事件处理方法
    void OnHealthChanged(int newCurrentHealth)
    {
        if (unitIsAlive)
            // 更新健康条显示
            healthBarBehaviour.UpdateHealth(newCurrentHealth);
    }

    // 通知游戏屏幕启用帧特效
    void OnAbilityCharged(UnitAbilityBehaviour ability)
    {
        if (ability == m_SpecialAbility)
        {
            // 触发特殊技能充能事件
            SpecialCharged.Invoke(this);
        }
    }

    // 通知游戏屏幕禁用帧特效
    void OnAbilityDischarged(UnitAbilityBehaviour ability)
    {
        if (ability == m_SpecialAbility)
        {
            // 触发特殊技能释放事件
            SpecialDischarged.Invoke(this);
        }
    }

    // 分配目标单位的方法
    public void AssignTargetUnits(List<UnitController> units)
    {
        // 添加目标单位到目标行为组件
        targetsBehaviour.AddTargetUnits(units);
    }

    // 移除目标单位的方法
    public void RemoveTargetUnit(UnitController unit)
    {
        // 从目标行为组件中移除目标单位
        targetsBehaviour.RemoveTargetUnit(unit);
    }

    // 设置单位为存活状态的方法
    public void SetAlive()
    {
        // 初始化健康数据
        m_HealthData = new HealthData
        {
            CurrentHealth = data.totalHealth,
            MaximumHealth = data.totalHealth,
        };

        // 设置健康行为组件的健康数据
        healthBehaviour.SetupHealth(m_HealthData);
        // 显示健康条
        healthBarBehaviour.DisplayHealthBar(true);
        // 设置健康条的初始值
        healthBarBehaviour.SetHealth(m_HealthData.CurrentHealth, m_HealthData.MaximumHealth);

        // 标记单位为存活
        unitIsAlive = true;

        if (m_GameScreen != null && m_CharacterVisualTree != null)
        {
            // 添加角色卡片组件到游戏对象
            m_CharacterCard = gameObject.AddComponent<CharacterCard>();
            // 设置角色卡片的可视化树资产
            m_CharacterCard.CharacterVisualTreeAsset = m_CharacterVisualTree;
            // 设置角色卡片的英雄数据
            m_CharacterCard.HeroData = data;
            // 将角色卡片添加到游戏屏幕
            m_GameScreen.AddHero(m_CharacterCard);

            // 指定最后一个技能为特殊技能，用于UI计数器
            if (abilitiesBehaviour != null)
            {
                m_SpecialAbility = abilitiesBehaviour.abilities[abilitiesBehaviour.abilities.Length - 1];
                //Debug.Log("Special Ability for " + data.unitName + " is " + m_SpecialAbility.data.abilityName);
            }
        }
    }

    // 战斗开始方法
    public void BattleStarted()
    {
        if (m_GameScreen != null && m_CharacterVisualTree != null)
        {
            // 开始技能冷却，传入角色卡片的冷却条
            abilitiesBehaviour.StartAbilityCooldowns(m_CharacterCard.CooldownBar);
        }
        else
        {
            // 开始技能冷却
            abilitiesBehaviour.StartAbilityCooldowns();
        }
    }

    // 战斗结束方法
    public void BattleEnded()
    {
        // 停止所有技能
        abilitiesBehaviour.StopAllAbilities();
    }

    // 技能触发方法
    public void AbilityHappened(int abilityValue, TargetType unitTargetType)
    {
        // 过滤目标单位
        List<UnitController> targetUnits = targetsBehaviour.FilterTargetUnits(unitTargetType);

        if (targetUnits.Count > 0)
        {
            for (int i = 0; i < targetUnits.Count; i++)
            {
                // 目标单位接收技能值
                targetUnits[i].ReceiveAbilityValue(abilityValue);
            }
        }
    }

    // 接收技能值的方法
    public void ReceiveAbilityValue(int abilityValue)
    {
        if (unitIsAlive)
        {
            // 改变健康值
            healthBehaviour.ChangeHealth(abilityValue);
            // 触发角色被击中动画
            characterAnimationBehaviour.CharacterWasHit();

            // 更新健康条显示
            healthBarBehaviour.UpdateHealth(GetCurrentHealth());
        }
    }

    // 获取当前健康值的方法
    public int GetCurrentHealth()
    {
        return healthBehaviour.GetCurrentHealth();
    }

    // 单位死亡方法
    public void UnitHasDied()
    {
        // 标记单位为死亡
        unitIsAlive = false;

        // 隐藏健康条
        healthBarBehaviour.DisplayHealthBar(false);

        // 停止所有技能
        abilitiesBehaviour.StopAllAbilities();
        // 触发角色死亡动画
        characterAnimationBehaviour.CharacterHasDied();

        // 触发单位死亡静态事件
        UnitDied?.Invoke(this);

        // 通知战斗游戏管理器单位死亡
        DelegateEventUnitDied();
    }

    // 委托单位死亡事件的方法
    void DelegateEventUnitDied()
    {
        if (UnitDiedEvent != null)
        {
            // 触发单位死亡事件
            UnitDiedEvent(this);
        }
    }
}