using System.Collections;
using System.Collections.Generic;
using Template2DCommon;
using UnityEngine;
using UnityEngine.UIElements;
namespace HappyHarvest
{
    /// <summary>
    /// 处理仓库 UI，负责物品的存储和取回操作
    /// </summary>
    public class WarehouseUI
    {
        private VisualElement m_Root; // 仓库 UI 根元素

        private VisualTreeAsset m_EntryTemplate; // 条目模板

        private Button m_StoreButton; // 存储按钮
        private Button m_RetrieveButton; // 取回按钮

        private ScrollView m_Scrollview; // 内容滚动视图

        /// <summary>
        /// 初始化仓库 UI
        /// </summary>
        /// <param name="root">UI 根元素</param>
        /// <param name="entryTemplate">条目模板</param>
        public WarehouseUI(VisualElement root, VisualTreeAsset entryTemplate)
        {
            m_Root = root;

            m_EntryTemplate = entryTemplate;

            // 获取存储按钮并添加点击事件
            m_StoreButton = m_Root.Q<Button>("StoreButton");
            m_StoreButton.clicked += OpenStore;

            // 获取取回按钮并添加点击事件
            m_RetrieveButton = m_Root.Q<Button>("RetrieveButton");
            m_RetrieveButton.clicked += OpenRetrieve;

            // 获取关闭按钮并添加点击事件
            m_Root.Q<Button>("CloseButton").clicked += Close;

            m_Scrollview = m_Root.Q<ScrollView>("ContentScrollView");
        }

        /// <summary>
        /// 打开仓库 UI
        /// </summary>
        public void Open()
        {
            m_Root.visible = true;
            GameManager.Instance.Pause();
            SoundManager.Instance.PlayUISound();
        }

        /// <summary>
        /// 关闭仓库 UI
        /// </summary>
        public void Close()
        {
            m_Root.visible = false;
            GameManager.Instance.Resume();
        }

        /// <summary>
        /// 打开物品存储界面
        /// </summary>
        void OpenStore()
        {
            // 更新按钮状态
            m_StoreButton.AddToClassList("activeButton");
            m_RetrieveButton.RemoveFromClassList("activeButton");

            m_StoreButton.SetEnabled(false);
            m_RetrieveButton.SetEnabled(true);

            // 清空滚动视图
            m_Scrollview.contentContainer.Clear();

            var player = GameManager.Instance.Player;

            // 生成存储条目
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
                    // 执行存储操作
                    GameManager.Instance.Storage.Store(invEntry);
                    player.Inventory.Remove(i1, invEntry.StackSize);
                    m_Scrollview.contentContainer.Remove(entry);
                };

                button.text = "存储";

                m_Scrollview.contentContainer.Add(entry);
            }
        }

        /// <summary>
        /// 打开物品取回界面
        /// </summary>
        void OpenRetrieve()
        {
            // 更新按钮状态
            m_RetrieveButton.AddToClassList("activeButton");
            m_StoreButton.RemoveFromClassList("activeButton");

            m_RetrieveButton.SetEnabled(false);
            m_StoreButton.SetEnabled(true);

            // 清空滚动视图
            m_Scrollview.contentContainer.Clear();

            var storage = GameManager.Instance.Storage;
            var inventory = GameManager.Instance.Player.Inventory;

            // 生成取回条目
            for (var i = 0; i < storage.Content.Count; ++i)
            {
                var entry = storage.Content[i];

                // 跳过数量为 0 的条目
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
                    // 计算可取回的数量
                    var retrieveAmount = Mathf.Min(entry.StackSize, entry.Item.MaxStackSize);
                    var existing = inventory.GetIndexOfItem(entry.Item, true);
                    if (existing != -1)
                    {// 优先填充已有同类型物品的格子
                        retrieveAmount -= inventory.Entries[existing].StackSize;
                    }

                    if (retrieveAmount > 0)
                    {
                        // 执行取回操作
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

                        // 更新所有剩余条目的状态
                        foreach (var child in m_Scrollview.contentContainer.Children())
                        {
                            var entryItem = child.userData as Item;
                            var entryButton = child.Q<Button>("ActionButton");

                            if (inventory.GetMaximumAmountFit(entryItem) == 0)
                            {
                                entryButton.text = "背包已满";
                                entryButton.SetEnabled(false);
                            }
                        }
                    }
                };

                button.text = "取回";

                // 如果背包无法容纳至少 1 个该物品，则禁用按钮
                if (!inventory.CanFitItem(entry.Item, 1))
                {
                    button.text = "背包已满";
                    button.SetEnabled(false);
                }

                m_Scrollview.Add(retrieveEntry);
            }
        }
    }
}
