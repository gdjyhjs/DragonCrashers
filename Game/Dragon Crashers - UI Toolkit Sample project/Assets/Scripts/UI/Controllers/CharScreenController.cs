using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UIToolkitDemo
{
    /// <summary>
    /// 控制角色屏幕的逻辑，包括角色预览、装备管理和升级。
    /// 处理角色选择、库存交互以及由事件驱动的UI更新。
    /// </summary>

    public class CharScreenController : MonoBehaviour
    {
        [Tooltip("可供选择的角色。")]
        [SerializeField] List<CharacterData> m_Characters;

        [Tooltip("所有角色预览的父变换。")]
        [SerializeField] Transform m_PreviewTransform;

        [Header("库存")]
        [Tooltip("选中此选项以允许每个角色仅有一种类型的装备（盔甲、武器等）。")]
        [SerializeField] bool m_UnequipDuplicateGearType;

        [Header("升级")]
        [SerializeField] [Tooltip("控制升级特效的播放。")] PlayableDirector m_LevelUpPlayable;

        public CharacterData CurrentCharacter { get => m_Characters[m_CurrentIndex]; }

        int m_CurrentIndex;
        int m_ActiveGearSlot;

        LevelMeterData m_LevelMeterData;

        /// <summary>
        /// 当屏幕启用时注册事件回调。
        /// </summary>
        void OnEnable()
        {
            CharEvents.LevelIncremented += OnLevelIncremented;

            CharEvents.ScreenStarted += OnCharScreenStarted;
            CharEvents.ScreenEnded += OnCharScreenEnded;
            CharEvents.NextCharacterSelected += SelectNextCharacter;
            CharEvents.LastCharacterSelected += SelectLastCharacter;
            CharEvents.InventoryOpened += OnInventoryOpened;
            CharEvents.GearAutoEquipped += OnGearAutoEquipped;
            CharEvents.GearAllUnequipped += OnGearUnequipped;
            CharEvents.LevelUpClicked += OnLevelUpClicked;
            CharEvents.CharacterLeveledUp += OnCharacterLeveledUp;

            InventoryEvents.GearSelected += OnGearSelected;
            InventoryEvents.GearAutoEquipped += OnGearAutoEquipped;

            SettingsEvents.PlayerLevelReset += OnResetPlayerLevel;
        }

        /// <summary>
        /// 当屏幕禁用时取消注册事件回调，以防止内存泄漏。
        /// </summary>
        void OnDisable()
        {
            CharEvents.LevelIncremented -= OnLevelIncremented;

            CharEvents.ScreenStarted -= OnCharScreenStarted;
            CharEvents.ScreenEnded -= OnCharScreenEnded;

            CharEvents.NextCharacterSelected -= SelectNextCharacter;
            CharEvents.LastCharacterSelected -= SelectLastCharacter;
            CharEvents.InventoryOpened -= OnInventoryOpened;
            CharEvents.GearAutoEquipped -= OnGearAutoEquipped;
            CharEvents.GearAllUnequipped -= OnGearUnequipped;
            CharEvents.LevelUpClicked -= OnLevelUpClicked;
            CharEvents.CharacterLeveledUp -= OnCharacterLeveledUp;

            InventoryEvents.GearSelected -= OnGearSelected;
            InventoryEvents.GearAutoEquipped -= OnGearAutoEquipped;

            SettingsEvents.PlayerLevelReset -= OnResetPlayerLevel;
        }

        /// <summary>
        /// 通过为每个角色实例化视觉表示来初始化角色预览。
        /// </summary>
        void Awake()
        {
            InitializeCharPreview();
            SetupLevelMeter();
        }

        void Start()
        {
            // 通知InventoryScreenController
            CharEvents.GearDataInitialized?.Invoke(m_Characters);
        }

        /// <summary>
        /// 为每个角色设置初始装备数据，并通知库存系统。
        /// </summary>
        void UpdateView()
        {
            if (m_Characters.Count == 0)
                return;

            // 显示角色预制体
            CharEvents.CharacterShown?.Invoke(CurrentCharacter);

            // 更新四个装备槽
            UpdateGearSlots();
        }

        // 角色预览方法
        public void SelectNextCharacter()
        {
            if (m_Characters.Count == 0)
                return;

            ShowCharacterPreview(false);

            m_CurrentIndex++;
            if (m_CurrentIndex >= m_Characters.Count)
                m_CurrentIndex = 0;

            // 从m_Characters中选择下一个角色并刷新角色屏幕
            UpdateView();
        }

        public void SelectLastCharacter()
        {
            if (m_Characters.Count == 0)
                return;

            ShowCharacterPreview(false);

            m_CurrentIndex--;
            if (m_CurrentIndex < 0)
                m_CurrentIndex = m_Characters.Count - 1;

            // 从m_Characters中选择上一个角色并刷新角色屏幕
            UpdateView();
        }

        /// <summary>
        /// 初始化角色预览。
        /// </summary>
        void InitializeCharPreview()
        {
            foreach (CharacterData charData in m_Characters)
            {
                if (charData == null)
                {
                    Debug.LogWarning("[CharScreenController] InitializeCharPreview警告: 缺少角色数据。");
                    continue;
                }
                GameObject previewInstance = Instantiate(charData.CharacterBaseData.CharacterVisualsPrefab, m_PreviewTransform);

                previewInstance.transform.localPosition = Vector3.zero;
                previewInstance.transform.localRotation = Quaternion.identity;
                previewInstance.transform.localScale = Vector3.one;
                charData.PreviewInstance = previewInstance;
                previewInstance.gameObject.SetActive(false);
            }

            CharEvents.PreviewInitialized?.Invoke();

        }

        /// <summary>
        /// 显示当前选择角色的预览。
        /// </summary>
        void ShowCharacterPreview(bool state)
        {
            if (m_Characters.Count == 0)
                return;

            CharacterData currentCharacter = m_Characters[m_CurrentIndex];
            currentCharacter.PreviewInstance.gameObject.SetActive(state);
        }


        /// <summary>
        /// 更新屏幕上角色的装备槽。
        /// </summary>
        void UpdateGearSlots()
        {
            if (CurrentCharacter == null)
                return;

            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                CharEvents.GearSlotUpdated?.Invoke(CurrentCharacter.GearData[i], i);
            }
        }

        /// <summary>
        /// 从角色身上移除特定的装备类型（头盔、盾牌/盔甲、武器、手套、靴子）；
        /// 使用此方法可防止库存中出现重复的装备类型。
        /// </summary>
        /// <param name="typeToRemove">要移除的装备类型。</param>
        public void RemoveGearType(EquipmentType typeToRemove)
        {
            if (CurrentCharacter == null)
                return;

            // 如果在每个角色的库存槽中找到该类型，则将其移除并通知CharView
            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                if (CurrentCharacter.GearData[i] != null && CurrentCharacter.GearData[i].equipmentType == typeToRemove)
                {
                    CharEvents.GearItemUnequipped(CurrentCharacter.GearData[i]);

                    CurrentCharacter.GearData[i] = null;

                    CharEvents.GearSlotUpdated?.Invoke(null, i);
                }
            }
        }

        // 角色等级方法

        /// <summary>
        /// 计算并返回所有角色的总等级。
        /// </summary>
        int GetTotalLevels()
        {
            int totalLevels = 0;
            foreach (CharacterData charData in m_Characters)
            {
                totalLevels += charData.CurrentLevel;
            }
            return totalLevels;
        }

        /// <summary>
        /// 设置等级计量器数据，跟踪所有角色的总等级。
        /// </summary>
        void SetupLevelMeter()
        {
            m_LevelMeterData = new LevelMeterData(GetTotalLevels());

            CharEvents.GetLevelMeterData = () => m_LevelMeterData;
        }

        /// <summary>
        /// 更新等级计量器以反映所有角色总等级的任何变化。
        /// </summary>
        void UpdateLevelMeter()
        {
            m_LevelMeterData.TotalLevels = GetTotalLevels();
        }

        // 事件处理方法

        /// <summary>
        /// 重置所有角色的等级，并更新屏幕以反映这些变化。
        /// </summary>
        void OnResetPlayerLevel()
        {
            foreach (CharacterData charData in m_Characters)
            {
                charData.CurrentLevel = 0;
            }
            CharEvents.CharacterShown?.Invoke(CurrentCharacter);
            UpdateLevelMeter();
        }

        void OnCharScreenStarted()
        {
            UpdateView();
            ShowCharacterPreview(true);
        }

        void OnCharScreenEnded()
        {
            ShowCharacterPreview(false);
        }

        // 点击升级按钮
        void OnLevelUpClicked()
        {
            // 通知GameDataManager我们想使用升级药剂
            CharEvents.LevelPotionUsed?.Invoke(CurrentCharacter);
        }

        // 更新角色统计信息UI
        void OnLevelIncremented(CharacterData charData)
        {
            if (charData == CurrentCharacter)
            {
                CharEvents.CharacterShown?.Invoke(CurrentCharacter);
                UpdateLevelMeter();
            }
        }

        /// <summary>
        /// 处理角色升级的结果，增加他们的等级并播放视觉效果。
        /// </summary>
        /// <param name="didLevel">角色升级是否成功。</param>
        // 
        void OnCharacterLeveledUp(bool didLevel)
        {
            if (didLevel)
            {
                CurrentCharacter.IncrementLevel();

                // 播放特效序列
                m_LevelUpPlayable.Play();
            }
        }

        /// <summary>
        /// 跟踪用于打开库存的装备槽
        /// </summary>
        /// <param name="gearSlot">装备槽的索引。</param>
        void OnInventoryOpened(int gearSlot)
        {
            m_ActiveGearSlot = gearSlot;
        }

        /// <summary>
        /// 将选择的装备装备到活动装备槽并更新UI。
        /// </summary>
        /// <param name="gearObject">选择的装备/物品。</param>
        // 处理从库存屏幕选择装备
        void OnGearSelected(EquipmentSO gearObject)
        {
            // 如果槽位已经有物品，通知InventoryScreenController并将其返回库存
            if (CurrentCharacter.GearData[m_ActiveGearSlot] != null)
            {

                CharEvents.GearItemUnequipped?.Invoke(CurrentCharacter.GearData[m_ActiveGearSlot]);
                CurrentCharacter.GearData[m_ActiveGearSlot] = null;
            }

            // 移除任何重复的装备类型（只允许一种类型的头盔、盾牌/盔甲、武器、手套或靴子）
            if (m_UnequipDuplicateGearType)
            {
                RemoveGearType(gearObject.equipmentType);
            }

            // 将装备设置到活动槽位
            CurrentCharacter.GearData[m_ActiveGearSlot] = gearObject;

            // 通知CharScreen更新
            CharEvents.GearSlotUpdated?.Invoke(gearObject, m_ActiveGearSlot);
        }

        /// <summary>
        /// 卸下所有装备槽。
        /// </summary>
        void OnGearUnequipped()
        {
            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                // 如果我们当前在四个装备槽之一中有装备，将其移除
                if (CurrentCharacter.GearData[i] != null)
                {
                    // 通知InventoryScreenController卸下装备并更新列表
                    CharEvents.GearItemUnequipped?.Invoke(CurrentCharacter.GearData[i]);

                    // 从角色的装备数据中清除装备
                    CurrentCharacter.GearData[i] = null;

                    // 通知CharScreen UI更新
                    CharEvents.GearSlotUpdated?.Invoke(null, i);
                }
            }
        }

        /// <summary>
        /// 为当前角色自动装备装备并更新UI。
        /// </summary>
        void OnGearAutoEquipped()
        {
            CharEvents.CharacterAutoEquipped?.Invoke(CurrentCharacter);
        }

        /// <summary>
        /// 自动将装备装备到角色的空装备槽并更新屏幕。
        /// </summary>
        void OnGearAutoEquipped(List<EquipmentSO> gearToEquip)
        {
            if (CurrentCharacter == null)
                return;

            int gearCounter = 0;

            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                if (CurrentCharacter.GearData[i] == null && gearCounter < gearToEquip.Count)
                {
                    CurrentCharacter.GearData[i] = gearToEquip[gearCounter];

                    // 通知CharView更新
                    CharEvents.GearSlotUpdated?.Invoke(gearToEquip[gearCounter], i);
                    gearCounter++;
                }
            }
        }
    }
}