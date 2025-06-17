using System.Collections.Generic;
using UnityEngine;

// 创建一个可在Unity编辑器中创建的脚本化对象，用于存储单位信息数据
[CreateAssetMenu(fileName = "Data_Unit_", menuName = "Dragon Crashers/Unit/Info Data", order = 1)]
public class UnitInfoData : ScriptableObject
{
    [Header("显示信息")]
    public string unitName; // 单位名称
    public Sprite unitAvatar; // 单位头像

    [Header("生命值设置")]
    public int totalHealth; // 总生命值
}