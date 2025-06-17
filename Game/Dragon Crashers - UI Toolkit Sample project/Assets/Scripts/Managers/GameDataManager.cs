using UnityEngine;
using System;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(SaveManager))]
    public class GameDataManager : MonoBehaviour
    {
        // ����Ϸ���ݱ�����ʱ�������¼�
        public static Action GameDataRequested;
        // ����Ϸ���ݱ�����ʱ�������¼�
        public static event Action<GameData> GameDataReceived;

        [SerializeField] GameData m_GameData;
        public GameData GameData { set => m_GameData = value; get => m_GameData; }

        SaveManager m_SaveManager;
        bool m_IsGameDataInitialized;

        // ���ػ���ӭ��Ϣ�ַ���
        LocalizedString m_WelcomeMessageLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.WelcomeMessage"
        };
        string m_LocalizedWelcomeMessage;

        // ���ű�����ʱ��ע���¼�������
        void OnEnable()
        {
            MainMenuUIEvents.HomeScreenShown += OnHomeScreenShown;

            CharEvents.CharacterShown += OnCharacterShown;
            CharEvents.LevelPotionUsed += OnLevelPotionUsed;

            SettingsEvents.SettingsUpdated += OnSettingsUpdated;
            SettingsEvents.PlayerFundsReset += OnResetFunds;

            ShopEvents.ShopItemPurchasing += OnPurchaseItem;

            MailEvents.RewardClaimed += OnRewardClaimed;

            // ������Ϸ���������¼�
            GameDataRequested += OnGameDataRequested;

            // ������ӭ��Ϣ���ػ��ַ����ĸ����¼�
            m_WelcomeMessageLocalized.StringChanged += OnWelcomeMessageChanged;
        }

        // ���ű�����ʱ���Ƴ��¼�������
        void OnDisable()
        {
            MainMenuUIEvents.HomeScreenShown -= OnHomeScreenShown;

            CharEvents.CharacterShown -= OnCharacterShown;
            CharEvents.LevelPotionUsed -= OnLevelPotionUsed;

            SettingsEvents.SettingsUpdated -= OnSettingsUpdated;
            SettingsEvents.PlayerFundsReset -= OnResetFunds;

            ShopEvents.ShopItemPurchasing -= OnPurchaseItem;

            MailEvents.RewardClaimed -= OnRewardClaimed;

            GameDataRequested -= OnGameDataRequested;

            m_WelcomeMessageLocalized.StringChanged -= OnWelcomeMessageChanged;
        }

        // �ڽű�ʵ��������ʱ����
        void Awake()
        {
            m_SaveManager = GetComponent<SaveManager>();
        }

        // �ڽű�ʵ����ʼʱ����
        void Start()
        {
            // ������ڱ�������ݣ�����ر��������
            m_SaveManager.LoadGame();

            // �����Ϸ�����״μ������
            m_IsGameDataInitialized = true;

            // ǿ�Ƽ��ر��ػ���ӭ��Ϣ�ַ���
            m_LocalizedWelcomeMessage = m_WelcomeMessageLocalized.GetLocalizedString();
            // ��ʾ��ӭ��Ϣ
            ShowWelcomeMessage();

            // �����ʽ���Ϣ
            UpdateFunds();
            // ����ҩˮ��Ϣ
            UpdatePotions();
        }

        // ������ط��� 
        // �����ʽ���ʾ
        void UpdateFunds()
        {
            if (m_GameData != null)
                ShopEvents.FundsUpdated?.Invoke(m_GameData);
        }

        // ����ҩˮ��ʾ
        void UpdatePotions()
        {
            if (m_GameData != null)
                ShopEvents.PotionsUpdated?.Invoke(m_GameData);
        }

        // ����Ƿ����㹻���ʽ�����Ʒ
        bool HasSufficientFunds(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return false;

            CurrencyType currencyType = shopItem.CostInCurrencyType;

            float discountedPrice = (((100 - shopItem.Discount) / 100f) * shopItem.Cost);

            switch (currencyType)
            {
                case CurrencyType.Gold:
                    return m_GameData.Gold >= discountedPrice;

                case CurrencyType.Gems:
                    return m_GameData.Gems >= discountedPrice;

                case CurrencyType.USD:
                    return true;

                default:
                    return false;
            }
        }

        // ����Ƿ����㹻��ҩˮ��������ɫ�ȼ�
        public bool CanLevelUp(CharacterData character)
        {
            if (m_GameData == null || character == null)
                return false;

            return (character.GetPotionsForNextLevel() <= m_GameData.LevelUpPotions);
        }

        // ֧�����׷���
        void PayTransaction(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return;

            CurrencyType currencyType = shopItem.CostInCurrencyType;

            float discountedPrice = (((100 - shopItem.Discount) / 100f) * shopItem.Cost);

            switch (currencyType)
            {
                case CurrencyType.Gold:
                    m_GameData.Gold -= (uint)discountedPrice;
                    break;

                case CurrencyType.Gems:
                    m_GameData.Gems -= (uint)discountedPrice;
                    break;

                // �ǻ��һ�ռλ�� - ��ʵ��Ӧ���е����ڹ��߼�/����
                case CurrencyType.USD:
                    break;
            }
        }

        // ֧������ҩˮ����
        void PayLevelUpPotions(uint numberPotions)
        {
            if (m_GameData != null)
            {
                m_GameData.LevelUpPotions -= numberPotions;

                ShopEvents.PotionsUpdated?.Invoke(m_GameData);
            }
        }

        // ���չ������Ʒ
        void ReceivePurchasedGoods(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return;

            ShopItemType contentType = shopItem.ContentType;
            uint contentValue = shopItem.ContentAmount;

            ReceiveContent(contentType, contentValue);
        }

        // ��������������Ʒ
        // �����յ������ݣ����ҡ���ʯ��ҩˮ�ȣ�
        void ReceiveContent(ShopItemType contentType, uint contentValue)
        {
            switch (contentType)
            {
                case ShopItemType.Gold:
                    m_GameData.Gold += contentValue;
                    UpdateFunds();
                    break;

                case ShopItemType.Gems:
                    m_GameData.Gems += contentValue;
                    UpdateFunds();
                    break;

                case ShopItemType.HealthPotion:
                    m_GameData.HealthPotions += contentValue;
                    UpdatePotions();
                    UpdateFunds();
                    break;

                case ShopItemType.LevelUpPotion:
                    m_GameData.LevelUpPotions += contentValue;

                    UpdatePotions();
                    UpdateFunds();
                    break;
            }
        }

        // ��ʾ��ӭ��Ϣ
        void ShowWelcomeMessage()
        {
            if (string.IsNullOrEmpty(m_LocalizedWelcomeMessage))
            {
                // �����ӭ��Ϣ�ַ�����δ���أ���ǿ�Ƽ���
                m_LocalizedWelcomeMessage = m_WelcomeMessageLocalized.GetLocalizedString();
            }

            string message = string.Format(m_LocalizedWelcomeMessage, GameData.UserName);
            // ������ʾ��ӭ��Ϣ���¼�
            HomeEvents.HomeMessageShown?.Invoke(message);
        }


        // �¼�������

        // ����ӭ��Ϣ���ػ��ַ�������ʱ����
        void OnWelcomeMessageChanged(string localizedText)
        {
            m_LocalizedWelcomeMessage = localizedText;
        }

        // ��� UI ���������Ϸ���ݣ����ṩ�����Խ������ݰ�
        void OnGameDataRequested()
        {
            GameDataReceived?.Invoke(m_GameData);
        }

        // ���̵���Ļ������Ʒ�����ݰ�ť��Ļλ�� 
        void OnPurchaseItem(ShopItemSO shopItem, Vector2 screenPos)
        {
            if (shopItem == null)
                return;

            // �������׳ɹ���ʧ�ܵ��¼�
            if (HasSufficientFunds(shopItem))
            {
                PayTransaction(shopItem);
                ReceivePurchasedGoods(shopItem);
                // �������״�����ɵ��¼�
                ShopEvents.TransactionProcessed?.Invoke(shopItem, screenPos);

                // ����Ĭ�Ͻ�����Ч
                AudioManager.PlayDefaultTransactionSound();
            }
            else
            {
                // ֪ͨ�������������ı�����Ч�ȣ�
                ShopEvents.TransactionFailed?.Invoke(shopItem);
                // ����Ĭ�Ͼ�����Ч
                AudioManager.PlayDefaultWarningSound();
            }
        }

        // ���ʼ���Ϣ����ȡ����
        void OnRewardClaimed(MailMessageSO msg, Vector2 screenPos)
        {
            if (msg == null)
                return;

            ShopItemType rewardType = msg.RewardType;

            uint rewardValue = msg.RewardValue;

            ReceiveContent(rewardType, rewardValue);

            // ��������������ɵ��¼�
            ShopEvents.RewardProcessed?.Invoke(rewardType, rewardValue, screenPos);
            // ����Ĭ�Ͻ�����Ч
            AudioManager.PlayDefaultTransactionSound();
        }

        // ��������Ļ����ֵ
        void OnSettingsUpdated(GameData gameData)
        {

            if (gameData == null)
                return;

            m_GameData.SfxVolume = gameData.SfxVolume;
            m_GameData.MusicVolume = gameData.MusicVolume;
            m_GameData.LanguageSelection = gameData.LanguageSelection;
            m_GameData.IsFpsCounterEnabled = gameData.IsFpsCounterEnabled;
            m_GameData.IsToggled = gameData.IsToggled;
            m_GameData.Theme = gameData.Theme;
            m_GameData.UserName = gameData.UserName;
            m_GameData.TargetFrameRateSelection = gameData.TargetFrameRateSelection;
        }

        // ����ʹ��ҩˮ������ɫ�ȼ�
        void OnLevelPotionUsed(CharacterData charData)
        {
            if (charData == null)
                return;

            bool isLeveled = false;
            if (CanLevelUp(charData))
            {
                PayLevelUpPotions(charData.GetPotionsForNextLevel());
                isLeveled = true;
                // ����ʤ����Ч
                AudioManager.PlayVictorySound();
            }
            else
            {
                // ����Ĭ�Ͼ�����Ч
                AudioManager.PlayDefaultWarningSound();
            }

            // ֪ͨ���������ɫ�����Ƿ�ɹ�
            CharEvents.CharacterLeveledUp?.Invoke(isLeveled);
        }

        // �����ʽ�
        void OnResetFunds()
        {
            m_GameData.Gold = 0;
            m_GameData.Gems = 0;
            m_GameData.HealthPotions = 0;
            m_GameData.LevelUpPotions = 0;
            UpdateFunds();
            UpdatePotions();
        }

        // ������Ļ��ʾʱ����
        void OnHomeScreenShown()
        {
            if (m_IsGameDataInitialized)
            {
                ShowWelcomeMessage();
            }
        }

        // ����ɫ��ʾʱ����
        void OnCharacterShown(CharacterData charData)
        {
            // ֪ͨ��ɫ��Ļ���û����������ť����Ч
            CharEvents.LevelUpButtonEnabled?.Invoke(CanLevelUp(charData));
        }

    }
}