using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEditor;

namespace UIToolkitDemo
{
    /// <summary>
    /// 用于帮助管理本地化设置的类。
    /// </summary>
    public class LocalizationManager
    {
        [SerializeField] bool m_Debug;

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="debug">标志位，用于在控制台记录调试信息。</param>
        public LocalizationManager(bool debug = false)
        {
            m_Debug = debug;

#if UNITY_EDITOR
            // 监听播放模式状态的改变
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        // 当播放模式状态改变时调用
        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // 当退出播放模式时，重置本地化系统以防止出错
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                LocalizationSettings.Instance.ResetState();
            }
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~LocalizationManager()
        {
            // 移除播放模式状态改变的监听
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
#endif
        // 设置当前的语言环境
        public void SetLocale(string localeName)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            // 去除输入的语言环境名称前后的空白字符
            string inputLocaleName = localeName.Trim();

            foreach (var locale in locales)
            {
                // 获取显示名称，直到第一个 '(' 为止，并去除前后的空白字符
                string localeDisplayName = locale.name.Split('(')[0].Trim();

                // 检查语言环境的显示名称是否以输入的语言环境名称开头（不区分大小写）
                if (localeDisplayName.StartsWith(inputLocaleName, StringComparison.OrdinalIgnoreCase))
                {
                    LocalizationSettings.SelectedLocale = locale;

                    if (m_Debug)
                        // 记录语言环境已更改的调试信息
                        Debug.Log("语言环境已更改为: " + locale.name);
                    return;
                }
            }

            // 如果未找到匹配的语言环境，且调试模式开启，则记录警告信息
            if (m_Debug)
                Debug.LogWarning("未找到名称为: " + localeName + " 的语言环境");
        }

        /// <summary>
        /// 语言环境使用正式名称加上括号内的双字母代码，例如 "English (en)"。
        /// 此方法在仅提供语言环境名称的第一部分时返回双字母代码（"English" -> "en"）
        /// </summary>
        /// <param name="localeName">要转换的语言环境名称。</param>
        /// <returns>语言环境的双字母代码，如果未找到则返回空字符串。</returns>
        // 从语言环境名称中获取语言环境代码的方法
        public static string GetLocaleCode(string localeName)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            // 去除括号内的部分（例如，"English (en)" -> "English"）
            string strippedLocaleName = localeName.Split('(')[0].Trim();

            foreach (var locale in locales)
            {
                string localeDisplayName = locale.name.Split('(')[0].Trim();

                if (localeDisplayName.Equals(strippedLocaleName, StringComparison.OrdinalIgnoreCase))
                {
                    return locale.Identifier.Code; // 返回语言环境代码（例如，"en", "fr"）
                }
            }

            return string.Empty; // 未找到匹配时返回空字符串
        }

        /// <summary>
        /// 记录可用的语言环境以进行调试
        /// </summary>
        // 记录可用语言环境的调试信息
        public void LogAvailableLocales()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            Debug.Log("可用的语言环境:");
            foreach (var locale in locales)
            {
                Debug.Log($"语言环境名称: {locale.name}, 语言环境代码: {locale.Identifier.Code}");
            }
        }
    }
}