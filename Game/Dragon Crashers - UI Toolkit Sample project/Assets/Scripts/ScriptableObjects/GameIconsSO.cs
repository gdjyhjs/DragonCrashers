using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Properties;

namespace UIToolkitDemo
{
    [Serializable]
    public struct CurrencyIcon
    {
        public Sprite icon;  // 图标
        public CurrencyType currencyType;  // 货币类型
    }

    [Serializable]
    public struct ShopItemTypeIcon
    {
        public Sprite icon;  // 图标
        public ShopItemType shopItemType;  // 商店物品类型
    }

    [Serializable]
    public struct CharacterClassIcon
    {
        public Sprite icon;  // 图标
        public CharacterClass characterClass;  // 角色职业
    }

    [Serializable]
    public struct RarityIcon
    {
        public Sprite icon;  // 图标
        public Rarity rarity;  // 稀有度
    }

    [Serializable]
    public struct AttackTypeIcon
    {
        public Sprite icon;  // 图标
        public AttackType attackType;  // 攻击类型
    }

    // 返回与商店物品、货币图标、角色职业、稀有度或攻击类型匹配的图标
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Icons", menuName = "UIToolkitDemo/Icons", order = 10)]
    public class GameIconsSO : ScriptableObject
    {
        [Header("角色")]
        public List<CharacterClassIcon> characterClassIcons;  // 角色职业图标列表
        public List<RarityIcon> rarityIcons;  // 稀有度图标列表
        public List<AttackTypeIcon> attackTypeIcons;  // 攻击类型图标列表

        [Header("库存")]
        public Sprite emptyGearSlotIcon;  // 空装备槽图标

        [Header("商店")]
        public List<CurrencyIcon> currencyIcons;  // 货币图标列表
        public List<ShopItemTypeIcon> shopItemTypeIcons;  // 商店物品类型图标列表

        [Header("邮件")]
        public Sprite newMailIcon;  // 新邮件图标
        public Sprite oldMailIcon;  // 旧邮件图标

        // 用于 UI 数据绑定的可绑定属性
        [CreateProperty]
        public Sprite CharacterClassIcon { get; private set; }  // 角色职业图标

        [CreateProperty]
        public Sprite RarityIcon { get; private set; }  // 稀有度图标

        [CreateProperty]
        public Sprite AttackTypeIcon { get; private set; }  // 攻击类型图标

        // 根据当前角色属性更新图标
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

        // 获取稀有度图标
        public Sprite GetRarityIcon(Rarity rarity)
        {
            if (rarityIcons == null || rarityIcons.Count == 0)
                return null;

            RarityIcon match = rarityIcons.Find(x => x.rarity == rarity);
            return match.icon;
        }

        // 获取攻击类型图标
        public Sprite GetAttackTypeIcon(AttackType attackType)
        {
            if (attackTypeIcons == null || attackTypeIcons.Count == 0)
                return null;

            AttackTypeIcon match = attackTypeIcons.Find(x => x.attackType == attackType);
            return match.icon;
        }

    }
}