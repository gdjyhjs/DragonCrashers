using System.Collections;
using System.Collections.Generic;
using Template2DCommon;
using UnityEngine;
using UnityEngine.UIElements;
namespace HappyHarvest
{
    /// <summary>
    /// ����ֿ� UI��������Ʒ�Ĵ洢��ȡ�ز���
    /// </summary>
    public class WarehouseUI
    {
        private VisualElement m_Root; // �ֿ� UI ��Ԫ��

        private VisualTreeAsset m_EntryTemplate; // ��Ŀģ��

        private Button m_StoreButton; // �洢��ť
        private Button m_RetrieveButton; // ȡ�ذ�ť

        private ScrollView m_Scrollview; // ���ݹ�����ͼ

        /// <summary>
        /// ��ʼ���ֿ� UI
        /// </summary>
        /// <param name="root">UI ��Ԫ��</param>
        /// <param name="entryTemplate">��Ŀģ��</param>
        public WarehouseUI(VisualElement root, VisualTreeAsset entryTemplate)
        {
            m_Root = root;

            m_EntryTemplate = entryTemplate;

            // ��ȡ�洢��ť����ӵ���¼�
            m_StoreButton = m_Root.Q<Button>("StoreButton");
            m_StoreButton.clicked += OpenStore;

            // ��ȡȡ�ذ�ť����ӵ���¼�
            m_RetrieveButton = m_Root.Q<Button>("RetrieveButton");
            m_RetrieveButton.clicked += OpenRetrieve;

            // ��ȡ�رհ�ť����ӵ���¼�
            m_Root.Q<Button>("CloseButton").clicked += Close;

            m_Scrollview = m_Root.Q<ScrollView>("ContentScrollView");
        }

        /// <summary>
        /// �򿪲ֿ� UI
        /// </summary>
        public void Open()
        {
            m_Root.visible = true;
            GameManager.Instance.Pause();
            SoundManager.Instance.PlayUISound();
        }

        /// <summary>
        /// �رղֿ� UI
        /// </summary>
        public void Close()
        {
            m_Root.visible = false;
            GameManager.Instance.Resume();
        }

        /// <summary>
        /// ����Ʒ�洢����
        /// </summary>
        void OpenStore()
        {
            // ���°�ť״̬
            m_StoreButton.AddToClassList("activeButton");
            m_RetrieveButton.RemoveFromClassList("activeButton");

            m_StoreButton.SetEnabled(false);
            m_RetrieveButton.SetEnabled(true);

            // ��չ�����ͼ
            m_Scrollview.contentContainer.Clear();

            var player = GameManager.Instance.Player;

            // ���ɴ洢��Ŀ
            for (var i = 0; i < player.Inventory.Entries.Length; ++i)
            {
                var invEntry = player.Inventory.Entries[i];
                var item = invEntry.Item;

                if (item == null) continue;

                var entry = m_EntryTemplate.CloneTree();
                entry.Q<Label>("ItemName").text = item.DisplayName;
                entry.Q<VisualElement>("ItemIcone").style.backgroundImage = new StyleBackground(item.ItemSprite);

                var button = entry.Q<Button>("ActionButton");
                var i1 = i;
                button.clicked += () =>
                {
                    // ִ�д洢����
                    GameManager.Instance.Storage.Store(invEntry);
                    player.Inventory.Remove(i1, invEntry.StackSize);
                    m_Scrollview.contentContainer.Remove(entry);
                };

                button.text = "�洢";

                m_Scrollview.contentContainer.Add(entry);
            }
        }

        /// <summary>
        /// ����Ʒȡ�ؽ���
        /// </summary>
        void OpenRetrieve()
        {
            // ���°�ť״̬
            m_RetrieveButton.AddToClassList("activeButton");
            m_StoreButton.RemoveFromClassList("activeButton");

            m_RetrieveButton.SetEnabled(false);
            m_StoreButton.SetEnabled(true);

            // ��չ�����ͼ
            m_Scrollview.contentContainer.Clear();

            var storage = GameManager.Instance.Storage;
            var inventory = GameManager.Instance.Player.Inventory;

            // ����ȡ����Ŀ
            for (var i = 0; i < storage.Content.Count; ++i)
            {
                var entry = storage.Content[i];

                // ��������Ϊ 0 ����Ŀ
                if (entry.StackSize == 0)
                    continue;

                var retrieveEntry = m_EntryTemplate.CloneTree();
                retrieveEntry.userData = entry.Item;

                var itemLabel = retrieveEntry.Q<Label>("ItemName");
                itemLabel.text = entry.Item.DisplayName + $"(x{entry.StackSize})";
                retrieveEntry.Q<VisualElement>("ItemIcone").style.backgroundImage = new StyleBackground(entry.Item.ItemSprite);

                var button = retrieveEntry.Q<Button>("ActionButton");
                var i1 = i;
                button.clicked += () =>
                {
                    // �����ȡ�ص�����
                    var retrieveAmount = Mathf.Min(entry.StackSize, entry.Item.MaxStackSize);
                    var existing = inventory.GetIndexOfItem(entry.Item, true);
                    if (existing != -1)
                    {// �����������ͬ������Ʒ�ĸ���
                        retrieveAmount -= inventory.Entries[existing].StackSize;
                    }

                    if (retrieveAmount > 0)
                    {
                        // ִ��ȡ�ز���
                        GameManager.Instance.Storage.Retrieve(i1, retrieveAmount);
                        inventory.AddItem(entry.Item, retrieveAmount);

                        if (GameManager.Instance.Storage.Content[i1].StackSize == 0)
                        {
                            m_Scrollview.Remove(retrieveEntry);
                        }
                        else
                        {
                            itemLabel.text = entry.Item.DisplayName + $"(x{entry.StackSize})";
                        }

                        // ��������ʣ����Ŀ��״̬
                        foreach (var child in m_Scrollview.contentContainer.Children())
                        {
                            var entryItem = child.userData as Item;
                            var entryButton = child.Q<Button>("ActionButton");

                            if (inventory.GetMaximumAmountFit(entryItem) == 0)
                            {
                                entryButton.text = "��������";
                                entryButton.SetEnabled(false);
                            }
                        }
                    }
                };

                button.text = "ȡ��";

                // ��������޷��������� 1 ������Ʒ������ð�ť
                if (!inventory.CanFitItem(entry.Item, 1))
                {
                    button.text = "��������";
                    button.SetEnabled(false);
                }

                m_Scrollview.Add(retrieveEntry);
            }
        }
    }
}
