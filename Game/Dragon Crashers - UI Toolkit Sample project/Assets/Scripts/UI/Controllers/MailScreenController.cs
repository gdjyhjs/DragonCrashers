using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // ģ���ʼ���Ϣ�ĳ�����/������
    // ������UI�߼�������ScriptableObjects�е����ݷ��͵��ʼ���Ļ��
    public class MailController : MonoBehaviour
    {
        const string k_InboxTabName = "��ǩ__�ռ����ǩ";
        const string k_DeletedTabName = "��ǩ__��ɾ����ǩ";
        const string k_ResourcePath = "��Ϸ����/�ʼ���Ϣ";

        string m_SelectedTab;
        // �洢ΪScriptableObjects���ʼ���Ϣ������ģ���ʼ�����
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

            // ����ԴĿ¼����ScriptableObjects��Ĭ�� = ��Դ/��Ϸ����/�ʼ���Ϣ��
            m_MailMessages.AddRange(Resources.LoadAll<MailMessageSO>(k_ResourcePath));

            // �ֿ��б��Ա�����ʾ
            m_InboxMessages = m_MailMessages.Where(x => !x.IsDeleted).ToList();
            m_DeletedMessages = m_MailMessages.Where(x => x.IsDeleted).ToList();
        }

        // ���ʼ���Ļ��������ʾ����
        void UpdateView()
        {
            // ���򲢴�MailScreen����Ԫ��
            m_InboxMessages = SortMailbox(m_InboxMessages);
            m_DeletedMessages = SortMailbox(m_DeletedMessages);

            UpdateMailboxByTab(m_SelectedTab);
            ShowMessage(0);
        }

        // ���ݸ����ı�ǩ����ѡ�����䣨�ռ������ɾ����
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
                Debug.LogWarning($"[MailScreenController] UpdateMailboxByTab: ��Ч�ı�ǩ���� {tabName}");
            }
        }

        // ����֤���������Զ���Ϣ��������
        List<MailMessageSO> SortMailbox(List<MailMessageSO> originalList)
        {
            return originalList.OrderBy(x => x.Date).Reverse().ToList();
        }

        // ͨ���������ʼ���Ϣ�б��з���һ���ʼ���Ϣ

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

        // ���ʼ���Ϣͼ�������Ϣ����Ϊ����Ϣ
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

            // ���Ϊ��ɾ�������ռ����ƶ�����ɾ���б�
            msgToDelete.IsDeleted = true;
            m_DeletedMessages.Add(msgToDelete);
            m_InboxMessages.Remove(msgToDelete);

            // �ؽ�����
            UpdateView();
        }

        // �¼�������

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

            // �ؽ�����
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

            // ͨ�����������ʼ���Ϣ
            MailMessageSO messageToShow = GetMessageByIndex(selectedList, index);

            // �����Ч������Ϣ���͵�MailContentView
            if (messageToShow != null)
                MailEvents.MessageShown?.Invoke(messageToShow);

            // ������ʾ��δѡ����Ϣ��
            else
                MailEvents.ShowEmptyMessage?.Invoke();
        }


    }
}