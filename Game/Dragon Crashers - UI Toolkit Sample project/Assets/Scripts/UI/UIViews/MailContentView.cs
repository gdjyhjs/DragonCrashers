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

        // "未选择消息" 的本地化字符串
        LocalizedString m_NoMessagesLocalizedString;

        // 当前选中的邮件项目（来自当前选中的邮箱标签），默认为顶部项目
        int m_CurrentMessageIndex = 0;

        /// <summary>
        /// 构造函数从资源中分配游戏图标。
        /// </summary>
        /// <param name="topElement"></param>
        public MailContentView(VisualElement topElement) : base(topElement)
        {
            // 引用本地化字符串
            m_NoMessagesLocalizedString = new LocalizedString()
            {
                TableReference = "SettingsTable",
                TableEntryReference = "Mail_NoMessage"
            };

            // 监听语言环境变化
            m_NoMessagesLocalizedString.StringChanged += OnNoMessagesStringChanged;
        }

        /// <summary>
        /// 当本地化字符串更新时，更新 "未选择消息" 标签。
        /// </summary>
        /// <param name="localizedText">新的本地化文本。</param>
        void OnNoMessagesStringChanged(string localizedText)
        {
            m_NoMessagesLabel.text = localizedText;
        }

        public override void Initialize()
        {
            base.Initialize();

            // 获取从邮箱中选中的当前消息索引
            MailEvents.MessageSelected += OnMessageSelected;

            // 显示没有可用消息的标签
            MailEvents.ShowEmptyMessage += OnShowEmptyMessage;

            // 在邮件内容中显示特定的邮件消息
            MailEvents.MessageShown += OnMessageShown;

            // 绑定元素
            BindElements();
        }

        // 消息选择事件处理方法
        void OnMessageSelected(int index)
        {
            m_CurrentMessageIndex = index;
        }

        /// <summary>
        /// 取消订阅事件以防止内存泄漏。
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            // 取消注册消息选择事件
            MailEvents.MessageSelected -= OnMessageSelected;
            // 取消注册显示空消息事件
            MailEvents.ShowEmptyMessage -= OnShowEmptyMessage;
            // 取消注册消息显示事件
            MailEvents.MessageShown -= OnMessageShown;

            // 取消注册本地化字符串更改事件
            m_NoMessagesLocalizedString.StringChanged -= OnNoMessagesStringChanged;

            // 注销按钮回调
            UnregisterButtonCallbacks();
        }

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // 获取领取奖励按钮
            m_ClaimButton = m_TopElement.Q<Button>("content__gift-button");
            // 获取删除邮件按钮
            m_DeleteButton = m_TopElement.Q<Button>("content__delete-button");
            // 获取恢复邮件按钮
            m_UndeleteButton = m_TopElement.Q<Button>("content__undelete-button");

            // 获取邮件主题标签
            m_MessageSubject = m_TopElement.Q<Label>("content__message-subject");
            // 获取邮件文本标签
            m_MessageText = m_TopElement.Q<Label>("content__message-text");
            // 获取邮件附件元素
            m_MessageAttachment = m_TopElement.Q("content__message-attachment");

            // 获取礼物图标元素
            m_GiftIcon = m_TopElement.Q("content__gift-icon");
            // 获取礼物数量标签
            m_GiftAmount = m_TopElement.Q<Label>("content__gift-amount");

            // 获取页脚元素
            m_Footer = m_TopElement.Q("content__footer");
            // 获取框架边框元素
            m_FrameBorder = m_TopElement.Q("content__frame-border");
            // 获取框架条元素
            m_FrameBar = m_TopElement.Q("content__frame-bar");

            // 获取无消息标签
            m_NoMessagesLabel = m_TopElement.Q<Label>("content__no-messages");
        }

        protected override void RegisterButtonCallbacks()
        {
            // 注册领取奖励按钮点击事件
            m_ClaimButton.RegisterCallback<ClickEvent>(ClaimReward);
            // 注册删除邮件按钮点击事件
            m_DeleteButton.RegisterCallback<ClickEvent>(DeleteMailMessage);
            // 注册恢复邮件按钮点击事件
            m_UndeleteButton.RegisterCallback<ClickEvent>(UndeleteMailMessage);
        }

        // 可选：注销按钮回调在大多数情况下不是严格必要的
        // 这取决于你的应用程序的生命周期管理。
        // 你可以根据具体情况选择注销它们。
        void UnregisterButtonCallbacks()
        {
            m_ClaimButton.UnregisterCallback<ClickEvent>(ClaimReward);
            m_DeleteButton.UnregisterCallback<ClickEvent>(DeleteMailMessage);
            m_UndeleteButton.UnregisterCallback<ClickEvent>(UndeleteMailMessage);
        }

        // 显示 "未选择消息" 标签
        void ShowEmptyMessage()
        {
            // 当没有活动的邮件消息时，手动禁用样式和事件
            m_MessageSubject.style.display = DisplayStyle.None;
            m_MessageText.style.display = DisplayStyle.None;
            m_MessageAttachment.style.display = DisplayStyle.None;
            m_ClaimButton.style.display = DisplayStyle.None;
            m_DeleteButton.style.display = DisplayStyle.None;
            m_UndeleteButton.style.display = DisplayStyle.None;

            m_ClaimButton.SetEnabled(false);
            m_DeleteButton.SetEnabled(false);

            // 显示无消息标签
            ShowNoMessages(true);

            // 隐藏页脚
            ShowFooter(false);
        }

        // 显示或隐藏无消息标签
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

        // 在右侧面板中填充邮件文本
        void ShowMailContents(MailMessageSO msg)
        {
            // 空消息，当前邮箱中没有内容
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

            // 隐藏无消息标签
            ShowNoMessages(false);

            m_TopElement.dataSource = msg;

            if (!msg.IsDeleted)
            {
                m_GiftAmount.RemoveFromClassList(k_GiftDeletedClass);
                m_GiftIcon.RemoveFromClassList(k_GiftDeletedClass);
            }

            // 仅当消息未删除且礼物未领取时显示页脚
            ShowFooter(!msg.IsDeleted && !msg.IsClaimed);
        }

        // 绑定元素
        void BindElements()
        {
            // 使用Unity的运行时数据绑定将UI元素绑定到MailMessageSO属性
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

            // 绑定奖励元素（礼物数量和图标）
            m_GiftAmount.SetBinding("text", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.RewardValue)),
                bindingMode = BindingMode.ToTarget
            });

            // 绑定礼物数量的可见性
            m_GiftAmount.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.GiftAmountDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 绑定奖励图标（基于奖励类型）
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

            // 绑定领取按钮的可见性
            m_ClaimButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.ClaimButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 绑定删除按钮的可见性
            m_DeleteButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.DeleteButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });

            // 绑定恢复按钮的可见性
            m_UndeleteButton.SetBinding("style.display", new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(MailMessageSO.UndeleteButtonDisplayStyle)),
                bindingMode = BindingMode.ToTarget
            });
        }

        // 显示/隐藏内容窗口的底部
        void ShowFooter(bool state)
        {
            m_Footer.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;

            // 显示框架边框和条
            if (state)
            {
                m_FrameBar.RemoveFromClassList(k_FrameBarClaimedClass);
                m_FrameBar.AddToClassList(k_FrameBarUnclaimedClass);

                m_FrameBorder.RemoveFromClassList(k_FrameBorderClaimedClass);
                m_FrameBorder.AddToClassList(k_FrameBorderUnclaimedClass);
            }
            // 隐藏框架边框和条
            else
            {
                m_FrameBar.RemoveFromClassList(k_FrameBarUnclaimedClass);
                m_FrameBar.AddToClassList(k_FrameBarClaimedClass);

                m_FrameBorder.RemoveFromClassList(k_FrameBorderUnclaimedClass);
                m_FrameBorder.AddToClassList(k_FrameBorderClaimedClass);
            }
        }

        // 奖励方法

        // 通知MailController/GameDataManager领取礼物
        void ClaimReward(ClickEvent evt)
        {
            // 将点击事件的位置转换为像素屏幕坐标
            Vector2 clickPos = new Vector2(evt.position.x, evt.position.y);

            // 获取相对于整个屏幕/根元素的屏幕坐标
            VisualElement rootElement = m_TopElement.panel.visualTree;
            Vector2 screenPos = clickPos.GetScreenCoordinate(rootElement);

            // 启动领取奖励的异步任务
            _ = ClaimRewardRoutineAsync();

            // 通知MailController点击位置
            MailEvents.ClaimRewardClicked?.Invoke(m_CurrentMessageIndex, screenPos);

            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();
        }

        // 非MonoBehaviours使用异步等待
        async Task ClaimRewardRoutineAsync()
        {
            // 应用USS过渡效果到礼物图标和标签
            m_GiftAmount.AddToClassList(k_GiftDeletedClass);
            m_GiftIcon.AddToClassList(k_GiftDeletedClass);

            // 将秒转换为毫秒
            await Task.Delay((int)(k_TransitionTime * 1000));

            // 动画化页脚消失并禁用领取按钮
            ShowFooter(false);
        }

        // 删除 - 恢复方法

        // 删除邮件消息
        void DeleteMailMessage(ClickEvent evt)
        {
            // 通知邮箱播放动画
            MailEvents.DeleteClicked?.Invoke();

            // 启动删除邮件消息的异步任务
            _ = DeleteMailMessageRoutine();
        }

        // 等待USS过渡，然后通知控制器
        async Task DeleteMailMessageRoutine()
        {
            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();

            // 等待过渡
            await Task.Delay(TimeSpan.FromSeconds(k_TransitionTime));

            // 通知Mail Presenter/Controller删除当前消息，然后重建界面
            MailEvents.MessageDeleted?.Invoke(m_CurrentMessageIndex);

            m_MessageAttachment.style.backgroundImage = null;
        }

        // 通知控制器恢复当前选择
        void UndeleteMailMessage(ClickEvent evt)
        {
            // 播放默认按钮音效
            AudioManager.PlayDefaultButtonSound();
            // 触发恢复点击事件
            MailEvents.UndeleteClicked?.Invoke(m_CurrentMessageIndex);
        }

        // 事件处理方法

        // 消息显示时的处理方法
        void OnMessageShown(MailMessageSO msg)
        {
            if (msg != null)
            {
                // 显示邮件内容
                ShowMailContents(msg);
            }
        }

        // 显示空消息时的处理方法
        void OnShowEmptyMessage()
        {
            // 显示空消息提示
            ShowEmptyMessage();
        }
    }
}