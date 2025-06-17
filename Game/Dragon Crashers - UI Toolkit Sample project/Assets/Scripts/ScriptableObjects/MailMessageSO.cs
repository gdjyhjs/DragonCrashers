using UnityEngine;
using System;
using Unity.Properties;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/MailMessages/MailMessageGameData",
        menuName = "UIToolkitDemo/MailMessage", order = 5)]
    public class MailMessageSO : ScriptableObject
    {
        // 常量

        // 主题行的最大字符数
        const int k_MaxSubjectLine = 14;

        // 精灵图/图标的资源位置
        const string k_ResourcePath = "GameData/GameIcons";

        // 显示在左侧
        [SerializeField] string sender;  // 发件人

        // 显示为标题
        [SerializeField] string subjectLine;  // 主题行

        // 格式：MM/dd/yyyy
        [SerializeField] string date;  // 日期

        // 邮件正文文本
        [TextArea] [SerializeField] string emailText;  // 邮件文本

        // 邮件末尾的图片
        [SerializeField] Sprite emailPicAttachment;  // 邮件图片附件

        // 邮件页脚显示的免费商店物品
        [SerializeField] uint rewardValue;  // 奖励值

        // 免费商店物品的类型
        [SerializeField] ShopItemType rewardType;  // 奖励类型

        // 礼物是否已领取
        [SerializeField] bool isClaimed;  // 是否已领取

        // 重要消息在发件人旁边显示徽章
        [SerializeField] bool isImportant;  // 是否重要

        // 未读
        [SerializeField] bool isNew;  // 是否为新邮件

        // 已删除的消息显示在第二个标签中
        [SerializeField] bool isDeleted;  // 是否已删除

        // 配对图标与货币/商店物品类型的可脚本化对象
        GameIconsSO m_GameIconsData;

        // 属性

        // 显示在左侧
        [CreateProperty] public string Sender => sender;  // 发件人

        // 显示为标题
        [CreateProperty]
        public string SubjectLine => string.IsNullOrEmpty(subjectLine)
            ? "..."
            : (subjectLine.Length <= k_MaxSubjectLine
                ? subjectLine
                : subjectLine.Substring(0, k_MaxSubjectLine) + "...");  // 主题行

        [CreateProperty]
        public DateTime Date =>
            (DateTime.TryParse(date, out var parsedDate)) ? parsedDate : DateTime.MinValue;  // 日期

        // 格式：MM/dd/yyyy
        [CreateProperty] public string FormattedDate => Date.ToString("MM/dd/yyyy");  // 格式化日期

        // 邮件正文文本
        [CreateProperty] public string EmailText => emailText;  // 邮件文本

        // 邮件末尾的图片
        [CreateProperty] public Sprite EmailPicAttachment => emailPicAttachment;  // 邮件图片附件

        // 邮件页脚显示的免费商店物品
        [CreateProperty] public uint RewardValue => rewardValue;  // 奖励值

        // 免费商店物品的类型
        [CreateProperty] public ShopItemType RewardType => rewardType;  // 奖励类型

        // 根据奖励类型检索图标
        [CreateProperty]
        public Sprite RewardIcon
        {
            get
            {
                if (m_GameIconsData != null)
                {
                    return m_GameIconsData.GetShopTypeIcon(rewardType);
                }
                Debug.LogWarning("[MailMessageSO] RewardIcon: GameIconsSO data not found.");
                return null;
            }
        }

        // 礼物是否已领取
        [CreateProperty]
        public bool IsClaimed
        {
            get => isClaimed;
            set => isClaimed = value;
        }

        // 重要消息在发件人旁边显示徽章
        [CreateProperty] public bool IsImportant => isImportant;  // 是否重要

        // 未读
        [CreateProperty]
        public bool IsNew
        {
            get => isNew;
            set => isNew = value;
        }

        // 已删除的消息显示在第二个标签中
        [CreateProperty]
        public bool IsDeleted
        {
            get => isDeleted;
            set => isDeleted = value;
        }

        // 控制礼物数量的可见性
        [CreateProperty]
        public DisplayStyle GiftAmountDisplayStyle =>
            !IsClaimed && RewardValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;  // 礼物数量显示样式

        // 控制礼物图标的可见性
        [CreateProperty]
        public DisplayStyle GiftIconDisplayStyle =>
            !IsClaimed && RewardValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;  // 礼物图标显示样式

        // 控制领取按钮的可见性
        [CreateProperty]
        public DisplayStyle ClaimButtonDisplayStyle =>
            !IsClaimed && RewardValue > 0 && !IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;  // 领取按钮显示样式

        // 控制删除按钮的可见性
        [CreateProperty]
        public DisplayStyle DeleteButtonDisplayStyle =>
            !IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;  // 删除按钮显示样式

        // 控制恢复删除按钮的可见性
        [CreateProperty]
        public DisplayStyle UndeleteButtonDisplayStyle =>
            IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;  // 恢复删除按钮显示样式

        void OnEnable()
        {
            // 从资源中加载图标数据（类似于 ShopItemSO）
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
        }
    }
}