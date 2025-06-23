using System;
using System.Collections.Generic;
using HappyHarvest;
using Template2DCommon;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace HappyHarvest
{
    /// <summary>
    /// �����Ʒ����ϵͳ��������Ʒ�Ĵ洢��ʹ�ú�װ���ȹ���
    /// Ŀǰ�̶���СΪ 9 ��
    /// </summary>
    [Serializable]
    public class InventorySystem
    {
        // �����������
        public const int InventorySize = 9;

        /// <summary>
        /// �����Ŀ�ṹ��������Ʒ�Ͷѵ�����
        /// </summary>
        [Serializable]
        public class InventoryEntry
        {
            public Item Item; // ��Ʒ����
            public int StackSize; // �ѵ�����
        }

        // ��ǰװ������Ʒ����
        public int EquippedItemIdx { get; private set; }
        // ��ǰװ������Ʒ����
        public Item EquippedItem => Entries[EquippedItemIdx].Item;

        // �����Ŀ����
        public InventoryEntry[] Entries = new InventoryEntry[InventorySize];

        /// <summary>
        /// ��ʼ�����ϵͳ
        /// </summary>
        public void Init()
        {
            EquippedItemIdx = 0;
        }

        /// <summary>
        /// ʹ����װ������Ʒ
        /// </summary>
        /// <param name="target">Ŀ��λ��</param>
        /// <returns>ʹ���Ƿ�ɹ�</returns>
        public bool UseEquippedObject(Vector3Int target)
        {
            // ����Ƿ���װ������Ʒ�ҿ���Ŀ��λ��ʹ��
            if (EquippedItem == null || !EquippedItem.CanUse(target))
                return false;

            bool used = EquippedItem.Use(target);

            if (used)
            {
                // ������Ʒʹ����Ч
                if (EquippedItem.UseSound != null && EquippedItem.UseSound.Length > 0)
                {
                    SoundManager.Instance.PlaySFXAt(GameManager.Instance.Player.transform.position,
                    EquippedItem.UseSound[Random.Range(0, EquippedItem.UseSound.Length)], false);
                }

                // ����Ʒʹ�ú���ٶѵ�����
                if (EquippedItem.Consumable)
                {
                    Entries[EquippedItemIdx].StackSize -= 1;

                    if (Entries[EquippedItemIdx].StackSize == 0)
                    {
                        Entries[EquippedItemIdx].Item = null;
                    }

                    // ���¿�� UI
                    UIHandler.UpdateInventory(this);
                }
            }

            return used;
        }

        /// <summary>
        /// ������Ƿ�������ָ����������Ʒ
        /// </summary>
        /// <param name="newItem">Ҫ��ӵ���Ʒ</param>
        /// <param name="amount">Ҫ��ӵ�����</param>
        /// <returns>�Ƿ������</returns>
        public bool CanFitItem(Item newItem, int amount)
        {
            int toFit = amount;

            // �������ͬ������Ʒ�ĸ���
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == newItem)
                {
                    int size = newItem.MaxStackSize - Entries[i].StackSize;
                    toFit -= size;

                    if (toFit <= 0)
                        return true;
                }
            }

            // ���ո���
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == null)
                {
                    toFit -= newItem.MaxStackSize;
                    if (toFit <= 0)
                        return true;
                }
            }

            return toFit == 0;
        }

        /// <summary>
        /// ��ȡ��������ָ����Ʒ���������
        /// </summary>
        /// <param name="item">��Ʒ����</param>
        /// <returns>����������</returns>
        public int GetMaximumAmountFit(Item item)
        {
            int canFit = 0;
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == null)
                {
                    canFit += item.MaxStackSize;
                }
                else if (Entries[i].Item == item)
                {
                    canFit += item.MaxStackSize - Entries[i].StackSize;
                }
            }

            return canFit;
        }

        /// <summary>
        /// ��ȡָ����Ʒ�ڿ���е�����
        /// </summary>
        /// <param name="item">��Ʒ����</param>
        /// <param name="returnOnlyNotFull">�Ƿ�ֻ����δ���ĸ���</param>
        /// <returns>��Ʒ������δ�ҵ����� - 1</returns>
        public int GetIndexOfItem(Item item, bool returnOnlyNotFull)
        {
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == item &&
                (!returnOnlyNotFull || Entries[i].StackSize != Entries[i].Item.MaxStackSize))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// ���������Ʒ
        /// </summary>
        /// <param name="newItem">Ҫ��ӵ���Ʒ</param>
        /// <param name="amount">Ҫ��ӵ�������Ĭ�� 1</param>
        /// <returns>����Ƿ�ɹ�</returns>
        public bool AddItem(Item newItem, int amount = 1)
        {
            int remainingToFit = amount;

            // �ȼ������ͬ������Ʒ�ĸ���
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == newItem && Entries[i].StackSize < newItem.MaxStackSize)
                {
                    int fit = Mathf.Min(newItem.MaxStackSize - Entries[i].StackSize, remainingToFit);
                    Entries[i].StackSize += fit;
                    remainingToFit -= fit;
                    UIHandler.UpdateInventory(this);

                    if (remainingToFit == 0)
                        return true;
                }
            }

            // ���ո���
            for (int i = 0; i < InventorySize; ++i)
            {
                if (Entries[i].Item == null)
                {
                    Entries[i].Item = newItem;
                    int fit = Mathf.Min(newItem.MaxStackSize - Entries[i].StackSize, remainingToFit);
                    remainingToFit -= fit;
                    Entries[i].StackSize = fit;

                    UIHandler.UpdateInventory(this);

                    if (remainingToFit == 0)
                        return true;
                }
            }

            // ����������޷����
            return remainingToFit == 0;
        }

        /// <summary>
        /// �ӿ���Ƴ���Ʒ
        /// </summary>
        /// <param name="index">��������</param>
        /// <param name="count">Ҫ�Ƴ�������</param>
        /// <returns>ʵ���Ƴ�������</returns>
        public int Remove(int index, int count)
        {
            if (index < 0 || index >= Entries.Length)
                return 0;

            int amount = Mathf.Min(count, Entries[index].StackSize);

            Entries[index].StackSize -= amount;

            if (Entries[index].StackSize == 0)
            {
                Entries[index].Item = null;
            }

            UIHandler.UpdateInventory(this);
            return amount;
        }

        /// <summary>
        /// װ����һ����Ʒ
        /// </summary>
        public void EquipNext()
        {
            EquippedItemIdx += 1;
            if (EquippedItemIdx >= InventorySize) EquippedItemIdx = 0;

            UIHandler.UpdateInventory(this);
        }

        /// <summary>
        /// װ����һ����Ʒ
        /// </summary>
        public void EquipPrev()
        {
            EquippedItemIdx -= 1;
            if (EquippedItemIdx < 0) EquippedItemIdx = InventorySize - 1;

            UIHandler.UpdateInventory(this);
        }

        /// <summary>
        /// װ��ָ����������Ʒ
        /// </summary>
        /// <param name="index">��Ʒ����</param>
        public void EquipItem(int index)
        {
            if (index < 0 || index >= Entries.Length)
                return;

            EquippedItemIdx = index;
            UIHandler.UpdateInventory(this);
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="data">���������б�</param>
        public void Save(ref List<InventorySaveData> data)
        {
            foreach (var entry in Entries)
            {
                if (entry.Item != null)
                {
                    data.Add(new InventorySaveData()
                    {
                        Amount = entry.StackSize,
                        ItemID = entry.Item.UniqueID
                    });
                }
                else
                {
                    data.Add(null);
                }
            }
        }

        /// <summary>
        /// ���ؿ������
        /// </summary>
        /// <param name="data">���������б�</param>
        public void Load(List<InventorySaveData> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                if (data[i] != null)
                {
                    Entries[i].Item = GameManager.Instance.ItemDatabase.GetFromID(data[i].ItemID);
                    Entries[i].StackSize = data[i].Amount;
                }
                else
                {
                    Entries[i].Item = null;
                    Entries[i].StackSize = 0;
                }
            }
        }
    }

    /// <summary>
    /// ��汣�����ݽṹ
    /// </summary>
    [Serializable]
    public class InventorySaveData
    {
        public int Amount; // ��Ʒ����
        public string ItemID; // ��ƷΨһ��ʶ
    }

