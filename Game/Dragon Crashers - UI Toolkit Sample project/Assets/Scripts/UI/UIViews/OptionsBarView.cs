using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// ���������� UI�����ڴ�������ͼ���̵���ͼ��ʹ�ü򵥵�
    /// �ı��������±�ʯ�ͽ��������
    /// </summary>
    public class OptionsBarView : UIView
    {
        VisualElement m_OptionsButton;
        VisualElement m_ShopGemButton;
        VisualElement m_ShopGoldButton;
        Label m_GoldLabel;
        Label m_GemLabel;

        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="topElement"></param>
        public OptionsBarView(VisualElement topElement) : base(topElement)
        {
            // ���� GameDataReceived �¼������¼����յ�����Ϸ����ʱ������
            GameDataManager.GameDataReceived += OnGameDataReceived;

            // �� GameDataManager ������Ϸ���ݡ�
            GameDataManager.GameDataRequested?.Invoke();
        }

        /// <summary>
        /// ������Ϸ���ݽ��գ�������Һͱ�ʯֵ�󶨵����Եı�ǩ��
        /// </summary>
        /// <param name="gameData">�յ�����Ϸ���ݡ�</param>
        void OnGameDataReceived(GameData gameData)
        {
            // ����������ݰ�
            m_GoldLabel.SetBinding("text", new AnimatedTextBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(nameof(GameData.Gold)),
            });

            m_GemLabel.SetBinding("text", new AnimatedTextBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(nameof(GameData.Gems)),
            });
        }

        /// <summary>
        /// ȡ�������¼���ȡ��ע�ᰴť�ص���
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            GameDataManager.GameDataReceived -= OnGameDataReceived;
            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// ����ѡ���� UI �е��Ӿ�Ԫ�����á�
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            m_OptionsButton = m_TopElement.Q("options-bar__button");
            m_ShopGoldButton = m_TopElement.Q("options-bar__gold-button");
            m_ShopGemButton = m_TopElement.Q("options-bar__gem-button");
            m_GoldLabel = m_TopElement.Q<Label>("options-bar__gold-count");
            m_GemLabel = m_TopElement.Q<Label>("options-bar__gem-count");
        }

        /// <summary>
        /// ���ð�ť����¼�
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            m_OptionsButton.RegisterCallback<ClickEvent>(ShowOptionsScreen);
            m_ShopGemButton.RegisterCallback<ClickEvent>(OpenGemShop);
            m_ShopGoldButton.RegisterCallback<ClickEvent>(OpenGoldShop);
        }

        /// <summary>
        /// ȡ��ע��ѡ����̵갴ť�ĵ���¼��������
        /// </summary>
        void UnregisterButtonCallbacks()
        {
            m_OptionsButton.UnregisterCallback<ClickEvent>(ShowOptionsScreen);
            m_ShopGemButton.UnregisterCallback<ClickEvent>(OpenGemShop);
            m_ShopGoldButton.UnregisterCallback<ClickEvent>(OpenGoldShop);
        }

        /// <summary>
        /// ���ѡ�ťʱ��������ͼ��
        /// </summary>
        /// <param name="evt">����¼���</param>
        void ShowOptionsScreen(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            MainMenuUIEvents.SettingsScreenShown?.Invoke();
        }

        /// <summary>
        /// �����Ұ�ťʱ�򿪽���̵��ǩ��
        /// </summary>
        /// <param name="evt">����¼���</param>
        void OpenGoldShop(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // ��ʾ�̵���Ļ
            MainMenuUIEvents.OptionsBarShopScreenShown?.Invoke();

            // �򿪵���Ҳ�Ʒ�ı�ǩ
            ShopEvents.TabSelected?.Invoke("gold");
        }

        /// <summary>
        /// �����ʯ��ťʱ�򿪱�ʯ�̵��ǩ��
        /// </summary>
        /// <param name="evt">����¼���</param>
        void OpenGemShop(ClickEvent evt)
        {
            AudioManager.PlayDefaultButtonSound();

            // ��ʾ�̵���Ļ
            MainMenuUIEvents.OptionsBarShopScreenShown?.Invoke();

            // �򿪵���ʯ��Ʒ�ı�ǩ
            ShopEvents.TabSelected?.Invoke("gem");
        }
    }
}