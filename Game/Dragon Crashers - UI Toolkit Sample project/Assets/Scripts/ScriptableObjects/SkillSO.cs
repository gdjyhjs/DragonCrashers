using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    /// <summary>
    /// 特殊技能类别枚举（基础、中级、高级）
    /// </summary>
    public enum SkillCategory
    {
        Basic = 0,  // 基础
        Intermediate = 1,  // 中级
        Advanced = 2  // 高级
    }

    /// <summary>
    /// 表示角色特殊攻击技能的可脚本化对象。
    /// 每个角色最多可以拥有三个特殊攻击技能。
    /// 
    /// 注意：此类使用了几个预处理器处理过的字符串属性，以便在 CharStatsView 技能标签中实现消息本地化。
    /// 我们可以使用智能字符串简化大部分设置：
    /// 
    /// https://docs.unity3d.com/Packages/com.unity.localization@1.5/manual/Smart/SmartStrings.html
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Skills/SkillGameData", menuName = "UIToolkitDemo/Skill",
        order = 3)]
    public class SkillSO : ScriptableObject
    {
        // 提升到下一级所需的等级数
        const int k_LevelsPerTier = 5;

        // 后备字段
        [Tooltip("技能名称，显示在角色属性窗口中")]
        [SerializeField]
        LocalizedString m_SkillNameLocalized = new LocalizedString { TableReference = "SettingsTable" };

        [Tooltip("基础、中级和高级技能类别。")]
        [SerializeField]
        SkillCategory m_Category;

        [Tooltip("在伤害时间内造成的伤害")]
        [SerializeField] int m_DamagePoints;

        [Tooltip("时间，单位为秒")]
        [SerializeField] float m_DamageTime;

        [Tooltip("角色界面的图标")]
        [SerializeField] Sprite m_IconSprite;

        // 技能名称
        string m_LocalizedSkillName;

        // 技能类别
        LocalizedString m_CategoryLocalized = new LocalizedString { TableReference = "SettingsTable" };
        string m_LocalizedCategoryName;

        // 技能等级
        LocalizedString m_TierTextLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "Skill.TierFormat"
        };

        LocalizedString m_NextTierTextLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "Skill.NextTierLevelFormat"
        };
        string m_LocalizedTierText;
        string m_LocalizedNextTierText;
        int m_CurrentLevel;  // 存储当前等级


        // 伤害文本的本地化字符串
        LocalizedString m_DamageTextLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "Skill.DamageFormat"
        };
        string m_LocalizedDamageText;

        // 数据绑定需要属性而不是方法。使用 CreateProperty 属性
        // 为数据绑定准备属性。

        [CreateProperty] public string SkillName => m_LocalizedSkillName;
        [CreateProperty] public int DamagePoints => m_DamagePoints;
        [CreateProperty] public float DamageTime => m_DamageTime;
        [CreateProperty] public Sprite IconSprite => m_IconSprite;

        /// <summary>
        /// 获取技能类别的显示文本，将本地化字符串转换为普通字符串。
        /// </summary>
        [CreateProperty]
        public string CategoryText => m_LocalizedCategoryName;

        // 当前等级和下一级的逻辑
        int GetCurrentTier(int level) => (int)Mathf.Floor((float)level / k_LevelsPerTier) + 1;
        int GetNextTierLevel(int tier) => tier * k_LevelsPerTier;

        /// <summary>
        /// 获取格式化的本地化等级文本。
        /// 
        /// 英语："Tier 2"
        /// 法语："Rang 2"
        /// 丹麦语："Rang 2"
        /// 西班牙语："Rango 2"
        /// </summary>
        [CreateProperty]
        public string TierText
        {
            get
            {
                int tier = GetCurrentTier(m_CurrentLevel);
                return string.Format(m_LocalizedTierText, tier);
            }
        }

        /// <summary>
        /// 获取格式化的本地化文本，指示下一级解锁的时间。
        /// 
        /// 英语："Next tier at Level 5"
        /// 法语："Rang suivant au niveau 5"
        /// 丹麦语："Næste rang ved niveau 5"
        /// 西班牙语："Siguiente rango en nivel 5"
        /// </summary>
        [CreateProperty]
        public string NextTierLevelText
        {
            get
            {
                int tier = GetCurrentTier(m_CurrentLevel);
                int nextLevel = GetNextTierLevel(tier);
                return string.Format(m_LocalizedNextTierText, nextLevel);
            }
        }

        /// <summary>
        /// 获取格式化的本地化文本，描述技能的持续伤害。
        /// 
        /// 英语："Deals 100 Damage points to an enemy every 2.5 seconds"
        /// 法语："Inflige 100 points de dégâts à un ennemi toutes les 2,5 secondes"
        /// 丹麦语："Giver 100 skadepoint til en fjende hvert 2,5 sekund"
        /// 西班牙语："Inflige 100 puntos de daño a un enemigo cada 2,5 segundos"
        /// </summary>
        [CreateProperty]
        public string DamageText => string.Format(m_LocalizedDamageText, DamagePoints, DamageTime);

        /// <summary>
        /// 初始化技能并注册本地化更改事件。
        /// </summary>
        void OnEnable()
        {
            // 注册本地化更改事件
            m_CategoryLocalized.StringChanged += OnCategoryTextChanged;
            m_SkillNameLocalized.StringChanged += OnSkillNameChanged;
            m_TierTextLocalized.StringChanged += OnTierTextChanged;
            m_NextTierTextLocalized.StringChanged += OnNextTierTextChanged;
            m_DamageTextLocalized.StringChanged += OnDamageTextChanged;
        }

        /// <summary>
        /// 清理事件处理程序并取消注册。
        /// </summary>
        void OnDisable()
        {
            m_CategoryLocalized.StringChanged -= OnCategoryTextChanged;
            m_SkillNameLocalized.StringChanged -= OnSkillNameChanged;
            m_TierTextLocalized.StringChanged -= OnTierTextChanged;
            m_NextTierTextLocalized.StringChanged -= OnNextTierTextChanged;
            m_DamageTextLocalized.StringChanged -= OnDamageTextChanged;
        }

        /// <summary>
        /// 更新 SkillCategory 的本地化字符串键
        /// </summary>
        void OnValidate()
        {
            // 根据类别枚举更新本地化字符串键
            m_CategoryLocalized.TableEntryReference = $"SkillCategory.{m_Category}";
        }

        /// <summary>
        /// 缓存当前等级。
        /// </summary>
        /// <param name="level">当前角色等级。</param>
        public void UpdateLevel(int level)
        {
            m_CurrentLevel = level;
        }

        /// <summary>
        /// 当语言环境更改时更新类别字符串。
        /// </summary>
        /// <param name="localizedText"></param>
        void OnCategoryTextChanged(string localizedText)
        {
            m_LocalizedCategoryName = localizedText;
        }

        /// <summary>
        /// 当语言环境更改时更新技能名称。
        /// </summary>
        /// <param name="localizedText"></param>
        void OnSkillNameChanged(string localizedText)
        {
            m_LocalizedSkillName = localizedText;
        }

        /// <summary>
        /// 处理语言环境更改以更新等级文本。
        /// </summary>
        /// <param name="localizedText"></param>
        void OnTierTextChanged(string localizedText)
        {
            m_LocalizedTierText = localizedText;
        }

        /// <summary>
        /// 处理语言环境更改以更新下一级解锁等级文本。
        /// </summary>
        /// <param name="localizedText"></param>
        void OnNextTierTextChanged(string localizedText)
        {
            m_LocalizedNextTierText = localizedText;
        }

        /// <summary>
        /// 处理语言环境更改以更新伤害描述文本。
        /// </summary>
        void OnDamageTextChanged(string localizedText)
        {
            m_LocalizedDamageText = localizedText;
        }

    }
}