#if UNITY_EDITOR

    /// <summary>
    /// ���ϵͳ���Գ��루�༭����չ��
    /// </summary>
    [CustomPropertyDrawer(typeof(InventorySystem))]
    public class InventoryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            container.Add(new Label("��ʼ���"));

            ListView list = new ListView();
            list.showBoundCollectionSize = false;
            list.bindingPath = nameof(InventorySystem.Entries);
            list.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            list.style.flexGrow = 1;
            list.reorderable = true;
            list.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            list.showBorder = true;

            container.Add(list);

            return container;
        }
    }

    /// <summary>
    /// �����Ŀ���Գ��루�༭����չ��
    /// </summary>
    [CustomPropertyDrawer(typeof(InventorySystem.InventoryEntry))]
    public class InventoryEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var itemProperty = property.FindPropertyRelative(nameof(InventorySystem.InventoryEntry.Item));
            var stackProperty = property.FindPropertyRelative(nameof(InventorySystem.InventoryEntry.StackSize));

            container.style.flexDirection = FlexDirection.Row;

            var itemLabel = new Label($"��Ʒ :");
            itemLabel.style.width = Length.Percent(10);
            var itemField = new PropertyField(itemProperty, "");
            itemField.style.width = Length.Percent(40);
            var stackSizeLabel = new Label("���� :");
            stackSizeLabel.style.width = Length.Percent(10);
            var stackField = new PropertyField(stackProperty, "");
            stackField.style.width = Length.Percent(40);

            container.Add(itemLabel);
            container.Add(itemField);
            container.Add(stackSizeLabel);
            container.Add(stackField);

            return container;
        }
    }

#endif
}