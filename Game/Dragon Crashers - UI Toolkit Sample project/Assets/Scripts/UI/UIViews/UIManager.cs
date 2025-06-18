using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // 主菜单 UI 各个部分的高级管理器。这里我们使用一个主 UXML 和一个 UIDocument。

    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {

        UIDocument m_MainMenuDocument;

        UIView m_CurrentView;
        UIView m_PreviousView;

        // 所有 UIView 的列表
        List<UIView> m_AllViews = new List<UIView>();

        // 模态屏幕
        UIView m_HomeView;  // 着陆屏幕
        UIView m_CharView;  // 角色屏幕
        UIView m_InfoView;  // 更多信息的资源链接
        UIView m_ShopView;  // 金币/宝石/药水的商店屏幕
        UIView m_MailView;  // 邮件屏幕

        // 覆盖屏幕
        UIView m_InventoryView;  // 角色屏幕的库存
        UIView m_SettingsView;  // 设置的覆盖屏幕

        // 工具栏
        UIView m_OptionsBarView;  // 快速访问金币/宝石和设置
        UIView m_MenuBarView;  // 菜单屏幕的导航栏
        UIView m_LevelMeterView;  // 显示总角色进度的径向进度条

        // UIView 的 VisualTree 字符串 ID；每个代表树的一个分支
        const string k_HomeViewName = "HomeScreen";
        const string k_InfoViewName = "InfoScreen";
        const string k_CharViewName = "CharScreen";
        const string k_ShopViewName = "ShopScreen";
        const string k_MailViewName = "MailScreen";
        const string k_InventoryViewName = "InventoryScreen";
        const string k_SettingsViewName = "SettingsScreen";
        const string k_OptionsBarViewName = "OptionsBar";
        const string k_MenuBarViewName = "MenuBar";
        const string k_LevelMeterViewName = "LevelMeter";

        void OnEnable()
        {
            m_MainMenuDocument = GetComponent<UIDocument>();

            SetupViews();

            SubscribeToEvents();

            // 从主页屏幕开始
            ShowModalView(m_HomeView);

        }

        void SubscribeToEvents()
        {
            MainMenuUIEvents.HomeScreenShown += OnHomeScreenShown;
            MainMenuUIEvents.CharScreenShown += OnCharScreenShown;
            MainMenuUIEvents.InfoScreenShown += OnInfoScreenShown;
            MainMenuUIEvents.ShopScreenShown += OnShopScreenShown;
            MainMenuUIEvents.MailScreenShown += OnMailScreenShown;

            MainMenuUIEvents.InventoryScreenShown += OnInventoryScreenShown;
            MainMenuUIEvents.InventoryScreenHidden += OnInventoryScreenHidden;
            MainMenuUIEvents.SettingsScreenShown += OnSettingsScreenShown;
            MainMenuUIEvents.SettingsScreenHidden += OnSettingsScreenHidden;
        }

        void OnDisable()
        {
            UnsubscribeFromEvents();

            foreach (UIView view in m_AllViews)
            {
                view.Dispose();
            }
        }

        void UnsubscribeFromEvents()
        {
            MainMenuUIEvents.HomeScreenShown -= OnHomeScreenShown;
            MainMenuUIEvents.CharScreenShown -= OnCharScreenShown;
            MainMenuUIEvents.InfoScreenShown -= OnInfoScreenShown;
            MainMenuUIEvents.ShopScreenShown -= OnShopScreenShown;
            MainMenuUIEvents.MailScreenShown -= OnMailScreenShown;

            MainMenuUIEvents.InventoryScreenShown -= OnInventoryScreenShown;
            MainMenuUIEvents.InventoryScreenHidden -= OnInventoryScreenHidden;
            MainMenuUIEvents.SettingsScreenShown -= OnSettingsScreenShown;
            MainMenuUIEvents.SettingsScreenHidden -= OnSettingsScreenHidden;
        }

        void Start()
        {
            Time.timeScale = 1f;
        }

        void SetupViews()
        {
            VisualElement root = m_MainMenuDocument.rootVisualElement;

            // 创建全屏模态视图：HomeView、CharView、InfoView、ShopView、MailView
            m_HomeView = new HomeView(root.Q<VisualElement>(k_HomeViewName)); // 着陆模态屏幕
            m_CharView = new CharView(root.Q<VisualElement>(k_CharViewName)); // 角色屏幕
            m_InfoView = new InfoView(root.Q<VisualElement>(k_InfoViewName)); // 链接和资源屏幕
            m_ShopView = new ShopView(root.Q<VisualElement>(k_ShopViewName)); // 商店屏幕
            m_MailView = new MailView(root.Q<VisualElement>(k_MailViewName)); // 邮件屏幕

            // 覆盖视图（带有背景的弹出模态）
            m_InventoryView = new InventoryView(root.Q<VisualElement>(k_InventoryViewName));  // 装备覆盖层
            m_SettingsView = new SettingsView(root.Q<VisualElement>(k_SettingsViewName)); // 游戏设置覆盖层

            // 工具栏 
            LevelMeterData meterData = CharEvents.GetLevelMeterData.Invoke();
            m_LevelMeterView = new LevelMeterView(root.Q<VisualElement>(k_LevelMeterViewName), meterData); // 径向等级计量器
            m_LevelMeterView.Initialize();

            m_OptionsBarView = new OptionsBarView(root.Q<VisualElement>(k_OptionsBarViewName)); // 设置/商店工具栏
            m_MenuBarView = new MenuBarView(root.Q<VisualElement>(k_MenuBarViewName)); // 屏幕选择工具栏

            // 在列表中跟踪模态 UI 视图以进行处置 
            m_AllViews.Add(m_HomeView);
            m_AllViews.Add(m_CharView);
            m_AllViews.Add(m_InfoView);
            m_AllViews.Add(m_ShopView);
            m_AllViews.Add(m_MailView);
            m_AllViews.Add(m_InventoryView);
            m_AllViews.Add(m_SettingsView);
            m_AllViews.Add(m_LevelMeterView);
            m_AllViews.Add(m_OptionsBarView);
            m_AllViews.Add(m_MenuBarView);

            // 默认启用的 UI 视图
            m_HomeView.Show();
            m_OptionsBarView.Show();
            m_MenuBarView.Show();
            m_LevelMeterView.Show();
        }

        // 切换模态屏幕的开/关
        void ShowModalView(UIView newView)
        {
            if (m_CurrentView != null)
                m_CurrentView.Hide();

            m_PreviousView = m_CurrentView;
            m_CurrentView = newView;

            // 显示屏幕并通知任何监听器主菜单已更新

            if (m_CurrentView != null)
            {
                m_CurrentView.Show();
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }

        // 模态屏幕方法。 
        void OnHomeScreenShown()
        {
            ShowModalView(m_HomeView);
        }

        void OnCharScreenShown()
        {
            ShowModalView(m_CharView);
        }

        void OnInfoScreenShown()
        {
            ShowModalView(m_InfoView);
        }

        void OnShopScreenShown()
        {
            ShowModalView(m_ShopView);
        }

        void OnMailScreenShown()
        {
            ShowModalView(m_MailView);
        }

        // 覆盖屏幕方法。这些打开模态 UIView，但带有对前一个屏幕的引用。

        void OnSettingsScreenShown()
        {

            m_PreviousView = m_CurrentView;
            m_SettingsView.Show();
        }

        void OnInventoryScreenShown()
        {
            m_PreviousView = m_CurrentView;
            m_InventoryView.Show();
        }

        void OnSettingsScreenHidden()
        {
            m_SettingsView.Hide();

            if (m_PreviousView != null)
            {
                m_PreviousView.Show();
                m_CurrentView = m_PreviousView;
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }

        void OnInventoryScreenHidden()
        {
            // 隐藏库存屏幕
            m_InventoryView.Hide();

            // 将当前屏幕更新为前一个屏幕
            if (m_PreviousView != null)
            {
                m_PreviousView.Show();
                m_CurrentView = m_PreviousView;
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }
    }
}