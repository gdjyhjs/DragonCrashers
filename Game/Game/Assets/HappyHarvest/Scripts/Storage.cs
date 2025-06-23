using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// �ֿ�洢ϵͳ�����ڹ�����Ʒ�Ĵ洢��ȡ�ز���
    /// </summary>
    public class Storage
    {
        // �洢����Ʒ��Ŀ�б�
        public List<InventorySystem.InventoryEntry> Content { get; private set; }

        /// <summary>
        /// ��ʼ���ֿ�洢ϵͳ
        /// </summary>
        public Storage()
        {
            Content = new List<InventorySystem.InventoryEntry>();
        }

        /// <summary>
        /// ��ֿ�洢��Ʒ
        /// </summary>
        /// <param name="entry">Ҫ�洢����Ʒ��Ŀ</param>
        public void Store(InventorySystem.InventoryEntry entry)
        {
            // �����Ѵ洢����ͬ��Ʒ��ͨ��Ψһ��ƥ�䣩
            var idx = Content.FindIndex(inventoryEntry => inventoryEntry.Item.Key == entry.Item.Key);
            if (idx != -1)
            {
                // �ҵ���ͬ��Ʒ���������
                Content[idx].StackSize += entry.StackSize;
            }
            else
            {
                // δ�ҵ����������Ŀ
                Content.Add(new InventorySystem.InventoryEntry()
                {
                    Item = entry.Item,
                    StackSize = entry.StackSize
                });
            }
        }

        /// <summary>
        /// �Ӳֿ�ȡ����Ʒ
        /// </summary>
        /// <param name="contentIndex">��Ʒ����</param>
        /// <param name="amount">Ҫȡ�ص�����</param>
        /// <returns>ʵ��ȡ�ص�����</returns>
        public int Retrieve(int contentIndex, int amount)
        {
            Debug.Assert(contentIndex < Content.Count, "���ԴӲֿ�ȡ�ز����ڵ���Ʒ��Ŀ");

            // ����ʵ�ʿ�ȡ�ص����������������������
            int actualAmount = Mathf.Min(amount, Content[contentIndex].StackSize);

            // ���¿������
            Content[contentIndex].StackSize -= actualAmount;

            return actualAmount;
        }
    }
}
