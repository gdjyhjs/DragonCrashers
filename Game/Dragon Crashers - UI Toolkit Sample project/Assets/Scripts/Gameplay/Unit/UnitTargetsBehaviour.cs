using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ฺ����λ��Ŀ������߼�
public class UnitTargetsBehaviour : MonoBehaviour
{
    [Header("Ŀ��")]
    // Ŀ�굥λ�б�
    public List<UnitController> targetUnits;

    // ���Ŀ�굥λ
    public void AddTargetUnits(List<UnitController> addedUnits)
    {
        targetUnits.Clear();

        for (int i = 0; i < addedUnits.Count; i++)
        {
            targetUnits.Add(addedUnits[i]);
        }
    }

    // �Ƴ�Ŀ�굥λ
    public void RemoveTargetUnit(UnitController removedUnit)
    {
        targetUnits.Remove(removedUnit);
    }

    // ����Ŀ�굥λ
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

    // ��ȡ���Ŀ�굥λ
    public UnitController GetRandomTargetUnit()
    {
        int randomUnit = Random.Range(0, targetUnits.Count);
        return targetUnits[randomUnit];
    }

    // ��ȡ����Ŀ�굥λ
    public List<UnitController> GetAllTargetUnits()
    {
        return targetUnits;
    }
}