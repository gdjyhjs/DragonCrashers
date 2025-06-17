using System.Globalization;
using UnityEngine;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UIToolkitDemo
{
    public enum CharacterClass
    {
        Paladin,  // 圣骑士
        Wizard,  // 巫师
        Barbarian,  // 野蛮人
        Necromancer  // 死灵法师
    }

    public enum Rarity
    {
        Common,  // 普通
        Rare,  // 稀有
        Special,  // 特殊
        All,  // 用于过滤
    }

    public enum AttackType
    {
        Melee,  // 近战
        Magic,  // 魔法
        Ranged  // 远程
    }

    // 特定角色的基础数据
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Characters/CharacterGameData",
        menuName = "UIToolkitDemo/Character", order = 1)]
    public class CharacterBaseSO : ScriptableObject
    {
        [SerializeField] string m_CharacterName;  // 角色名称

        [SerializeField] GameObject m_CharacterVisualsPrefab;  // 角色视觉预制体

        [Header("职业属性")]
        [SerializeField] CharacterClass m_CharacterClass;  // 角色职业
        [SerializeField] LocalizedString m_CharacterClassLocalized;  // 本地化的角色职业
        [SerializeField] Rarity m_Rarity;  // 稀有度
        [SerializeField] LocalizedString m_RarityLocalized;  // 本地化的稀有度
        [SerializeField] AttackType m_AttackType;  // 攻击类型
        [SerializeField] LocalizedString m_AttackTypeLocalized;  // 本地化的攻击类型

        [Header("简介")]
        [SerializeField] string m_BioTitle;  // 简介标题
        [SerializeField] LocalizedString m_BioTitleLocalized;  // 本地化的简介标题

        [TextArea] [SerializeField] string m_Bio;  // 简介

        [Header("基础属性点")]
        [SerializeField] float m_BasePointsLife;  // 基础生命点数

        [SerializeField] float m_BasePointsDefense;  // 基础防御点数

        [SerializeField] float m_BasePointsAttack;  // 基础攻击点数

        [SerializeField] float m_BasePointsAttackSpeed;  // 基础攻击速度点数

        [SerializeField] float m_BasePointsSpecialAttack;  // 基础特殊攻击点数

        [SerializeField] float m_BasePointsCriticalHit;  // 基础暴击点数

        [Header("技能")]
        [SerializeField] SkillSO m_Skill1;  // 技能 1

        [SerializeField] SkillSO m_Skill2;  // 技能 2

        [SerializeField] SkillSO m_Skill3;  // 技能 3

        [Header("默认库存装备")]
        // 起始装备（武器、盾牌/盔甲、头盔、靴子、手套）
        [SerializeField] EquipmentSO m_DefaultWeapon;  // 默认武器

        [SerializeField] EquipmentSO m_DefaultShieldAndArmor;  // 默认盾牌和盔甲

        [SerializeField] EquipmentSO m_DefaultHelmet;  // 默认头盔

        [SerializeField] EquipmentSO m_DefaultBoots;  // 默认靴子

        [SerializeField] EquipmentSO m_DefaultGloves;  // 默认手套

        // 用于数据绑定的属性

        [CreateProperty] public LocalizedString CharacterClassLocalized => m_CharacterClassLocalized;  // 本地化的角色职业

        [CreateProperty] public LocalizedString RarityLocalized => m_RarityLocalized;  // 本地化的稀有度

        [CreateProperty] public LocalizedString AttackTypeLocalized => m_AttackTypeLocalized;  // 本地化的攻击类型

        [CreateProperty] public string CharacterName => m_CharacterName;  // 角色名称

        [CreateProperty] public CharacterClass CharacterClass => m_CharacterClass;  // 角色职业

        [CreateProperty] public Rarity Rarity => m_Rarity;  // 稀有度

        [CreateProperty] public AttackType AttackType => m_AttackType;  // 攻击类型

        [CreateProperty] public string BioTitle => m_BioTitle;  // 简介标题

        [CreateProperty] public LocalizedString BioTitleLocalized => m_BioTitleLocalized;  // 本地化的简介标题

        [CreateProperty] public string Bio => m_Bio;  // 简介

        [CreateProperty] public string BasePointsLife => m_BasePointsLife.ToString();  // 基础生命点数

        [CreateProperty] public string BasePointsDefense => m_BasePointsDefense.ToString();  // 基础防御点数

        [CreateProperty] public string BasePointsAttack => m_BasePointsAttack.ToString();  // 基础攻击点数

        [CreateProperty] public string BasePointsAttackSpeed => $"{m_BasePointsAttackSpeed:F1} /s";  // 基础攻击速度点数

        [CreateProperty] public string BasePointsSpecialAttack => $"{m_BasePointsSpecialAttack:F0} /s";  // 基础特殊攻击点数

        [CreateProperty] public string BasePointsCriticalHit => m_BasePointsAttack.ToString();  // 基础暴击点数

        /// <summary>
        /// 用于计算角色的 "实力" 等级。
        /// </summary>
        public float TotalBasePoints =>
            m_BasePointsAttack + m_BasePointsDefense + m_BasePointsLife + m_BasePointsCriticalHit;  // 总基础点数

        [CreateProperty] public SkillSO Skill1 => m_Skill1;  // 技能 1
        [CreateProperty] public Sprite Skill1Icon => m_Skill1.IconSprite;  // 技能 1 图标

        [CreateProperty] public SkillSO Skill2 => m_Skill2;  // 技能 2
        [CreateProperty] public Sprite Skill2Icon => m_Skill2.IconSprite;  // 技能 2 图标

        [CreateProperty] public SkillSO Skill3 => m_Skill3;  // 技能 3
        [CreateProperty] public Sprite Skill3Icon => m_Skill3.IconSprite;  // 技能 3 图标

        [CreateProperty] public EquipmentSO DefaultWeapon => m_DefaultWeapon;  // 默认武器

        [CreateProperty] public EquipmentSO DefaultShieldAndArmor => m_DefaultShieldAndArmor;  // 默认盾牌和盔甲

        [CreateProperty] public EquipmentSO DefaultHelmet => m_DefaultHelmet;  // 默认头盔

        [CreateProperty] public EquipmentSO DefaultBoots => m_DefaultBoots;  // 默认靴子

        [CreateProperty] public EquipmentSO DefaultGloves => m_DefaultGloves;  // 默认手套

        [CreateProperty] public GameObject CharacterVisualsPrefab => m_CharacterVisualsPrefab;  // 角色视觉预制体

        void OnValidate()
        {
            // 当枚举值更改时更新本地化键

            if (m_CharacterClassLocalized == null || m_CharacterClassLocalized.IsEmpty)
            {
                m_CharacterClassLocalized = new LocalizedString { TableReference = "SettingsTable" };
            }

            if (m_RarityLocalized == null || m_RarityLocalized.IsEmpty)
            {
                m_RarityLocalized = new LocalizedString { TableReference = "SettingsTable" };
            }

            if (m_AttackTypeLocalized == null || m_AttackTypeLocalized.IsEmpty)
            {
                m_AttackTypeLocalized = new LocalizedString { TableReference = "SettingsTable" };
            }


            // 始终更新 TableEntryReference 以匹配当前枚举值
            m_CharacterClassLocalized.TableEntryReference = $"CharacterClass.{m_CharacterClass}";
            m_RarityLocalized.TableEntryReference = $"Rarity.{m_Rarity}";
            m_AttackTypeLocalized.TableEntryReference = $"AttackType.{m_AttackType}";


            if (m_DefaultWeapon != null && m_DefaultWeapon.equipmentType != EquipmentType.Weapon)
                m_DefaultWeapon = null;

            if (m_DefaultShieldAndArmor != null && m_DefaultShieldAndArmor.equipmentType != EquipmentType.Shield)
                m_DefaultShieldAndArmor = null;

            if (m_DefaultHelmet != null && m_DefaultHelmet.equipmentType != EquipmentType.Helmet)
                m_DefaultHelmet = null;

            if (m_DefaultGloves != null && m_DefaultGloves.equipmentType != EquipmentType.Gloves)
                m_DefaultGloves = null;

            if (m_DefaultBoots != null && m_DefaultBoots.equipmentType != EquipmentType.Boots)
                m_DefaultBoots = null;
        }
    }
}