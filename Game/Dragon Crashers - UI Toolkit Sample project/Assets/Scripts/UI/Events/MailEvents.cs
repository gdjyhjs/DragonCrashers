using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与MailScreen/MailScreenController相关的公共静态委托。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class MailEvents
    {
        // 在邮件标签视图中选择标签按钮
        public static Action<string> TabSelected;

        // 用邮件消息列表（收件箱或已删除）更新邮箱
        public static Action<List<MailMessageSO>> MailboxUpdated;

        // 更改邮箱视图中邮件消息的图标
        public static Action<int> MarkedAsRead;

        // 在邮箱中按索引选择邮件消息
        public static Action<int> MessageSelected;

        // 显示没有可用消息的标签
        public static Action ShowEmptyMessage;

        // 在邮件内容中显示特定的邮件消息
        public static Action<MailMessageSO> MessageShown;

        // 播放邮件消息中的金币/宝石效果
        public static Action<MailMessageSO, Vector2> RewardClaimed;

        // 点击删除按钮
        public static Action DeleteClicked;
        // 删除邮件消息
        public static Action<int> MessageDeleted;

        // 点击恢复删除按钮
        public static Action<int> UndeleteClicked;

        // 点击领取奖励按钮
        public static Action<int, Vector2> ClaimRewardClicked;
    }
}