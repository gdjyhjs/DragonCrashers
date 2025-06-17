using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ฺ�������λ�˺���ʾ�¼���������ʾ������������ʾ
public class UnitsDamageDisplayListener : MonoBehaviour
{

    [Header("�˺�����Դ")]
    // ��λ�˺���ʾ��Ϊ����
    public UnitDamageDisplayBehaviour[] unitDamageDisplayBehaviours;

    [Header("��ʾ������")]
    // ������ʾ������
    public NumberDisplayManager numberDisplayManager;
    // ������Ч��ʾ������
    public HitVFXDisplayManager hitVFXDisplayManager;

    void OnEnable()
    {
        for (int i = 0; i < unitDamageDisplayBehaviours.Length; i++)
        {
            unitDamageDisplayBehaviours[i].DamageDisplayEvent += ShowDamageDisplays;
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < unitDamageDisplayBehaviours.Length; i++)
        {
            unitDamageDisplayBehaviours[i].DamageDisplayEvent -= ShowDamageDisplays;
        }
    }

    // ��ʾ�˺����ֺͻ�����Ч
    void ShowDamageDisplays(int damageAmount, Transform damageLocation, Color damageColor)
    {
        numberDisplayManager.ShowNumber(damageAmount, damageLocation, damageColor);
        hitVFXDisplayManager.ShowHitVFX(damageLocation);
    }
}