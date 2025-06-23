using System;
using System.Collections;
using System.Collections.Generic;
using Template2DCommon;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
namespace HappyHarvest
{
    /// <summary>
    /// ����������Ϸ���� UI ��ص��������ݡ����������� UI Ԫ�ز��������־�̬������
    /// ��Щ�������ڸ��� / ���� UI���Ա���Դ��� UI �������κ��������е������ǡ�
    /// </summary>
    public class UIHandler : MonoBehaviour
    {
        protected static UIHandler s_Instance;

        // �������ö�٣������룬����Ӣ���Ա�����Ϸ����
        public enum CursorType
        {
            Normal,
            Interact,
            System
        }

        [Header("�������")]
        // ��ͨ�������
        public Texture2D NormalCursor;
        // �����������
        public Texture2D InteractCursor;

        [Header("UI �ĵ�")]
        // �г���Ŀģ��
        public VisualTreeAsset MarketEntryTemplate;

        [Header("����")]
        // �г�������Ч
        public AudioClip MarketSellSound;

        protected UIDocument m_Document;

        // ����λ�Ӿ�Ԫ���б�
        protected List<VisualElement> m_InventorySlots;
        // ��Ʒ������ǩ�б�
        protected List<Label> m_ItemCountLabels;

        // ��Ҽ�������ǩ
        protected Label m_CointCounter;

        // �г���������
        protected VisualElement m_MarketPopup;
        // �г����ݹ�����ͼ
        protected VisualElement m_MarketContentScrollview;

        // ��ʱ����ǩ
        protected Label m_TimerLabel;

        // ����ť
        protected Button m_BuyButton;
        // ���۰�ť
        protected Button m_SellButton;

        // Ӧ�ý���״̬
        protected bool m_HaveFocus = true;
        // ��ǰ�������
        protected CursorType m_CurrentCursorType;

        // ���ò˵�
        protected SettingMenu m_SettingMenu;
        // �ֿ� UI
        protected WarehouseUI m_WarehouseUI;

        // ��������
        protected VisualElement m_Blocker;
        // ������ɻص�
        protected System.Action m_FadeFinishClbk;

        // ������ǩ
        private Label m_SunLabel;
        private Label m_RainLabel;
        private Label m_ThunderLabel;

        void Awake()
        {
            s_Instance = this;

            m_Document = GetComponent<UIDocument>();

            // ��ȡ����λ��������ǩ
            m_InventorySlots = m_Document.rootVisualElement.Query<VisualElement>("InventoryEntry").ToList();
            m_ItemCountLabels = m_Document.rootVisualElement.Query<Label>("ItemCount").ToList();

            // Ϊÿ������λ��ӵ���¼�
            for (int i = 0; i < m_InventorySlots.Count; ++i)
            {
                var i1 = i;
                m_InventorySlots[i].AddManipulator(new Clickable(() =>
                {
                    GameManager.Instance.Player.ChangeEquipItem(i1);
                }));
            }

            Debug.Assert(m_InventorySlots.Count == InventorySystem.InventorySize,
            "UI �е���Ʒ��λ���������ɿ��");

            // ��ȡ��Ҽ�������ǩ
            m_CointCounter = m_Document.rootVisualElement.Q<Label>("CoinAmount");

            // ��ʼ���г���������
            m_MarketPopup = m_Document.rootVisualElement.Q<VisualElement>("MarketPopup");
            m_MarketPopup.Q<Button>("CloseButton").clicked += CloseMarket;
            m_MarketPopup.visible = false;

            // ��ʼ��������ť
            m_BuyButton = m_MarketPopup.Q<Button>("BuyButton");
            m_BuyButton.clicked += ToggleToBuy;
            m_SellButton = m_MarketPopup.Q<Button>("SellButton");
            m_SellButton.clicked += ToggleToSell;

            // ��ȡ�г����ݹ�����ͼ
            m_MarketContentScrollview = m_MarketPopup.Q<ScrollView>("ContentScrollView");

            // ��ȡ��ʱ����ǩ
            m_TimerLabel = m_Document.rootVisualElement.Q<Label>("Timer");

            // ��ʼ�����ò˵��Ͳֿ� UI
            m_SettingMenu = new SettingMenu(m_Document.rootVisualElement);
            m_SettingMenu.OnOpen += () => { GameManager.Instance.Pause(); };
            m_SettingMenu.OnClose += () => { GameManager.Instance.Resume(); };

            m_WarehouseUI = new WarehouseUI(m_Document.rootVisualElement.Q<VisualElement>("WarehousePopup"), MarketEntryTemplate);

            // ��ʼ����������
            m_Blocker = m_Document.rootVisualElement.Q<VisualElement>("Blocker");

            m_Blocker.style.opacity = 1.0f;
            m_Blocker.schedule.Execute(() => { FadeFromBlack(() => { }); }).ExecuteLater(500);

            m_Blocker.RegisterCallback<TransitionEndEvent>(evt =>
            {
                m_FadeFinishClbk?.Invoke();
            });

            // ��ʼ��������ǩ����ӵ���¼�
            m_SunLabel = m_Document.rootVisualElement.Q<Label>("SunLabel");
            m_RainLabel = m_Document.rootVisualElement.Q<Label>("RainLabel");
            m_ThunderLabel = m_Document.rootVisualElement.Q<Label>("ThunderLabel");

            m_SunLabel.AddManipulator(new Clickable(() => { GameManager.Instance.WeatherSystem?.ChangeWeather(WeatherSystem.WeatherType.Sun); }));
            m_RainLabel.AddManipulator(new Clickable(() => { GameManager.Instance.WeatherSystem?.ChangeWeather(WeatherSystem.WeatherType.Rain); }));
            m_ThunderLabel.AddManipulator(new Clickable(() => { GameManager.Instance.WeatherSystem?.ChangeWeather(WeatherSystem.WeatherType.Thunder); }));
        }

