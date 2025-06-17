using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{

    // �ı���: https://docs.unity3d.com/2021.2/Documentation/Manual/UIE-create-tabbed-menu-for-runtime.html

    public class TabbedMenuController
    {
        // �¼�������֪ͨ��������
        public static event Action TabSelected;

        // UI�Ļ���VisualElement������MailScreen��CharScreen��ShopScreen��
        readonly VisualElement m_Root;

        // ���ڲ�ѯVisualElements���ַ���
        readonly TabbedMenuIDs m_IDs;

        // ��ʼ����Visual Element�Ա����ã���MonoBehaviour�Ĺ��캯����
        public TabbedMenuController(VisualElement root, TabbedMenuIDs ids)
        {
            m_Root = root;
            m_IDs = ids;
        }

        // Ϊ��ǩ��ť���õ���¼�
        public void RegisterTabCallbacks()
        {
            // ʶ��ÿ����ǩ 
            UQueryBuilder<VisualElement> tabs = GetAllTabs();

            // Ϊÿ��Visual Elementע��ClickTab�¼��������
            tabs.ForEach(
                (t) =>
                {
                    t.RegisterCallback<ClickEvent>(OnTabClick);
                });
        }

        // �������¼�
        void OnTabClick(ClickEvent evt)
        {
            VisualElement clickedTab = evt.currentTarget as VisualElement;

            // �������ı�ǩδ��ѡ�У���ѡ����ȷ�ı�ǩ
            if (!IsTabSelected(clickedTab))
            {
                // ȡ��ѡ��ǰ���������ǩ
                GetAllTabs().Where(
                    (tab) => tab != clickedTab && IsTabSelected(tab)
                    ).ForEach(UnselectTab);

                // ѡ�����ı�ǩ
                SelectTab(clickedTab);
                AudioManager.PlayDefaultButtonSound();
            }
        }

        // ���ظ�����ǩ��Ӧ������Ԫ��
        VisualElement FindContent(VisualElement tab)
        {
            return m_Root.Q(GetContentName(tab));
        }

        // ���ض�Ӧ���������ƣ�����name1-tab��Ӧ��name1-content
        string GetContentName(VisualElement tab)
        {
            return tab.name.Replace(m_IDs.tabNameSuffix, m_IDs.contentNameSuffix);
        }

        // ��λ���о��б�ǩ������VisualElements
        UQueryBuilder<VisualElement> GetAllTabs()
        {
            return m_Root.Query<VisualElement>(className: m_IDs.tabClassName);
        }

        // ��λ�ض�Visual Element��������Ļ���ϵĵ�һ����ǩ
        public VisualElement GetFirstTab(VisualElement visualElement)
        {
            return visualElement.Query<VisualElement>(className: m_IDs.tabClassName).First();
        }

        public bool IsTabSelected(string tabName)
        {
            VisualElement tabElement = m_Root.Query<VisualElement>(className: m_IDs.tabClassName, name: tabName);
            return IsTabSelected(tabElement);
        }

        public bool IsTabSelected(VisualElement tab)
        {
            return tab.ClassListContains(m_IDs.selectedTabClassName);
        }

        void UnselectOtherTabs(VisualElement tab)
        {
            GetAllTabs().Where(
                (t) => t != tab && IsTabSelected(t)).
                ForEach(UnselectTab);
        }

        // ������ѡ���ڴ���Ļ���ض���ǩʱʹ�ã�
        public void SelectTab(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                return;
            }

            VisualElement namedTab = m_Root.Query<VisualElement>(tabName, className: m_IDs.tabClassName);

            if (namedTab == null)
            {
                Debug.Log("TabbedMenuController.SelectTab: ָ���ı�ǩ��Ч");
                return;
            }

            UnselectOtherTabs(namedTab);
            SelectTab(namedTab);
        }

        // ѡ������ı�ǩ���ҵ���Ӧ�����ݲ���ʾ����
        void SelectTab(VisualElement tab)
        {
            // ͻ����ʾ��ǩ
            tab.AddToClassList(m_IDs.selectedTabClassName);

            // ȡ����������
            VisualElement content = FindContent(tab);
            content.RemoveFromClassList(m_IDs.unselectedContentClassName);

            // ֪ͨ�������� 
            TabSelected?.Invoke();
        }

        // ѡ�����Visual Element��������Ļ���ĵ�һ����ǩ
        public void SelectFirstTab(VisualElement visualElement)
        {
            VisualElement firstTab = GetFirstTab(visualElement);

            if (firstTab != null)
            {
                // ȡ��ѡ��ǰ���������ǩ
                GetAllTabs().Where(
                    (tab) => tab != firstTab && IsTabSelected(tab)
                    ).ForEach(UnselectTab);

                // ��ѡ���һ����ǩ
                SelectTab(firstTab);
            }
        }

        // ȡ��ѡ���ض��ı�ǩ���ҵ���Ӧ�����ݲ���������
        void UnselectTab(VisualElement tab)
        {
            // ȡ��ͻ����ʾ
            tab.RemoveFromClassList(m_IDs.selectedTabClassName);

            // ���ض�Ӧ������
            VisualElement content = FindContent(tab);
            content.AddToClassList(m_IDs.unselectedContentClassName);
        }
    }
}