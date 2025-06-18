using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System;
using Unity.Properties;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    public class MailContentView : UIView
    {
        const string k_GiftDeletedClass = "mail-gift-button--deleted";
        const string k_FrameBarUnclaimedClass = "mail-frame_bar--unclaimed";
        const string k_FrameBarClaimedClass = "mail-frame_bar--claimed";
        const string k_FrameBorderUnclaimedClass = "mail-frame_border--unclaimed";
        const string k_FrameBorderClaimedClass = "mail-frame_border--claimed";
        const string k_MailNoMessagesClass = "mail-no-messages";
        const string k_MailNoMessagesInactiveClass = "mail-no-messages--inactive";
        const float k_TransitionTime = 0.1f;

        Button m_ClaimButton;
        Button m_DeleteButton;
        Button m_UndeleteButton;

        VisualElement m_Footer;
        VisualElement m_FrameBorder;
        VisualElement m_FrameBar;

        Label m_MessageSubject;
        Label m_MessageText;
        VisualElement m_MessageAttachment;
        Label m_GiftAmount;
        VisualElement m_GiftIcon;

        Label m_NoMessagesLabel;

        // "δѡ����Ϣ" �ı��ػ��ַ���
        LocalizedString m_NoMessagesLocalizedString;

        // ��ǰѡ�е��ʼ���Ŀ�����Ե�ǰѡ�е������ǩ����Ĭ��Ϊ������Ŀ
        int m_CurrentMessageIndex = 0;

        /// <summary>
        /// ���캯������Դ�з�����Ϸͼ�ꡣ
        /// </summary>
        /// <param name="topElement"></param>
        public MailContentView(VisualElement topElement) : base(topElement)
        {
            // ���ñ��ػ��ַ���
            m_NoMessagesLocalizedString = new LocalizedString()
            {
                TableReference = "SettingsTable",
                TableEntryReference = "Mail_NoMessage"
            };

            // �������Ի����仯
            m_NoMessagesLocalizedString.StringChanged += OnNoMessagesStringChanged;
        }

        /// <summary>
        /// �����ػ��ַ�������ʱ������ "δѡ����Ϣ" ��ǩ��
        /// </summary>
        /// <param name="localizedText">�µı��ػ��ı���</param>
        void OnNoMessagesStringChanged(string localizedText)
        {
            m_NoMessagesLabel.text = localizedText;
        }

        public override void Initialize()
        {
            base.Initialize();

            // ��ȡ��������ѡ�еĵ�ǰ��Ϣ����
            MailEvents.MessageSelected += OnMessageSelected;

            // ��ʾû�п�����Ϣ�ı�ǩ
            MailEvents.ShowEmptyMessage += OnShowEmptyMessage;

            // ���ʼ���������ʾ�ض����ʼ���Ϣ
            MailEvents.MessageShown += OnMessageShown;

            // ��Ԫ��
            BindElements();
        }

        // ��Ϣѡ���¼�������
        void OnMessageSelected(int index)
        {
            m_CurrentMessageIndex = index;
        }

        /// <summary>
        /// ȡ�������¼��Է�ֹ�ڴ�й©��
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            // ȡ��ע����Ϣѡ���¼�
            MailEvents.MessageSelected -= OnMessageSelected;
            // ȡ��ע����ʾ����Ϣ�¼�
            MailEvents.ShowEmptyMessage -= OnShowEmptyMessage;
            // ȡ��ע����Ϣ��ʾ�¼�
            MailEvents.MessageShown -= OnMessageShown;

            // ȡ��ע�᱾�ػ��ַ��������¼�
            m_NoMessagesLocalizedString.StringChanged -= OnNoMessagesStringChanged;

            // ע����ť�ص�
            UnregisterButtonCallbacks();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // ��ȡ��ȡ������ť
            m_ClaimButton = m_TopElement.Q<Button>("content__gift-button");
            // ��ȡɾ���ʼ���ť
            m_DeleteButton = m_TopElement.Q<Button>("content__delete-button");
            // ��ȡ�ָ��ʼ���ť
            m_UndeleteButton = m_TopElement.Q<Button>("content__undelete-button");

            // ��ȡ�ʼ������ǩ
            m_MessageSubject = m_TopElement.Q<Label>("content__message-subject");
            // ��ȡ�ʼ��ı���ǩ
            m_MessageText = m_TopElement.Q<Label>("content__message-text");
            // ��ȡ�ʼ�����Ԫ��
            m_MessageAttachment = m_TopElement.Q("content__message-attachment");

            // ��ȡ����ͼ��Ԫ��
            m_GiftIcon = m_TopElement.Q("content__gift-icon");
            // ��ȡ����������ǩ
            m_GiftAmount = m_TopElement.Q<Label>("content__gift-amount");

            // ��ȡҳ��Ԫ��
            m_Footer = m_TopElement.Q("content__footer");
            // ��ȡ��ܱ߿�Ԫ��
            m_FrameBorder = m_TopElement.Q("content__frame-border");
            // ��ȡ�����Ԫ��
            m_FrameBar = m_TopElement.Q("content__frame-bar");

            // ��ȡ����Ϣ��ǩ
            m_NoMessagesLabel = m_TopElement.Q<Label>("content__no-messages");
        }

        protected override void RegisterButtonCallbacks()
        {
            // ע����ȡ������ť����¼�
            m_ClaimButton.RegisterCallback<ClickEvent>(ClaimReward);
            // ע��ɾ���ʼ���ť����¼�
            m_DeleteButton.RegisterCallback<ClickEvent>(DeleteMailMessage);
            // ע��ָ��ʼ���ť����¼�
            m_UndeleteButton.RegisterCallback<ClickEvent>(UndeleteMailMessage);
        }

        // ��ѡ��ע����ť�ص��ڴ��������²����ϸ��Ҫ��
        // ��ȡ�������Ӧ�ó�����������ڹ���
        // ����Ը��ݾ������ѡ��ע�����ǡ�
        void UnregisterButtonCallbacks()
        {
            m_ClaimButton.UnregisterCallback<ClickEvent>(ClaimReward);
            m_DeleteButton.UnregisterCallback<ClickEvent>(DeleteMailMessage);
            m_UndeleteButton.UnregisterCallback<ClickEvent>(UndeleteMailMessage);
        }

        // ��ʾ "δѡ����Ϣ" ��ǩ
        void ShowEmptyMessage()
        {
            // ��û�л���ʼ���Ϣʱ���ֶ�������ʽ���¼�
            m_MessageSubject.style.display = DisplayStyle.None;
            m_MessageText.style.display = DisplayStyle.None;
            m_MessageAttachment.style.display = DisplayStyle.None;
            m_ClaimButton.style.display = DisplayStyle.None;
            m_DeleteButton.style.display = DisplayStyle.None;
            m_UndeleteButton.style.display = DisplayStyle.None;

            m_ClaimButton.SetEnabled(false);
            m_DeleteButton.SetEnabled(false);

            // ��ʾ����Ϣ��ǩ
            ShowNoMessages(true);

            // ����ҳ��
            ShowFooter(false);
        }

        // ��ʾ����������Ϣ��ǩ
        void ShowNoMessages(bool state)
        {
            if (state)
            {
                m_NoMessagesLabel.RemoveFromClassList(k_MailNoMessagesInactiveClass);
                m_NoMessagesLabel.AddToClassList(k_MailNoMessagesClass);
            }
            else
            {
                m_NoMessagesLabel.RemoveFromClassList(k_MailNoMessagesClass);
                m_NoMessagesLabel.AddToClassList(k_MailNoMessagesInactiveClass);
            }
        }

        // ���Ҳ����������ʼ��ı�
        void ShowMailContents(MailMessageSO msg)
        {
            // ����Ϣ����ǰ������û������
            if (msg == null)
            {
                ShowEmptyMessage();
                return;
            }

            m_ClaimButton.SetEnabled(true);

            m_MessageSubject.style.display = DisplayStyle.Flex;
            m_MessageText.style.display = DisplayStyle.Flex;
            m_MessageAttachment.style.display = DisplayStyle.Flex;
            m_ClaimButton.style.display = DisplayStyle.Flex;
            m_DeleteButton.style.display = DisplayStyle.Flex;
            m_UndeleteButton.style.display = DisplayStyle.Flex;

            m_DeleteButton.SetEnabled(true);

            // ��������Ϣ��ǩ
            ShowNoMessages(false);

            m_TopElement.dataSource = msg;

            if (!msg.IsDeleted)
            {
                m_GiftAmount.RemoveFromClassList(k_GiftDeletedClass);
                m_GiftIcon.RemoveFromClassList(k_GiftDeletedClass);
            }

            // ������Ϣδɾ��������δ��ȡʱ��ʾҳ��
            ShowFooter(!msg.IsDeleted && !msg.IsClaimed);
        }

        // ��Ԫ��
        void BindElements()
        {
            // ʹ��Unity������ʱ���ݰ󶨽�UIԪ�ذ󶨵�MailMessageSO����
            m_MessageSubject.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.SubjectLine)),
                bindingMode = BindingMode.ToTarget
            });

            m_MessageText.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.EmailText)),
                bindingMode = BindingMode.ToTarget
            });

            m_MessageAttachment.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.EmailPicAttachment)),
                bindingMode = BindingMode.ToTarget
            });

            // �󶨽���Ԫ�أ�����������ͼ�꣩
            m_GiftAmount.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.RewardValue)),
                bindingMode = BindingMode.ToTarget
            });

            // �����������Ŀɼ���
            m_GiftAmount.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.GiftAmountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // �󶨽���ͼ�꣨���ڽ������ͣ�
            m_GiftIcon.SetBinding("style.backgroundImage", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.RewardIcon)),
                bindingMode = BindingMode.ToTarget
            });

            m_GiftIcon.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.GiftIconDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // ����ȡ��ť�Ŀɼ���
            m_ClaimButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.ClaimButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // ��ɾ����ť�Ŀɼ���
            m_DeleteButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.DeleteButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // �󶨻ָ���ť�Ŀɼ���
            m_UndeleteButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.UndeleteButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }

        // ��ʾ/�������ݴ��ڵĵײ�
        void ShowFooter(bool state)
        {
            m_Footer.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;

            // ��ʾ��ܱ߿����
            if (state)
            {
                m_FrameBar.RemoveFromClassList(k_FrameBarClaimedClass);
                m_FrameBar.AddToClassList(k_FrameBarUnclaimedClass);

                m_FrameBorder.RemoveFromClassList(k_FrameBorderClaimedClass);
                m_FrameBorder.AddToClassList(k_FrameBorderUnclaimedClass);
            }
            // ���ؿ�ܱ߿����
            else
            {
                m_FrameBar.RemoveFromClassList(k_FrameBarUnclaimedClass);
                m_FrameBar.AddToClassList(k_FrameBarClaimedClass);

                m_FrameBorder.RemoveFromClassList(k_FrameBorderUnclaimedClass);
                m_FrameBorder.AddToClassList(k_FrameBorderClaimedClass);
            }
        }

        // ��������

        // ֪ͨMailController/GameDataManager��ȡ����
        void ClaimReward(ClickEvent evt)
        {
            // ������¼���λ��ת��Ϊ������Ļ����
            Vector2 clickPos = new Vector2(evt.position.x, evt.position.y);

            // ��ȡ�����������Ļ/��Ԫ�ص���Ļ����
            VisualElement rootElement = m_TopElement.panel.visualTree;
            Vector2 screenPos = clickPos.GetScreenCoordinate(rootElement);

            // ������ȡ�������첽����
            _ = ClaimRewardRoutineAsync();

            // ֪ͨMailController���λ��
            MailEvents.ClaimRewardClicked?.Invoke(m_CurrentMessageIndex, screenPos);

            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();
        }

        // ��MonoBehavioursʹ���첽�ȴ�
        async Task ClaimRewardRoutineAsync()
        {
            // Ӧ��USS����Ч��������ͼ��ͱ�ǩ
            m_GiftAmount.AddToClassList(k_GiftDeletedClass);
            m_GiftIcon.AddToClassList(k_GiftDeletedClass);

            // ����ת��Ϊ����
            await Task.Delay((int)(k_TransitionTime * 1000));

            // ������ҳ����ʧ��������ȡ��ť
            ShowFooter(false);
        }

        // ɾ�� - �ָ�����

        // ɾ���ʼ���Ϣ
        void DeleteMailMessage(ClickEvent evt)
        {
            // ֪ͨ���䲥�Ŷ���
            MailEvents.DeleteClicked?.Invoke();

            // ����ɾ���ʼ���Ϣ���첽����
            _ = DeleteMailMessageRoutine();
        }

        // �ȴ�USS���ɣ�Ȼ��֪ͨ������
        async Task DeleteMailMessageRoutine()
        {
            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();

            // �ȴ�����
            await Task.Delay(TimeSpan.FromSeconds(k_TransitionTime));

            // ֪ͨMail Presenter/Controllerɾ����ǰ��Ϣ��Ȼ���ؽ�����
            MailEvents.MessageDeleted?.Invoke(m_CurrentMessageIndex);

            m_MessageAttachment.style.backgroundImage = null;
        }

        // ֪ͨ�������ָ���ǰѡ��
        void UndeleteMailMessage(ClickEvent evt)
        {
            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();
            // �����ָ�����¼�
            MailEvents.UndeleteClicked?.Invoke(m_CurrentMessageIndex);
        }

        // �¼�������

        // ��Ϣ��ʾʱ�Ĵ�����
        void OnMessageShown(MailMessageSO msg)
        {
            if (msg != null)
            {
                // ��ʾ�ʼ�����
                ShowMailContents(msg);
            }
        }

        // ��ʾ����Ϣʱ�Ĵ�����
        void OnShowEmptyMessage()
        {
            // ��ʾ����Ϣ��ʾ
            ShowEmptyMessage();
        }
    }
}