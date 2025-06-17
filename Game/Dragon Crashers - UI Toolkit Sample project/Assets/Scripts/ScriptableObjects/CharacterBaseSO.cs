using System.Globalization;
using UnityEngine;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UIToolkitDemo
{
    public enum CharacterClass
    {
        Paladin,  // ʥ��ʿ
        Wizard,  // ��ʦ
        Barbarian,  // Ұ����
        Necromancer  // ���鷨ʦ
    }

    public enum Rarity
    {
        Common,  // ��ͨ
        Rare,  // ϡ��
        Special,  // ����
        All,  // ���ڹ���
    }

    public enum AttackType
    {
        Melee,  // ��ս
        Magic,  // ħ��
        Ranged  // Զ��
    }

    // �ض���ɫ�Ļ�������
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Characters/CharacterGameData",
        menuName = "UIToolkitDemo/Character", order = 1)]
    public class CharacterBaseSO : ScriptableObject
    {
        [SerializeField] string m_CharacterName;  // ��ɫ����

        [SerializeField] GameObject m_CharacterVisualsPrefab;  // ��ɫ�Ӿ�Ԥ����

        [Header("ְҵ����")]
        [SerializeField] CharacterClass m_CharacterClass;  // ��ɫְҵ
        [SerializeField] LocalizedString m_CharacterClassLocalized;  // ���ػ��Ľ�ɫְҵ
        [SerializeField] Rarity m_Rarity;  // ϡ�ж�
        [SerializeField] LocalizedString m_RarityLocalized;  // ���ػ���ϡ�ж�
        [SerializeField] AttackType m_AttackType;  // ��������
        [SerializeField] LocalizedString m_AttackTypeLocalized;  // ���ػ��Ĺ�������

        [Header("���")]
        [SerializeField] string m_BioTitle;  // ������
        [SerializeField] LocalizedString m_BioTitleLocalized;  // ���ػ��ļ�����

        [TextArea] [SerializeField] string m_Bio;  // ���

        [Header("�������Ե�")]
        [SerializeField] float m_BasePointsLife;  // ������������

        [SerializeField] float m_BasePointsDefense;  // ������������

        [SerializeField] float m_BasePointsAttack;  // ������������

        [SerializeField] float m_BasePointsAttackSpeed;  // ���������ٶȵ���

        [SerializeField] float m_BasePointsSpecialAttack;  // �������⹥������

        [SerializeField] float m_BasePointsCriticalHit;  // ������������

        [Header("����")]
        [SerializeField] SkillSO m_Skill1;  // ���� 1

        [SerializeField] SkillSO m_Skill2;  // ���� 2

        [SerializeField] SkillSO m_Skill3;  // ���� 3

        [Header("Ĭ�Ͽ��װ��")]
        // ��ʼװ��������������/���ס�ͷ����ѥ�ӡ����ף�
        [SerializeField] EquipmentSO m_DefaultWeapon;  // Ĭ������

        [SerializeField] EquipmentSO m_DefaultShieldAndArmor;  // Ĭ�϶��ƺͿ���

        [SerializeField] EquipmentSO m_DefaultHelmet;  // Ĭ��ͷ��

        [SerializeField] EquipmentSO m_DefaultBoots;  // Ĭ��ѥ��

        [SerializeField] EquipmentSO m_DefaultGloves;  // Ĭ������

        // �������ݰ󶨵�����

        [CreateProperty] public LocalizedString CharacterClassLocalized => m_CharacterClassLocalized;  // ���ػ��Ľ�ɫְҵ

        [CreateProperty] public LocalizedString RarityLocalized => m_RarityLocalized;  // ���ػ���ϡ�ж�

        [CreateProperty] public LocalizedString AttackTypeLocalized => m_AttackTypeLocalized;  // ���ػ��Ĺ�������

        [CreateProperty] public string CharacterName => m_CharacterName;  // ��ɫ����

        [CreateProperty] public CharacterClass CharacterClass => m_CharacterClass;  // ��ɫְҵ

        [CreateProperty] public Rarity Rarity => m_Rarity;  // ϡ�ж�

        [CreateProperty] public AttackType AttackType => m_AttackType;  // ��������

        [CreateProperty] public string BioTitle => m_BioTitle;  // ������

        [CreateProperty] public LocalizedString BioTitleLocalized => m_BioTitleLocalized;  // ���ػ��ļ�����

        [CreateProperty] public string Bio => m_Bio;  // ���

        [CreateProperty] public string BasePointsLife => m_BasePointsLife.ToString();  // ������������

        [CreateProperty] public string BasePointsDefense => m_BasePointsDefense.ToString();  // ������������

        [CreateProperty] public string BasePointsAttack => m_BasePointsAttack.ToString();  // ������������

        [CreateProperty] public string BasePointsAttackSpeed => $"{m_BasePointsAttackSpeed:F1} /s";  // ���������ٶȵ���

        [CreateProperty] public string BasePointsSpecialAttack => $"{m_BasePointsSpecialAttack:F0} /s";  // �������⹥������

        [CreateProperty] public string BasePointsCriticalHit => m_BasePointsAttack.ToString();  // ������������

        /// <summary>
        /// ���ڼ����ɫ�� "ʵ��" �ȼ���
        /// </summary>
        public float TotalBasePoints =>
            m_BasePointsAttack + m_BasePointsDefense + m_BasePointsLife + m_BasePointsCriticalHit;  // �ܻ�������

        [CreateProperty] public SkillSO Skill1 => m_Skill1;  // ���� 1
        [CreateProperty] public Sprite Skill1Icon => m_Skill1.IconSprite;  // ���� 1 ͼ��

        [CreateProperty] public SkillSO Skill2 => m_Skill2;  // ���� 2
        [CreateProperty] public Sprite Skill2Icon => m_Skill2.IconSprite;  // ���� 2 ͼ��

        [CreateProperty] public SkillSO Skill3 => m_Skill3;  // ���� 3
        [CreateProperty] public Sprite Skill3Icon => m_Skill3.IconSprite;  // ���� 3 ͼ��

        [CreateProperty] public EquipmentSO DefaultWeapon => m_DefaultWeapon;  // Ĭ������

        [CreateProperty] public EquipmentSO DefaultShieldAndArmor => m_DefaultShieldAndArmor;  // Ĭ�϶��ƺͿ���

        [CreateProperty] public EquipmentSO DefaultHelmet => m_DefaultHelmet;  // Ĭ��ͷ��

        [CreateProperty] public EquipmentSO DefaultBoots => m_DefaultBoots;  // Ĭ��ѥ��

        [CreateProperty] public EquipmentSO DefaultGloves => m_DefaultGloves;  // Ĭ������

        [CreateProperty] public GameObject CharacterVisualsPrefab => m_CharacterVisualsPrefab;  // ��ɫ�Ӿ�Ԥ����

        void OnValidate()
        {
            // ��ö��ֵ����ʱ���±��ػ���

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


            // ʼ�ո��� TableEntryReference ��ƥ�䵱ǰö��ֵ
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