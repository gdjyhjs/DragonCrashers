using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 单位伤害显示行为类
public class UnitDamageDisplayBehaviour : MonoBehaviour
{
    // 伤害颜色色调设置部分
    [Header("伤害颜色色调")]
    // 伤害颜色色调，默认为红色
    public Color damageColorTint = Color.red;

    // 治疗颜色色调设置部分
    [Header("治疗颜色色调")]
    // 治疗颜色色调，默认为绿色
    public Color healColorTint = Color.green;

    // 伤害显示位置设置部分
    [Header("伤害显示位置")]
    // 伤害显示的变换位置
    public Transform damageDisplayTransform;

    // 定义伤害显示事件处理程序委托
    public delegate void DamageDisplayEventHandler(int newDamageAmount, Transform displayLocation, Color damageColor);
    // 伤害显示事件
    public event DamageDisplayEventHandler DamageDisplayEvent;

    // 显示伤害的方法
    public void DisplayDamage(int damageTaken)
    {
        if (DamageDisplayEvent != null)
        {
            // 根据伤害值为正或负设置颜色色调，负值为伤害，正值为治疗
            Color colorTint = (damageTaken < 0) ? damageColorTint : healColorTint;
            // 触发伤害显示事件
            DamageDisplayEvent(damageTaken, damageDisplayTransform, colorTint);
        }
    }
}