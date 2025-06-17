using UnityEngine;

namespace UIToolkitDemo
{
    // <summary>
    /// �����������ݲ����ƴ�������SaveManager��UI֮���������
    /// </summary>
    public class SettingsScreenController : MonoBehaviour
    {
        GameData m_SettingsData;

        // �����ػ�����
        LocalizationManager m_LocalizationManager;

        // ������ݺ��
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
        /// ���ݵ�ѡ��ťѡ��Ŀ��֡�ʣ� -1 = �����ܿ죬60fps��30fps��
        /// </summary>
        /// <param name="selectedIndex"></param>
        void SelectTargetFrameRate(int selectedIndex)
        {
            // ����ť����ת��ΪĿ��֡��
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
        /// ������º����������
        /// </summary>
        /// <param name="newSettingsData"></param>

        void OnUISettingsUpdated(GameData newSettingsData)
        {
            if (newSettingsData == null)
                return;

            m_SettingsData = newSettingsData;

            // ���ݻ�������λ���л�֡�ʼ�����
            SettingsEvents.FpsCounterToggled?.Invoke(m_SettingsData.IsFpsCounterEnabled);
            SelectTargetFrameRate(m_SettingsData.TargetFrameRateSelection);

            // ֪ͨGameDataManager������������
            SettingsEvents.SettingsUpdated?.Invoke(m_SettingsData);

            // Ӧ����������
            ApplyThemeSettings();

            // Ӧ�ñ��ػ�����
            ApplyLocalizationSettings();
        }

        /// <summary>
        /// ����SaveManager���ص�����ͬ����UI
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
        /// �洢���������/������Ϣ
        /// </summary>
        /// <param name="resolution"></param>
        void OnResolutionUpdated(Vector2 resolution)
        {
            m_MediaAspectRatio = MediaQuery.CalculateAspectRatio(resolution);
        }


        /// <summary>
        /// ����m_SettingsData�е�����ѡ�������������á�
        /// </summary>
        void ApplyLocalizationSettings()
        {
            m_LocalizationManager.SetLocale(m_SettingsData.LanguageSelection);
        }

        /// <summary>
        /// ����m_SettingsData��������
        /// </summary>
        void ApplyThemeSettings()
        {
            // SettingsData������洢Ϊ�ַ������������������ݺ�ȣ���������򣩼���
            // ������װ�ε����η�����ʾ������6�����⣨����--Ĭ�ϡ�����--��ʥ�ڡ�
            // ����--ʥ���ڡ�����--Ĭ�ϡ�����--��ʥ�ڡ�����--ʥ���ڣ���

            string newTheme = m_MediaAspectRatio.ToString() + "--" + m_SettingsData.Theme;
            ThemeEvents.ThemeChanged(newTheme);
        }
    }
}