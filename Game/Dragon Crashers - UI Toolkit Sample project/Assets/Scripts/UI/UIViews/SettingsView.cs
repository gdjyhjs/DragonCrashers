using System;
using UnityEngine;
using UnityEngine.UIElements;
using MyUILibrary;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UIToolkitDemo
{
    /// <summary>
    /// �������Ϸ��һ�����á��������ʾ�У�����һЩѡ��û��ʵ�ʹ��ܣ���
    /// չʾ����ν����ݷ��͵� GameDataManager��
    /// </summary>
    public class SettingsView : UIView
    {
        // ��Щ��ѡ������������/��ʾ������Ļ���ǲ㣻������ USS ����
        // ����/���� UI��
        const string k_ScreenActiveClass = "settings__screen";
        const string k_ScreenInactiveClass = "settings__screen--inactive";

        // �Ӿ�Ԫ��
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
        VisualElement m_ScreenContainer; // ���ڹ��ɵĶ��� UI Ԫ��

        // ��ʱ�洢�����ڽ��������ݷ��ͻ� SettingsController
        GameData m_LocalUISettings = new GameData();

        LocalizationManager m_LocalizationManager;

        // ��Ϊ�����Ǳ��ػ��Ķ����ǹ̶��ַ���������ʹ����Щ���齫����ѡ��ӳ�䵽
        // ���ǵ��ڲ�ֵ������ʹ�����飨�������ֵ䣩������Ԥ�ڵ�˳��

        // ����ѡ�������ѡ������� Locale �ĵ�һ������ƥ�䣩
        public static readonly string[] LanguageKeys = { "English", "Spanish", "French", "Danish", "Chinese" };

        // ����ѡ�������ѡ�������������ʽ��ƥ�䣩
        public static readonly string[] ThemeOptionKeys = { "Default", "Halloween", "Christmas" };

        // ���캯�����������ڷ���

        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="topElement"></param>
        public SettingsView(VisualElement topElement) : base(topElement)
        {
            // ʹ��֮ǰ������������� m_SettingsData
            SettingsEvents.GameDataLoaded += OnGameDataLoaded;

            base.SetVisualElements();

            // Ĭ������/����
            m_ScreenContainer.AddToClassList(k_ScreenInactiveClass);
            m_ScreenContainer.RemoveFromClassList(k_ScreenActiveClass);

            m_LocalizationManager = new LocalizationManager();

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        /// <summary>
        /// �ͷ� <see cref="SettingsView"/> ʵ����ȡ��ע���¼��������
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            SettingsEvents.GameDataLoaded -= OnGameDataLoaded;
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;

            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// ��ʾ������ͼ������ UI ���ɡ�
        /// </summary>
        public override void Show()
        {
            base.Show();

            // ʹ����ʽ���е������
            m_ScreenContainer.RemoveFromClassList(k_ScreenInactiveClass);
            m_ScreenContainer.AddToClassList(k_ScreenActiveClass);

            // ֪ͨ GameDataManager
            SettingsEvents.SettingsShown?.Invoke();
        }

        // UI ��ʼ������

        /// <summary>
        /// �� UI �ĵ��г�ʼ���������Ӿ� UI Ԫ�ء�
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
        /// Ϊ UI Ԫ���¼�ע��ص�������
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
        /// �� UI Ԫ��ȡ��ע��ص������Է�ֹ�ڴ�й©��
        /// </summary>
        void UnregisterButtonCallbacks()
        {
            // �� UI Ԫ��ȡ��ע�����лص�
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

        // ���ػ�����

        /// <summary>
        /// ������ѡ���Ը��±��ػ����á�
        /// </summary>
        void UpdateLocalization()
        {
            // ��������˵�δ��ʼ�����򲻼���
            if (m_LanguageDropdown == null)
                return;

            // ���ȸ��ݱ����ѡ��������������
            if (!string.IsNullOrEmpty(m_LocalUISettings.LanguageSelection))
            {
                string localeCode = LocalizationManager.GetLocaleCode(m_LocalUISettings.LanguageSelection);
                m_LocalizationManager?.SetLocale(localeCode);

                // �ȴ�һ֡��ȷ����������������ɺ��ٸ����ı�
                m_TopElement.schedule.Execute(() => { UpdateLocalizedText(); });
            }
            else
            {
                UpdateLocalizedText();
            }
        }

        /// <summary>
        /// ���¶�̬ UI Ԫ�صı��ػ��ı�����ЩԪ��δ�� UI Builder �����ã����������˵������������˵��������л���֡��
        /// ��ѡ��ť��
        /// </summary>
        void UpdateLocalizedText()
        {
            // �������������˵�ѡ��
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

            // ʹ�ñ��ػ����е���ȷ����ȡ���Եı��ػ�����
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

            // ���¿�/�ػ����л���ǩ
            string onLabel =
                m_SlideToggle.OffLabel =
                    LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                        "Settings_FpsSlideToggle_Off");

            m_SlideToggle.OnLabel =
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable", "Settings_FpsSlideToggle_On");
            m_SlideToggle.SetValueWithoutNotify(m_SlideToggle.value);

            // �������֡�ʵ�ѡ��ť��ǩ
            var radioButtons = m_FrameRateRadioButtonsGroup.Query<RadioButton>().ToList();
            radioButtons[0].text =
                LocalizationSettings.StringDatabase.GetLocalizedString("SettingsTable",
                    "Settings_FrameRateRadioButtons_Max");
        }

        /// <summary>
        /// ������������ʱ�������¼��������
        /// </summary>
        /// <param name="newLocale"></param>
        void OnSelectedLocaleChanged(Locale newLocale)
        {
            UpdateLocalizedText();
        }

        // һ���¼�������

        /// <summary>
        /// ������Ϸ���ݼ����¼����ñ����ֵ���� UI Ԫ�ء�
        /// </summary>
        /// <param name="loadedGameData">���ص���Ϸ���ݡ�</param>
        void OnGameDataLoaded(GameData loadedGameData)
        {
            if (loadedGameData == null)
                return;

            m_LocalUISettings = loadedGameData;

            // ���ȸ��·Ǳ��ػ� UI Ԫ��
            m_PlayerTextfield.value = loadedGameData.UserName;
            m_ThemeDropdown.value = loadedGameData.Theme;
            m_FrameRateRadioButtonsGroup.value = loadedGameData.TargetFrameRateSelection;
            m_MusicSlider.value = loadedGameData.MusicVolume;
            m_SfxSlider.value = loadedGameData.SfxVolume;
            m_SlideToggle.value = loadedGameData.IsFpsCounterEnabled;
            m_ExampleToggle.value = loadedGameData.IsToggled;

            // Ȼ�����ػ����⽫�������������˵�
            UpdateLocalization();

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// �ر����ô��ڲ����� UI ���ɡ�
        /// </summary>
        /// <param name="evt">�����رղ����ĵ���¼���</param>
        void CloseWindow(ClickEvent evt)
        {
            m_ScreenContainer.RemoveFromClassList(k_ScreenActiveClass);
            m_ScreenContainer.AddToClassList(k_ScreenInactiveClass);

            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);

            Hide();
        }

        /// <summary>
        /// ������������ı��ֶ��ڰ��� Return/Enter ��ʱ�ĸ��ġ�
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
        /// ���� FPS �����������л��ĸ��ġ�
        /// </summary>
        /// <param name="evt"></param>
        void ChangeSlideToggle(ChangeEvent<bool> evt)
        {
            // �л������л���ֵ������/���� FPS ��������
            m_LocalUISettings.IsFpsCounterEnabled = evt.newValue;

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        // �л��¼��Ļص�����
        void ChangeToggle(ChangeEvent<bool> evt)
        {
            // ������ʾĿ�ĵ���ʵ�ʹ��ܵ�����
            m_LocalUISettings.IsToggled = evt.newValue;

            // ֪ͨ GameDataManager
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// ������Ч��������ĸ��ġ�
        /// </summary>
        /// <param name="evt"></param>
        void ChangeSfxVolume(ChangeEvent<float> evt)
        {
            evt.StopPropagation();
            m_LocalUISettings.SfxVolume = evt.newValue;
        }

        /// <summary>
        /// ����������������ĸ��ġ�
        /// </summary>
        /// <param name="evt">�����¸���ֵ�ĸ����¼���</param>
        void ChangeMusicVolume(ChangeEvent<float> evt)
        {
            evt.StopPropagation();
            m_LocalUISettings.MusicVolume = evt.newValue;
        }

        /// <summary>
        /// ��������ѡ�������˵��ĸ��ġ�
        /// </summary>
        /// <param name="evt">�������ַ���ֵ�ĸ����¼����˴�δʹ�ã���</param>
        void ChangeThemeDropdown(ChangeEvent<string> evt)
        {
            // �������˵��л�ȡ��ѡ����
            int selectedIndex = m_ThemeDropdown.index;

            // ��֤��ѡ����
            if (selectedIndex >= 0 && selectedIndex < ThemeOptionKeys.Length)
            {
                // ������ӳ�䵽�߼���
                m_LocalUISettings.Theme = ThemeOptionKeys[selectedIndex];
            }
            else
            {
                // ��������Ĭ�����
                m_LocalUISettings.Theme = ThemeOptionKeys[0]; // Ĭ�����⣨ԭʼ���ƣ�
            }

            // ֪ͨ�����������
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// ��������ѡ�������˵��ĸ��ġ�
        /// </summary>
        /// <param name="evt">�������ַ���ֵ�ĸ����¼����˴�δʹ�ã���</param>
        void ChangeLanguageDropdown(ChangeEvent<string> evt)
        {
            int selectedIndex = m_LanguageDropdown.index;

            if (selectedIndex >= 0 && selectedIndex < LanguageKeys.Length)
            {
                m_LocalUISettings.LanguageSelection = LanguageKeys[selectedIndex];

                // ����������������
                string localeCode = LocalizationManager.GetLocaleCode(m_LocalUISettings.LanguageSelection);
                m_LocalizationManager?.SetLocale(localeCode);

                // Ȼ����� UI
                UpdateLocalizedText();
            }

            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }


        /// <summary>
        /// ����֡�ʵ�ѡ��ťѡ��ĸ��ġ�
        /// </summary>
        /// <param name="evt">��������ѡ�����ĸ����¼���</param>
        void ChangeRadioButton(ChangeEvent<int> evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // ������ʾĿ�ĵ���ʵ�ʹ��ܵ�����
            m_LocalUISettings.TargetFrameRateSelection = evt.newValue;

            // ֪ͨ GameDataManager
            SettingsEvents.UIGameDataUpdated?.Invoke(m_LocalUISettings);
        }

        /// <summary>
        /// �������õȼ���ť�ĵ���¼���
        /// </summary>
        /// <param name="evt">�������ò����ĵ���¼���</param>
        void ResetLevel(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.PlayerLevelReset?.Invoke();
        }

        /// <summary>
        /// ���������ʽ�ť�ĵ���¼���
        /// </summary>
        /// <param name="evt">�������ò����ĵ���¼���</param>
        void ResetFunds(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            SettingsEvents.PlayerFundsReset?.Invoke();
        }
    }
}