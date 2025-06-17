using System.Collections.Generic;
using UnityEngine;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��MailScreen/MailScreenController��صĹ�����̬ί�С�
    ///
    /// ע�⣺�Ӹ����Ͻ�����Щ�ǡ��¼����������ϸ������ϵ�C#�¼���
    /// </summary>
    public static class MailEvents
    {
        // ���ʼ���ǩ��ͼ��ѡ���ǩ��ť
        public static Action<string> TabSelected;

        // ���ʼ���Ϣ�б��ռ������ɾ������������
        public static Action<List<MailMessageSO>> MailboxUpdated;

        // ����������ͼ���ʼ���Ϣ��ͼ��
        public static Action<int> MarkedAsRead;

        // �������а�����ѡ���ʼ���Ϣ
        public static Action<int> MessageSelected;

        // ��ʾû�п�����Ϣ�ı�ǩ
        public static Action ShowEmptyMessage;

        // ���ʼ���������ʾ�ض����ʼ���Ϣ
        public static Action<MailMessageSO> MessageShown;

        // �����ʼ���Ϣ�еĽ��/��ʯЧ��
        public static Action<MailMessageSO, Vector2> RewardClaimed;

        // ���ɾ����ť
        public static Action DeleteClicked;
        // ɾ���ʼ���Ϣ
        public static Action<int> MessageDeleted;

        // ����ָ�ɾ����ť
        public static Action<int> UndeleteClicked;

        // �����ȡ������ť
        public static Action<int, Vector2> ClaimRewardClicked;
    }
}