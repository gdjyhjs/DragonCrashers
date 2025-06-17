using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    /// <summary>
    /// ����һ����ģ̬������Ļ������UIManager���������Գ���������ʱ�ı�֪ͨ�û���
    /// </summary>
    public class PopUpText : MenuScreen
    {
        // ���ı���ɫ����
        public static readonly string TextHighlight = "F8BB19";

        // ���������ı���ʽ
        const string k_PopUpTextClass = "popup-text";

        // ÿ����Ϣ�������Լ�����ʽ

        // �̵���Ļ��Ϣ��
        const string k_ShopActiveClass = "popup-text-active";
        const string k_ShopInactiveClass = "popup-text-inactive";

        // ��ɫ��Ļ��Ϣ��
        const string k_GearActiveClass = "popup-text-active--left";
        const string k_GearInactiveClass = "popup-text-inactive--left";

        // ����Ļ��Ϣ��
        const string k_HomeActiveClass = "popup-text-active--home";
        const string k_HomeInactiveClass = "popup-text-inactive--home";

        // �ʼ�������Ϣ��
        const string k_MailActiveClass = "popup-text-active--reward";
        const string k_MailInactiveClass = "popup-text-inactive--reward";

        // ��ӭ��Ϣ֮����ӳ�
        const float k_HomeMessageCooldown = 15f;

        float m_TimeToNextHomeMessage = 0f;
        Label m_PopUpText;

        // �Զ���ÿ���ı���ʾ�Ļ/�ǻ��ʽ������ʱ����ӳ�
        float m_Delay = 0f;
        float m_Duration = 1f;
        string m_ActiveClass;
        string m_InactiveClass;

        // �������/����仯
        MediaQuery m_MediaQuery;
        MediaAspectRatio m_MediaAspectRatio;

        // ���ػ��ַ����ͻ����ı�
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

        // �洢��ǰ��Ϣ����
        ShopItemSO m_CurrentShopItem;
        string m_CurrentGearName;
        (uint quantity, ShopItemType type) m_CurrentReward;

        // ��ʽ���ı�������
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

            // ���ػ��¼��������
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

            // ȡ������
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

                // �����κ�ʣ���ѡ����
                SetupText();

                // �����ı�
                HideText();

                // �ȴ��ӳ�
                yield return new WaitForSeconds(m_Delay);

                // ��ʾ�ı����ȴ�����ʱ��
                ShowText();
                yield return new WaitForSeconds(m_Duration);

                // �ٴ������ı�
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

        // ����κ�ʣ���ѡ��������ӻ�����ʽ
        void SetupText()
        {
            m_PopUpText.ClearClassList();
            m_PopUpText.AddToClassList(k_PopUpTextClass);
        }

        // �¼�������

        // ���ػ��¼��������
        void OnInsufficientFundsChanged(string localizedText) => m_LocalizedInsufficientFunds = localizedText;
        void OnGearEquippedChanged(string localizedText) => m_LocalizedGearEquipped = localizedText;
        void OnInsufficientPotionsChanged(string localizedText) => m_LocalizedInsufficientPotions = localizedText;
        void OnItemAddedChanged(string localizedText) => m_LocalizedItemAdded = localizedText;
        void OnPotionsAddedChanged(string localizedText) => m_LocalizedPotionsAdded = localizedText;


        void OnTransactionFailed(ShopItemSO shopItemSO)
        {
            // ʹ�ø������ӳ٣��������Ϣ����
            m_Delay = 0f;
            m_Duration = 1.2f;

            // ���̵���Ļ����
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

            // �ڽ�ɫ��Ļ����
            m_ActiveClass = k_GearActiveClass;
            m_InactiveClass = k_GearInactiveClass;

            m_CurrentGearName = gear.equipmentName;
            ShowMessage(GetGearEquippedText);
            m_CurrentGearName = null;
        }

        void OnCharacterLeveledUp(bool state)
        {
            // ����ʧ��ʱ��ʾ�ı�����
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
            // ������Ļ��ӭ���
            if (Time.time >= m_TimeToNextHomeMessage)
            {
                // ʱ���λ��
                m_Delay = 0.25f;
                m_Duration = 1.5f;

                // �ڱ����·�����
                m_ActiveClass = k_HomeActiveClass;
                m_InactiveClass = k_HomeInactiveClass;

                ShowMessage(message);

                // ��ȴ�ӳ��Ա�����Ϣˢ��
                m_TimeToNextHomeMessage = Time.time + k_HomeMessageCooldown;
            }
        }

        void OnShopTransactionProcessed(ShopItemSO shopItem, Vector2 screenPos)
        {
            // ����ҩˮ�����ǽ�һ�ʯ��ʱ��ʾ��Ϣ
            if (shopItem.ContentType == ShopItemType.LevelUpPotion || shopItem.ContentType == ShopItemType.HealthPotion)
            {

                // ʱ���λ��
                m_Delay = 0f;
                m_Duration = 0.8f;

                // ���̵���Ļ����
                m_ActiveClass = k_ShopActiveClass;
                m_InactiveClass = k_ShopInactiveClass;

                m_CurrentShopItem = shopItem;
                ShowMessage(GetItemAddedText);
                m_CurrentShopItem = null;
            }
        }

        void OnMailRewardProcessed(ShopItemType rewardType, uint rewardQuantity, Vector2 screenPos)
        {
            // ����ҩˮ�����ǽ�һ�ʯ��ʱ��ʾ��Ϣ
            if (rewardType == ShopItemType.LevelUpPotion || rewardType == ShopItemType.HealthPotion)
            {

                // ʱ���λ��
                m_Delay = 0f;
                m_Duration = 1.2f;

                // ���Ҳ��ʼ�������Ļ����
                m_ActiveClass = k_MailActiveClass;
                m_InactiveClass = k_MailInactiveClass;

                m_CurrentReward = (rewardQuantity, rewardType);
                ShowMessage(GetPotionsAddedText);
                m_CurrentReward = default;
            }
        }
    }
}