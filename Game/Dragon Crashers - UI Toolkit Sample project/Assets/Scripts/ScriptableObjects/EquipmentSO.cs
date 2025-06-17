using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIToolkitDemo
{
    // 表示库存装备
    public enum EquipmentType
    {
        Weapon,  // 武器
        Helmet,  // 头盔
        Boots,  // 靴子
        Gloves,  // 手套
        Shield,  // 盾牌
        Accessories,  // 饰品
        All  // 用于过滤
    }

    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Equipment/EquipmentGameData", menuName = "UIToolkitDemo/Equipment", order = 2)]
    public class EquipmentSO : ScriptableObject
    {
        public string equipmentName;  // 装备名称
        public EquipmentType equipmentType;  // 装备类型
        public Rarity rarity;  // 稀有度
        public int points;  // 点数
        public Sprite sprite;  // 精灵图
    }

}