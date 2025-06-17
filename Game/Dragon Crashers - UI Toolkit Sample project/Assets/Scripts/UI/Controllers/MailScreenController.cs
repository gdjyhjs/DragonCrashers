using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // 模拟邮件消息的呈现器/控制器
    // 包含非UI逻辑，并将ScriptableObjects中的数据发送到邮件屏幕。
    public class MailController : MonoBehaviour
    {
        const string k_InboxTabName = "标签__收件箱标签";
        const string k_DeletedTabName = "标签__已删除标签";
        const string k_ResourcePath = "游戏数据/邮件消息";

        string m_SelectedTab;
        // 存储为ScriptableObjects的邮件消息，用于模拟邮件数据
        List<MailMessageSO> m_MailMessages = new List<MailMessageSO>();
        List<MailMessageSO> m_InboxMessages = new List<MailMessageSO>();
        List<MailMessageSO> m_DeletedMessages = new List<MailMessageSO>();

        void OnEnable()
        {
            MailEvents.MarkedAsRead += OnMarkedAsRead;
            MailEvents.ClaimRewardClicked += OnClaimReward;
            MailEvents.MessageDeleted += OnDeleteMessage;
            MailEvents.UndeleteClicked += OnUndeleteMessage;

            MailEvents.TabSelected += OnTabSelected;
            MailEvents.MessageSelected += OnMessageSelected;
        }

        void OnDisable()
        {
            MailEvents.MarkedAsRead -= OnMarkedAsRead;
            MailEvents.ClaimRewardClicked -= OnClaimReward;
            MailEvents.MessageDeleted -= OnDeleteMessage;
            MailEvents.UndeleteClicked -= OnUndeleteMessage;

            MailEvents.TabSelected -= OnTabSelected;
            MailEvents.MessageSelected -= OnMessageSelected;
        }

        void Start()
        {
            LoadMailMessages();

            m_SelectedTab = k_InboxTabName;
            UpdateView();
        }

        void LoadMailMessages()
        {
            m_MailMessages.Clear();

            // 从资源目录加载ScriptableObjects（默认 = 资源/游戏数据/邮件消息）
            m_MailMessages.AddRange(Resources.LoadAll<MailMessageSO>(k_ResourcePath));

            // 分开列表以便于显示
            m_InboxMessages = m_MailMessages.Where(x => !x.IsDeleted).ToList();
            m_DeletedMessages = m_MailMessages.Where(x => x.IsDeleted).ToList();
        }

        // 在邮件屏幕界面中显示邮箱
        void UpdateView()
        {
            // 排序并从MailScreen生成元素
            m_InboxMessages = SortMailbox(m_InboxMessages);
            m_DeletedMessages = SortMailbox(m_DeletedMessages);

            UpdateMailboxByTab(m_SelectedTab);
            ShowMessage(0);
        }

        // 根据给定的标签名称选择邮箱（收件箱或已删除）
        void UpdateMailboxByTab(string tabName)
        {
            if (tabName == k_InboxTabName)
            {
                MailEvents.MailboxUpdated?.Invoke(m_InboxMessages);
            }
            else if (tabName == k_DeletedTabName)
            {
                MailEvents.MailboxUpdated?.Invoke(m_DeletedMessages);
            }
            else
            {
                Debug.LogWarning($"[MailScreenController] UpdateMailboxByTab: 无效的标签名称 {tabName}");
            }
        }

        // 按验证的日期属性对消息进行排序
        List<MailMessageSO> SortMailbox(List<MailMessageSO> originalList)
        {
            return originalList.OrderBy(x => x.Date).Reverse().ToList();
        }

        // 通过索引从邮件消息列表中返回一条邮件消息

        MailMessageSO GetInboxMessage(int index)
        {
            return GetMessageByIndex(m_InboxMessages, index);
        }

        MailMessageSO GetDeletedMessage(int index)
        {
            return GetMessageByIndex(m_DeletedMessages, index);
        }

        MailMessageSO GetMessageByIndex(List<MailMessageSO> selectedList, int index)
        {
            if (index < 0 || index >= selectedList.Count)
                return null;

            return selectedList[index];
        }

        // 将邮件消息图标从新消息更改为旧消息
        void MarkMessageAsRead(int indexToRead)
        {
            MailMessageSO msgToRead = GetInboxMessage(indexToRead);

            if (msgToRead != null && msgToRead.IsNew)
            {
                msgToRead.IsNew = false;
            }
        }

        void DeleteMessage(int indexToDelete)
        {
            MailMessageSO msgToDelete = GetInboxMessage(indexToDelete);

            if (msgToDelete == null)
                return;

            // 标记为已删除，从收件箱移动到已删除列表
            msgToDelete.IsDeleted = true;
            m_DeletedMessages.Add(msgToDelete);
            m_InboxMessages.Remove(msgToDelete);

            // 重建界面
            UpdateView();
        }

        // 事件处理方法

        void OnDeleteMessage(int index)
        {
            DeleteMessage(index);
        }

        void OnUndeleteMessage(int indexToUndelete)
        {
            MailMessageSO msgToUndelete = GetDeletedMessage(indexToUndelete);

            if (msgToUndelete == null)
                return;

            msgToUndelete.IsDeleted = false;
            m_DeletedMessages.Remove(msgToUndelete);
            m_InboxMessages.Add(msgToUndelete);

            // 重建界面
            UpdateView();
        }


        void OnClaimReward(int indexToClaim, Vector2 screenPos)
        {
            MailMessageSO messageWithReward = GetInboxMessage(indexToClaim);

            if (messageWithReward == null)
                return;

            MailEvents.RewardClaimed?.Invoke(messageWithReward, screenPos);

            messageWithReward.IsClaimed = true;
        }

        void OnMarkedAsRead(int index)
        {
            MarkMessageAsRead(index);
        }

        void OnTabSelected(string tabName)
        {
            m_SelectedTab = tabName;
            UpdateView();
        }

        void OnMessageSelected(int index)
        {
            ShowMessage(index);
        }

        void ShowMessage(int index)
        {
            List<MailMessageSO> selectedList = (m_SelectedTab == k_InboxTabName) ? m_InboxMessages : m_DeletedMessages;

            // 通过索引查找邮件消息
            MailMessageSO messageToShow = GetMessageByIndex(selectedList, index);

            // 如果有效，则将消息发送到MailContentView
            if (messageToShow != null)
                MailEvents.MessageShown?.Invoke(messageToShow);

            // 否则，显示“未选择消息”
            else
                MailEvents.ShowEmptyMessage?.Invoke();
        }


    }
}