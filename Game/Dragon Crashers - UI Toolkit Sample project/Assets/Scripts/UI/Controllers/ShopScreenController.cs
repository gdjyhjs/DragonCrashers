using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UIToolkitDemo
{
    // 商店屏幕的呈现器/控制器
    public class ShopScreenController : MonoBehaviour
    {

        [Tooltip("资源文件夹中MailMessage ScriptableObjects的路径。")]
        [SerializeField] string m_ResourcePath = "游戏数据/商店物品";

        // 来自资源的ScriptableObject游戏数据
        List<ShopItemSO> m_ShopItems = new List<ShopItemSO>();

        // 按类别过滤的游戏数据
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

        // 用数据填充商店屏幕
        void LoadShopData()
        {
            // 从资源目录加载ScriptableObjects（默认 = 资源/游戏数据/）
            m_ShopItems.AddRange(Resources.LoadAll<ShopItemSO>(m_ResourcePath));

            // 按类型排序
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

        // 尝试购买物品，传递购买按钮的屏幕坐标
        void OnTryBuyItem(ShopItemSO shopItemData, Vector2 screenPos)
        {
            if (shopItemData == null)
                return;

            // 通知其他对象我们正在尝试购买物品
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