using System;
using System.Collections.Generic;
using UIToolkitDemo;
using UnityEngine;
using UnityEngine.UIElements;

// ��λ��������
public class UnitController : MonoBehaviour
{
    // �������ò���
    [Header("����")]
    // ��λ��Ϣ����
    public UnitInfoData data;

    // �������ò���
    [Header("��������")]
    // ��λ������Ϊ���
    public UnitHealthBehaviour healthBehaviour;
    // ��λ�Ƿ���ı�־
    private bool unitIsAlive;
    // ���������������
    public HealthBarController healthBarBehaviour;

    // Ŀ�����ò���
    [Header("Ŀ������")]
    // ��λĿ����Ϊ���
    public UnitTargetsBehaviour targetsBehaviour;

    // �������ò���
    [Header("��������")]
    // ��λ������Ϊ���
    public UnitAbilitiesBehaviour abilitiesBehaviour;

    // �������ò���
    [Header("��������")]
    // ��λ��ɫ������Ϊ���
    public UnitCharacterAnimationBehaviour characterAnimationBehaviour;

    // ��Ƶ���ò���
    [Header("��Ƶ����")]
    // ��λ��Ƶ��Ϊ���
    public UnitAudioBehaviour audioBehaviour;

    // UI���ò���
    [Header("UI")]
    // ��Ϸ��Ļ����
    [SerializeField] private GameScreen m_GameScreen;
    // ��ɫ���ӻ����ʲ�
    [SerializeField] private VisualTreeAsset m_CharacterVisualTree;

    // �������ò���
    [Header("����")]
    // �Ƿ��ʼ������ı�־
    public bool initializeSelf;

    // ���嵥λ�����¼��������ί��
    public delegate void UnitDiedEventHandler(UnitController unit);
    // ��λ�����¼�
    public event UnitDiedEventHandler UnitDiedEvent;

    // ���⼼�ܳ����¼�
    public static Action<UnitController> SpecialCharged;
    // ���⼼���ͷ��¼�
    public static Action<UnitController> SpecialDischarged;
    // ��λ������̬�¼�
    public static Action<UnitController> UnitDied;

    // ���⼼����Ϊ���
    private UnitAbilityBehaviour m_SpecialAbility;
    // ��ɫ��Ƭ���
    private CharacterCard m_CharacterCard;
    // ��ɫ��Ƭ���Է�����
    public CharacterCard CharacterCard => m_CharacterCard;

    // ����Ľ�������ʵ��
    private HealthData m_HealthData;

    // ��ʼ��������Ϸ��ʼʱ����
    void Start()
    {
        if (initializeSelf)
        {
            // ���õ�λΪ���״̬
            SetAlive();
            // ս����ʼ
            BattleStarted();
        }
    }

    // ����ʱ���ã�����ע���¼�
    void OnEnable()
    {
        if (healthBehaviour != null)
            // ע�ὡ��ֵ�ı��¼�
            healthBehaviour.HealthChanged += OnHealthChanged;

        // ע����Ϸ��ͣ�¼�
        GameplayEvents.GamePaused += OnGamePaused;
        // ע����Ϸ�ָ��¼�
        GameplayEvents.GameResumed += OnGameResumed;

        // ע�Ἴ�ܳ����¼�
        UnitAbilityBehaviour.AbilityCharged += OnAbilityCharged;
        // ע�Ἴ���ͷ��¼�
        UnitAbilityBehaviour.AbilityDischarged += OnAbilityDischarged;
    }

    // ����ʱ���ã�����ע���¼�
    void OnDisable()
    {
        if (healthBehaviour != null)
            // ע������ֵ�ı��¼�
            healthBehaviour.HealthChanged -= OnHealthChanged;

        // ע����Ϸ��ͣ�¼�
        GameplayEvents.GamePaused -= OnGamePaused;
        // ע����Ϸ�ָ��¼�
        GameplayEvents.GameResumed -= OnGameResumed;

        // ע�����ܳ����¼�
        UnitAbilityBehaviour.AbilityCharged -= OnAbilityCharged;
        // ע�������ͷ��¼�
        UnitAbilityBehaviour.AbilityDischarged -= OnAbilityDischarged;
    }

    // ��Ϸ��ͣ�¼�������
    void OnGamePaused(float delay)
    {
        // ���ؽ�����
        healthBarBehaviour.DisplayHealthBar(false);
    }

    // ��Ϸ�ָ��¼�������
    void OnGameResumed()
    {
        // ��ʾ������
        healthBarBehaviour.DisplayHealthBar(true);
    }

    // ����ֵ�ı��¼�������
    void OnHealthChanged(int newCurrentHealth)
    {
        if (unitIsAlive)
            // ���½�������ʾ
            healthBarBehaviour.UpdateHealth(newCurrentHealth);
    }

    // ֪ͨ��Ϸ��Ļ����֡��Ч
    void OnAbilityCharged(UnitAbilityBehaviour ability)
    {
        if (ability == m_SpecialAbility)
        {
            // �������⼼�ܳ����¼�
            SpecialCharged.Invoke(this);
        }
    }

