using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace UIToolkitDemo
{
    /// <summary>
    /// ���ƽ�ɫ��Ļ���߼���������ɫԤ����װ�������������
    /// �����ɫѡ�񡢿�潻���Լ����¼�������UI���¡�
    /// </summary>

    public class CharScreenController : MonoBehaviour
    {
        [Tooltip("�ɹ�ѡ��Ľ�ɫ��")]
        [SerializeField] List<CharacterData> m_Characters;

        [Tooltip("���н�ɫԤ���ĸ��任��")]
        [SerializeField] Transform m_PreviewTransform;

        [Header("���")]
        [Tooltip("ѡ�д�ѡ��������ÿ����ɫ����һ�����͵�װ�������ס������ȣ���")]
        [SerializeField] bool m_UnequipDuplicateGearType;

        [Header("����")]
        [SerializeField] [Tooltip("����������Ч�Ĳ��š�")] PlayableDirector m_LevelUpPlayable;

        public CharacterData CurrentCharacter { get => m_Characters[m_CurrentIndex]; }

        int m_CurrentIndex;
        int m_ActiveGearSlot;

        LevelMeterData m_LevelMeterData;

        /// <summary>
        /// ����Ļ����ʱע���¼��ص���
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
        /// ����Ļ����ʱȡ��ע���¼��ص����Է�ֹ�ڴ�й©��
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
        /// ͨ��Ϊÿ����ɫʵ�����Ӿ���ʾ����ʼ����ɫԤ����
        /// </summary>
        void Awake()
        {
            InitializeCharPreview();
            SetupLevelMeter();
        }

        void Start()
        {
            // ֪ͨInventoryScreenController
            CharEvents.GearDataInitialized?.Invoke(m_Characters);
        }

        /// <summary>
        /// Ϊÿ����ɫ���ó�ʼװ�����ݣ���֪ͨ���ϵͳ��
        /// </summary>
        void UpdateView()
        {
            if (m_Characters.Count == 0)
                return;

            // ��ʾ��ɫԤ����
            CharEvents.CharacterShown?.Invoke(CurrentCharacter);

            // �����ĸ�װ����
            UpdateGearSlots();
        }

        // ��ɫԤ������
        public void SelectNextCharacter()
        {
            if (m_Characters.Count == 0)
                return;

            ShowCharacterPreview(false);

            m_CurrentIndex++;
            if (m_CurrentIndex >= m_Characters.Count)
                m_CurrentIndex = 0;

            // ��m_Characters��ѡ����һ����ɫ��ˢ�½�ɫ��Ļ
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

            // ��m_Characters��ѡ����һ����ɫ��ˢ�½�ɫ��Ļ
            UpdateView();
        }

        /// <summary>
        /// ��ʼ����ɫԤ����
        /// </summary>
        void InitializeCharPreview()
        {
            foreach (CharacterData charData in m_Characters)
            {
                if (charData == null)
                {
                    Debug.LogWarning("[CharScreenController] InitializeCharPreview����: ȱ�ٽ�ɫ���ݡ�");
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
        /// ��ʾ��ǰѡ���ɫ��Ԥ����
        /// </summary>
        void ShowCharacterPreview(bool state)
        {
            if (m_Characters.Count == 0)
                return;

            CharacterData currentCharacter = m_Characters[m_CurrentIndex];
            currentCharacter.PreviewInstance.gameObject.SetActive(state);
        }


        /// <summary>
        /// ������Ļ�Ͻ�ɫ��װ���ۡ�
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
        /// �ӽ�ɫ�����Ƴ��ض���װ�����ͣ�ͷ��������/���ס����������ס�ѥ�ӣ���
        /// ʹ�ô˷����ɷ�ֹ����г����ظ���װ�����͡�
        /// </summary>
        /// <param name="typeToRemove">Ҫ�Ƴ���װ�����͡�</param>
        public void RemoveGearType(EquipmentType typeToRemove)
        {
            if (CurrentCharacter == null)
                return;

            // �����ÿ����ɫ�Ŀ������ҵ������ͣ������Ƴ���֪ͨCharView
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

        // ��ɫ�ȼ�����

        /// <summary>
        /// ���㲢�������н�ɫ���ܵȼ���
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
        /// ���õȼ����������ݣ��������н�ɫ���ܵȼ���
        /// </summary>
        void SetupLevelMeter()
        {
            m_LevelMeterData = new LevelMeterData(GetTotalLevels());

            CharEvents.GetLevelMeterData = () => m_LevelMeterData;
        }

        /// <summary>
        /// ���µȼ��������Է�ӳ���н�ɫ�ܵȼ����κα仯��
        /// </summary>
        void UpdateLevelMeter()
        {
            m_LevelMeterData.TotalLevels = GetTotalLevels();
        }

        // �¼�������

        /// <summary>
        /// �������н�ɫ�ĵȼ�����������Ļ�Է�ӳ��Щ�仯��
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

        // ���������ť
        void OnLevelUpClicked()
        {
            // ֪ͨGameDataManager������ʹ������ҩ��
            CharEvents.LevelPotionUsed?.Invoke(CurrentCharacter);
        }

        // ���½�ɫͳ����ϢUI
        void OnLevelIncremented(CharacterData charData)
        {
            if (charData == CurrentCharacter)
            {
                CharEvents.CharacterShown?.Invoke(CurrentCharacter);
                UpdateLevelMeter();
            }
        }

        /// <summary>
        /// �����ɫ�����Ľ�����������ǵĵȼ��������Ӿ�Ч����
        /// </summary>
        /// <param name="didLevel">��ɫ�����Ƿ�ɹ���</param>
        // 
        void OnCharacterLeveledUp(bool didLevel)
        {
            if (didLevel)
            {
                CurrentCharacter.IncrementLevel();

                // ������Ч����
                m_LevelUpPlayable.Play();
            }
        }

        /// <summary>
        /// �������ڴ򿪿���װ����
        /// </summary>
        /// <param name="gearSlot">װ���۵�������</param>
        void OnInventoryOpened(int gearSlot)
        {
            m_ActiveGearSlot = gearSlot;
        }

        /// <summary>
        /// ��ѡ���װ��װ�����װ���۲�����UI��
        /// </summary>
        /// <param name="gearObject">ѡ���װ��/��Ʒ��</param>
        // ����ӿ����Ļѡ��װ��
        void OnGearSelected(EquipmentSO gearObject)
        {
            // �����λ�Ѿ�����Ʒ��֪ͨInventoryScreenController�����䷵�ؿ��
            if (CurrentCharacter.GearData[m_ActiveGearSlot] != null)
            {

                CharEvents.GearItemUnequipped?.Invoke(CurrentCharacter.GearData[m_ActiveGearSlot]);
                CurrentCharacter.GearData[m_ActiveGearSlot] = null;
            }

            // �Ƴ��κ��ظ���װ�����ͣ�ֻ����һ�����͵�ͷ��������/���ס����������׻�ѥ�ӣ�
            if (m_UnequipDuplicateGearType)
            {
                RemoveGearType(gearObject.equipmentType);
            }

            // ��װ�����õ����λ
            CurrentCharacter.GearData[m_ActiveGearSlot] = gearObject;

            // ֪ͨCharScreen����
            CharEvents.GearSlotUpdated?.Invoke(gearObject, m_ActiveGearSlot);
        }

        /// <summary>
        /// ж������װ���ۡ�
        /// </summary>
        void OnGearUnequipped()
        {
            for (int i = 0; i < CurrentCharacter.GearData.Length; i++)
            {
                // ������ǵ�ǰ���ĸ�װ����֮һ����װ���������Ƴ�
                if (CurrentCharacter.GearData[i] != null)
                {
                    // ֪ͨInventoryScreenControllerж��װ���������б�
                    CharEvents.GearItemUnequipped?.Invoke(CurrentCharacter.GearData[i]);

                    // �ӽ�ɫ��װ�����������װ��
                    CurrentCharacter.GearData[i] = null;

                    // ֪ͨCharScreen UI����
                    CharEvents.GearSlotUpdated?.Invoke(null, i);
                }
            }
        }

        /// <summary>
        /// Ϊ��ǰ��ɫ�Զ�װ��װ��������UI��
        /// </summary>
        void OnGearAutoEquipped()
        {
            CharEvents.CharacterAutoEquipped?.Invoke(CurrentCharacter);
        }

        /// <summary>
        /// �Զ���װ��װ������ɫ�Ŀ�װ���۲�������Ļ��
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

                    // ֪ͨCharView����
                    CharEvents.GearSlotUpdated?.Invoke(gearToEquip[gearCounter], i);
                    gearCounter++;
                }
            }
        }
    }
}