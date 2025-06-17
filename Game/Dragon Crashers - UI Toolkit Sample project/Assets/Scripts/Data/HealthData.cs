using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 健康条UI组件的数据源。支持运行时数据绑定。
/// </summary>
public class HealthData : INotifyBindablePropertyChanged
{

    // 私有字段
    int m_CurrentHealth;
    int m_MaximumHealth;

    /// <summary>
    /// 当可绑定属性更改时通知。
    /// </summary>
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    /// <summary>
    /// 实体的当前生命值。
    /// </summary>
    [CreateProperty]
    public int CurrentHealth
    {
        get => m_CurrentHealth;
        set
        {
            if (m_CurrentHealth != value)
            {
                m_CurrentHealth = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HealthStatText));
                NotifyPropertyChanged(nameof(HealthPercentage));
                NotifyPropertyChanged(nameof(HealthProgressStyleLength));
            }
        }
    }

    /// <summary>
    /// 最大可能的生命值。
    /// </summary>
    [CreateProperty]
    public int MaximumHealth
    {
        get => m_MaximumHealth;
        set
        {
            if (m_MaximumHealth != value)
            {
                m_MaximumHealth = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(HealthStatText));
                NotifyPropertyChanged(nameof(HealthPercentage));
                NotifyPropertyChanged(nameof(HealthProgressStyleLength));
            }
        }
    }

    /// <summary>
    /// 格式化字符串，显示“当前生命值/最大生命值”。
    /// </summary>
    [CreateProperty]
    public string HealthStatText => $"{m_CurrentHealth}/{m_MaximumHealth}";

    /// <summary>
    /// 当前生命值占最大生命值的百分比（0 - 100）。
    /// </summary>
    [CreateProperty]
    public float HealthPercentage => m_MaximumHealth > 0 ? Mathf.Clamp((float)m_CurrentHealth / m_MaximumHealth * 100f, 0f, 100f) : 0f;

    /// <summary>
    /// 基于当前生命值百分比的健康条宽度的StyleLength。
    /// </summary>
    [CreateProperty]
    public StyleLength HealthProgressStyleLength => new StyleLength(Length.Percent(HealthPercentage));


    /// <summary>
    /// 触发数据绑定更新的通知。
    /// </summary>
    /// <param name="propertyName">已更改的属性名称。</param>
    void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
    }

}