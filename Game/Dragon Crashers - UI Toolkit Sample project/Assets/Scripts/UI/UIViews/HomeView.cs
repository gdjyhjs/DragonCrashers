using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��������Ļ��ʾ����ʾ�ؿ���Ϣ���ؿ�ѡ�������ѡ��������Ի����仯����Ӧ������ʾ�Ĺؿ���Ϣ��
    /// </summary>
    public class HomeView : UIView
    {
        // ����ؿ���ť
        VisualElement m_PlayLevelButton;
        // �ؿ�����ͼ
        VisualElement m_LevelThumbnail;

        // �ؿ����
        Label m_LevelNumber;
        // �ؿ���ǩ
        Label m_LevelLabel;

        // ��ǰ�ؿ�����
        LevelSO m_CurrentLevelData;

        // ������ͼ
        ChatView m_ChatView;
        public ChatView ChatView => m_ChatView;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="topElement">VisualTree�����/��Ԫ�ء�</param>
        public HomeView(VisualElement topElement) : base(topElement)
        {
            m_ChatView = new ChatView(topElement);

            // ���Ĺؿ���Ϣ��ʾ�¼�
            HomeEvents.LevelInfoShown += OnShowLevelInfo;

            // �������Ի����仯
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        }

        /// <summary>
        /// ���ö�UIԪ�ص����á�
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            // ��ȡ����ؿ���ť
            m_PlayLevelButton = m_TopElement.Q("home-play__level-button");
            // ��ȡ�ؿ����Ʊ�ǩ
            m_LevelLabel = m_TopElement.Q<Label>("home-play__level-name");
            // ��ȡ�ؿ���ű�ǩ
            m_LevelNumber = m_TopElement.Q<Label>("home-play__level-number");

            // ��ȡ�ؿ���������ͼ
            m_LevelThumbnail = m_TopElement.Q("home-play__background");
        }

        /// <summary>
        /// ע�����水ť����¼��Լ�����Ϸ������
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // ע�������水ť�¼�
            m_PlayLevelButton.RegisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// ȡ�����ĺ�ע���Է�ֹ�ڴ�й©��
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            // ȡ�����Ĺؿ���Ϣ��ʾ�¼�
            HomeEvents.LevelInfoShown -= OnShowLevelInfo;
            // ȡ���������Ի����仯
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

            // ȡ��ע�������水ť�¼�
            m_PlayLevelButton.UnregisterCallback<ClickEvent>(ClickPlayButton);
        }

        /// <summary>
        /// ������Ч��֪ͨ�κι����������߼���
        /// </summary>
        /// <param name="evt"></param>
        void ClickPlayButton(ClickEvent evt)
        {
            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();
            // �������Ű�ť����¼�
            HomeEvents.PlayButtonClicked?.Invoke();
        }

        // �¼�������

        /// <summary>
        /// ������ͼ����ʾָ���Ĺؿ���Ϣ��
        /// </summary>
        /// <param name="levelData">ScriptableObject�ؿ����ݡ�</param>
        void OnShowLevelInfo(LevelSO levelData)
        {
            if (levelData == null)
                return;

            // ����һ���������Ի�������
            m_CurrentLevelData = levelData;

            // ��ʾ�ؿ���Ϣ
            ShowLevelInfo(levelData.LevelNumberFormatted, levelData.LevelSubtitle, levelData.Thumbnail);
        }

        /// <summary>
        /// �������Ի����仯���»�ȡ�����±��ػ��ַ�����
        /// </summary>
        /// <param name="locale">�µ����Ի�����</param>
        void OnLocaleChanged(Locale locale)
        {
            // ��ʾ�ؿ���Ϣ
            ShowLevelInfo(m_CurrentLevelData.LevelNumberFormatted, m_CurrentLevelData.LevelSubtitle,
                m_CurrentLevelData.Thumbnail);
        }

        /// <summary>
        /// ��ʾ�ؿ���Ϣ
        /// </summary>
        /// <param name="levelNumberFormatted"></param>
        /// <param name="levelName"></param>
        /// <param name="thumbnail"></param>
        public void ShowLevelInfo(string levelNumberFormatted, string levelName, Sprite thumbnail)
        {
            if (m_LevelNumber == null || m_LevelLabel == null || m_LevelThumbnail == null)
                return;

            // ���ùؿ�����ı�
            m_LevelNumber.text = levelNumberFormatted;
            // ���ùؿ������ı�
            m_LevelLabel.text = levelName;
            // ���ùؿ�����ͼ����
            m_LevelThumbnail.style.backgroundImage = new StyleBackground(thumbnail);
        }
    }
}