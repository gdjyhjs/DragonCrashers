using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// 目标类型
public enum TargetType
{
    RandomTarget, // 随机目标
    AllTargets // 所有目标
}

// 值修改器
public enum ValueModifier
{
    Positive, // 正值
    Negative // 负值
}

// 创建一个可在Unity编辑器中创建的脚本化对象，用于存储单位技能数据
[CreateAssetMenu(fileName = "UnitData_NAME_Ability_", menuName = "Dragon Crashers/Unit/Ability Data", order = 2)]
public class UnitAbilityData : ScriptableObject
{
    [Header("显示信息")]
    public string abilityName; // 技能名称

    [Header("值设置")]
    public ValueModifier valueModifer; // 值修改器
    public Vector2Int valueRange; // 值范围

    [Header("冷却时间")]
    public float cooldownTime; // 冷却时间

    [Header("目标类型")]
    public TargetType targetType; // 目标类型

    // 获取值范围内的随机值
    public int GetRandomValueInRange()
    {
        return Random.Range(valueRange.x, valueRange.y) * ValueModifierResult();
    }

    // 获取值修改器的结果
    private int ValueModifierResult()
    {
        int modifier = 0;

        switch (valueModifer)
        {
            case ValueModifier.Positive:
                modifier = 1;
                break;

            case ValueModifier.Negative:
                modifier = -1;
                break;
        }

        return modifier;
    }
}