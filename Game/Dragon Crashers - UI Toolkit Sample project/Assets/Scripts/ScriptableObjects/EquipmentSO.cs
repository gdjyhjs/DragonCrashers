using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIToolkitDemo
{
    // ��ʾ���װ��
    public enum EquipmentType
    {
        Weapon,  // ����
        Helmet,  // ͷ��
        Boots,  // ѥ��
        Gloves,  // ����
        Shield,  // ����
        Accessories,  // ��Ʒ
        All  // ���ڹ���
    }

    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Equipment/EquipmentGameData", menuName = "UIToolkitDemo/Equipment", order = 2)]
    public class EquipmentSO : ScriptableObject
    {
        public string equipmentName;  // װ������
        public EquipmentType equipmentType;  // װ������
        public Rarity rarity;  // ϡ�ж�
        public int points;  // ����
        public Sprite sprite;  // ����ͼ
    }

}