    // ֪ͨ��Ϸ��Ļ����֡��Ч
    void OnAbilityDischarged(UnitAbilityBehaviour ability)
    {
        if (ability == m_SpecialAbility)
        {
            // �������⼼���ͷ��¼�
            SpecialDischarged.Invoke(this);
        }
    }

    // ����Ŀ�굥λ�ķ���
    public void AssignTargetUnits(List<UnitController> units)
    {
        // ���Ŀ�굥λ��Ŀ����Ϊ���
        targetsBehaviour.AddTargetUnits(units);
    }

    // �Ƴ�Ŀ�굥λ�ķ���
    public void RemoveTargetUnit(UnitController unit)
    {
        // ��Ŀ����Ϊ������Ƴ�Ŀ�굥λ
        targetsBehaviour.RemoveTargetUnit(unit);
    }

    // ���õ�λΪ���״̬�ķ���
    public void SetAlive()
    {
        // ��ʼ����������
        m_HealthData = new HealthData
        {
            CurrentHealth = data.totalHealth,
            MaximumHealth = data.totalHealth,
        };

        // ���ý�����Ϊ����Ľ�������
        healthBehaviour.SetupHealth(m_HealthData);
        // ��ʾ������
        healthBarBehaviour.DisplayHealthBar(true);
        // ���ý������ĳ�ʼֵ
        healthBarBehaviour.SetHealth(m_HealthData.CurrentHealth, m_HealthData.MaximumHealth);

        // ��ǵ�λΪ���
        unitIsAlive = true;

        if (m_GameScreen != null && m_CharacterVisualTree != null)
        {
            // ��ӽ�ɫ��Ƭ�������Ϸ����
            m_CharacterCard = gameObject.AddComponent<CharacterCard>();
            // ���ý�ɫ��Ƭ�Ŀ��ӻ����ʲ�
            m_CharacterCard.CharacterVisualTreeAsset = m_CharacterVisualTree;
            // ���ý�ɫ��Ƭ��Ӣ������
            m_CharacterCard.HeroData = data;
            // ����ɫ��Ƭ��ӵ���Ϸ��Ļ
            m_GameScreen.AddHero(m_CharacterCard);

            // ָ�����һ������Ϊ���⼼�ܣ�����UI������
            if (abilitiesBehaviour != null)
            {
                m_SpecialAbility = abilitiesBehaviour.abilities[abilitiesBehaviour.abilities.Length - 1];
                //Debug.Log("Special Ability for " + data.unitName + " is " + m_SpecialAbility.data.abilityName);
            }
        }
    }

    // ս����ʼ����
    public void BattleStarted()
    {
        if (m_GameScreen != null && m_CharacterVisualTree != null)
        {
            // ��ʼ������ȴ�������ɫ��Ƭ����ȴ��
            abilitiesBehaviour.StartAbilityCooldowns(m_CharacterCard.CooldownBar);
        }
        else
        {
            // ��ʼ������ȴ
            abilitiesBehaviour.StartAbilityCooldowns();
        }
    }

    // ս����������
    public void BattleEnded()
    {
        // ֹͣ���м���
        abilitiesBehaviour.StopAllAbilities();
    }

    // ���ܴ�������
    public void AbilityHappened(int abilityValue, TargetType unitTargetType)
    {
        // ����Ŀ�굥λ
        List<UnitController> targetUnits = targetsBehaviour.FilterTargetUnits(unitTargetType);

        if (targetUnits.Count > 0)
        {
            for (int i = 0; i < targetUnits.Count; i++)
            {
                // Ŀ�굥λ���ռ���ֵ
                targetUnits[i].ReceiveAbilityValue(abilityValue);
            }
        }
    }

    // ���ռ���ֵ�ķ���
    public void ReceiveAbilityValue(int abilityValue)
    {
        if (unitIsAlive)
        {
            // �ı佡��ֵ
            healthBehaviour.ChangeHealth(abilityValue);
            // ������ɫ�����ж���
            characterAnimationBehaviour.CharacterWasHit();

            // ���½�������ʾ
            healthBarBehaviour.UpdateHealth(GetCurrentHealth());
        }
    }

    // ��ȡ��ǰ����ֵ�ķ���
    public int GetCurrentHealth()
    {
        return healthBehaviour.GetCurrentHealth();
    }

    // ��λ��������
    public void UnitHasDied()
    {
        // ��ǵ�λΪ����
        unitIsAlive = false;

        // ���ؽ�����
        healthBarBehaviour.DisplayHealthBar(false);

        // ֹͣ���м���
        abilitiesBehaviour.StopAllAbilities();
        // ������ɫ��������
        characterAnimationBehaviour.CharacterHasDied();

        // ������λ������̬�¼�
        UnitDied?.Invoke(this);

        // ֪ͨս����Ϸ��������λ����
        DelegateEventUnitDied();
    }

    // ί�е�λ�����¼��ķ���
    void DelegateEventUnitDied()
    {
        if (UnitDiedEvent != null)
        {
            // ������λ�����¼�
            UnitDiedEvent(this);
        }
    }
}