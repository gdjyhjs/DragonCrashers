using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// �����ɫ��ͼUI������������ť��װ���ۺͽ�ɫͳ�ơ�
    /// ʹ���¼��������£���棩������ʱ���ݰ󶨣�ҩˮ��ǩ����ɫ���ơ��������Ļ�Ϸ�ʽ��
    /// </summary>
    public class CharView : UIView
    {
        // ������ť��������
        const string k_LevelUpButtonInactiveClass = "footer__level-up-button--inactive";
        // ������ť����
        const string k_LevelUpButtonClass = "footer__level-up-button";

        // װ���۰�ť����
        readonly Button[] m_GearSlots = new Button[4];
        // ��װ���۾���
        readonly Sprite m_EmptyGearSlotSprite;

        // ��һ����ɫ��ť
        Button m_LastCharButton;
        // ��һ����ɫ��ť
        Button m_NextCharButton;
        // �Զ�װ����ť
        Button m_AutoEquipButton;
        // ж��װ����ť
        Button m_UnequipButton;
        // ������ť
        Button m_LevelUpButton;

        // ��ɫ��ǩ
        Label m_CharacterLabel;
        // ��һ������ҩˮ��ǩ
        Label m_PotionsForNextLevel;
        // ҩˮ������ǩ
        Label m_PotionCount;
        // ������ǩ
        Label m_PowerLabel;

        // ������ť��Ч
        VisualElement m_LevelUpButtonVFX;

        // ��ɫͳ����ͼ
        CharStatsView m_CharStatsView; // ��ʾ��ɫͳ�ƵĴ���

        /// <summary>
        /// ʹ��ָ���Ķ���UIԪ�س�ʼ����ɫ��ͼ��
        /// </summary>
        /// <param name="topElement">UI�ĸ��Ӿ�Ԫ�ء�</param>
        public CharView(VisualElement topElement) : base(topElement)
        {
            // ����������ť�����¼�
            CharEvents.LevelUpButtonEnabled += OnLevelUpButtonEnabled;
            // ���Ľ�ɫ��ʾ�¼�
            CharEvents.CharacterShown += OnCharacterUpdated;
            // ����Ԥ����ʼ���¼�
            CharEvents.PreviewInitialized += OnInitialized;
            // ����װ���۸����¼�
            CharEvents.GearSlotUpdated += OnGearSlotUpdated;

            // ������Ϸ���ݽ����¼�
            GameDataManager.GameDataReceived += OnGameDataReceived;

            // ������Ϸ����
            GameDataManager.GameDataRequested?.Invoke();

            // ��ScriptableObjectͼ���ж�λ��װ���۾���
            var gameIconsData = Resources.Load("GameData/GameIcons") as GameIconsSO;
            m_EmptyGearSlotSprite = gameIconsData.emptyGearSlotIcon;

            // ��ʼ����ɫͳ����ͼ
            m_CharStatsView = new CharStatsView(topElement.Q<VisualElement>("CharStatsWindow"));
            // ��ʾ��ɫͳ����ͼ
            m_CharStatsView.Show();
        }

        /// <summary>
        /// ��GameDataManager����GameDataʱ���á�
        /// Ӧ������ʱ���ݰ󶨡�
        /// </summary>
        /// <param name="gameData">Ҫ�󶨵�GameData����</param>
        void OnGameDataReceived(GameData gameData)
        {
            // ����Ϸ���ݰ󶨵�UI
            BindGameDataToUI(gameData);
        }

        /// <summary>
        /// Ϊҩˮ��������һ������ҩˮ����ɫ�����ͽ�ɫ���Ƶı�ǩ��Ӱ�
        /// </summary>
        /// <param name="gameData"></param>
        void BindGameDataToUI(GameData gameData)
        {
            // ��ɫ������ǩ�İ�
            m_PowerLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.CurrentPower)),
                bindingMode = BindingMode.ToTarget
            });

            // ��ɫ���Ʊ�ǩ�İ�
            m_CharacterLabel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.CharacterName)),
                bindingMode = BindingMode.ToTarget
            });

            // ҩˮ������ǩ�İ�
            var potionBinding = new DataBinding()
            {
                dataSource = gameData,
                dataSourcePath = new PropertyPath(string.Empty), // ��ֱ��·�� -- ʹ��ת����
                bindingMode = BindingMode.ToTarget
            };

            // ��ʽ���ַ�����ǩ������ҩˮ���� / ��������ҩˮ������
            potionBinding.sourceToUiConverters.AddConverter((ref GameData data) =>
                FormatPotionCountLabel(data.LevelUpPotions));
            m_PotionCount.SetBinding("text", potionBinding);

            // ��һ������ҩˮ��ǩ�İ�
            m_PotionsForNextLevel.SetBinding("text", new DataBinding()
            {
                dataSourcePath = new PropertyPath(nameof(CharacterData.PotionsForNextLevel)),
                bindingMode = BindingMode.ToTarget
            });
        }

        /// <summary>
        /// ����ҩˮ���������ʵ�����ɫ��ʽ��ҩˮ������ǩ��
        /// </summary>
        /// <param name="potionCount">��ǰҩˮ������</param>
        /// <returns>��ʽ�����ҩˮ�����ַ�����</returns>
        string FormatPotionCountLabel(uint potionCount)
        {
            if (m_PotionsForNextLevel == null)
            {
                Debug.LogWarning("[CharView] FormatPotionCountLabel: PotionsForNextLevel��ǩδ���á�");
                return potionCount.ToString();
            }

            string potionsForNextLevelString = m_PotionsForNextLevel.text.TrimStart('/');

            if (!string.IsNullOrEmpty(potionsForNextLevelString) &&
                int.TryParse(potionsForNextLevelString, out int potionsForNextLevel))
            {
                int potionsCount = (int)potionCount;

                // ���ݱȽϽ������ҩˮ������ǩ����ɫ
                m_PotionCount.style.color = (potionsForNextLevel > potionsCount)
                    ? new Color(0.88f, 0.36f, 0f) // ҩˮ����ʱΪ��ɫ
                    : new Color(0.81f, 0.94f, 0.48f); // ҩˮ����ʱΪ��ɫ
            }

            return potionCount.ToString();
        }

        /// <summary>
        /// �����¼��������������Դ��
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            // ȡ������������ť�����¼�
            CharEvents.LevelUpButtonEnabled -= OnLevelUpButtonEnabled;
            // ȡ�����Ľ�ɫ��ʾ�¼�
            CharEvents.CharacterShown -= OnCharacterUpdated;
            // ȡ������Ԥ����ʼ���¼�
            CharEvents.PreviewInitialized -= OnInitialized;
            // ȡ������װ���۸����¼�
            CharEvents.GearSlotUpdated -= OnGearSlotUpdated;

            // ȡ��������Ϸ���ݽ����¼�
            GameDataManager.GameDataReceived -= OnGameDataReceived;

            // ע����ť�ص�
            UnregisterButtonCallbacks();
        }

        /// <summary>
        /// ���ö�UI��Visual Elements�����á�
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // ��ȡװ���۰�ť
            m_GearSlots[0] = m_TopElement.Q<Button>("char-inventory__slot1");
            m_GearSlots[1] = m_TopElement.Q<Button>("char-inventory__slot2");
            m_GearSlots[2] = m_TopElement.Q<Button>("char-inventory__slot3");
            m_GearSlots[3] = m_TopElement.Q<Button>("char-inventory__slot4");

            // ��ȡ��һ����ɫ��ť
            m_NextCharButton = m_TopElement.Q<Button>("char__next-button");
            // ��ȡ��һ����ɫ��ť
            m_LastCharButton = m_TopElement.Q<Button>("char__last-button");

            // ��ȡ�Զ�װ����ť
            m_AutoEquipButton = m_TopElement.Q<Button>("char__auto-equip-button");
            // ��ȡж��װ����ť
            m_UnequipButton = m_TopElement.Q<Button>("char__unequip-button");
            // ��ȡ������ť
            m_LevelUpButton = m_TopElement.Q<Button>("char__level-up-button");
            // ��ȡ������ť��Ч
            m_LevelUpButtonVFX = m_TopElement.Q<VisualElement>("char__level-up-button-vfx");

            // ��ȡ��ɫ��ǩ
            m_CharacterLabel = m_TopElement.Q<Label>("char__label");
            // ��ȡҩˮ������ǩ
            m_PotionCount = m_TopElement.Q<Label>("char__potion-count");
            // ��ȡ��һ������ҩˮ��ǩ
            m_PotionsForNextLevel = m_TopElement.Q<Label>("char__potion-to-advance");
            // ��ȡ������ǩ
            m_PowerLabel = m_TopElement.Q<Label>("char__power-label");
        }

        /// <summary>
        /// ע�ᰴť�ص��Դ���ť����¼���
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // ע��װ���۰�ť����¼�
            m_GearSlots[0].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[1].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[2].RegisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[3].RegisterCallback<ClickEvent>(ShowInventory);

            // ע����һ����ɫ��ť����¼�
            m_NextCharButton.RegisterCallback<ClickEvent>(GoToNextCharacter);
            // ע����һ����ɫ��ť����¼�
            m_LastCharButton.RegisterCallback<ClickEvent>(GoToLastCharacter);

            // ע���Զ�װ����ť����¼�
            m_AutoEquipButton.RegisterCallback<ClickEvent>(AutoEquipSlots);
            // ע��ж��װ����ť����¼�
            m_UnequipButton.RegisterCallback<ClickEvent>(UnequipSlots);
            // ע��������ť����¼�
            m_LevelUpButton.RegisterCallback<ClickEvent>(LevelUpCharacter);
        }

        /// <summary>
        /// ע����ť�ص��Է�ֹ�ڴ�й©���ڴ��������¿�ѡ��
        /// ȡ����Ӧ�ó�����������ڹ���
        /// </summary>
        protected void UnregisterButtonCallbacks()
        {
            // ע��װ���۰�ť����¼�
            m_GearSlots[0].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[1].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[2].UnregisterCallback<ClickEvent>(ShowInventory);
            m_GearSlots[3].UnregisterCallback<ClickEvent>(ShowInventory);

            // ע����һ����ɫ��ť����¼�
            m_NextCharButton.UnregisterCallback<ClickEvent>(GoToNextCharacter);
            // ע����һ����ɫ��ť����¼�
            m_LastCharButton.UnregisterCallback<ClickEvent>(GoToLastCharacter);

            // ע���Զ�װ����ť����¼�
            m_AutoEquipButton.UnregisterCallback<ClickEvent>(AutoEquipSlots);
            // ע��ж��װ����ť����¼�
            m_UnequipButton.UnregisterCallback<ClickEvent>(UnequipSlots);
            // ע��������ť����¼�
            m_LevelUpButton.UnregisterCallback<ClickEvent>(LevelUpCharacter);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Show()
        {
            base.Show();

            // ���ñ�ǩʽUI
            MainMenuUIEvents.TabbedUIReset?.Invoke("CharScreen");
            // ������Ļ��ʼ�¼�
            CharEvents.ScreenStarted?.Invoke();
        }

        /// <summary>
        /// ���ؽ�ɫ��ͼUI��֪ͨ��Ļ������
        /// </summary>
        public override void Hide()
        {
            base.Hide();
            // ������Ļ�����¼�
            CharEvents.ScreenEnded?.Invoke();
        }

        /// <summary>
        /// ���������ťʱ������ɫ�������ܡ�
        /// </summary>
        /// <param name="evt"></param>
        void LevelUpCharacter(ClickEvent evt)
        {
            // ������������¼�
            CharEvents.LevelUpClicked?.Invoke();
        }

        /// <summary>
        /// ֪ͨCharScreenControllerж������װ��
        /// </summary>
        /// <param name="evt"></param>
        void UnequipSlots(ClickEvent evt)
        {
            // ���ű��ð�ť��Ч
            AudioManager.PlayAltButtonSound();
            // ��������װ��ж���¼�
            CharEvents.GearAllUnequipped?.Invoke();
        }

        /// <summary>
        /// װ���ղ��п��õ����װ����
        /// </summary>
        void AutoEquipSlots(ClickEvent evt)
        {
            // ���ű��ð�ť��Ч
            AudioManager.PlayAltButtonSound();
            // �����Զ�װ���¼�
            CharEvents.GearAutoEquipped?.Invoke();
        }

        /// <summary>
        /// ѡ���ɫ��ͼ�е���һ����ɫ��
        /// </summary>
        void GoToLastCharacter(ClickEvent evt)
        {
            // ���ű��ð�ť��Ч
            AudioManager.PlayAltButtonSound();
            // ������һ����ɫѡ���¼�
            CharEvents.LastCharacterSelected?.Invoke();
        }

        /// <summary>
        /// ѡ���ɫ��ͼ�е���һ����ɫ��
        /// </summary>
        void GoToNextCharacter(ClickEvent evt)
        {
            // ���ű��ð�ť��Ч
            AudioManager.PlayAltButtonSound();
            // ������һ����ɫѡ���¼�
            CharEvents.NextCharacterSelected?.Invoke();
        }

        /// <summary>
        /// ���װ����ʱ�򿪿����Ļ��
        /// </summary>
        void ShowInventory(ClickEvent evt)
        {
            VisualElement clickedElement = evt.target as VisualElement;

            if (clickedElement == null)
                return;

            char slotNumber = clickedElement.name[clickedElement.name.Length - 1];
            int slot = (int)char.GetNumericValue(slotNumber) - 1;

            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();

            // ���������Ļ��ʾ�¼�
            MainMenuUIEvents.InventoryScreenShown?.Invoke();

            // ���������¼�
            CharEvents.InventoryOpened?.Invoke(slot);
        }

        // �¼�������

        void OnInitialized()
        {
            // �����Ӿ�Ԫ��
            SetVisualElements();
            // ע�ᰴť�ص�
            RegisterButtonCallbacks();
        }

        /// <summary>
        /// ѡ���½�ɫʱ���½�ɫ��ͼ��
        /// </summary>
        /// <param name="characterToShow">Ҫ��ʾ�Ľ�ɫ���ݡ�</param>
        void OnCharacterUpdated(CharacterData characterToShow)
        {
            if (characterToShow == null)
                return;

            // ���½�ɫ��ǩ������Դ
            m_CharacterLabel.dataSource = characterToShow;
            m_PowerLabel.dataSource = characterToShow;
            m_PotionsForNextLevel.dataSource = characterToShow;

            // ���½�ɫͳ����ͼ
            m_CharStatsView.UpdateCharacterStats(characterToShow);

            // �����ɫԤ��ʵ��
            characterToShow.PreviewInstance.gameObject.SetActive(true);
        }

        /// <summary>
        /// ����װ���۵��Ӿ���ʾ��
        /// </summary>
        /// <param name="gearData">Ҫ��ʾ��װ�����ݡ�</param>
        /// <param name="slotToUpdate">Ҫ���µ�װ����������</param>
        void OnGearSlotUpdated(EquipmentSO gearData, int slotToUpdate)
        {
            Button activeSlot = m_GearSlots[slotToUpdate];

            // �Ӻ�ͼ����char-inventory__slot-n�ĵ�һ����Ԫ��
            VisualElement addSymbol = activeSlot.ElementAt(0);

            // ����������char-inventory__slot-n�ĵڶ�����Ԫ��
            VisualElement gearElement = activeSlot.ElementAt(1);

            if (gearData == null)
            {
                if (gearElement != null)
                    // ����װ���۱���Ϊ�հ׾���
                    gearElement.style.backgroundImage = new StyleBackground(m_EmptyGearSlotSprite);

                if (addSymbol != null)
                    // ��ʾ�Ӻ�ͼ��
                    addSymbol.style.display = DisplayStyle.Flex;
            }
            else
            {
                if (gearElement != null)
                    // ����װ���۱���Ϊװ������
                    gearElement.style.backgroundImage = new StyleBackground(gearData.sprite);

                if (addSymbol != null)
                    // ���ؼӺ�ͼ��
                    addSymbol.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// ����ҩˮ�������л�������ť��Ч��״̬��
        /// </summary>
        /// <param name="state">�������������Ϊtrue������Ϊfalse��</param>
        void OnLevelUpButtonEnabled(bool state)
        {
            if (m_LevelUpButtonVFX == null || m_LevelUpButton == null)
                return;

            // ��ʾ������������ť��Ч
            m_LevelUpButtonVFX.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;

            if (state)
            {
                // ���ð�ť���������ָ�뼤��:hoverα״̬
                m_LevelUpButton.SetEnabled(true);
                m_LevelUpButton.pickingMode = PickingMode.Position;

                // ��Ӻ��Ƴ���ʽ���Լ��ť
                m_LevelUpButton.AddToClassList(k_LevelUpButtonClass);
                m_LevelUpButton.RemoveFromClassList(k_LevelUpButtonInactiveClass);
            }
            else
            {
                // ���ð�ť����ֹ���ָ�뼤��:hoverα״̬
                m_LevelUpButton.SetEnabled(false);
                m_LevelUpButton.pickingMode = PickingMode.Ignore;
                m_LevelUpButton.AddToClassList(k_LevelUpButtonInactiveClass);
                m_LevelUpButton.RemoveFromClassList(k_LevelUpButtonClass);
            }
        }
    }
}