        void Update()
        {
            // ���¼�ʱ����ǩ��ʾ��ǰʱ��
            m_TimerLabel.text = GameManager.Instance.CurrentTimeAsString();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            m_HaveFocus = hasFocus;
            if (!hasFocus)
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            else
                ChangeCursor(m_CurrentCursorType);
        }

        // ��ҿ��仯ʱ���ã����¿�� UI
        public static void UpdateInventory(InventorySystem system)
        {
            s_Instance.UpdateInventory_Internal(system);
        }

        // ���½������ UI
        public static void UpdateCoins(int amount)
        {
            s_Instance.UpdateCoins_Internal(amount);
        }

        // ���г� UI ����ͣ��Ϸ
        public static void OpenMarket()
        {
            s_Instance.OpenMarket_Internal();
            GameManager.Instance.Pause();
        }

        // �ر��г� UI ���ָ���Ϸ
        public static void CloseMarket()
        {
            SoundManager.Instance.PlayUISound();
            s_Instance.m_MarketPopup.visible = false;
            GameManager.Instance.Resume();
        }

        // �򿪲ֿ� UI
        public static void OpenWarehouse()
        {
            s_Instance.m_WarehouseUI.Open();
        }

        // ���Ĺ������
        public static void ChangeCursor(CursorType cursorType)
        {
            if (s_Instance.m_HaveFocus)
            {
                switch (cursorType)
                {
                    case CursorType.Interact:
                        Cursor.SetCursor(s_Instance.InteractCursor, Vector2.zero, CursorMode.Auto);
                        break;
                    case CursorType.Normal:
                        Cursor.SetCursor(s_Instance.NormalCursor, Vector2.zero, CursorMode.Auto);
                        break;
                    case CursorType.System:
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                        break;
                }
            }

            s_Instance.m_CurrentCursorType = cursorType;
        }

        // ��������ͼ��״̬
        public static void UpdateWeatherIcons(WeatherSystem.WeatherType currentWeather)
        {
            s_Instance.m_SunLabel.EnableInClassList("on-button", currentWeather == WeatherSystem.WeatherType.Sun);
            s_Instance.m_RainLabel.EnableInClassList("on-button", currentWeather == WeatherSystem.WeatherType.Rain);
            s_Instance.m_ThunderLabel.EnableInClassList("on-button", currentWeather == WeatherSystem.WeatherType.Thunder);

            s_Instance.m_SunLabel.EnableInClassList("off-button", currentWeather != WeatherSystem.WeatherType.Sun);
            s_Instance.m_RainLabel.EnableInClassList("off-button", currentWeather != WeatherSystem.WeatherType.Rain);
            s_Instance.m_ThunderLabel.EnableInClassList("off-button", currentWeather != WeatherSystem.WeatherType.Thunder);
        }

        // ����������ɺ���ã�����������ǩ��ʾ
        public static void SceneLoaded()
        {
            s_Instance.m_SunLabel.parent.style.display =
            GameManager.Instance.WeatherSystem == null ? DisplayStyle.None : DisplayStyle.Flex;
        }

        // �ڲ����������г� UI
        private void OpenMarket_Internal()
        {
            m_MarketPopup.visible = true;

            // Ĭ��Ϊ���۱�ǩҳ
            ToggleToSell();

            GameManager.Instance.Player.ToggleControl(false);
        }

