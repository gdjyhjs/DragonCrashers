using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// ���ݿ���Ŀ�Ľӿڶ��壬���пɱ����ݿ����Ķ�����ʵ�ִ˽ӿ�
    /// </summary>
    public interface IDatabaseEntry
    {
        // ��Ŀ��Ψһ��ʶ��
        string Key { get; }
    }

    /// <summary>
    /// ����һ�����࣬���ڶ��彫����/�ַ���ID���ӵ�������������ݿ⡣
    /// �����ڽ���Ʒ����ID������������Ա����ǿ���ͨ��ID������Ʒ�������ڶ�ȡ�浵ʱ����
    /// �й���δ�����Щ���ݿ��ʾ������μ�ItemDatabase��CropDatabase��
    /// </summary>
    /// <typeparam name="T">ʵ��IDatabaseEntry�ӿڵ���Ŀ����</typeparam>
    public abstract class BaseDatabase<T> : ScriptableObject where T : class, IDatabaseEntry
    {
        // ���ݿ��е�������Ŀ�б�
        [SerializeReference]
        public List<T> Entries;

        // ���ڿ��ٲ�����Ŀ���ֵ�
        private Dictionary<string, T> m_LookupDictionnary;

        /// <summary>
        /// ͨ��ΨһID�����ݿ��л�ȡ��Ŀ
        /// </summary>
        /// <param name="uniqueID">Ҫ���ҵ���Ŀ��ΨһID</param>
        /// <returns>�ҵ�����Ŀ�����δ�ҵ��򷵻�null</returns>
        public T GetFromID(string uniqueID)
        {
            if (m_LookupDictionnary.TryGetValue(uniqueID, out var entry))
            {
                return entry;
            }

            return null;
        }

        /// <summary>
        /// ��ʼ�����ݿ⣬�ؽ������ֵ�
        /// ������ʹ�ô����ݿ�Ĵ�����ã���ȷ�������ֵ���ȷ����
        /// ���޷�ʹ��OnAfterDeserialize����Ϊ�޷����Ʒ����л�˳��
        /// </summary>
        public void Init()
        {
            m_LookupDictionnary = new Dictionary<string, T>();

            // �ؽ������ֵ�
            foreach (var entry in Entries)
            {
                if (entry == null)
                {
                    continue;
                }

                // ʹ��TryAdd�����ظ���������ק�������ܵ����ظ���Ŀ��
                m_LookupDictionnary.TryAdd(entry.Key, entry);
            }
        }
    }
}