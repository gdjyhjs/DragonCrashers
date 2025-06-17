using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 创建资产菜单，用于创建对象池数据
[CreateAssetMenu(fileName = "Data_ObjectPool_", menuName = "Utilities/Object Pool", order = 1)]
public class ObjectPoolData : ScriptableObject
{
    [Header("设置")]
    // 要池化的对象
    public GameObject objectToPool;
    // 要池化的对象数量
    public int amountToPool;
    // 对象池是否应该扩展
    public bool shouldExpand;
}