using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // 库存屏幕的非UI逻辑
    public class InventoryScreenController : MonoBehaviour
    {

        [Tooltip("资源文件夹中Equipment ScriptableObjects的路径。")]
        [SerializeField] string m_ResourcePath = "游戏数据/装备";

        [SerializeField] List<EquipmentSO> m_AllGear;
        List<EquipmentSO> m_UnequippedGear = new List<EquipmentSO>();
        List<EquipmentSO> m_EquippedGear = new List<EquipmentSO>();

        void OnEnable()
        {
            InventoryEvents.ScreenEnabled += OnInventoryScreenEnabled;
            InventoryEvents.GearFiltered += OnGearFiltered;
            InventoryEvents.GearSelected += OnGearEquipped;

            CharEvents.GearDataInitialized += OnGearInitialized;
            CharEvents.GearItemUnequipped += OnGearUnequipped;
            CharEvents.CharacterAutoEquipped += OnCharacterAutoEquipped;
        }

        void OnDisable()
        {
            InventoryEvents.ScreenEnabled -= OnInventoryScreenEnabled;
            InventoryEvents.GearFiltered -= OnGearFiltered;
            InventoryEvents.GearSelected -= OnGearEquipped;

            CharEvents.GearDataInitialized -= OnGearInitialized;
            CharEvents.GearItemUnequipped -= OnGearUnequipped;
            CharEvents.CharacterAutoEquipped -= OnCharacterAutoEquipped;
        }

        void Awake()
        {
            m_AllGear = LoadGearData();
            m_UnequippedGear = SortGearList(m_AllGear);
        }

        // 从资源目录加载Equipment ScriptableObject数据（默认 = 资源/游戏数据/装备）
        List<EquipmentSO> LoadGearData()
        {
            m_AllGear.AddRange(Resources.LoadAll<EquipmentSO>(m_ResourcePath));
            return SortGearList(m_AllGear);
        }

        // 排序并显示未装备的装备
        void UpdateInventory(List<EquipmentSO> gearToShow)
        {
            m_UnequippedGear = SortGearList(m_UnequippedGear);
            InventoryEvents.InventoryUpdated?.Invoke(gearToShow);
        }

        // 返回一个新列表，按特定稀有度和装备类型过滤
        List<EquipmentSO> FilterGearList(List<EquipmentSO> gearList, Rarity rarity, EquipmentType gearType)
        {
            List<EquipmentSO> filteredList = gearList;

            if (rarity != Rarity.All)
            {
                filteredList = filteredList.Where(x => x.rarity == rarity).ToList();
            }

            if (gearType != EquipmentType.All)
            {
                filteredList = filteredList.Where(x => x.equipmentType == gearType).ToList();
            }

            return filteredList;
        }
        // 按稀有度然后按装备类型对装备列表进行排序
        List<EquipmentSO> SortGearList(List<EquipmentSO> gearToSort)
        {
            if (gearToSort.Count <= 1)
                return gearToSort;

            // 按稀有度排序，然后按装备类型排序
            return gearToSort.OrderBy(x => x.rarity).
                ThenBy(x => x.equipmentType).
                ToList();
        }

        // 返回特定装备类型中最稀有的装备 
        public EquipmentSO GetBestByType(EquipmentType gearType, List<EquipmentSO> gearList)
        {
            EquipmentSO gearToReturn = null;
            foreach (EquipmentSO g in gearList)
            {
                if (g.equipmentType != gearType)
                {
                    continue;
                }

                if (gearToReturn == null || (int)g.rarity > (int)gearToReturn.rarity)
                {
                    gearToReturn = g;
                }
            }

            return gearToReturn;
        }

        // 返回角色未使用的装备类型列表
        public List<EquipmentType> GetUnusedTypes(CharacterData charData)
        {
            // 自动装备的装备类型
            List<EquipmentType> unusedTypes = new List<EquipmentType>() { EquipmentType.Weapon, EquipmentType.Shield, EquipmentType.Helmet, EquipmentType.Gloves, EquipmentType.Boots };

            for (int i = 0; i < charData.GearData.Length; i++)
            {
                if (charData.GearData[i] != null)
                {
                    unusedTypes.Remove(charData.GearData[i].equipmentType);
                }
            }

            return unusedTypes;
        }

        // 返回一个列表，包含用于填充角色空装备槽的最佳可用装备
        public List<EquipmentSO> GetUnusedGear(CharacterData charData)
        {

            List<EquipmentSO> unusedGear = new List<EquipmentSO>();
            List<EquipmentType> unusedTypes = GetUnusedTypes(charData);

            // 计算角色装备中的空槽数量
            int slotsToFill = charData.GearData.Count(s => s == null);

            for (int i = 0; i < unusedTypes.Count; i++)
            {
                if (slotsToFill <= 0)
                    break;

                EquipmentSO nextGear = GetBestByType(unusedTypes[i], m_UnequippedGear);
                if (nextGear != null)
                {
                    unusedGear.Add(nextGear);
                    slotsToFill--;
                }
            }

            return unusedGear;
        }

        // 填充角色Data上的下一个空GearData槽
        void EquipGear(CharacterData charData, EquipmentSO gearToEquip)
        {
            if (gearToEquip == null)
            {
                return;
            }

            if (!m_UnequippedGear.Contains(gearToEquip))
            {
                return;
            }

            for (int i = 0; i < charData.GearData.Length; i++)
            {
                if (charData.GearData[i] == null)
                {
                    charData.GearData[i] = gearToEquip;
                    m_UnequippedGear.Remove(gearToEquip);
                    m_EquippedGear.Add(gearToEquip);
                    break;
                }
            }
        }

        // 加载角色首选的装备（来自基础数据）
        void SetupDefaultGear(CharacterData charData)
        {
            // 从ScriptableObject基础数据中装备默认装备
            EquipGear(charData, charData.CharacterBaseData.DefaultWeapon);
            EquipGear(charData, charData.CharacterBaseData.DefaultShieldAndArmor);
            EquipGear(charData, charData.CharacterBaseData.DefaultHelmet);
            EquipGear(charData, charData.CharacterBaseData.DefaultGloves);
            EquipGear(charData, charData.CharacterBaseData.DefaultBoots);
        }

        // 事件处理方法
        void OnInventoryScreenEnabled()
        {
            UpdateInventory(m_UnequippedGear);
        }

        void OnGearFiltered(Rarity rarity, EquipmentType gearType)
        {
            List<EquipmentSO> filteredGear = new List<EquipmentSO>();

            filteredGear = FilterGearList(m_UnequippedGear, rarity, gearType);

            UpdateInventory(filteredGear);
        }

        // 如果我们从UI装备装备，则更新库存
        void OnGearEquipped(EquipmentSO gearToEquip)
        {
            if (gearToEquip == null)
                return;

            // 临时从非装备列表中移除并重新加载装备数据
            m_UnequippedGear.Remove(gearToEquip);

            m_EquippedGear.Add(gearToEquip);

            // 通知InventoryScreen刷新
            InventoryEvents.InventoryUpdated?.Invoke(m_UnequippedGear);
        }

        // 卸下装备后更新并排序库存
        void OnGearUnequipped(EquipmentSO gearToUnequip)
        {
            if (gearToUnequip == null)
                return;

            // 将卸下的物品返回库存
            m_UnequippedGear.Add(gearToUnequip);
            m_EquippedGear.Remove(gearToUnequip);

            // 按稀有度、装备类型排序
            m_UnequippedGear = SortGearList(m_UnequippedGear);
        }

        // 搜索未使用的库存以填充角色的空装备槽
        void OnCharacterAutoEquipped(CharacterData charData)
        {
            if (charData == null)
                return;

            // 从未使用的库存中获取最佳装备
            List<EquipmentSO> gearToEquip = GetUnusedGear(charData);

            // 将每个装备分类为已装备/未装备
            foreach (EquipmentSO gearData in gearToEquip)
            {
                OnGearEquipped(gearData);
            }

            // 通知CharScreenController更新
            InventoryEvents.GearAutoEquipped?.Invoke(gearToEquip);
        }

        // 设置默认装备数据
        void OnGearInitialized(List<CharacterData> allCharData)
        {
            // 在每个角色中设置默认Equipment SO
            foreach (CharacterData charData in allCharData)
            {
                SetupDefaultGear(charData);
            }

            // 通知InventoryScreen设置UI
            InventoryEvents.InventorySetup?.Invoke();
        }
    }
}