using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // ���˵� UI �������ֵĸ߼�����������������ʹ��һ���� UXML ��һ�� UIDocument��

    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoBehaviour
    {

        UIDocument m_MainMenuDocument;

        UIView m_CurrentView;
        UIView m_PreviousView;

        // ���� UIView ���б�
        List<UIView> m_AllViews = new List<UIView>();

        // ģ̬��Ļ
        UIView m_HomeView;  // ��½��Ļ
        UIView m_CharView;  // ��ɫ��Ļ
        UIView m_InfoView;  // ������Ϣ����Դ����
        UIView m_ShopView;  // ���/��ʯ/ҩˮ���̵���Ļ
        UIView m_MailView;  // �ʼ���Ļ

        // ������Ļ
        UIView m_InventoryView;  // ��ɫ��Ļ�Ŀ��
        UIView m_SettingsView;  // ���õĸ�����Ļ

        // ������
        UIView m_OptionsBarView;  // ���ٷ��ʽ��/��ʯ������
        UIView m_MenuBarView;  // �˵���Ļ�ĵ�����
        UIView m_LevelMeterView;  // ��ʾ�ܽ�ɫ���ȵľ��������

        // UIView �� VisualTree �ַ��� ID��ÿ����������һ����֧
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

            // ����ҳ��Ļ��ʼ
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

            // ����ȫ��ģ̬��ͼ��HomeView��CharView��InfoView��ShopView��MailView
            m_HomeView = new HomeView(root.Q<VisualElement>(k_HomeViewName)); // ��½ģ̬��Ļ
            m_CharView = new CharView(root.Q<VisualElement>(k_CharViewName)); // ��ɫ��Ļ
            m_InfoView = new InfoView(root.Q<VisualElement>(k_InfoViewName)); // ���Ӻ���Դ��Ļ
            m_ShopView = new ShopView(root.Q<VisualElement>(k_ShopViewName)); // �̵���Ļ
            m_MailView = new MailView(root.Q<VisualElement>(k_MailViewName)); // �ʼ���Ļ

            // ������ͼ�����б����ĵ���ģ̬��
            m_InventoryView = new InventoryView(root.Q<VisualElement>(k_InventoryViewName));  // װ�����ǲ�
            m_SettingsView = new SettingsView(root.Q<VisualElement>(k_SettingsViewName)); // ��Ϸ���ø��ǲ�

            // ������ 
            LevelMeterData meterData = CharEvents.GetLevelMeterData.Invoke();
            m_LevelMeterView = new LevelMeterView(root.Q<VisualElement>(k_LevelMeterViewName), meterData); // ����ȼ�������
            m_LevelMeterView.Initialize();

            m_OptionsBarView = new OptionsBarView(root.Q<VisualElement>(k_OptionsBarViewName)); // ����/�̵깤����
            m_MenuBarView = new MenuBarView(root.Q<VisualElement>(k_MenuBarViewName)); // ��Ļѡ�񹤾���

            // ���б��и���ģ̬ UI ��ͼ�Խ��д��� 
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

            // Ĭ�����õ� UI ��ͼ
            m_HomeView.Show();
            m_OptionsBarView.Show();
            m_MenuBarView.Show();
            m_LevelMeterView.Show();
        }

        // �л�ģ̬��Ļ�Ŀ�/��
        void ShowModalView(UIView newView)
        {
            if (m_CurrentView != null)
                m_CurrentView.Hide();

            m_PreviousView = m_CurrentView;
            m_CurrentView = newView;

            // ��ʾ��Ļ��֪ͨ�κμ��������˵��Ѹ���

            if (m_CurrentView != null)
            {
                m_CurrentView.Show();
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }

        // ģ̬��Ļ������ 
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

        // ������Ļ��������Щ��ģ̬ UIView�������ж�ǰһ����Ļ�����á�

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
            // ���ؿ����Ļ
            m_InventoryView.Hide();

            // ����ǰ��Ļ����Ϊǰһ����Ļ
            if (m_PreviousView != null)
            {
                m_PreviousView.Show();
                m_CurrentView = m_PreviousView;
                MainMenuUIEvents.CurrentViewChanged?.Invoke(m_CurrentView.GetType().Name);
            }
        }
    }
}