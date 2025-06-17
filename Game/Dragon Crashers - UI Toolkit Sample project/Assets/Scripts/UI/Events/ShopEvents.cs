using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与ShopScreen/ShopScreenController相关的公共静态委托。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class ShopEvents
    {
        // 在商店界面选择金币标签时触发
        public static Action GoldSelected;
        // 在商店界面选择宝石标签时触发
        public static Action GemSelected;
        // 在商店界面选择药剂标签时触发
        public static Action PotionSelected;

        // 通过名称选择标签时触发
        public static Action<string> TabSelected;

        // 点击ShopItemComponent上的购买按钮（传递商店物品数据和屏幕点击位置）
        public static Action<ShopItemSO, Vector2> ShopItemClicked;

        // 通知ShopScreenController
        public static Action<ShopItemSO, Vector2> ShopItemPurchasing;

        // 商店更新时触发
        public static Action<List<ShopItemSO>> ShopUpdated;

        // 消耗药剂时更新游戏数据
        public static Action<GameData> PotionsUpdated;

        // 购买物品时更新选项栏
        public static Action<GameData> FundsUpdated;

        // 处理邮件消息中的免费礼物
        public static Action<ShopItemType, uint, Vector2> RewardProcessed;

        // 物品购买成功
        public static Action<ShopItemSO, Vector2> TransactionProcessed;

        // 资金不足导致购买失败
        public static Action<ShopItemSO> TransactionFailed;
    }
}