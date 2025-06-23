using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 数据库条目的接口定义，所有可被数据库管理的对象需实现此接口
    /// </summary>
    public interface IDatabaseEntry
    {
        // 条目的唯一标识键
        string Key { get; }
    }

    /// <summary>
    /// 这是一个基类，用于定义将名称/字符串ID链接到给定对象的数据库。
    /// 适用于将物品与其ID关联的情况，以便我们可以通过ID检索物品（例如在读取存档时）。
    /// 有关如何创建这些数据库的示例，请参见ItemDatabase和CropDatabase。
    /// </summary>
    /// <typeparam name="T">实现IDatabaseEntry接口的条目类型</typeparam>
    public abstract class BaseDatabase<T> : ScriptableObject where T : class, IDatabaseEntry
    {
        // 数据库中的所有条目列表
        [SerializeReference]
        public List<T> Entries;

        // 用于快速查找条目的字典
        private Dictionary<string, T> m_LookupDictionnary;

        /// <summary>
        /// 通过唯一ID从数据库中获取条目
        /// </summary>
        /// <param name="uniqueID">要查找的条目的唯一ID</param>
        /// <returns>找到的条目，如果未找到则返回null</returns>
        public T GetFromID(string uniqueID)
        {
            if (m_LookupDictionnary.TryGetValue(uniqueID, out var entry))
            {
                return entry;
            }

            return null;
        }

        /// <summary>
        /// 初始化数据库，重建查找字典
        /// 必须由使用此数据库的代码调用，以确保查找字典正确构建
        /// （无法使用OnAfterDeserialize，因为无法控制反序列化顺序）
        /// </summary>
        public void Init()
        {
            m_LookupDictionnary = new Dictionary<string, T>();

            // 重建查找字典
            foreach (var entry in Entries)
            {
                if (entry == null)
                {
                    continue;
                }

                // 使用TryAdd避免重复键错误（拖拽操作可能导致重复条目）
                m_LookupDictionnary.TryAdd(entry.Key, entry);
            }
        }
    }
}