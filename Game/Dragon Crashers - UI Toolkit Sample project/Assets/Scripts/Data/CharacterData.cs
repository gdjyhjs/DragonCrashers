using System;
using Unity.Properties;
using UnityEngine;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // 存储角色实例的数据 + 来自ScriptableObject的静态数据

    /// <summary>
    /// 管理角色的动态数据，包括装备的装备和当前等级。
    /// 使用运行时数据绑定系统通知UI元素属性的更改。
    /// 计算升级所需的经验值、下一级所需的药剂数量和当前的实力。
    /// </summary>
    public class CharacterData : MonoBehaviour, INotifyBindablePropertyChanged
    {
        // 随着等级提升，经验值需求增长的速度
        const float k_ProgressionFactor = 10f;

        /// <summary>
        /// 获取角色装备的装备物品数组。
        /// </summary>
        [SerializeField] EquipmentSO[] m_GearData = new EquipmentSO[4];

        /// <summary>
        /// 从ScriptableObject获取角色的静态数据。
        /// </summary>
        [SerializeField] CharacterBaseSO m_CharacterBaseData;

        [SerializeField] int m_CurrentLevel;

        /// <summary>
        /// 获取或设置角色的当前等级，并在更改时通知绑定系统。
        /// </summary>
        [CreateProperty]
        public int CurrentLevel
        {
            get => m_CurrentLevel;
            set
            {
                if (m_CurrentLevel == value)
                    return;

                m_CurrentLevel = value;

                Notify();

                // 当CurrentLevel更改时，通知CurrentPower和PotionsForNextLevel也需要更新。
                Notify(nameof(CurrentPower));
                Notify(nameof(PotionsForNextLevel));
            }
        }

        /// <summary>
        /// 计算角色的当前实力等级，基于等级和基础属性。
        /// 当CurrentLevel更改时更新。
        /// </summary>
        [CreateProperty]
        public int CurrentPower
        {
            get
            {
                float basePoints = m_CharacterBaseData.TotalBasePoints;

                // 这里可以选择添加逻辑以包含角色的装备
                float equipmentPoints = 0f;

                return (int)(CurrentLevel * basePoints + equipmentPoints) / 10;
            }
        }

        /// <summary>
        /// 从CharacterBaseData获取角色名称。
        /// </summary>
        [CreateProperty]
        public string CharacterName => m_CharacterBaseData.CharacterName;

        /// <summary>
        /// 计算下一级所需的药剂数量并返回格式化的字符串。
        /// </summary>
        [CreateProperty]
        public string PotionsForNextLevel => "/" + GetPotionsForNextLevel(CurrentLevel, k_ProgressionFactor);

        /// <summary>
        /// 当可绑定属性更改时引发的事件（实现INotifyBindablePropertyChanged所需）
        /// </summary>
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        /// <summary>
        /// 通知监听器某个属性已更改。 
        /// </summary>
        /// <param name="property">已更改的属性名称</param>
        void Notify([CallerMemberName] string property = "")
        {
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        /// <summary>
        /// 获取或设置角色视觉表示的预览实例。
        /// </summary>
        public GameObject PreviewInstance { get; set; }

        /// <summary>
        /// 从ScriptableObject获取角色的静态数据。
        /// </summary>
        public CharacterBaseSO CharacterBaseData => m_CharacterBaseData;

        /// <summary>
        /// 获取角色装备的装备物品数组。
        /// </summary>
        public EquipmentSO[] GearData => m_GearData;

        /// <summary>
        /// 计算升至下一级所需的升级药剂数量，以无符号整数表示。
        /// </summary>
        /// <returns>升至下一级所需的经验值数量。</returns>
        public uint GetPotionsForNextLevel()
        {
            return (uint)GetPotionsForNextLevel(m_CurrentLevel, k_ProgressionFactor);
        }

        /// <summary>
        /// 计算角色升级所需的药剂数量。
        /// </summary>
        /// <param name="currentLevel">角色的当前等级。</param>
        /// <param name="progressionFactor">用于计算的进度因子。</param>
        /// <returns>升至下一级所需的药剂数量。</returns>
        int GetPotionsForNextLevel(int currentLevel, float progressionFactor)
        {
            currentLevel = Mathf.Clamp(currentLevel, 1, currentLevel);
            progressionFactor = Mathf.Clamp(progressionFactor, 1f, progressionFactor);

            float xp = (progressionFactor * (currentLevel));
            xp = Mathf.Ceil((float)xp);
            return (int)xp;
        }

        /// <summary>
        /// 将角色的等级提升1级。
        /// </summary>
        public void IncrementLevel()
        {
            CurrentLevel++;

            // 通知其他系统（如CharScreenController等）该角色已升级
            CharEvents.LevelIncremented?.Invoke(this);
        }
    }
}