using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // �̵���Ļ�ĳ�����/������
    public class ShopScreenController : MonoBehaviour
    {

        [Tooltip("��Դ�ļ�����MailMessage ScriptableObjects��·����")]
        [SerializeField] string m_ResourcePath = "��Ϸ����/�̵���Ʒ";

        // ������Դ��ScriptableObject��Ϸ����
        List<ShopItemSO> m_ShopItems = new List<ShopItemSO>();

        // �������˵���Ϸ����
        List<ShopItemSO> m_GoldShopItems = new List<ShopItemSO>();
        List<ShopItemSO> m_GemShopItems = new List<ShopItemSO>();
        List<ShopItemSO> m_PotionShopItems = new List<ShopItemSO>();

        void OnEnable()
        {
            ShopEvents.ShopItemClicked += OnTryBuyItem;

            ShopEvents.GoldSelected += OnGoldSelected;
            ShopEvents.GemSelected += OnGemSelected;
            ShopEvents.PotionSelected += OnPotionSelected;
        }

        void OnDisable()
        {
            ShopEvents.ShopItemClicked -= OnTryBuyItem;

            ShopEvents.GoldSelected -= OnGoldSelected;
            ShopEvents.GemSelected -= OnGemSelected;
            ShopEvents.PotionSelected -= OnPotionSelected;
        }

        void Start()
        {
            LoadShopData();

            ShopEvents.ShopUpdated?.Invoke(m_GoldShopItems);
        }

        // ����������̵���Ļ
        void LoadShopData()
        {
            // ����ԴĿ¼����ScriptableObjects��Ĭ�� = ��Դ/��Ϸ����/��
            m_ShopItems.AddRange(Resources.LoadAll<ShopItemSO>(m_ResourcePath));

            // ����������
            m_GoldShopItems = m_ShopItems.Where(c => c.ContentType == ShopItemType.Gold).ToList();
            m_GemShopItems = m_ShopItems.Where(c => c.ContentType == ShopItemType.Gems).ToList();
            m_PotionShopItems = m_ShopItems.Where(c => c.ContentType == ShopItemType.HealthPotion || c.ContentType == ShopItemType.LevelUpPotion).ToList();

            m_GoldShopItems = SortShopItems(m_GoldShopItems);
            m_GemShopItems = SortShopItems(m_GemShopItems);
            m_PotionShopItems = SortShopItems(m_PotionShopItems);
        }

        List<ShopItemSO> SortShopItems(List<ShopItemSO> originalList)
        {
            return originalList.OrderBy(x => x.Cost).ToList();
        }

        // ���Թ�����Ʒ�����ݹ���ť����Ļ����
        void OnTryBuyItem(ShopItemSO shopItemData, Vector2 screenPos)
        {
            if (shopItemData == null)
                return;

            // ֪ͨ���������������ڳ��Թ�����Ʒ
            ShopEvents.ShopItemPurchasing?.Invoke(shopItemData, screenPos);
        }

        void OnPotionSelected()
        {
            ShopEvents.ShopUpdated?.Invoke(m_PotionShopItems);
        }

        void OnGemSelected()
        {
            ShopEvents.ShopUpdated?.Invoke(m_GemShopItems);
        }

        void OnGoldSelected()
        {
            ShopEvents.ShopUpdated?.Invoke(m_GoldShopItems);
        }
    }
}