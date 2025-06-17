using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 该类负责监听单位伤害显示事件并调用显示管理器进行显示
public class UnitsDamageDisplayListener : MonoBehaviour
{

    [Header("伤害数据源")]
    // 单位伤害显示行为数组
    public UnitDamageDisplayBehaviour[] unitDamageDisplayBehaviours;

    [Header("显示管理器")]
    // 数字显示管理器
    public NumberDisplayManager numberDisplayManager;
    // 击中特效显示管理器
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

    // 显示伤害数字和击中特效
    void ShowDamageDisplays(int damageAmount, Transform damageLocation, Color damageColor)
    {
        numberDisplayManager.ShowNumber(damageAmount, damageLocation, damageColor);
        hitVFXDisplayManager.ShowHitVFX(damageLocation);
    }
}