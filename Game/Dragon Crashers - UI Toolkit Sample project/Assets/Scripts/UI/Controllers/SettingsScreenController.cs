using UnityEngine;

namespace UIToolkitDemo
{
    // <summary>
    /// 管理设置数据并控制此数据在SaveManager和UI之间的流动。
    /// </summary>
    public class SettingsScreenController : MonoBehaviour
    {
        GameData m_SettingsData;

        // 管理本地化设置
        LocalizationManager m_LocalizationManager;

        // 主题的纵横比
        MediaAspectRatio m_MediaAspectRatio = MediaAspectRatio.Undefined;

        [SerializeField] bool m_Debug;

        void OnEnable()
        {
            MediaQueryEvents.ResolutionUpdated += OnResolutionUpdated;
            SettingsEvents.UIGameDataUpdated += OnUISettingsUpdated;
            SaveManager.GameDataLoaded += OnGameDataLoaded;

            m_LocalizationManager = new LocalizationManager(m_Debug);
        }

        void OnDisable()
        {
            MediaQueryEvents.ResolutionUpdated -= OnResolutionUpdated;
            SettingsEvents.UIGameDataUpdated -= OnUISettingsUpdated;
            SaveManager.GameDataLoaded -= OnGameDataLoaded;
        }

        void Awake()
        {
            if (m_Debug)
                m_LocalizationManager.LogAvailableLocales();
        }

        /// <summary>
        /// 根据单选按钮选择目标帧率（ -1 = 尽可能快，60fps，30fps）
        /// </summary>
        /// <param name="selectedIndex"></param>
        void SelectTargetFrameRate(int selectedIndex)
        {
            // 将按钮索引转换为目标帧率
            switch (selectedIndex)
            {
                case 0:
                    SettingsEvents.TargetFrameRateSet?.Invoke(-1);
                    break;
                case 1:
                    SettingsEvents.TargetFrameRateSet?.Invoke(60);
                    break;
                case 2:
                    SettingsEvents.TargetFrameRateSet?.Invoke(30);
                    break;
                default:
                    SettingsEvents.TargetFrameRateSet?.Invoke(60);
                    break;
            }
        }

        /// <summary>
        /// 处理更新后的设置数据
        /// </summary>
        /// <param name="newSettingsData"></param>

        void OnUISettingsUpdated(GameData newSettingsData)
        {
            if (newSettingsData == null)
                return;

            m_SettingsData = newSettingsData;

            // 根据滑动开关位置切换帧率计数器
            SettingsEvents.FpsCounterToggled?.Invoke(m_SettingsData.IsFpsCounterEnabled);
            SelectTargetFrameRate(m_SettingsData.TargetFrameRateSelection);

            // 通知GameDataManager和其他监听器
            SettingsEvents.SettingsUpdated?.Invoke(m_SettingsData);

            // 应用主题设置
            ApplyThemeSettings();

            // 应用本地化设置
            ApplyLocalizationSettings();
        }

        /// <summary>
        /// 将从SaveManager加载的数据同步到UI
        /// </summary>
        /// <param name="gameData"></param>
        void OnGameDataLoaded(GameData gameData)
        {
            if (gameData == null)
                return;

            m_SettingsData = gameData;
            SettingsEvents.GameDataLoaded?.Invoke(m_SettingsData);
        }

        /// <summary>
        /// 存储主题的纵向/横向信息
        /// </summary>
        /// <param name="resolution"></param>
        void OnResolutionUpdated(Vector2 resolution)
        {
            m_MediaAspectRatio = MediaQuery.CalculateAspectRatio(resolution);
        }


        /// <summary>
        /// 根据m_SettingsData中的语言选择设置区域设置。
        /// </summary>
        void ApplyLocalizationSettings()
        {
            m_LocalizationManager.SetLocale(m_SettingsData.LanguageSelection);
        }

        /// <summary>
        /// 根据m_SettingsData设置主题
        /// </summary>
        void ApplyThemeSettings()
        {
            // SettingsData将主题存储为字符串键。基本名称是纵横比（横向或纵向）加上
            // 季节性装饰的修饰符。此示例包括6个主题（横向--默认、横向--万圣节、
            // 横向--圣诞节、纵向--默认、纵向--万圣节、纵向--圣诞节）。

            string newTheme = m_MediaAspectRatio.ToString() + "--" + m_SettingsData.Theme;
            ThemeEvents.ThemeChanged(newTheme);
        }
    }
}