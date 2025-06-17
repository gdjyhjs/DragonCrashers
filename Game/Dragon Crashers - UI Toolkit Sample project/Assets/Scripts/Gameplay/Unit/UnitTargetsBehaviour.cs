using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 该类负责处理单位的目标相关逻辑
public class UnitTargetsBehaviour : MonoBehaviour
{
    [Header("目标")]
    // 目标单位列表
    public List<UnitController> targetUnits;

    // 添加目标单位
    public void AddTargetUnits(List<UnitController> addedUnits)
    {
        targetUnits.Clear();

        for (int i = 0; i < addedUnits.Count; i++)
        {
            targetUnits.Add(addedUnits[i]);
        }
    }

    // 移除目标单位
    public void RemoveTargetUnit(UnitController removedUnit)
    {
        targetUnits.Remove(removedUnit);
    }

    // 过滤目标单位
    public List<UnitController> FilterTargetUnits(TargetType targetType)
    {
        List<UnitController> filteredUnits = new List<UnitController>();

        if (targetUnits.Count <= 0)
        {
            return filteredUnits;
        }

        switch (targetType)
        {
            case TargetType.RandomTarget:
                int randomUnit = Random.Range(0, targetUnits.Count);
                filteredUnits.Add(targetUnits[randomUnit]);
                break;

            case TargetType.AllTargets:
                filteredUnits = targetUnits;
                break;
        }

        return filteredUnits;
    }

    // 获取随机目标单位
    public UnitController GetRandomTargetUnit()
    {
        int randomUnit = Random.Range(0, targetUnits.Count);
        return targetUnits[randomUnit];
    }

    // 获取所有目标单位
    public List<UnitController> GetAllTargetUnits()
    {
        return targetUnits;
    }
}