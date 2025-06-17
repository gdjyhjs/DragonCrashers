using UnityEngine;
using System;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(SaveManager))]
    public class GameDataManager : MonoBehaviour
    {
        // 当游戏数据被请求时触发的事件
        public static Action GameDataRequested;
        // 当游戏数据被接收时触发的事件
        public static event Action<GameData> GameDataReceived;

        [SerializeField] GameData m_GameData;
        public GameData GameData { set => m_GameData = value; get => m_GameData; }

        SaveManager m_SaveManager;
        bool m_IsGameDataInitialized;

        // 本地化欢迎消息字符串
        LocalizedString m_WelcomeMessageLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.WelcomeMessage"
        };
        string m_LocalizedWelcomeMessage;

        // 当脚本启用时，注册事件监听器
        void OnEnable()
        {
            MainMenuUIEvents.HomeScreenShown += OnHomeScreenShown;

            CharEvents.CharacterShown += OnCharacterShown;
            CharEvents.LevelPotionUsed += OnLevelPotionUsed;

            SettingsEvents.SettingsUpdated += OnSettingsUpdated;
            SettingsEvents.PlayerFundsReset += OnResetFunds;

            ShopEvents.ShopItemPurchasing += OnPurchaseItem;

            MailEvents.RewardClaimed += OnRewardClaimed;

            // 监听游戏数据请求事件
            GameDataRequested += OnGameDataRequested;

            // 监听欢迎消息本地化字符串的更改事件
            m_WelcomeMessageLocalized.StringChanged += OnWelcomeMessageChanged;
        }

        // 当脚本禁用时，移除事件监听器
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

        // 在脚本实例被唤醒时调用
        void Awake()
        {
            m_SaveManager = GetComponent<SaveManager>();
        }

        // 在脚本实例开始时调用
        void Start()
        {
            // 如果存在保存的数据，则加载保存的数据
            m_SaveManager.LoadGame();

            // 标记游戏数据首次加载完成
            m_IsGameDataInitialized = true;

            // 强制加载本地化欢迎消息字符串
            m_LocalizedWelcomeMessage = m_WelcomeMessageLocalized.GetLocalizedString();
            // 显示欢迎消息
            ShowWelcomeMessage();

            // 更新资金信息
            UpdateFunds();
            // 更新药水信息
            UpdatePotions();
        }

        // 交易相关方法 
        // 更新资金显示
        void UpdateFunds()
        {
            if (m_GameData != null)
                ShopEvents.FundsUpdated?.Invoke(m_GameData);
        }

        // 更新药水显示
        void UpdatePotions()
        {
            if (m_GameData != null)
                ShopEvents.PotionsUpdated?.Invoke(m_GameData);
        }

        // 检查是否有足够的资金购买商品
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

        // 检查是否有足够的药水来提升角色等级
        public bool CanLevelUp(CharacterData character)
        {
            if (m_GameData == null || character == null)
                return false;

            return (character.GetPotionsForNextLevel() <= m_GameData.LevelUpPotions);
        }

        // 支付交易费用
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

                // 非货币化占位符 - 在实际应用中调用内购逻辑/界面
                case CurrencyType.USD:
                    break;
            }
        }

        // 支付升级药水费用
        void PayLevelUpPotions(uint numberPotions)
        {
            if (m_GameData != null)
            {
                m_GameData.LevelUpPotions -= numberPotions;

                ShopEvents.PotionsUpdated?.Invoke(m_GameData);
            }
        }

        // 接收购买的商品
        void ReceivePurchasedGoods(ShopItemSO shopItem)
        {
            if (shopItem == null)
                return;

            ShopItemType contentType = shopItem.ContentType;
            uint contentValue = shopItem.ContentAmount;

            ReceiveContent(contentType, contentValue);
        }

        // 接收礼物或购买的物品
        // 处理收到的内容（如金币、宝石、药水等）
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

        // 显示欢迎消息
        void ShowWelcomeMessage()
        {
            if (string.IsNullOrEmpty(m_LocalizedWelcomeMessage))
            {
                // 如果欢迎消息字符串尚未加载，则强制加载
                m_LocalizedWelcomeMessage = m_WelcomeMessageLocalized.GetLocalizedString();
            }

            string message = string.Format(m_LocalizedWelcomeMessage, GameData.UserName);
            // 触发显示欢迎消息的事件
            HomeEvents.HomeMessageShown?.Invoke(message);
        }


        // 事件处理方法

        // 当欢迎消息本地化字符串更改时调用
        void OnWelcomeMessageChanged(string localizedText)
        {
            m_LocalizedWelcomeMessage = localizedText;
        }

        // 如果 UI 组件请求游戏数据，则提供数据以进行数据绑定
        void OnGameDataRequested()
        {
            GameDataReceived?.Invoke(m_GameData);
        }

        // 从商店屏幕购买物品，传递按钮屏幕位置 
        void OnPurchaseItem(ShopItemSO shopItem, Vector2 screenPos)
        {
            if (shopItem == null)
                return;

            // 触发交易成功或失败的事件
            if (HasSufficientFunds(shopItem))
            {
                PayTransaction(shopItem);
                ReceivePurchasedGoods(shopItem);
                // 触发交易处理完成的事件
                ShopEvents.TransactionProcessed?.Invoke(shopItem, screenPos);

                // 播放默认交易音效
                AudioManager.PlayDefaultTransactionSound();
            }
            else
            {
                // 通知监听器（弹出文本、音效等）
                ShopEvents.TransactionFailed?.Invoke(shopItem);
                // 播放默认警告音效
                AudioManager.PlayDefaultWarningSound();
            }
        }

        // 从邮件消息中领取奖励
        void OnRewardClaimed(MailMessageSO msg, Vector2 screenPos)
        {
            if (msg == null)
                return;

            ShopItemType rewardType = msg.RewardType;

            uint rewardValue = msg.RewardValue;

            ReceiveContent(rewardType, rewardValue);

            // 触发奖励处理完成的事件
            ShopEvents.RewardProcessed?.Invoke(rewardType, rewardValue, screenPos);
            // 播放默认交易音效
            AudioManager.PlayDefaultTransactionSound();
        }

        // 从设置屏幕更新值
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

        // 尝试使用药水提升角色等级
        void OnLevelPotionUsed(CharacterData charData)
        {
            if (charData == null)
                return;

            bool isLeveled = false;
            if (CanLevelUp(charData))
            {
                PayLevelUpPotions(charData.GetPotionsForNextLevel());
                isLeveled = true;
                // 播放胜利音效
                AudioManager.PlayVictorySound();
            }
            else
            {
                // 播放默认警告音效
                AudioManager.PlayDefaultWarningSound();
            }

            // 通知其他对象角色升级是否成功
            CharEvents.CharacterLeveledUp?.Invoke(isLeveled);
        }

        // 重置资金
        void OnResetFunds()
        {
            m_GameData.Gold = 0;
            m_GameData.Gems = 0;
            m_GameData.HealthPotions = 0;
            m_GameData.LevelUpPotions = 0;
            UpdateFunds();
            UpdatePotions();
        }

        // 当主屏幕显示时调用
        void OnHomeScreenShown()
        {
            if (m_IsGameDataInitialized)
            {
                ShowWelcomeMessage();
            }
        }

        // 当角色显示时调用
        void OnCharacterShown(CharacterData charData)
        {
            // 通知角色屏幕启用或禁用升级按钮的特效
            CharEvents.LevelUpButtonEnabled?.Invoke(CanLevelUp(charData));
        }

    }
}