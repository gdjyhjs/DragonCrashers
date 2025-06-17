using UnityEngine;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// 存储消耗品数据 / 玩家设置。
    /// </summary>
    [System.Serializable]
    public class GameData
    {
        [SerializeField] uint m_Gold = 500;
        [SerializeField] uint m_Gems = 50;
        [SerializeField] uint m_HealthPotions = 6;
        [SerializeField] uint m_LevelUpPotions = 80;

        [SerializeField] string m_UserName;

        [SerializeField] bool m_IsToggled;
        [SerializeField] string m_Theme;

        [SerializeField] string m_LanguageSelection;
        [SerializeField] float m_MusicVolume;
        [SerializeField] float m_SfxVolume;

        [SerializeField] bool m_IsFpsCounterEnabled;
        [SerializeField] int m_TargetFrameRateSelection;

        // 具有简单数据绑定的属性
        [CreateProperty]
        public uint Gold
        {
            get => m_Gold;
            set => m_Gold = value;
        }

        [CreateProperty]
        public uint Gems
        {
            get => m_Gems;
            set => m_Gems = value;
        }

        [CreateProperty]
        public uint HealthPotions
        {
            get => m_HealthPotions;
            set => m_HealthPotions = value;
        }

        [CreateProperty]
        public uint LevelUpPotions
        {
            get => m_LevelUpPotions;
            set => m_LevelUpPotions = value;
        }

        [CreateProperty]
        public string UserName
        {
            get => m_UserName;
            set => m_UserName = value;
        }

        [CreateProperty]
        public bool IsToggled
        {
            get => m_IsToggled;
            set => m_IsToggled = value;
        }

        [CreateProperty]
        public string Theme
        {
            get => m_Theme;
            set => m_Theme = value;
        }

        [CreateProperty]
        public string LanguageSelection
        {
            get => m_LanguageSelection;
            set => m_LanguageSelection = value;
        }

        [CreateProperty]
        public float MusicVolume
        {
            get => m_MusicVolume;
            set => m_MusicVolume = value;
        }

        [CreateProperty]
        public float SfxVolume
        {
            get => m_SfxVolume;
            set => m_SfxVolume = value;
        }

        [CreateProperty]
        public bool IsFpsCounterEnabled
        {
            get => m_IsFpsCounterEnabled;
            set => m_IsFpsCounterEnabled = value;
        }

        [CreateProperty]
        public int TargetFrameRateSelection
        {
            get => m_TargetFrameRateSelection;
            set => m_TargetFrameRateSelection = value;
        }

        /// <summary>
        /// 带有初始值的构造函数。
        /// </summary>
        // 
        public GameData()
        {
            // 玩家统计数据/数据
            this.m_Gold = 500;
            this.m_Gems = 50;
            this.m_HealthPotions = 6;
            this.m_LevelUpPotions = 200;

            // 设置

            this.m_UserName = "访客_123456";
            this.m_IsToggled = false;
            this.m_Theme = "默认";

            this.m_LanguageSelection = "选项1";
            this.m_MusicVolume = 80f;
            this.m_SfxVolume = 80f;

            this.m_IsFpsCounterEnabled = false;
            this.m_TargetFrameRateSelection = 0;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void LoadJson(string jsonFilepath)
        {
            JsonUtility.FromJsonOverwrite(jsonFilepath, this);
        }
    }
}