        // �л�������ģʽ
        private void ToggleToSell()
        {
            m_SellButton.AddToClassList("activeButton");
            m_BuyButton.RemoveFromClassList("activeButton");

            m_SellButton.SetEnabled(false);
            m_BuyButton.SetEnabled(true);

            // ���������Ŀ�����Ż�Ϊ����أ�
            m_MarketContentScrollview.contentContainer.Clear();

            // ���ɳ�����Ŀ
            for (int i = 0; i < GameManager.Instance.Player.Inventory.Entries.Length; ++i)
            {
                var item = GameManager.Instance.Player.Inventory.Entries[i].Item;
                if (item == null)
                    continue;

                var clone = MarketEntryTemplate.CloneTree();

                clone.Q<Label>("ItemName").text = item.DisplayName;
                clone.Q<VisualElement>("ItemIcone").style.backgroundImage = new StyleBackground(item.ItemSprite);

                var button = clone.Q<Button>("ActionButton");

                if (item is Product product)
                {
                    int count = GameManager.Instance.Player.Inventory.Entries[i].StackSize;
                    button.text = $"���� {count} ��� {product.SellPrice * count}";

                    int i1 = i;
                    button.clicked += () =>
                    {
                        GameManager.Instance.Player.SellItem(i1, count);
                        m_MarketContentScrollview.contentContainer.Remove(clone.contentContainer);
                    };
                }
                else
                {
                    button.SetEnabled(false);
                    button.text = "�޷�����";
                }

                m_MarketContentScrollview.Add(clone.contentContainer);
            }
        }

        // �л�������ģʽ
        private void ToggleToBuy()
        {
            m_SellButton.RemoveFromClassList("activeButton");
            m_BuyButton.AddToClassList("activeButton");

            m_BuyButton.SetEnabled(false);
            m_SellButton.SetEnabled(true);

            // ���������Ŀ�����Ż�Ϊ����أ�
            m_MarketContentScrollview.contentContainer.Clear();

            // ���ɹ�����Ŀ
            for (int i = 0; i < GameManager.Instance.MarketEntries.Length; ++i)
            {
                var item = GameManager.Instance.MarketEntries[i];

                var clone = MarketEntryTemplate.CloneTree();

                clone.Q<Label>("ItemName").text = item.DisplayName;
                clone.Q<VisualElement>("ItemIcone").style.backgroundImage = new StyleBackground(item.ItemSprite);

                var button = clone.Q<Button>("ActionButton");

                if (GameManager.Instance.Player.Coins >= item.BuyPrice)
                {
                    button.text = $"���� 1 ���� {item.BuyPrice}";
                    int i1 = i;
                    button.clicked += () =>
                    {
                        if (GameManager.Instance.Player.BuyItem(item))
                        {
                            if (GameManager.Instance.Player.Coins < item.BuyPrice)
                            {
                                button.text = $"���㣬��Ҫ {item.BuyPrice}";
                                button.SetEnabled(false);
                            }
                        }
                    };
                    button.SetEnabled(true);
                }
                else
                {
                    button.text = $"���㣬��Ҫ {item.BuyPrice}";
                    button.SetEnabled(false);
                }

                m_MarketContentScrollview.Add(clone.contentContainer);
            }
        }

        // ����������Ч
        public static void PlayBuySellSound(Vector3 location)
        {
            SoundManager.Instance.PlaySFXAt(location, s_Instance.MarketSellSound, false);
        }

        // �������Ч��
        public static void FadeToBlack(System.Action onFinished)
        {
            s_Instance.m_FadeFinishClbk = onFinished;

            s_Instance.m_Blocker.schedule.Execute(() =>
            {
                s_Instance.m_Blocker.style.opacity = 1.0f;
            }).ExecuteLater(10);
        }

        // �Ӻ�������
        public static void FadeFromBlack(System.Action onFinished)
        {
            s_Instance.m_FadeFinishClbk = onFinished;

            s_Instance.m_Blocker.schedule.Execute(() =>
            {
                s_Instance.m_Blocker.style.opacity = 0.0f;
            }).ExecuteLater(10);
        }

        // �ڲ����������½�Ҽ�����
        private void UpdateCoins_Internal(int amount)
        {
            m_CointCounter.text = amount.ToString();
        }

        // �ڲ����������¿�� UI
        private void UpdateInventory_Internal(InventorySystem system)
        {
            for (int i = 0; i < system.Entries.Length; ++i)
            {
                var item = system.Entries[i].Item;
                m_InventorySlots[i][0].style.backgroundImage =
                item == null ? new StyleBackground((Sprite)null) : new StyleBackground(item.ItemSprite);

                // ��ʾ / ������Ʒ������ǩ
                if (item == null || system.Entries[i].StackSize < 2)
                {
                    m_ItemCountLabels[i].style.visibility = Visibility.Hidden;
                }
                else
                {
                    m_ItemCountLabels[i].style.visibility = Visibility.Visible;
                    m_ItemCountLabels[i].text = system.Entries[i].StackSize.ToString();
                }

                // ��ǵ�ǰװ������Ʒ��λ
                if (system.EquippedItemIdx == i)
                {
                    m_InventorySlots[i].AddToClassList("equipped");
                }
                else
                {
                    m_InventorySlots[i].RemoveFromClassList("equipped");
                }
            }
        }
    }
}