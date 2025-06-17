using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Properties;

namespace UIToolkitDemo
{
    [Serializable]
    public struct CurrencyIcon
    {
        public Sprite icon;  // ͼ��
        public CurrencyType currencyType;  // ��������
    }

    [Serializable]
    public struct ShopItemTypeIcon
    {
        public Sprite icon;  // ͼ��
        public ShopItemType shopItemType;  // �̵���Ʒ����
    }

    [Serializable]
    public struct CharacterClassIcon
    {
        public Sprite icon;  // ͼ��
        public CharacterClass characterClass;  // ��ɫְҵ
    }

    [Serializable]
    public struct RarityIcon
    {
        public Sprite icon;  // ͼ��
        public Rarity rarity;  // ϡ�ж�
    }

    [Serializable]
    public struct AttackTypeIcon
    {
        public Sprite icon;  // ͼ��
        public AttackType attackType;  // ��������
    }

    // �������̵���Ʒ������ͼ�ꡢ��ɫְҵ��ϡ�жȻ򹥻�����ƥ���ͼ��
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Icons", menuName = "UIToolkitDemo/Icons", order = 10)]
    public class GameIconsSO : ScriptableObject
    {
        [Header("��ɫ")]
        public List<CharacterClassIcon> characterClassIcons;  // ��ɫְҵͼ���б�
        public List<RarityIcon> rarityIcons;  // ϡ�ж�ͼ���б�
        public List<AttackTypeIcon> attackTypeIcons;  // ��������ͼ���б�

        [Header("���")]
        public Sprite emptyGearSlotIcon;  // ��װ����ͼ��

        [Header("�̵�")]
        public List<CurrencyIcon> currencyIcons;  // ����ͼ���б�
        public List<ShopItemTypeIcon> shopItemTypeIcons;  // �̵���Ʒ����ͼ���б�

        [Header("�ʼ�")]
        public Sprite newMailIcon;  // ���ʼ�ͼ��
        public Sprite oldMailIcon;  // ���ʼ�ͼ��

        // ���� UI ���ݰ󶨵Ŀɰ�����
        [CreateProperty]
        public Sprite CharacterClassIcon { get; private set; }  // ��ɫְҵͼ��

        [CreateProperty]
        public Sprite RarityIcon { get; private set; }  // ϡ�ж�ͼ��

        [CreateProperty]
        public Sprite AttackTypeIcon { get; private set; }  // ��������ͼ��

        // ���ݵ�ǰ��ɫ���Ը���ͼ��
        public void UpdateIcons(CharacterClass charClass, Rarity rarity, AttackType attackType)
        {
            CharacterClassIcon = GetCharacterClassIcon(charClass);
            RarityIcon = GetRarityIcon(rarity);
            AttackTypeIcon = GetAttackTypeIcon(attackType);
        }

        public Sprite GetCurrencyIcon(CurrencyType currencyType)
        {
            if (currencyIcons == null || currencyIcons.Count == 0)
                return null;

            CurrencyIcon match = currencyIcons.Find(x => x.currencyType == currencyType);
            return match.icon;
        }

        public Sprite GetShopTypeIcon(ShopItemType shopItemType)
        {
            if (shopItemTypeIcons == null || shopItemTypeIcons.Count == 0)
                return null;

            ShopItemTypeIcon match = shopItemTypeIcons.Find(x => x.shopItemType == shopItemType);
            return match.icon;
        }

        public Sprite GetCharacterClassIcon(CharacterClass charClass)
        {
            if (characterClassIcons == null || characterClassIcons.Count == 0)
                return null;

            CharacterClassIcon match = characterClassIcons.Find(x => x.characterClass == charClass);
            return match.icon;
        }

        // ��ȡϡ�ж�ͼ��
        public Sprite GetRarityIcon(Rarity rarity)
        {
            if (rarityIcons == null || rarityIcons.Count == 0)
                return null;

            RarityIcon match = rarityIcons.Find(x => x.rarity == rarity);
            return match.icon;
        }

        // ��ȡ��������ͼ��
        public Sprite GetAttackTypeIcon(AttackType attackType)
        {
            if (attackTypeIcons == null || attackTypeIcons.Count == 0)
                return null;

            AttackTypeIcon match = attackTypeIcons.Find(x => x.attackType == attackType);
            return match.icon;
        }

    }
}