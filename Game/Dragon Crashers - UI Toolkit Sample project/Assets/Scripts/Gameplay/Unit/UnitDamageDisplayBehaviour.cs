using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��λ�˺���ʾ��Ϊ��
public class UnitDamageDisplayBehaviour : MonoBehaviour
{
    // �˺���ɫɫ�����ò���
    [Header("�˺���ɫɫ��")]
    // �˺���ɫɫ����Ĭ��Ϊ��ɫ
    public Color damageColorTint = Color.red;

    // ������ɫɫ�����ò���
    [Header("������ɫɫ��")]
    // ������ɫɫ����Ĭ��Ϊ��ɫ
    public Color healColorTint = Color.green;

    // �˺���ʾλ�����ò���
    [Header("�˺���ʾλ��")]
    // �˺���ʾ�ı任λ��
    public Transform damageDisplayTransform;

    // �����˺���ʾ�¼��������ί��
    public delegate void DamageDisplayEventHandler(int newDamageAmount, Transform displayLocation, Color damageColor);
    // �˺���ʾ�¼�
    public event DamageDisplayEventHandler DamageDisplayEvent;

    // ��ʾ�˺��ķ���
    public void DisplayDamage(int damageTaken)
    {
        if (DamageDisplayEvent != null)
        {
            // �����˺�ֵΪ����������ɫɫ������ֵΪ�˺�����ֵΪ����
            Color colorTint = (damageTaken < 0) ? damageColorTint : healColorTint;
            // �����˺���ʾ�¼�
            DamageDisplayEvent(damageTaken, damageDisplayTransform, colorTint);
        }
    }
}