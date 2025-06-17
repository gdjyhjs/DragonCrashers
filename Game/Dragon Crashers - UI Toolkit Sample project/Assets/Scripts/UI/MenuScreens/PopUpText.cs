using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    /// <summary>
    /// 这是一个非模态覆盖屏幕，不由UIManager管理。它可以出现以用临时文本通知用户。
    /// </summary>
    public class PopUpText : MenuScreen
    {
        // 富文本颜色高亮
        public static readonly string TextHighlight = "F8BB19";

        // 包含基本文本样式
        const string k_PopUpTextClass = "popup-text";

        // 每个消息包含其自己的样式

        // 商店屏幕消息类
        const string k_ShopActiveClass = "popup-text-active";
        const string k_ShopInactiveClass = "popup-text-inactive";

        // 角色屏幕消息类
        const string k_GearActiveClass = "popup-text-active--left";
        const string k_GearInactiveClass = "popup-text-inactive--left";

        // 主屏幕消息类
        const string k_HomeActiveClass = "popup-text-active--home";
        const string k_HomeInactiveClass = "popup-text-inactive--home";

        // 邮件奖励消息类
        const string k_MailActiveClass = "popup-text-active--reward";
        const string k_MailInactiveClass = "popup-text-inactive--reward";

        // 欢迎消息之间的延迟
        const float k_HomeMessageCooldown = 15f;

        float m_TimeToNextHomeMessage = 0f;
        Label m_PopUpText;

        // 自定义每个文本提示的活动/非活动样式、持续时间和延迟
        float m_Delay = 0f;
        float m_Duration = 1f;
        string m_ActiveClass;
        string m_InactiveClass;

        // 检查纵向/横向变化
        MediaQuery m_MediaQuery;
        MediaAspectRatio m_MediaAspectRatio;

        // 本地化字符串和缓存文本
        readonly LocalizedString m_InsufficientFundsLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.InsufficientFunds"
        };
        string m_LocalizedInsufficientFunds;

        readonly LocalizedString m_GearEquippedLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.GearEquipped"
        };
        string m_LocalizedGearEquipped;

        readonly LocalizedString m_InsufficientPotionsLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.InsufficientPotions"
        };
        string m_LocalizedInsufficientPotions;

        readonly LocalizedString m_ItemAddedLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.ItemAddedToInventory"
        };
        string m_LocalizedItemAdded;

        readonly LocalizedString m_PotionsAddedLocalized = new LocalizedString
        {
            TableReference = "SettingsTable",
            TableEntryReference = "PopUp.PotionsAddedToInventory"
        };
        string m_LocalizedPotionsAdded;

        // 存储当前消息参数
        ShopItemSO m_CurrentShopItem;
        string m_CurrentGearName;
        (uint quantity, ShopItemType type) m_CurrentReward;

        // 格式化文本的属性
        string GetInsufficientFundsText => string.Format(m_LocalizedInsufficientFunds,
            m_CurrentShopItem.ItemName,
            m_CurrentShopItem.CostInCurrencyType);

        string GetGearEquippedText => string.Format(m_LocalizedGearEquipped,
            m_CurrentGearName);

        string GetItemAddedText => string.Format(m_LocalizedItemAdded,
            m_CurrentShopItem.ItemName);

        string GetPotionsAddedText => string.Format(m_LocalizedPotionsAdded,
            m_CurrentReward.quantity,
            m_CurrentReward.type,
            m_CurrentReward.quantity > 1 ? "s" : string.Empty);

        void OnEnable()
        {
            InventoryEvents.GearSelected += OnGearSelected;

            ShopEvents.TransactionProcessed += OnShopTransactionProcessed;
            ShopEvents.RewardProcessed += OnMailRewardProcessed;
            ShopEvents.TransactionFailed += OnTransactionFailed;

            CharEvents.CharacterLeveledUp += OnCharacterLeveledUp;

            HomeEvents.HomeMessageShown += OnHomeMessageShown;

            // 本地化事件处理程序
            m_InsufficientFundsLocalized.StringChanged += OnInsufficientFundsChanged;
            m_GearEquippedLocalized.StringChanged += OnGearEquippedChanged;
            m_InsufficientPotionsLocalized.StringChanged += OnInsufficientPotionsChanged;
            m_ItemAddedLocalized.StringChanged += OnItemAddedChanged;
            m_PotionsAddedLocalized.StringChanged += OnPotionsAddedChanged;
        }

        void OnDisable()
        {
            InventoryEvents.GearSelected -= OnGearSelected;

            ShopEvents.TransactionProcessed -= OnShopTransactionProcessed;
            ShopEvents.RewardProcessed -= OnMailRewardProcessed;
            ShopEvents.TransactionFailed -= OnTransactionFailed;

            CharEvents.CharacterLeveledUp -= OnCharacterLeveledUp;

            HomeEvents.HomeMessageShown -= OnHomeMessageShown;

            // 取消订阅
            m_InsufficientFundsLocalized.StringChanged -= OnInsufficientFundsChanged;
            m_GearEquippedLocalized.StringChanged -= OnGearEquippedChanged;
            m_InsufficientPotionsLocalized.StringChanged -= OnInsufficientPotionsChanged;
            m_ItemAddedLocalized.StringChanged -= OnItemAddedChanged;
            m_PotionsAddedLocalized.StringChanged -= OnPotionsAddedChanged;
        }


        protected override void Awake()
        {
            base.Awake();
            SetVisualElements();

            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            SetupText();
            HideText();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();
            m_PopUpText = m_Root.Q<Label>("main-menu-popup_text");

            if (m_PopUpText != null)
            {
                m_PopUpText.text = string.Empty;
            }
        }

        void ShowMessage(string message)
        {
            if (m_PopUpText == null || string.IsNullOrEmpty(message))
            {
                return;
            }

            StartCoroutine(ShowMessageRoutine(message));
        }

        IEnumerator ShowMessageRoutine(string message)
        {
            if (m_PopUpText != null)
            {
                m_PopUpText.text = message;

                // 重置任何剩余的选择器
                SetupText();

                // 隐藏文本
                HideText();

                // 等待延迟
                yield return new WaitForSeconds(m_Delay);

                // 显示文本并等待持续时间
                ShowText();
                yield return new WaitForSeconds(m_Duration);

                // 再次隐藏文本
                HideText();
            }
        }

        void HideText()
        {
            m_PopUpText.RemoveFromClassList(m_ActiveClass);
            m_PopUpText.AddToClassList(m_InactiveClass);
        }

        void ShowText()
        {
            m_PopUpText.RemoveFromClassList(m_InactiveClass);
            m_PopUpText.AddToClassList(m_ActiveClass);
        }

        // 清除任何剩余的选择器并添加基本样式
        void SetupText()
        {
            m_PopUpText.ClearClassList();
            m_PopUpText.AddToClassList(k_PopUpTextClass);
        }

        // 事件处理方法

        // 本地化事件处理程序
        void OnInsufficientFundsChanged(string localizedText) => m_LocalizedInsufficientFunds = localizedText;
        void OnGearEquippedChanged(string localizedText) => m_LocalizedGearEquipped = localizedText;
        void OnInsufficientPotionsChanged(string localizedText) => m_LocalizedInsufficientPotions = localizedText;
        void OnItemAddedChanged(string localizedText) => m_LocalizedItemAdded = localizedText;
        void OnPotionsAddedChanged(string localizedText) => m_LocalizedPotionsAdded = localizedText;


        void OnTransactionFailed(ShopItemSO shopItemSO)
        {
            // 使用更长的延迟，这里的消息更长
            m_Delay = 0f;
            m_Duration = 1.2f;

            // 在商店屏幕居中
            m_ActiveClass = k_ShopActiveClass;
            m_InactiveClass = k_ShopInactiveClass;

            m_CurrentShopItem = shopItemSO;
            ShowMessage(GetInsufficientFundsText);
            m_CurrentShopItem = null;
        }

        void OnGearSelected(EquipmentSO gear)
        {
            m_Delay = 0f;
            m_Duration = 0.8f;

            // 在角色屏幕居中
            m_ActiveClass = k_GearActiveClass;
            m_InactiveClass = k_GearInactiveClass;

            m_CurrentGearName = gear.equipmentName;
            ShowMessage(GetGearEquippedText);
            m_CurrentGearName = null;
        }

        void OnCharacterLeveledUp(bool state)
        {
            // 仅在失败时显示文本警告
            if (!state && m_PopUpText != null)
            {
                m_Delay = 0f;
                m_Duration = 0.8f;
                m_ActiveClass = k_GearActiveClass;
                m_InactiveClass = k_GearInactiveClass;

                ShowMessage(m_LocalizedInsufficientPotions);
            }
        }

        void OnHomeMessageShown(string message)
        {
            // 在主屏幕欢迎玩家
            if (Time.time >= m_TimeToNextHomeMessage)
            {
                // 时间和位置
                m_Delay = 0.25f;
                m_Duration = 1.5f;

                // 在标题下方居中
                m_ActiveClass = k_HomeActiveClass;
                m_InactiveClass = k_HomeInactiveClass;

                ShowMessage(message);

                // 冷却延迟以避免消息刷屏
                m_TimeToNextHomeMessage = Time.time + k_HomeMessageCooldown;
            }
        }

        void OnShopTransactionProcessed(ShopItemSO shopItem, Vector2 screenPos)
        {
            // 购买药水（不是金币或宝石）时显示消息
            if (shopItem.ContentType == ShopItemType.LevelUpPotion || shopItem.ContentType == ShopItemType.HealthPotion)
            {

                // 时间和位置
                m_Delay = 0f;
                m_Duration = 0.8f;

                // 在商店屏幕居中
                m_ActiveClass = k_ShopActiveClass;
                m_InactiveClass = k_ShopInactiveClass;

                m_CurrentShopItem = shopItem;
                ShowMessage(GetItemAddedText);
                m_CurrentShopItem = null;
            }
        }

        void OnMailRewardProcessed(ShopItemType rewardType, uint rewardQuantity, Vector2 screenPos)
        {
            // 购买药水（不是金币或宝石）时显示消息
            if (rewardType == ShopItemType.LevelUpPotion || rewardType == ShopItemType.HealthPotion)
            {

                // 时间和位置
                m_Delay = 0f;
                m_Duration = 1.2f;

                // 在右侧邮件内容屏幕居中
                m_ActiveClass = k_MailActiveClass;
                m_InactiveClass = k_MailInactiveClass;

                m_CurrentReward = (rewardQuantity, rewardType);
                ShowMessage(GetPotionsAddedText);
                m_CurrentReward = default;
            }
        }
    }
}