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
        // ����

        // �����е�����ַ���
        const int k_MaxSubjectLine = 14;

        // ����ͼ/ͼ�����Դλ��
        const string k_ResourcePath = "GameData/GameIcons";

        // ��ʾ�����
        [SerializeField] string sender;  // ������

        // ��ʾΪ����
        [SerializeField] string subjectLine;  // ������

        // ��ʽ��MM/dd/yyyy
        [SerializeField] string date;  // ����

        // �ʼ������ı�
        [TextArea] [SerializeField] string emailText;  // �ʼ��ı�

        // �ʼ�ĩβ��ͼƬ
        [SerializeField] Sprite emailPicAttachment;  // �ʼ�ͼƬ����

        // �ʼ�ҳ����ʾ������̵���Ʒ
        [SerializeField] uint rewardValue;  // ����ֵ

        // ����̵���Ʒ������
        [SerializeField] ShopItemType rewardType;  // ��������

        // �����Ƿ�����ȡ
        [SerializeField] bool isClaimed;  // �Ƿ�����ȡ

        // ��Ҫ��Ϣ�ڷ������Ա���ʾ����
        [SerializeField] bool isImportant;  // �Ƿ���Ҫ

        // δ��
        [SerializeField] bool isNew;  // �Ƿ�Ϊ���ʼ�

        // ��ɾ������Ϣ��ʾ�ڵڶ�����ǩ��
        [SerializeField] bool isDeleted;  // �Ƿ���ɾ��

        // ���ͼ�������/�̵���Ʒ���͵Ŀɽű�������
        GameIconsSO m_GameIconsData;

        // ����

        // ��ʾ�����
        [CreateProperty] public string Sender => sender;  // ������

        // ��ʾΪ����
        [CreateProperty]
        public string SubjectLine => string.IsNullOrEmpty(subjectLine)
            ? "..."
            : (subjectLine.Length <= k_MaxSubjectLine
                ? subjectLine
                : subjectLine.Substring(0, k_MaxSubjectLine) + "...");  // ������

        [CreateProperty]
        public DateTime Date =>
            (DateTime.TryParse(date, out var parsedDate)) ? parsedDate : DateTime.MinValue;  // ����

        // ��ʽ��MM/dd/yyyy
        [CreateProperty] public string FormattedDate => Date.ToString("MM/dd/yyyy");  // ��ʽ������

        // �ʼ������ı�
        [CreateProperty] public string EmailText => emailText;  // �ʼ��ı�

        // �ʼ�ĩβ��ͼƬ
        [CreateProperty] public Sprite EmailPicAttachment => emailPicAttachment;  // �ʼ�ͼƬ����

        // �ʼ�ҳ����ʾ������̵���Ʒ
        [CreateProperty] public uint RewardValue => rewardValue;  // ����ֵ

        // ����̵���Ʒ������
        [CreateProperty] public ShopItemType RewardType => rewardType;  // ��������

        // ���ݽ������ͼ���ͼ��
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

        // �����Ƿ�����ȡ
        [CreateProperty]
        public bool IsClaimed
        {
            get => isClaimed;
            set => isClaimed = value;
        }

        // ��Ҫ��Ϣ�ڷ������Ա���ʾ����
        [CreateProperty] public bool IsImportant => isImportant;  // �Ƿ���Ҫ

        // δ��
        [CreateProperty]
        public bool IsNew
        {
            get => isNew;
            set => isNew = value;
        }

        // ��ɾ������Ϣ��ʾ�ڵڶ�����ǩ��
        [CreateProperty]
        public bool IsDeleted
        {
            get => isDeleted;
            set => isDeleted = value;
        }

        // �������������Ŀɼ���
        [CreateProperty]
        public DisplayStyle GiftAmountDisplayStyle =>
            !IsClaimed && RewardValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;  // ����������ʾ��ʽ

        // ��������ͼ��Ŀɼ���
        [CreateProperty]
        public DisplayStyle GiftIconDisplayStyle =>
            !IsClaimed && RewardValue > 0 ? DisplayStyle.Flex : DisplayStyle.None;  // ����ͼ����ʾ��ʽ

        // ������ȡ��ť�Ŀɼ���
        [CreateProperty]
        public DisplayStyle ClaimButtonDisplayStyle =>
            !IsClaimed && RewardValue > 0 && !IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;  // ��ȡ��ť��ʾ��ʽ

        // ����ɾ����ť�Ŀɼ���
        [CreateProperty]
        public DisplayStyle DeleteButtonDisplayStyle =>
            !IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;  // ɾ����ť��ʾ��ʽ

        // ���ƻָ�ɾ����ť�Ŀɼ���
        [CreateProperty]
        public DisplayStyle UndeleteButtonDisplayStyle =>
            IsDeleted ? DisplayStyle.Flex : DisplayStyle.None;  // �ָ�ɾ����ť��ʾ��ʽ

        void OnEnable()
        {
            // ����Դ�м���ͼ�����ݣ������� ShopItemSO��
            m_GameIconsData = Resources.Load<GameIconsSO>(k_ResourcePath);
        }
    }
}