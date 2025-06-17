using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // �����Ļ�ķ�UI�߼�
    public class InventoryScreenController : MonoBehaviour
    {

        [Tooltip("��Դ�ļ�����Equipment ScriptableObjects��·����")]
        [SerializeField] string m_ResourcePath = "��Ϸ����/װ��";

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

        // ����ԴĿ¼����Equipment ScriptableObject���ݣ�Ĭ�� = ��Դ/��Ϸ����/װ����
        List<EquipmentSO> LoadGearData()
        {
            m_AllGear.AddRange(Resources.LoadAll<EquipmentSO>(m_ResourcePath));
            return SortGearList(m_AllGear);
        }

        // ������ʾδװ����װ��
        void UpdateInventory(List<EquipmentSO> gearToShow)
        {
            m_UnequippedGear = SortGearList(m_UnequippedGear);
            InventoryEvents.InventoryUpdated?.Invoke(gearToShow);
        }

        // ����һ�����б����ض�ϡ�жȺ�װ�����͹���
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
        // ��ϡ�ж�Ȼ��װ�����Ͷ�װ���б��������
        List<EquipmentSO> SortGearList(List<EquipmentSO> gearToSort)
        {
            if (gearToSort.Count <= 1)
                return gearToSort;

            // ��ϡ�ж�����Ȼ��װ����������
            return gearToSort.OrderBy(x => x.rarity).
                ThenBy(x => x.equipmentType).
                ToList();
        }

        // �����ض�װ����������ϡ�е�װ�� 
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

        // ���ؽ�ɫδʹ�õ�װ�������б�
        public List<EquipmentType> GetUnusedTypes(CharacterData charData)
        {
            // �Զ�װ����װ������
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

        // ����һ���б�������������ɫ��װ���۵���ѿ���װ��
        public List<EquipmentSO> GetUnusedGear(CharacterData charData)
        {

            List<EquipmentSO> unusedGear = new List<EquipmentSO>();
            List<EquipmentType> unusedTypes = GetUnusedTypes(charData);

            // �����ɫװ���еĿղ�����
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

        // ����ɫData�ϵ���һ����GearData��
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

        // ���ؽ�ɫ��ѡ��װ�������Ի������ݣ�
        void SetupDefaultGear(CharacterData charData)
        {
            // ��ScriptableObject����������װ��Ĭ��װ��
            EquipGear(charData, charData.CharacterBaseData.DefaultWeapon);
            EquipGear(charData, charData.CharacterBaseData.DefaultShieldAndArmor);
            EquipGear(charData, charData.CharacterBaseData.DefaultHelmet);
            EquipGear(charData, charData.CharacterBaseData.DefaultGloves);
            EquipGear(charData, charData.CharacterBaseData.DefaultBoots);
        }

        // �¼�������
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

        // ������Ǵ�UIװ��װ��������¿��
        void OnGearEquipped(EquipmentSO gearToEquip)
        {
            if (gearToEquip == null)
                return;

            // ��ʱ�ӷ�װ���б����Ƴ������¼���װ������
            m_UnequippedGear.Remove(gearToEquip);

            m_EquippedGear.Add(gearToEquip);

            // ֪ͨInventoryScreenˢ��
            InventoryEvents.InventoryUpdated?.Invoke(m_UnequippedGear);
        }

        // ж��װ������²�������
        void OnGearUnequipped(EquipmentSO gearToUnequip)
        {
            if (gearToUnequip == null)
                return;

            // ��ж�µ���Ʒ���ؿ��
            m_UnequippedGear.Add(gearToUnequip);
            m_EquippedGear.Remove(gearToUnequip);

            // ��ϡ�жȡ�װ����������
            m_UnequippedGear = SortGearList(m_UnequippedGear);
        }

        // ����δʹ�õĿ��������ɫ�Ŀ�װ����
        void OnCharacterAutoEquipped(CharacterData charData)
        {
            if (charData == null)
                return;

            // ��δʹ�õĿ���л�ȡ���װ��
            List<EquipmentSO> gearToEquip = GetUnusedGear(charData);

            // ��ÿ��װ������Ϊ��װ��/δװ��
            foreach (EquipmentSO gearData in gearToEquip)
            {
                OnGearEquipped(gearData);
            }

            // ֪ͨCharScreenController����
            InventoryEvents.GearAutoEquipped?.Invoke(gearToEquip);
        }

        // ����Ĭ��װ������
        void OnGearInitialized(List<CharacterData> allCharData)
        {
            // ��ÿ����ɫ������Ĭ��Equipment SO
            foreach (CharacterData charData in allCharData)
            {
                SetupDefaultGear(charData);
            }

            // ֪ͨInventoryScreen����UI
            InventoryEvents.InventorySetup?.Invoke();
        }
    }
}