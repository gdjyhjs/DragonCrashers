using UnityEngine;
using UnityEngine.Events;
using Utilities.Inspector;
using System;

// ���ฺ����λ������ֵ����߼�
public class UnitHealthBehaviour : MonoBehaviour
{

    [Header("�¼�")]

    // ������ֵ�����仯ʱ���������ݱ仯��ֵ
    [Tooltip("������ֵ�����仯ʱ���������ݱ仯��ֵ")]
    public UnityEvent<int> healthDifferenceEvent;

    // ������ֵ��Ϊ��ʱ����
    [Tooltip("������ֵ��Ϊ��ʱ����")]
    public UnityEvent healthIsZeroEvent;

    /// <summary>
    /// ֪ͨ��������ǰ����ֵ�ı仯��
    /// </summary>
    public Action<int> HealthChanged;

    HealthData m_HealthData;

    /// <summary>
    /// ��ȡ����˵�λ����ֵ�� HealthData ʵ����
    /// </summary>
    public HealthData HealthData => m_HealthData;

    /// <summary>
    /// ʹ���ṩ�� HealthData ��ʼ������ֵϵͳ��
    /// </summary>
    public void SetupHealth(HealthData healthData)
    {
        if (healthData == null)
        {
            Debug.LogError($"Ϊ {gameObject.name} �ṩ�� HealthData Ϊ��");
            return;
        }

        m_HealthData = healthData;
    }
    /// <summary>
    /// ��ָ���������޸ĵ�λ������ֵ��
    /// </summary>
    /// <param name="healthDifference">����ֵ�仯������������������</param>
    public void ChangeHealth(int healthDifference)
    {
        if (m_HealthData == null)
        {
            Debug.LogError($"�� {gameObject.name} ��δ���� HealthData");
            return;
        }

        int newHealth = Mathf.Clamp(
            m_HealthData.CurrentHealth + healthDifference,
            0,
            m_HealthData.MaximumHealth
        );

        m_HealthData.CurrentHealth = newHealth;
        healthDifferenceEvent.Invoke(healthDifference);

        if (newHealth <= 0)
        {
            healthIsZeroEvent.Invoke();
        }

        HealthChanged?.Invoke(newHealth);
    }

    /// <summary>
    /// ��ȡ��ǰ����ֵ��
    /// </summary>
    public int GetCurrentHealth() => m_HealthData?.CurrentHealth ?? 0;
}