using System;
using UnityEngine;
using UnityEngine.UIElements;
using MyUILibrary;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UIToolkitDemo
{
    /// <summary>
    /// 这控制游戏的一般设置。在这个演示中，其中一些选项没有实际功能，但
    /// 展示了如何将数据发送到 GameDataManager。
    /// </summary>
    public class SettingsView : UIView
    {
        // 这些类选择器用于隐藏/显示设置屏幕覆盖层；这允许 USS 过渡
        // 淡入/淡出 UI。
        const string k_ScreenActiveClass = "settings__screen";
        const string k_ScreenInactiveClass = "settings__screen--inactive";

        // 视觉元素
        Button m_BackButton;
        Button m_ResetLevelButton;
        Button m_ResetFundsButton;
        TextField m_PlayerTextfield;
        Toggle m_ExampleToggle;
        DropdownField m_ThemeDropdown;
        DropdownField m_LanguageDropdown;
        Slider m_MusicSlider;
        Slider m_SfxSlider;
        SlideToggle m_SlideToggle;
        RadioButtonGroup m_FrameRateRadioButtonsGroup;
        VisualElement m_ScreenContainer; // 用于过渡的顶级 UI 元素

        // 临时存储，用于将设置数据发送回 SettingsController
        GameData m_LocalUISettings = new GameData();

        LocalizationManager m_LocalizationManager;

        // 因为它们是本地化的而不是固定字符串，我们使用这些数组将下拉选项映射到
        // 它们的内部值。我们使用数组（而不是字典）来保留预期的顺序。

        // 语言选择的下拉选项（名称与 Locale 的第一个名称匹配）
        public static readonly string[] LanguageKeys = { "English", "Spanish", "French", "Danish", "Chinese" };

        // 主题选择的下拉选项（名称与主题样式表匹配）
        public static readonly string[] ThemeOptionKeys = { "Default", "Halloween", "Christmas" };

        // 构造函数和生命周期方法

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="topElement"></param>
        public SettingsView(VisualElement topElement) : base(topElement)
        {
            // 使用之前保存的数据设置 m_SettingsData
            SettingsEvents.GameDataLoaded += OnGameDataLoaded;

            base.SetVisualElements();

            // 默认隐藏/禁用
            m_ScreenContainer.AddToClassList(k_ScreenInactiveClass);
            m_ScreenContainer.RemoveFromClassList(k_ScreenActiveClass);

            m_LocalizationManager = new LocalizationManager();

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        /// <summary>
        /// 释放 <see cref="SettingsView"/> 实例并取消注册事件处理程序。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            SettingsEvents.GameDataLoaded -= OnGameDataLoaded;
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;

            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// 显示设置视图并触发 UI 过渡。
        /// </summary>
        public override void Show()
        {
            base.Show();

            // 使用样式进行淡入过渡
            m_ScreenContainer.RemoveFromClassList(k_ScreenInactiveClass);
            m_ScreenContainer.AddToClassList(k_ScreenActiveClass);

            // 通知 GameDataManager
            SettingsEvents.SettingsShown?.Invoke();
        }

        // UI 初始化方法

        /// <summary>
        /// 从 UI 文档中初始化并缓存视觉 UI 元素。
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_BackButton = m_TopElement.Q<Button>("settings__panel-back-button");
            m_ResetLevelButton = m_TopElement.Q<Button>("settings__social-button1");
            m_ResetFundsButton = m_TopElement.Q<Button>("settings__social-button2");
            m_PlayerTextfield = m_TopElement.Q<TextField>("settings__player-textfield");
            m_ExampleToggle = m_TopElement.Q<Toggle>("settings__toggle");
            m_ThemeDropdown = m_TopElement.Q<DropdownField>("settings__theme-dropdown");
            m_LanguageDropdown = m_TopElement.Q<DropdownField>("settings__dropdown");
            m_MusicSlider = m_TopElement.Q<Slider>("settings__slider1");
            m_SfxSlider = m_TopElement.Q<Slider>("settings__slider2");
            m_SlideToggle = m_TopElement.Q<SlideToggle>("settings__slide-toggle");
            m_FrameRateRadioButtonsGroup = m_TopElement.Q<RadioButtonGroup>("settings__radio-button-group");

            m_ScreenContainer = m_TopElement.Q<VisualElement>("settings__screen");

            UpdateLocalization();
        }

        /// <summary>
        /// 为 UI 元素事件注册回调方法。
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            m_BackButton.RegisterCallback<ClickEvent>(CloseWindow);

            m_ResetLevelButton.RegisterCallback<ClickEvent>(ResetLevel);
            m_ResetFundsButton.RegisterCallback<ClickEvent>(ResetFunds);

            m_PlayerTextfield.RegisterCallback<KeyDownEvent>(SetPlayerTextfield);
            m_ThemeDropdown.RegisterValueChangedCallback(ChangeThemeDropdown);
            m_ThemeDropdown.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_LanguageDropdown.RegisterValueChangedCallback(ChangeLanguageDropdown);

            m_LanguageDropdown.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_MusicSlider.RegisterValueChangedCallback(ChangeMusicVolume);
            m_MusicSlider.RegisterCallback<PointerCaptureOutEvent>(evt =>
                SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings));
            m_MusicSlider.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_SfxSlider.RegisterValueChangedCallback(ChangeSfxVolume);
            m_SfxSlider.RegisterCallback<PointerCaptureOutEvent>(evt =>
                SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings));
            m_SfxSlider.RegisterCallback<PointerDownEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_ExampleToggle.RegisterValueChangedCallback(ChangeToggle);
            m_ExampleToggle.RegisterCallback<ClickEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_SlideToggle.RegisterValueChangedCallback(ChangeSlideToggle);
            m_SlideToggle.RegisterCallback<ClickEvent>(evt => AudioManager.PlayDefaultButtonSound());

            m_FrameRateRadioButtonsGroup.RegisterCallback<ChangeEvent<int>>(ChangeRadioButton);
        }

        /// <summary>
        /// 从 UI 元素取消注册回调方法以防止内存泄漏。
        /// </summary>
        void UnregisterButtonCallbacks()
        {
            // 从 UI 元素取消注册所有回调
            m_BackButton?.UnregisterCallback<ClickEvent>(CloseWindow);
            m_ResetLevelButton?.UnregisterCallback<ClickEvent>(ResetLevel);
            m_ResetFundsButton?.UnregisterCallback<ClickEvent>(ResetFunds);
            m_PlayerTextfield?.UnregisterCallback<KeyDownEvent>(SetPlayerTextfield);
            m_ThemeDropdown?.UnregisterValueChangedCallback(ChangeThemeDropdown);
            m_LanguageDropdown?.UnregisterValueChangedCallback(ChangeLanguageDropdown);
            m_MusicSlider?.UnregisterValueChangedCallback(ChangeMusicVolume);
            m_SfxSlider?.UnregisterValueChangedCallback(ChangeSfxVolume);
            m_ExampleToggle?.UnregisterValueChangedCallback(ChangeToggle);
            m_SlideToggle?.UnregisterValueChangedCallback(ChangeSlideToggle);
            m_FrameRateRadioButtonsGroup?.UnregisterCallback<ChangeEvent<int>>(ChangeRadioButton);
        }

        // 本地化方法

        /// <summary>
        /// 根据所选语言更新本地化设置。
        /// </summary>
        void UpdateLocalization()
        {
            // 如果下拉菜单未初始化，则不继续
            if (m_LanguageDropdown == null)
                return;

            // 首先根据保存的选择设置区域设置
            if (!string.IsNullOrEmpty(m_LocalUISettings.LanguageSelection))
            {
                string localeCode = LocalizationManager.GetLocaleCode(m_LocalUISettings.LanguageSelection);
                m_LocalizationManager?.SetLocale(localeCode);

                // 等待一帧以确保区域设置设置完成后再更新文本
                m_TopElement.schedule.Execute(() => { UpdateLocalizedText(); });
            }
            else
            {
                UpdateLocalizedText();
            }
        }

        /// <summary>
        /// 更新动态 UI 元素的本地化文本，这些元素未在 UI Builder 中设置（主题下拉菜单、语言下拉菜单、滑动切换和帧率
        /// 单选按钮）
        /// </summary>
        void UpdateLocalizedText()
        {
            // 更新主题下拉菜单选项
            string[] themeChoices = new string[]
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_ThemeDropdown_Option1"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_ThemeDropdown_Option2"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_ThemeDropdown_Option3")
            };

            m_ThemeDropdown.UpdateLocalizedChoices(themeChoices, m_LocalUISettings.Theme, ThemeOptionKeys);

            // 使用本地化表中的正确键获取语言的本地化名称
            string[] languageChoices = new string[]
            {
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_English"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_Spanish"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_French"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_Danish"),
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_LanguageDropdown_Chinese")
            };

            m_LanguageDropdown.UpdateLocalizedChoices(languageChoices, m_LocalUISettings.LanguageSelection,
                LanguageKeys);

            // 更新开/关滑动切换标签
            string onLabel =
                m_SlideToggle.OffLabel =
                    LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                        "Settings_FpsSlideToggle_Off");

            m_SlideToggle.OnLabel =
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Settings_FpsSlideToggle_On");
            m_SlideToggle.SetValueWithoutNotify(m_SlideToggle.value);

            // 更新最大帧率单选按钮标签
            var radioButtons = m_FrameRateRadioButtonsGroup.Query<RadioButton>().ToList();
            radioButtons[0].text =
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_FrameRateRadioButtons_Max");
        }

        /// <summary>
        /// 更改区域设置时触发的事件处理程序。
        /// </summary>
        /// <param name="newLocale"></param>
        void OnSelectedLocaleChanged(Locale newLocale)
        {
            UpdateLocalizedText();
        }

        // 一般事件处理方法

        /// <summary>
        /// 处理游戏数据加载事件，用保存的值更新 UI 元素。
        /// </summary>
        /// <param name="loadedGameData">加载的游戏数据。</param>
        void OnGameDataLoaded(GameData loadedGameData)
        {
            if (loadedGameData == null)
                return;

            m_LocalUISettings = loadedGameData;

            // 首先更新非本地化 UI 元素
            m_PlayerTextfield.value = loadedGameData.UserName;
            m_ThemeDropdown.value = loadedGameData.Theme;
            m_FrameRateRadioButtonsGroup.value = loadedGameData.TargetFrameRateSelection;
            m_MusicSlider.value = loadedGameData.MusicVolume;
            m_SfxSlider.value = loadedGameData.SfxVolume;
            m_SlideToggle.value = loadedGameData.IsFpsCounterEnabled;
            m_ExampleToggle.value = loadedGameData.IsToggled;

            // 然后处理本地化，这将更新语言下拉菜单
            UpdateLocalization();

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// 关闭设置窗口并触发 UI 过渡。
        /// </summary>
        /// <param name="evt">触发关闭操作的点击事件。</param>
        void CloseWindow(ClickEvent evt)
        {
            m_ScreenContainer.RemoveFromClassList(k_ScreenActiveClass);
            m_ScreenContainer.AddToClassList(k_ScreenInactiveClass);

            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);

            Hide();
        }

        /// <summary>
        /// 处理玩家名称文本字段在按下 Return/Enter 键时的更改。
        /// </summary>
        /// <param name="evt"></param>
        void SetPlayerTextfield(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Return && m_LocalUISettings != null)
            {
                m_LocalUISettings.UserName = m_PlayerTextfield.text;
                SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
            }
        }

        /// <summary>
        /// 处理 FPS 计数器滑动切换的更改。
        /// </summary>
        /// <param name="evt"></param>
        void ChangeSlideToggle(ChangeEvent<bool> evt)
        {
            // 切换滑动切换的值（启用/禁用 FPS 计数器）
            m_LocalUISettings.IsFpsCounterEnabled = evt.newValue;

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        // 切换事件的回调函数
        void ChangeToggle(ChangeEvent<bool> evt)
        {
            // 用于演示目的的无实际功能的设置
            m_LocalUISettings.IsToggled = evt.newValue;

            // 通知 GameDataManager
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// 处理音效音量滑块的更改。
        /// </summary>
        /// <param name="evt"></param>
        void ChangeSfxVolume(ChangeEvent<float> evt)
        {
            evt.StopPropagation();
            m_LocalUISettings.SfxVolume = evt.newValue;
        }

        /// <summary>
        /// 处理音乐音量滑块的更改。
        /// </summary>
        /// <param name="evt">包含新浮点值的更改事件。</param>
        void ChangeMusicVolume(ChangeEvent<float> evt)
        {
            evt.StopPropagation();
            m_LocalUISettings.MusicVolume = evt.newValue;
        }

        /// <summary>
        /// 处理主题选择下拉菜单的更改。
        /// </summary>
        /// <param name="evt">包含新字符串值的更改事件（此处未使用）。</param>
        void ChangeThemeDropdown(ChangeEvent<string> evt)
        {
            // 从下拉菜单中获取所选索引
            int selectedIndex = m_ThemeDropdown.index;

            // 验证所选索引
            if (selectedIndex >= 0 && selectedIndex < ThemeOptionKeys.Length)
            {
                // 将索引映射到逻辑键
                m_LocalUISettings.Theme = ThemeOptionKeys[selectedIndex];
            }
            else
            {
                // 处理错误或默认情况
                m_LocalUISettings.Theme = ThemeOptionKeys[0]; // 默认主题（原始名称）
            }

            // 通知其他组件更改
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// 处理语言选择下拉菜单的更改。
        /// </summary>
        /// <param name="evt">包含新字符串值的更改事件（此处未使用）。</param>
        void ChangeLanguageDropdown(ChangeEvent<string> evt)
        {
            int selectedIndex = m_LanguageDropdown.index;

            if (selectedIndex >= 0 && selectedIndex < LanguageKeys.Length)
            {
                m_LocalUISettings.LanguageSelection = LanguageKeys[selectedIndex];

                // 首先设置区域设置
                string localeCode = LocalizationManager.GetLocaleCode(m_LocalUISettings.LanguageSelection);
                m_LocalizationManager?.SetLocale(localeCode);

                // 然后更新 UI
                UpdateLocalizedText();
            }

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }


        /// <summary>
        /// 处理帧率单选按钮选择的更改。
        /// </summary>
        /// <param name="evt">包含新所选索引的更改事件。</param>
        void ChangeRadioButton(ChangeEvent<int> evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // 用于演示目的的无实际功能的设置
            m_LocalUISettings.TargetFrameRateSelection = evt.newValue;

            // 通知 GameDataManager
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// 处理重置等级按钮的点击事件。
        /// </summary>
        /// <param name="evt">触发重置操作的点击事件。</param>
        void ResetLevel(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.PlayerLevelReset?.Invoke();
        }

        /// <summary>
        /// 处理重置资金按钮的点击事件。
        /// </summary>
        /// <param name="evt">触发重置操作的点击事件。</param>
        void ResetFunds(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.PlayerFundsReset?.Invoke();
        }
    }
}