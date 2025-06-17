using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ������UI���������Դ��֧������ʱ���ݰ󶨡�
/// </summary>
public class HealthData : INotifyBindablePropertyChanged
{

    // ˽���ֶ�
    int m_CurrentHealth;
    int m_MaximumHealth;

    /// <summary>
    /// ���ɰ����Ը���ʱ֪ͨ��
    /// </summary>
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    /// <summary>
    /// ʵ��ĵ�ǰ����ֵ��
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
    /// �����ܵ�����ֵ��
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
    /// ��ʽ���ַ�������ʾ����ǰ����ֵ/�������ֵ����
    /// </summary>
    [CreateProperty]
    public string HealthStatText => $"{m_CurrentHealth}/{m_MaximumHealth}";

    /// <summary>
    /// ��ǰ����ֵռ�������ֵ�İٷֱȣ�0 - 100����
    /// </summary>
    [CreateProperty]
    public float HealthPercentage => m_MaximumHealth > 0 ? Mathf.Clamp((float)m_CurrentHealth / m_MaximumHealth * 100f, 0f, 100f) : 0f;

    /// <summary>
    /// ���ڵ�ǰ����ֵ�ٷֱȵĽ�������ȵ�StyleLength��
    /// </summary>
    [CreateProperty]
    public StyleLength HealthProgressStyleLength => new StyleLength(Length.Percent(HealthPercentage));


    /// <summary>
    /// �������ݰ󶨸��µ�֪ͨ��
    /// </summary>
    /// <param name="propertyName">�Ѹ��ĵ��������ơ�</param>
    void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(propertyName));
    }

}