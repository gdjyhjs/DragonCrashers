using UnityEngine;
using UnityEngine.Events;
using Utilities.Inspector;
using System;

// 该类负责处理单位的生命值相关逻辑
public class UnitHealthBehaviour : MonoBehaviour
{

    [Header("事件")]

    // 当生命值发生变化时触发，传递变化的值
    [Tooltip("当生命值发生变化时触发，传递变化的值")]
    public UnityEvent<int> healthDifferenceEvent;

    // 当生命值降为零时触发
    [Tooltip("当生命值降为零时触发")]
    public UnityEvent healthIsZeroEvent;

    /// <summary>
    /// 通知监听器当前生命值的变化。
    /// </summary>
    public Action<int> HealthChanged;

    HealthData m_HealthData;

    /// <summary>
    /// 获取管理此单位生命值的 HealthData 实例。
    /// </summary>
    public HealthData HealthData => m_HealthData;

    /// <summary>
    /// 使用提供的 HealthData 初始化生命值系统。
    /// </summary>
    public void SetupHealth(HealthData healthData)
    {
        if (healthData == null)
        {
            Debug.LogError($"为 {gameObject.name} 提供的 HealthData 为空");
            return;
        }

        m_HealthData = healthData;
    }
    /// <summary>
    /// 按指定的数量修改单位的生命值。
    /// </summary>
    /// <param name="healthDifference">生命值变化的数量（正数或负数）</param>
    public void ChangeHealth(int healthDifference)
    {
        if (m_HealthData == null)
        {
            Debug.LogError($"在 {gameObject.name} 中未设置 HealthData");
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
    /// 获取当前生命值。
    /// </summary>
    public int GetCurrentHealth() => m_HealthData?.CurrentHealth ?? 0;
}